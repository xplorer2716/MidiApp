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

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;

namespace MidiApp.UIControls
{
    /// <summary>
    /// A radio button container; each radio button is linked to a specific value of the panel
    /// </summary>
    public partial class RadioButtonPanel : FakePanel, IValuedControl
    {
        private int _value = 0;
        private int _minimum = 0;
        private int _maximum = 127;
        private int _step = 1;

        // An event that clients can use to be notified whenever
        // the Value is Changed.
        public event ValueChangedEventHandler ValueChanged;

        public delegate void ValueChangedEventHandler(object sender);

        public RadioButtonPanel()
        {
            SuspendLayout();
            DoubleBuffered = true;
            InitializeComponent();
            _value = _minimum;
            ResumeLayout();
        }

        #region IValuedControl Membres

        //--
        /// <summary>
        /// return the current value of the control
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value > _maximum)
                {
                    _value = _maximum;
                }
                else if (value < _minimum)
                {
                    _value = _minimum;
                }
                else
                {
                    _value = value;
                }
                UpdateChildRadioButtonsFromValue();
            }
        }

        //--
        /// <summary>
        /// Updates the child radio buttons from value.
        /// </summary>
        private void UpdateChildRadioButtonsFromValue()
        {
            foreach (Control control in this.Controls)
            {
                RadioButton radio = control as RadioButton;
                if (radio != null)
                {
                    string sValue = radio.Tag as string;
                    if (sValue != null)
                    {
                        int radioValue;
                        int.TryParse(sValue, out radioValue);
                        if (radioValue == _value)
                        {
                            radio.Checked = true;
                        }
                    }
                }
            }
        }

        //--
        /// <summary>
        /// Updates the value from RadioButton.
        /// </summary>
        /// <param name="radio">The radio.</param>
        public void UpdateValueFromRadioButton(RadioButton radio)
        {
            string sValue = radio.Tag as string;
            if (sValue != null)
            {
                int iValue;
                if (int.TryParse(sValue, out iValue))
                {
                    this._value = iValue;
                    OnValueChanged(this);
                }
            }
        }

        //--
        /// <summary>
        /// Called when [value changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        protected void OnValueChanged(object sender)
        {
            if (ValueChanged != null)
            {
                ValueChanged(sender);
            }
        }

        //--
        /// <summary>
        /// minimal value
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Minimum
        {
            get
            {
                return _minimum;
            }
            set
            {
                _minimum = value;
                if (_value < _minimum) { _value = _minimum; }
            }
        }

        //--
        /// <summary>
        /// maximal value
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                _maximum = value;
                if (_value > _maximum) { _value = _maximum; }
            }
        }

        //--
        /// <summary>
        /// step increment
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Step
        {
            get
            {
                // not really used, since radio button can have whatever for values, and non linear
                return _step;
            }
            set
            {
                // not really used, since radio button can have whatever for values, and non linear
                //
            }
        }

        //--

        #endregion IValuedControl Membres
    }
}