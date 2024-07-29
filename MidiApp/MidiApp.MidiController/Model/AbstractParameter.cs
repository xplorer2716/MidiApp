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
using Sanford.Multimedia.Midi;
using System;

namespace MidiApp.MidiController.Model
{
    /// <summary>
    /// A generic parameter class. Thread safe.
    /// </summary>
    public abstract class AbstractParameter : ICloneable
    {
        // a lock object
        protected readonly object _lockObject = new object();

        private string _name;

        //Parameter's (internal) name
        public string Name
        {
            get
            {
                lock (_lockObject)
                { return _name; }
            }
            set { lock (_lockObject) { _name = value; } }
        }

        private string _label;

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label
        {
            get
            {
                lock (_lockObject)
                { return _label; }
            }
            set { lock (_lockObject) { _label = value; } }
        }

        private int _minValue;

        /// <summary>
        /// minimal value of parameter
        /// </summary>
        public virtual int MinValue
        {
            get
            {
                lock (_lockObject)
                { return _minValue; }
            }
            set
            {
                lock (_lockObject)
                {
                    _minValue = value;
                }
            }
        }

        private int _maxValue;

        /// <summary>
        /// max value of parameter
        /// </summary>
        public virtual int MaxValue
        {
            get
            {
                lock (_lockObject)
                { return _maxValue; }
            }
            set
            {
                lock (_lockObject)
                {
                    _maxValue = value;
                }
            }
        }

        private int _step;
        /// <summary>
        /// incremental step of parameter
        /// </summary>

        public virtual int Step
        {
            get
            {
                lock (_lockObject)
                { return _step; }
            }
            set
            {
                lock (_lockObject)
                {
                    if (value == 0)
                    {
                        throw new OverflowException("Step can not be equal to 0");
                    }
                    _step = value;
                }
            }
        }

        private int _value;

        /// <summary>
        ///  value of parameter
        /// </summary>

        public virtual int Value
        {
            get
            {
                lock (_lockObject)
                { return _value; }
            }
            set
            {
                lock (_lockObject)
                {
                    int computedValue = value / _step; // this keep only int part
                    computedValue *= Step;

                    if (computedValue < _minValue)
                    {
                        computedValue = _minValue;
                    }
                    else if (computedValue > _maxValue)
                    {
                        computedValue = _maxValue;
                    }

                    if (computedValue != _value)
                    {
                        _value = computedValue;

                        _changed = true;
                        UpdateMessageFromValue();
                    }
                }
            }
        }

        private bool _changed;

        /// <summary>
        /// flag indicating that a new different value was set
        /// reset of the flag has to be done by client
        /// </summary>
        public bool Changed
        {
            get
            {
                lock (_lockObject)
                { return _changed; }
            }
            set
            {
                lock (_lockObject)
                {
                    _changed = value;
                }
            }
        }

        public abstract SysExMessage Message { get; set; }

        protected abstract void UpdateMessageFromValue();

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractParameter"/> class.
        /// </summary>
        /// <param name="name">The (unique) name.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="step">The step.</param>
        /// <param name="message">The message.</param>
        /// <param name="value">The value.</param>
        /// <param name="label">The label.</param>
        protected AbstractParameter(string name, int minValue, int maxValue, int step, SysExMessage message, int value, string label = null)
        {
            Name = name;
            MinValue = minValue;
            MaxValue = maxValue;
            Step = step;
            Message = message;
            Value = value;
            Label = label;
        }

        /// <summary>
        /// ToString() overrride
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Name:{0} ,Min:{1} ,Max:{2} ,Step:{3} ,VALUE:{4} ,Message:{5}", Name, MinValue, MaxValue, Step, Value, Message.ToString());
        }

        #region ICloneable Membres

        /// <summary>
        /// for clone implementation
        /// </summary>
        /// <param name="param"></param>
        protected AbstractParameter(AbstractParameter param)
        {
            System.Diagnostics.Debug.Assert(param != this);
            Name = param.Name;
            Label = param.Label;
            MinValue = param.MinValue;
            MaxValue = param.MaxValue;
            Step = param.Step;
            Message = (SysExMessage)param.Message.Clone();
            Value = param.Value;
        }

        /// <summary>
        /// this must be implementer by inheriters
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        #endregion ICloneable Membres
    }
}