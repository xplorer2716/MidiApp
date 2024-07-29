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
    /// A usercontrol that "simulates" a panel with a self maintained transparency background bitmap
    /// </summary>
    public partial class FakePanel : Panel
    {
        //background image handling
        private readonly BackgroundImageControlImpl _backgroundImpl;

        public FakePanel()
        {
            SuspendLayout();
            _backgroundImpl = new BackgroundImageControlImpl();
            this.DoubleBuffered = true;
            ResumeLayout();
        }

        /// <summary>
        /// Paint override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // draw background
            Bitmap bitmap = _backgroundImpl.BackgroundBitmap;

            if (bitmap != null)
            {
                e.Graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            }
        }

        /// <summary>
        /// Paint background override - do nothing to avoir flickering
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //do nothing to avoid flickering
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
    }
}