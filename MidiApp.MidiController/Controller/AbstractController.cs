/*
Xplorer - A real-time editor for the Oberheim Xpander and Matrix-12 synths
Copyright (C) 2012-2026 Pascal Schmitt

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

using MidiApp.MidiController.Controller.Arguments;
using MidiApp.MidiController.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MidiApp.MidiController.Controller
{
    public abstract partial class AbstractController
    {
        /// <summary>
        /// The automation table. It's a dual entry dictionary
        /// with pametername - control change number associations
        /// </summary>
        private StringIntDualDictionary _controlChangeAutomationTable;

        private int _disabledControlChangeNumber = (int)ControlChangeType.None;

        private bool _isRunning = false;

        private bool _isSetParameterEnabled = true;

        private AbstractTone _tone = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractController"/> class.
        /// </summary>
        protected AbstractController()
        {
            //editing tone
            _tone = CreateToneInstance();

            // control change automation map
            _controlChangeAutomationTable = new StringIntDualDictionary();

            // sysex message queue and worker thread
            _parameterEntryQueue = new Queue<AbstractParameter>(PARAMETER_QUEUE_DEFAULT_CAPACITY);
        }

        public virtual StringIntDualDictionary ControlChangeAutomationTable
        {
            get { return _controlChangeAutomationTable; }
        }

        /// <summary>
        /// return program number
        /// </summary>
        public abstract int CurrentProgramNumber { get; }

        /// <summary>
        /// temporarily disable a control change automation when user is changing the parameter
        /// </summary>
        public virtual int DisabledControlChangeNumber
        {
            get
            {
                // use the table as lock object....
                lock (_controlChangeAutomationTable)
                {
                    return _disabledControlChangeNumber;
                }
            }
            set
            {
                lock (_controlChangeAutomationTable)
                {
                    _disabledControlChangeNumber = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is running (started/stopped)
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            protected set
            {
                _isRunning = value;
            }
        }

        /// <summary>
        /// returns tone name
        /// </summary>
        public virtual string ToneName
        {
            get { return Tone.ToneName; }
            set
            {
                Tone.ToneName = value;
            }
        }

        /// <summary>
        /// enable / disable the change of the parameter; in this case, nothing will happen when SetParameter is called
        /// </summary>
        protected internal bool EnableSetParameter
        {
            get { return _isSetParameterEnabled; }
            set { _isSetParameterEnabled = value; }
        }

        /// <summary>
        /// The Tone being currently edited
        /// </summary>
        /// <value>
        /// The tone.
        /// </value>
        protected AbstractTone Tone
        {
            get { return _tone; }
            set { _tone = value; }
        }

        /// <summary>
        /// Extracts the tones from bank to a given directory.
        /// </summary>
        /// <param name="bankFilename">The bank filename.</param>
        /// <param name="directoryName">Name of the directory.</param>
        /// <returns></returns>
        public abstract IEnumerable<Tuple<string, AbstractTone>> ExtractSinglePatchesFromAllDataDumpFileToDirectory(string bankFilename, string directoryName);

        /// <summary>
        /// used for init only
        /// we should not expose this to the view but come on, it's very handy...
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public virtual AbstractParameter GetParameter(string parameterName)
        {
            return (AbstractParameter)Tone.ParameterMap[parameterName];
        }

        /// <summary>
        /// Load the editing tone from the specified filename
        /// </summary>
        /// <param name="filename">the tone sysex filename</param>
        public virtual void LoadTone(string filename, IToneReader reader)
        {
            reader.ReadTone(filename, Tone);

            // send all parameters
            foreach (DictionaryEntry entry in Tone.ParameterMap)
            {
                AbstractParameter parameter = (AbstractParameter)entry.Value;
                // set values to editing tone
                //  SetParameter((string)entry.Key, (byte)parameter.value);
                AutomationParameterChangeEvent(this, new ParameterChangeEventArgs((string)entry.Key, parameter.Value));
            }
        }

        public virtual void RandomizeTone(RandomizeToneArgument argument)
        {
            Stop();
            // disable the change of parameter (automation input or change from UI)
            EnableSetParameter = false;

            Tone.RandomizeToneParameters(argument.ExcludedParameters, argument.HumanizeRatio);
            Tone.ToneName = "RANDOM";
            // we will send all the parameters update with the full tone
            foreach (AbstractParameter parameter in Tone.ParameterMap.Values)
            {
                parameter.Changed = true;
                NotifyAutomationParameterChangeEvent(new ParameterChangeEventArgs(parameter.Name, parameter.Value));
            }
            //re-enable the change of parameter
            EnableSetParameter = true;
            Start();
        }

        /// <summary>
        /// Saves the tone.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="writer">The writer.</param>
        public virtual void SaveTone(string fileName, IToneWriter writer)
        {
            if (Tone.ToneName == string.Empty)
            {
                // set the tone name if tone file does not previously exist
                FileInfo fi = new FileInfo(fileName);
                Tone.ToneName = fi.Name.Substring(0, fi.Name.Length - (fi.Extension.Length));
            }

            writer.WriteTone(fileName, Tone);
        }

        /// <summary>
        /// set the inpu/ouput midi channel
        /// </summary>
        /// <param name="midiChannel"></param>
        public virtual void SetMIDIChannel(int midiChannel)
        {
            Tone.MIDIChannel = midiChannel;
        }

        /// <summary>
        /// Sends to synthetizer the value of the named parameter, if exists.
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="value">the value</param>
        /// <returns></returns>
        public bool SetParameter(string parameterName, int value)
        {
            if (!VerifyParameterName(parameterName) || !EnableSetParameter)
            {
                return false;
            }
            AbstractParameter parameter = (AbstractParameter)Tone.ParameterMap[parameterName];
            parameter.Value = value;
            return true;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public virtual void Start()
        {
            //starts input ports
            if (VerifyAutomationInputDevice())
            {
                _automationInputDevice.StartRecording();
            }
            if (VerifySynthInputDevice())
            {
                _synthInputDevice.StartRecording();
            }
            StartWorkerThread();
            IsRunning = true;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public virtual void Stop()
        {
            //stop input ports
            if (VerifyAutomationInputDevice())
            {
                _automationInputDevice.StopRecording();
            }

            if (VerifySynthInputDevice())
            {
                _synthInputDevice.StopRecording();
            }
            StopWorkerThread();
            IsRunning = false;
        }

        /// <summary>
        /// Notifies the automation parameter change event.
        /// </summary>
        /// <param name="arg">The arg.</param>
        protected internal void NotifyAutomationParameterChangeEvent(ParameterChangeEventArgs arg)
        {
            AutomationParameterChangeEvent(this, arg);
        }

        /// <summary>
        /// to be implemented by inheriters. called in ctor
        /// </summary>
        /// <returns></returns>
        protected abstract AbstractTone CreateToneInstance();
    }
}