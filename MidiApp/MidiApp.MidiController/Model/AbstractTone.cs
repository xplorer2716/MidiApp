/*
Xplorer - A real-time editor for the Oberheim Xpander and Matrix-12 synths
Copyright (C) 2012-2024 Pascal Schmitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MidiApp.MidiController.Model
{
    /// <summary>
    /// An abstract tone. inheriter has to implement
    /// - tonename
    /// - init of parameter map. it is assumed that this map is:
    /// -- key: name of parameter
    /// -- value: derivative of AbstracParamter
    /// </summary>
    [DebuggerDisplay("ToString = {ToString()}")]
    public abstract class AbstractTone
    {
        public const int DEFAULT_MIDI_CHANNEL = 0;
        private int _midiChannel = DEFAULT_MIDI_CHANNEL;

        public virtual int MIDIChannel
        {
            get
            {
                return _midiChannel;
            }
            set
            {
                if (value < 0 || value > Sanford.Multimedia.Midi.ChannelMessage.MidiChannelMaxValue)
                {
                    throw new ArgumentOutOfRangeException("MIDIChannel", value,
                        "MIDI channel out of range.");
                }
                _midiChannel = value;
            }
        }

        public abstract string ToneName { get; set; }

        public abstract System.Collections.Specialized.OrderedDictionary ParameterMap { get; }

        protected abstract void InitializeParameterMap();

        protected AbstractTone()
        {
            InitializeParameterMap();

            // verify that user did put AbstractParameters in the map
            VerifyMapValueType();
        }

        private void VerifyMapValueType()
        {
            foreach (object value in ParameterMap.Values)
            {
                AbstractParameter param = value as AbstractParameter;
                if (param == null)
                {
                    throw new InvalidCastException("Tone's ParameterMap does not contain AbstractParameter subtypes");
                }
                else
                {
                    // we'll check only one
                    return;
                }
            }
        }

        /// <summary>
        /// randomizes tone parameters without modifying those marked as "excluded"
        /// </summary>
        /// <param name="excludedParametersNames">The excluded parameters names.</param>
        /// <param name="humnanizeRatio">The humnanize ratio.</param>
        public virtual void RandomizeToneParameters(HashSet<string> excludedParametersNames, float? humnanizeRatio)
        {
            Random randomizer = new Random(unchecked((int)DateTime.Now.Ticks));
            foreach (DictionaryEntry entry in ParameterMap)
            {
                AbstractParameter parameter = (AbstractParameter)entry.Value;
                if (!excludedParametersNames.Contains(entry.Key.ToString()))
                {
                    parameter.Value = GetNextRandomValueForParameter(randomizer, parameter, humnanizeRatio);
                }
            }
        }

        /// <summary>
        /// Gets the next random value for parameter.
        /// </summary>
        /// <param name="randomizer">The randomizer.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="humanizeRatio">The humanize ratio.</param>
        /// <returns>the next random value</returns>
        private int GetNextRandomValueForParameter(Random randomizer, AbstractParameter parameter, float? humanizeRatio)
        {
            int value = 0;
            if (!humanizeRatio.HasValue)
            {
                value = randomizer.Next(parameter.MinValue, parameter.MaxValue + 1);

                // special handling for "boolean" parameter since we'll rarely get 1
                if ((parameter.MaxValue - parameter.MinValue) == 1)
                {
                    if (randomizer.NextDouble() > 0.5F) { value = parameter.MaxValue; } else { value = parameter.MinValue; }
                }
            }
            else
            {
                // no sense to humanize a boolean
                // special handling for "boolean" parameter since we'll rarely get 1
                if ((parameter.MaxValue - parameter.MinValue) == 1)
                {
                    if (randomizer.NextDouble() > 0.5F) { value = parameter.MaxValue; } else { value = parameter.MinValue; }
                }
                else
                {
                    // determine the operation of the humanize ratio (+/-)
                    bool addValue = (randomizer.Next(0, 1) == 1);
                    value = randomizer.Next(
                        parameter.Value - (int)(parameter.Value * humanizeRatio.Value),
                        parameter.Value + (int)(parameter.Value * humanizeRatio.Value));

                    if (addValue)
                    {
                        value = (value > parameter.MaxValue) ? parameter.MaxValue : value;
                    }
                    else
                    {
                        value = (value < parameter.MinValue) ? parameter.MinValue : value;
                    }
                }
            } // humanizeRatio.HasValue

            return value;
        }

        /// <summary>
        /// Morphes the tones with the given tone morphing factor
        /// </summary>
        /// <param name="toneA">The tone a.</param>
        /// <param name="toneB">The tone b.</param>
        /// <param name="resultTone">The result tone.</param>
        /// <param name="eligibleParameters">The eligible parameters for tone morphing</param>
        /// <param name="morphingFactor">The morphing factor. 0.0F = 100% toneA, 1.0F = 100% toneB.</param>
        public void MorphTones(AbstractTone toneA, AbstractTone toneB, ref AbstractTone resultTone, float morphingFactor)
        {
            Debug.Assert(toneA != null);
            Debug.Assert(toneB != null);
            Debug.Assert(resultTone != null);
            // do only morphing of same tone class instances
            Debug.Assert(toneA.GetType().FullName == toneB.GetType().FullName);
            Debug.Assert(morphingFactor >= 0F && morphingFactor <= 1.0F);

            // go thru all the parameters and determine the morphing value
            try
            {
                foreach (string parameterName in GetEligibleParametersForToneMorhping())
                {
                    AbstractParameter parameterA = (AbstractParameter)toneA.ParameterMap[parameterName];
                    AbstractParameter parameterB = (AbstractParameter)toneB.ParameterMap[parameterName];

                    // clone parameterA, we will change only the value
                    AbstractParameter resultParameter = (AbstractParameter)parameterA.Clone();

                    // set the value
                    resultParameter.Value = (int)((1.0F - morphingFactor) * (float)parameterA.Value + morphingFactor * (float)parameterB.Value);
                    resultTone.ParameterMap[parameterName] = resultParameter;
                }
            }
            catch
            {
                resultTone = null;
            }
        }

        /// <summary>
        /// Returns the list of parameters that can be used for tone morphing
        /// </summary>
        /// <returns></returns>
        public virtual HashSet<string> GetEligibleParametersForToneMorhping()
        {
            return new HashSet<string>(ParameterMap.Keys.Cast<string>());
        }

        /// <summary>
        /// for debug purpose only
        /// </summary>
        protected string DumpParameterMap()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry entry in ParameterMap)
            {
                sb.AppendFormat(String.Format("{0}\t:{1}\r\n", (string)entry.Key, ((AbstractParameter)entry.Value).Value));
            }
            return sb.ToString();
        }

        /// <summary>
        /// override for debug purpose
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ToneName: {0} - MIDIChannel: {1}\r\n", ToneName, MIDIChannel);
            sb.Append(DumpParameterMap());
            return sb.ToString();
        }
    }
}