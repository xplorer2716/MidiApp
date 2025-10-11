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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MidiApp.UIControls
{
    /// <summary>
    /// A panel with tranparent background showing minimalistic LEDs
    /// </summary>
    public partial class LedPanelControl : UserControl
    {
        // background image
        private BackgroundImageControlImpl _backgroundImpl;

        // number of led
        private int _ledCount;

        // led colors
        private List<Color> _ledColors;

        //led states
        private List<bool> _ledStates;

        private static Color _DEFAULT_LED_ON_COLOR = Color.Red;
        private static Color _DEFAULT_LED_OFF_COLOR = Color.DarkGray;
        private static Color _DEFAULT_LED_BORDER_COLOR = Color.Black;

        /// <summary>
        /// number of LED in the control
        /// </summary>
        [Browsable(true), Category("MidiApp"), Description("number of LED in the control"), DefaultValue(1)]
        public int LedCount
        {
            get
            {
                return _ledCount;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("LedCount");
                }
                _ledCount = value;

                // reset colors
                _ledColors = new List<Color>(Enumerable.Repeat<Color>(_DEFAULT_LED_ON_COLOR, _ledCount));
                //reset states
                _ledStates = new List<bool>(Enumerable.Repeat<bool>(false, _ledCount));
            }
        }

        /// <summary>
        /// Gets the led colors.
        /// </summary>
        [Browsable(true), Category("MidiApp"), Description(" LED colors")]
        public List<Color> LedColors
        {
            get
            {
                return _ledColors;
            }
        }

        /// <summary>
        /// Gets the led states.
        /// </summary>
        [Browsable(true), Category("MidiApp"), Description(" LED states (true=on, false = off)")]
        public List<bool> LedStates
        {
            get
            {
                return _ledStates;
            }
        }

        /// <summary>
        /// Defines the led colors when led are off
        /// </summary>
        [Browsable(true), Category("MidiApp"), Description(" Led OFF color")]
        public Color LedOffColor { get; set; }

        [Browsable(true), Category("MidiApp"), Description(" Led Border color")]
        public Color LedBorderColor { get; set; }

        /// <summary>
        /// LED size
        /// </summary>
        [Browsable(true), Category("MidiApp"), Description(" LED size in pixels"), DefaultValue(5)]
        public int LedSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LedPanelControl"/> class.
        /// </summary>
        public LedPanelControl()
        {
            InitializeComponent();
            DoubleBuffered = true;

            _backgroundImpl = new BackgroundImageControlImpl();
            LedCount = 3;
            LedSize = 5;
            LedOffColor = _DEFAULT_LED_OFF_COLOR;
            LedBorderColor = _DEFAULT_LED_BORDER_COLOR;
        }

        /// <summary>
        /// override for background init
        /// </summary>
        /// <param name="e"><see cref="T:System.EventArgs"/> qui contient les données de l'événement.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            _backgroundImpl.GetBackGroundImage(this);
        }

        /// <summary>
        /// override to avoid flickering
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //override to avoid flickering
        }

        /// <summary>
        /// Déclenche l'événement <see cref="E:System.Windows.Forms.Control.Paint"/>.
        /// </summary>
        /// <param name="e"><see cref="T:System.Windows.Forms.PaintEventArgs"/> qui contient les données de l'événement.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // draw background
            Bitmap bitmap = _backgroundImpl.BackgroundBitmap;

            if (bitmap != null)
            {
                e.Graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            }

            Size controlSize = this.Size;
            int horizontalSpace = (int)((float)(controlSize.Width - (int)(LedCount * LedSize)) / (float)(LedCount + 1));
            int verticalSpace = (controlSize.Height - LedSize) / 2;
            // for debug purpose
            Debug.Assert(horizontalSpace > 0 && verticalSpace > 0);
            if (horizontalSpace <= 0 || verticalSpace <= 0)
            {
                using (Brush brush = new SolidBrush(Color.Red))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
                return;
            }

            using (Pen borderPen = new Pen(LedBorderColor, 1.0F))
            {
                for (int i = 0; i < LedCount; i++)
                {
                    Rectangle rect = new Rectangle()
                    {
                        X = horizontalSpace * (i + 1) + i * LedSize,
                        Y = verticalSpace,
                        Width = LedSize,
                        Height = LedSize,
                    };

                    using (Brush brush = LedStates[i] ? new SolidBrush(LedColors[i]) : new SolidBrush(LedOffColor))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                        e.Graphics.DrawRectangle(borderPen, rect);
                    }
                }
            }
        }
    }
}