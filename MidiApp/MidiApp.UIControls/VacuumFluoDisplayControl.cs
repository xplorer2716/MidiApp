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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MidiApp.UIControls
{
    /// <summary>
    /// A Vacuum Fluo Display control
    /// </summary>
    public class VacuumFluoDisplayControl : System.Windows.Forms.Control
    {
        private static int CHAR_WIDTH = 12;
        private static int CHAR_HEIGHT = 16;
        private static int X_SPACING = 0;
        private static int Y_SPACING = 0;

        // the current text being displayed
        private string _currentText = null;

        private string _previousText = null;

        private readonly int _charWidth, _charHeight, _xSpacing, _ySpacing;
        private int _maxLinesCount, _maxCharsPerLine;

        // the matrix image containing all the characters
        private readonly Bitmap _imageMatrix;

        // the current displayed display content
        private Bitmap _bufferBitmap;

        private Graphics _bufferGraphics;

        /// <summary>
        /// Initializes a new instance of the <see cref="VacuumFluoDisplayControl"/> class.
        /// </summary>
        public VacuumFluoDisplayControl()
        {
            SuspendLayout();
            DoubleBuffered = true;
            // get image from ressources
            _imageMatrix = MidiApp.UIControls.Properties.Resources.MATRIXTINY;

            // sizes
            _charWidth = CHAR_WIDTH;
            _charHeight = CHAR_HEIGHT;
            _xSpacing = X_SPACING;
            _ySpacing = Y_SPACING;

            //To prevent flick effect
            //this.SetStyle(ControlStyles.DoubleBuffer, true);
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.SetStyle(ControlStyles.UserPaint, true);

            this.BackColor = Color.Black;
            ResumeLayout();
        }

        /// <summary>
        /// CreateParams override (double buffering)
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                if (!ExecutionModeService.IsDesignModeActive)
                {
                    const int WS_ES_COMPOSITED = 0x02000000;
                    // Activate double buffering at the form level.  All child controls will be double buffered as well.
                    createParams.ExStyle |= WS_ES_COMPOSITED;   // WS_EX_COMPOSITED
                }
                return createParams;
            }
        }

        /// <summary>
        /// Initialize control's background
        /// </summary>
        private void InitializeBackground()
        {
            // reset the current image
            UIService.DisposeIfNotNull(_bufferBitmap);

            _bufferBitmap = new Bitmap(_maxCharsPerLine * (_charWidth + _xSpacing), _maxLinesCount * (_charHeight + _ySpacing));
            _bufferGraphics = Graphics.FromImage(_bufferBitmap);

            // force full redraw with blank text
            _previousText = String.Empty.PadRight(_maxCharsPerLine * _maxLinesCount, '*');
            SetText(String.Empty.PadRight(_maxCharsPerLine * _maxLinesCount));
        }

        /// <summary>
        /// Gets the char bitmap offset.
        /// </summary>
        /// <param name="theChar">The char.</param>
        /// <param name="rectangle">The offset rectangle.</param>
        public void GetCharBitmapOffset(char theChar, ref Rectangle rectangle)
        {
            int left, right;

            //These symbols are not present in our matrix.
            //Replace by "empty space"
            if ((int)theChar >= 127 || (int)theChar <= 31)
            {
                theChar = ' ';
            }

            int charValue = (int)theChar;
            left = (charValue - 32);

            if (left < 32)
            {
                left *= (_charWidth + _xSpacing);
            }
            else if (left < 64)
            {
                left = (left - 32) * (_charWidth + _xSpacing);
            }
            else
            {
                left = (left - 64) * (_charWidth + _xSpacing);
            }
            right = ((charValue - 32) / 32) * (_charHeight + _ySpacing);

            rectangle = new Rectangle(left, right, _charWidth, _charHeight);
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void SetText(string text)
        {
            // preconditions
            if (_currentText == text) return;
            Debug.Assert(text.Length <= _maxCharsPerLine * _maxLinesCount, "text too long");
            _currentText = text.PadRight(_maxCharsPerLine * _maxLinesCount);
            if (_maxLinesCount == 0)
            {
                NumberOfLines = 0;
            }

            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:Paint"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        protected override void OnPaint(PaintEventArgs args)
        {
            int x = 0, y = 0;
            Rectangle charRectangle = Rectangle.Empty;

            int charCount = 0;
            int lineCount = 1;

            //draw text
            for (int index = 0; index < _currentText.Length; index++)
            {
                // do not update character if it is the same
                if (_previousText[index] != _currentText[index])
                {
                    //get clip region for current char
                    GetCharBitmapOffset(_currentText[index], ref charRectangle);

                    //source/dest rectangles
                    //Initializes target clip
                    Rectangle destRectangle = new Rectangle(x, y, charRectangle.Width, charRectangle.Height);

                    //Draw current symbol in buffer
                    _bufferGraphics.DrawImage(_imageMatrix,
                        destRectangle,
                        charRectangle.X, charRectangle.Y, charRectangle.Width, charRectangle.Height,
                        GraphicsUnit.Pixel);
                }

                // next char calculation
                x += _charWidth + _xSpacing;
                charCount++;
                if ((charCount == _maxCharsPerLine) && _maxLinesCount == 1)
                {
                    break;
                }
                else if ((charCount == _maxCharsPerLine) && _maxLinesCount > 1)
                {
                    if (lineCount == _maxLinesCount)
                    {
                        break;
                    }
                    x = charCount = 0;
                    y += _charHeight + _ySpacing;
                    lineCount++;
                }
            } //for

            int left = Convert.ToInt32((this.ClientRectangle.Width - _bufferBitmap.Width) / 2);
            int top = Convert.ToInt32((this.ClientRectangle.Height - _bufferBitmap.Height) / 2);
            args.Graphics.DrawImageUnscaled(_bufferBitmap, left, top);

            // performance optimization
            _previousText = _currentText;
        }

        /// <summary>
        /// Defines the char count per lines
        /// </summary>
        public int MaxCharsPerLine
        {
            get
            {
                return _maxCharsPerLine;
            }
            private set
            {
                _maxCharsPerLine = value;
            }
        }

        /// <summary>
        /// Defines number of lines
        /// </summary>
        public int NumberOfLines
        {
            get
            {
                return _maxLinesCount;
            }
            private set
            {
                if (value == 0)
                {
                    if (_maxCharsPerLine != 0)
                    {
                        _maxLinesCount = _currentText.Length / _maxCharsPerLine;
                    }
                    else
                    {
                        _maxLinesCount = 1;
                    }
                }
                else
                {
                    _maxLinesCount = value;
                }
            }
        }

        /// <summary>
        /// Adjusts to client rectangle.
        /// </summary>
        private void AdjustToClientRectangle()
        {
            int symbolPerLine = this.Size.Width / this._charWidth;
            int lines = this.Size.Height / this._charHeight;
            NumberOfLines = lines;
            this.MaxCharsPerLine = symbolPerLine;
            InitializeBackground();
        }

        /// <summary>
        /// OnSizeChanged event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(System.EventArgs e)
        {
            base.OnSizeChanged(e);
            RectangleF clientRectangle = this.ClientRectangle;

            // when the hosting form is reduced in the trackbar, we can get a resize event with ClientRectangle==0,0
            // do not handle adjustement in this case
            if (clientRectangle == new RectangleF(0F, 0F, 0F, 0F))
            {
                return;
            }
            AdjustToClientRectangle();
        }

        #region IDisposable Membres (from Control)

        private bool _disposed;

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    UIService.DisposeIfNotNull(_bufferGraphics);
                    UIService.DisposeIfNotNull(_bufferBitmap);
                }
                // Note disposing has been done.
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable Membres (from Control)
    }
}