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
    /// A class to handle pseudo-transparency of background control
    /// </summary>
    /// <remarks>much better and efficient that using Transparent background color</remarks>
    internal class BackgroundImageControlImpl : IDisposable
    {
        // OffScreen image of control and graphics for performance
        private Bitmap _backgroundBitmap;

        /// <summary>
        /// The background bitmap
        /// </summary>
        public Bitmap BackgroundBitmap
        {
            get { return _backgroundBitmap; }
        }

        /// <summary>
        /// Gets the backColor ground image for a control
        /// </summary>
        /// <param name="control">The control.</param>
        public void GetBackGroundImage(/* in/out */Control control)
        {
            // do not recreate background bitmap if already generated
            if (_backgroundBitmap != null)
            {
                UIService.DisposeIfNotNull(_backgroundBitmap);
            }

            Control parentControl = control.Parent;

            if (parentControl != null)
            {
                Point location = Point.Empty;

                Image sourceBackgroundImage = null;
                if (parentControl.BackgroundImage != null)
                {
                    // parent's one
                    sourceBackgroundImage = parentControl.BackgroundImage;
                    location = control.Location;
                }
                else if (parentControl.BackColor == Color.Magenta && parentControl.Parent != null && parentControl.Parent.BackgroundImage != null)
                {
                    // grandma's one; I know I should go up until no parent is available, but who cares, I know what I need
                    sourceBackgroundImage = parentControl.Parent.BackgroundImage;
                    location = new Point(parentControl.Location.X + control.Location.X, parentControl.Location.Y + control.Location.Y);
                }

                if (sourceBackgroundImage != null)
                {
                    UIService.DisposeIfNotNull(_backgroundBitmap);
                    RectangleF sourceRectangle = control.ClientRectangle;
                    _backgroundBitmap = new Bitmap((int)sourceRectangle.Width, (int)sourceRectangle.Height);

                    // background graphics
                    using (Graphics graphicsBackground = Graphics.FromImage(_backgroundBitmap))
                    {
                        float ratioX, ratioY;

                        // Parent.BackgroundImage to get resolution
                        using (Bitmap sourceBitmap = new Bitmap(sourceBackgroundImage))
                        {
                            ratioX = sourceBitmap.HorizontalResolution / _backgroundBitmap.HorizontalResolution;
                            ratioY = sourceBitmap.VerticalResolution / _backgroundBitmap.VerticalResolution;
                        }
                        // compute parent stretch ratio, since background image has original size, we can compute the ratio
                        float stretchRatioHeight = 1F / (float)parentControl.ClientSize.Height * (float)sourceBackgroundImage.Height;
                        float stretchRatioWidth = 1F / (float)parentControl.ClientSize.Width * (float)sourceBackgroundImage.Width;

                        // destination rectangle on the client part of the control
                        RectangleF destRectangle = new RectangleF(0, 0, sourceRectangle.Width, sourceRectangle.Height);

                        // resize source rectangle depending on bitmap resolution and parentcontrol client size (stretch)
                        sourceRectangle.X = location.X * stretchRatioWidth;
                        sourceRectangle.Y = location.Y * stretchRatioHeight;
                        sourceRectangle.Height = sourceRectangle.Height * ratioY * stretchRatioHeight;
                        sourceRectangle.Width = sourceRectangle.Width * ratioX * stretchRatioWidth;

                        //copy bitmap and set it as background
                        graphicsBackground.DrawImage(sourceBackgroundImage, destRectangle, sourceRectangle, GraphicsUnit.Pixel);
                        control.BackgroundImage = _backgroundBitmap;
                    } //using
                }// if (sourceBackgroundImage != null)
                else
                {
                    // fill brackground with background color
                    UIService.DisposeIfNotNull(_backgroundBitmap);
                    RectangleF sourceRectangle = control.ClientRectangle;
                    _backgroundBitmap = new Bitmap((int)sourceRectangle.Width, (int)sourceRectangle.Height);

                    // background graphics
                    using (Graphics graphicsBackground = Graphics.FromImage(_backgroundBitmap))
                    using (SolidBrush brush = new SolidBrush(control.BackColor))
                    {
                        graphicsBackground.FillRectangle(brush, sourceRectangle);
                    }
                }
            } // if (parentControl!=null)
        }

        #region IDisposable Membres

        private bool _disposed;

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object offColor the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    UIService.DisposeIfNotNull(_backgroundBitmap);
                }
                // Note disposing has been done.
                _disposed = true;
            }
        }

        #endregion IDisposable Membres
    }
}