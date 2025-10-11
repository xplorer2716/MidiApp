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
using System.Drawing;
using System.Windows.Forms;

namespace MidiApp.UIControls
{
    /// <summary>
    /// Checkbox implementing IValuedControl with background bitmap handling
    /// </summary>
    public partial class CheckBoxValuedControl : CheckBox, IValuedControl
    {
        //Enabled property pseudo-override
        private bool _isEnabled = true;

        // foreground colorString for Enabled state
        private Color _enabledForeColor = Color.Empty;

        //background image handling
        private BackgroundImageControlImpl _backgroundImpl;

        public CheckBoxValuedControl()
        {
            SuspendLayout();
            DoubleBuffered = true;
            InitializeComponent();
            _backgroundImpl = new BackgroundImageControlImpl();
            ResumeLayout();
        }

        /// <summary>
        /// OnForeColorChanged to manage enabled/disabled forecolor
        /// </summary>
        /// <param name="e"></param>
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            if (_enabledForeColor == Color.Empty)
            {
                _enabledForeColor = ForeColor;
            }
        }

        /// <summary>
        /// pseudo-override of default Enabled property
        /// </summary>
        public new bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                if (!_isEnabled)
                {
                    // managed disabled color
                    int disabledOffsetColor = 100;
                    int red = ForeColor.R - disabledOffsetColor; red = red < 0 ? 0 : red;
                    int green = ForeColor.G - disabledOffsetColor; green = green < 0 ? 0 : green;
                    int blue = ForeColor.B - disabledOffsetColor; blue = blue < 0 ? 0 : blue;
                    this.ForeColor = Color.FromArgb(ForeColor.A, red, green, blue);
                }
                else
                {
                    this.ForeColor = _enabledForeColor;
                }
            }
        }

        /// <summary>
        /// OnClick override
        /// </summary>
        /// <remarks>do nothing if Enabled override is false</remarks>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            if (_isEnabled)
            {
                base.OnClick(e);
            }
        }

        /// <summary>
        /// OnVisibleChanged override (background image initialization)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            _backgroundImpl.GetBackGroundImage(this);
        }

        #region IValuedControl Membres

        /// <summary>
        /// 1 = checked; 0 = unchecked
        /// </summary>
        public int Value
        {
            get
            {
                if (this.Checked) { return 1; } else { return 0; }
            }
            set
            {
                if (value == 1) { this.Checked = true; } else if (value == 0) { this.Checked = false; }
            }
        }

        public int Minimum
        {
            get
            {
                return 0;
            }
            set
            {
                // _value is constant
                // override to avoid special handling
            }
        }

        public int Maximum
        {
            get
            {
                return 1;
            }
            set
            {
                // _value is constant
                // override to avoid special handling
            }
        }

        public int Step
        {
            get
            {
                return 1;
            }
            set
            {
                // _value is constant
                // override to avoid special handling
            }
        }

        #endregion IValuedControl Membres
    }
}