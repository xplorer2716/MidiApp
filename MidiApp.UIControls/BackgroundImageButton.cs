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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MidiApp.UIControls
{
    /// <summary>
    /// A button with customized background image when mouse is hover/down
    /// </summary>
    public class BackgroundImageButton : Button
    {
        private Image _hoverImage;
        private Image _downImage;

        // temp copy of background images
        private Image _hoverBackgroundCopyImage;

        private Image _downBackgroundCopyImage;

        /// <summary>
        /// Ctor
        /// </summary>
        public BackgroundImageButton()
            : base()
        {
            SuspendLayout();
            DoubleBuffered = true;
            ResumeLayout();
        }

        /// <summary>
        /// Gets or sets the hover image.
        /// </summary>
        /// <value>
        /// The hover image.
        /// </value>
        [Browsable(true), Category("MidiApp"), Description("Background image when mouse is hover")]
        public Image HoverImage
        {
            get { return _hoverImage; }
            set { _hoverImage = value; }
        }

        /// <summary>
        /// Gets or sets down image.
        /// </summary>
        /// <value>
        /// Down image.
        /// </value>
        [Browsable(true), Category("MidiApp"), Description("Background image when mouse is down")]
        public Image DownImage
        {
            get { return _downImage; }
            set { _downImage = value; }
        }

        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (HoverImage != null)
            {
                _hoverBackgroundCopyImage = this.BackgroundImage;
                this.BackgroundImage = HoverImage;
            }
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            if (HoverImage != null)
            {
                this.BackgroundImage = _hoverBackgroundCopyImage;
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// fire event <see cref="M:System.Windows.Forms.Control.OnMouseDown(System.Windows.Forms.MouseEventArgs)"/>.
        /// </summary>
        /// <param name="mevent"><see cref="T:System.Windows.Forms.MouseEventArgs"/> which contains event data.</param>
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (DownImage != null)
            {
                // down is already hover
                _downBackgroundCopyImage = this.BackgroundImage;
                this.BackgroundImage = DownImage;
            }

            base.OnMouseDown(mevent);
        }

        /// <summary>
        /// fire event <see cref="M:System.Windows.Forms.ButtonBase.OnMouseUp(System.Windows.Forms.MouseEventArgs)"/>.
        /// </summary>
        /// <param name="mevent"><see cref="T:System.Windows.Forms.MouseEventArgs"/> which contains event data.</param>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (DownImage != null)
            {
                this.BackgroundImage = _downBackgroundCopyImage;
            }
            base.OnMouseUp(mevent);
        }
    }
}