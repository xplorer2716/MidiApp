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
using ThirdParty.Win32;

namespace MidiApp.UIControls

{
    [Obsolete]
    public partial class BlueTrackBar : UserControl, IValuedControl
    {
        public enum TrackBarOrientation
        {
            Vertical = 0,
            Horizontal = 1
        }

        private double _value = 50;
        private double _minimum = 0;
        private double _maximum = 100;
        private int _step = 1;

        public int Step
        {
            get { return _step; }
            set
            {
                if (_step > 0)
                {
                    _step = value;
                }
            }
        }

        private bool _isThumbMoving = false;
        private static int _wasValue = 0;

        public BlueTrackBar()
        {
            SuspendLayout();
            DoubleBuffered = true;
            InitializeComponent();
            ResumeLayout();
        }

        private void TRACK_Load(object sender, EventArgs e)
        {
            if (_minimum == 0 && _maximum == 0)
            {
                // Set default _value
                _value = 50;
                if (Orientation == TrackBarOrientation.Horizontal)
                { _minimum = 0; _maximum = 100; }
                else
                { _minimum = 100; _maximum = 0; }
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Win32Api.WM_ERASEBKGND:

                    // Create a memory bitmap to use as double buffer
                    Bitmap offScreenBmp;
                    offScreenBmp = new Bitmap(this.Width, this.Height);
                    Graphics g = Graphics.FromImage(offScreenBmp);

                    SolidBrush myBrush = new SolidBrush(this.BackColor);
                    g.FillRectangle(myBrush, 0, 0, this.Width, this.Height);
                    myBrush.Dispose();

                    Pen LEFTorTOPblue = new Pen(Color.FromArgb(95, 140, 180));
                    Pen LEFTorTOPdark = new Pen(Color.FromArgb(55, 60, 74));
                    Pen MIDDLEblue = new Pen(Color.FromArgb(21, 56, 152));
                    Pen MIDDLEdark = new Pen(Color.FromArgb(0, 0, 0));
                    Pen RIGHTorBOTTOMblue = new Pen(Color.FromArgb(99, 130, 208));
                    Pen RIGHTorBOTTOMdark = new Pen(Color.FromArgb(87, 94, 110));

                    if (Orientation == TrackBarOrientation.Horizontal)
                    {
                        int y = ClientRectangle.Height / 2;

                        if (Minimum > Maximum)
                        {
                            g.DrawLine(LEFTorTOPdark, new Point(1, y - 1), new Point(_pictureBoxBlue.Left + _pictureBoxBlue.Width / 2, y - 1));
                            g.DrawLine(LEFTorTOPblue, new Point(_pictureBoxBlue.Left + 1 + _pictureBoxBlue.Width / 2, y - 1), new Point(ClientRectangle.Width - 2, y - 1));
                            g.DrawLine(MIDDLEdark, new Point(1, y), new Point(_pictureBoxBlue.Left + _pictureBoxBlue.Width / 2, y));
                            g.DrawLine(MIDDLEblue, new Point(_pictureBoxBlue.Left + 1 + _pictureBoxBlue.Width / 2, y), new Point(ClientRectangle.Width - 2, y));
                            g.DrawLine(RIGHTorBOTTOMdark, new Point(1, y + 1), new Point(_pictureBoxBlue.Left + _pictureBoxBlue.Width / 2, y + 1));
                            g.DrawLine(RIGHTorBOTTOMblue, new Point(_pictureBoxBlue.Left + 1 + _pictureBoxBlue.Width / 2, y + 1), new Point(ClientRectangle.Width - 2, y + 1));
                        }
                        else
                        {
                            g.DrawLine(LEFTorTOPblue, new Point(1, y - 1), new Point(_pictureBoxBlue.Left + _pictureBoxBlue.Width / 2, y - 1));
                            g.DrawLine(LEFTorTOPdark, new Point(_pictureBoxBlue.Left + 1 + _pictureBoxBlue.Width / 2, y - 1), new Point(ClientRectangle.Width - 2, y - 1));
                            g.DrawLine(MIDDLEblue, new Point(1, y), new Point(_pictureBoxBlue.Left + _pictureBoxBlue.Width / 2, y));
                            g.DrawLine(MIDDLEdark, new Point(_pictureBoxBlue.Left + 1 + _pictureBoxBlue.Width / 2, y), new Point(ClientRectangle.Width - 2, y));
                            g.DrawLine(RIGHTorBOTTOMblue, new Point(1, y + 1), new Point(_pictureBoxBlue.Left + _pictureBoxBlue.Width / 2, y + 1));
                            g.DrawLine(RIGHTorBOTTOMdark, new Point(_pictureBoxBlue.Left + 1 + _pictureBoxBlue.Width / 2, y + 1), new Point(ClientRectangle.Width - 2, y + 1));
                        }
                        g.DrawLine(LEFTorTOPdark, new Point(0, y - 1), new Point(0, y + 1));
                        g.DrawLine(RIGHTorBOTTOMdark, new Point(ClientRectangle.Width - 1, y - 1), new Point(ClientRectangle.Width - 1, y + 1));
                    }
                    else
                    {
                        int x = ClientRectangle.Width / 2;

                        if (Minimum > Maximum)
                        {
                            g.DrawLine(LEFTorTOPdark, new Point(x - 1, 1), new Point(x - 1, _pictureBoxBlue.Top + _pictureBoxBlue.Width / 2));
                            g.DrawLine(LEFTorTOPblue, new Point(x - 1, _pictureBoxBlue.Top + 1 + _pictureBoxBlue.Width / 2), new Point(x - 1, ClientRectangle.Height - 2));
                            g.DrawLine(MIDDLEdark, new Point(x, 1), new Point(x, _pictureBoxBlue.Top + _pictureBoxBlue.Width / 2));
                            g.DrawLine(MIDDLEblue, new Point(x, _pictureBoxBlue.Top + 1 + _pictureBoxBlue.Width / 2), new Point(x, ClientRectangle.Height - 2));
                            g.DrawLine(RIGHTorBOTTOMdark, new Point(x + 1, 1), new Point(x + 1, _pictureBoxBlue.Top + _pictureBoxBlue.Width / 2));
                            g.DrawLine(RIGHTorBOTTOMblue, new Point(x + 1, _pictureBoxBlue.Top + 1 + _pictureBoxBlue.Width / 2), new Point(x + 1, ClientRectangle.Height - 2));
                        }
                        else
                        {
                            g.DrawLine(LEFTorTOPblue, new Point(x - 1, 1), new Point(x - 1, _pictureBoxBlue.Top + _pictureBoxBlue.Width / 2));
                            g.DrawLine(LEFTorTOPdark, new Point(x - 1, _pictureBoxBlue.Top + 1 + _pictureBoxBlue.Width / 2), new Point(x - 1, ClientRectangle.Height - 2));
                            g.DrawLine(MIDDLEblue, new Point(x, 1), new Point(x, _pictureBoxBlue.Top + _pictureBoxBlue.Width / 2));
                            g.DrawLine(MIDDLEdark, new Point(x, _pictureBoxBlue.Top + 1 + _pictureBoxBlue.Width / 2), new Point(x, ClientRectangle.Height - 2));
                            g.DrawLine(RIGHTorBOTTOMblue, new Point(x + 1, 1), new Point(x + 1, _pictureBoxBlue.Top + _pictureBoxBlue.Width / 2));
                            g.DrawLine(RIGHTorBOTTOMdark, new Point(x + 1, _pictureBoxBlue.Top + 1 + _pictureBoxBlue.Width / 2), new Point(x + 1, ClientRectangle.Height - 2));
                        }
                        g.DrawLine(LEFTorTOPdark, new Point(x - 1, 0), new Point(x + 1, 0));
                        g.DrawLine(RIGHTorBOTTOMdark, new Point(x - 1, ClientRectangle.Height - 1), new Point(x + 1, ClientRectangle.Height - 1));
                    }

                    // Draw thumb tracker
                    Bitmap bmp = new Bitmap(_pictureBoxBlue.BackgroundImage);
                    bmp.MakeTransparent(Color.FromArgb(255, 0, 255));
                    Rectangle srceRect = new Rectangle(0, 0, _pictureBoxBlue.Width, _pictureBoxBlue.Height);
                    Rectangle destRect = new Rectangle(_pictureBoxBlue.Left, _pictureBoxBlue.Top, _pictureBoxBlue.Width, _pictureBoxBlue.Height);
                    g.DrawImage(bmp, destRect, srceRect, GraphicsUnit.Pixel);
                    bmp.Dispose();

                    // Release pen resources
                    LEFTorTOPblue.Dispose();
                    LEFTorTOPdark.Dispose();
                    MIDDLEblue.Dispose();
                    MIDDLEdark.Dispose();
                    RIGHTorBOTTOMblue.Dispose();
                    RIGHTorBOTTOMdark.Dispose();

                    // Release graphics
                    g.Dispose();

                    // Swap memory bitmap (End double buffer)
                    g = Graphics.FromHdc(m.WParam);
                    g.DrawImage(offScreenBmp, 0, 0);
                    g.Dispose();
                    offScreenBmp.Dispose();

                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        //-
        [Category("Action"), Description("Occurs when the _value is changed")]
        public event EventHandler ValueChanged;

        private void SendValueChangedNotification()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new EventArgs());
            }
            else
            {
                Win32Api.SendMessage(Win32Api.GetForegroundWindow(), Win32Api.WM_COMMAND, (uint)this.Handle, (int)this.Handle);
            }
        }

        //-

        private void SetThumbLocation()
        {
            Point pos = PointToClient(Cursor.Position);

            if (Orientation == TrackBarOrientation.Horizontal)
            {
                _pictureBoxBlue.Left = Math.Min(Math.Max(pos.X - _pictureBoxBlue.Width / 2, 0), _pictureBoxBlue.Parent.Width - _pictureBoxBlue.Width);

                int range = ClientRectangle.Width - _pictureBoxBlue.Width;
                double increment = (_maximum - _minimum) / range;
                _value = (increment * _pictureBoxBlue.Left) + _minimum;
            }
            else
            {
                _pictureBoxBlue.Top = Math.Min(
                    Math.Max(pos.Y - _pictureBoxBlue.Height / 2, 0),
                    _pictureBoxBlue.Parent.Height - _pictureBoxBlue.Height);

                int range = ClientRectangle.Height - _pictureBoxBlue.Height;
                double increment = (_maximum - _minimum) / range;
                _value = (increment * _pictureBoxBlue.Top) + _minimum;
            }

            int CalculatedValue = (int)(_value / _step); // this keep only int part
            CalculatedValue *= _step;

            Value = CalculatedValue;

            if (_wasValue != (int)CalculatedValue)
            {
                this.Invalidate();
                SendValueChangedNotification();
                _wasValue = (int)CalculatedValue;
            }
        }

        private void THUMB_MouseDown(object sender, MouseEventArgs e)
        {
            _isThumbMoving = true;
            //  OnMouseDown(e);
        }

        private void THUMB_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isThumbMoving)
            {
                SetThumbLocation();
                toolTip.SetToolTip(this, ((int)_value).ToString());
            }
        }

        private void TRACK_MouseDown(object sender, MouseEventArgs e)
        {
            _isThumbMoving = true;
            SetThumbLocation();
            //OnMouseDown(e);
        }

        private void THUMB_MouseUp(object sender, MouseEventArgs e)
        {
            _isThumbMoving = false;
            toolTip.SetToolTip(this, "");
            //OnMouseUp(e);
        }

        private bool CheckOverThumb(int x, int y)
        {
            const int OFFSET = 50;
            Win32Api.RECT r = new Win32Api.RECT();
            r.left = _pictureBoxBlue.Left - OFFSET;
            r.top = _pictureBoxBlue.Top - OFFSET;
            r.right = r.left + _pictureBoxBlue.Width + 2 * OFFSET;
            r.bottom = r.top + _pictureBoxBlue.Height + 2 * OFFSET;
            Win32Api.POINT p = new Win32Api.POINT();
            p.x = x; p.y = y;
            return (Win32Api.PtInRect(ref r, p));
        }

        // Retrieve the control orientation
        public TrackBarOrientation Orientation
        {
            get
            {
                TrackBarOrientation orientation = TrackBarOrientation.Vertical;
                if (this.Width > this.Height) return TrackBarOrientation.Horizontal;
                return orientation;
            }
        }

        public int Minimum
        {
            get
            {
                return ((int)_minimum);
            }
            set
            {
                double minimumBackup = _minimum;
                _minimum = value;
                ShowThumbPos();
            }
        }

        public int Maximum
        {
            get
            {
                return ((int)_maximum);
            }
            set
            {
                double maximumBackup = _maximum;
                _maximum = value;
                ShowThumbPos();
            }
        }

        public int Value
        {
            get
            {
                return ((int)_value);
            }
            set
            {
                double valueBackup = this._value;
                if (_minimum > _maximum)
                {
                    this._value = Math.Max(Math.Min(value, _minimum), _maximum);
                }
                else
                {
                    this._value = Math.Max(Math.Min(value, _maximum), _minimum);
                }
                ShowThumbPos();
            }
        }

        private void ShowThumbPos()
        {
            if (Orientation == TrackBarOrientation.Horizontal)
            {
                int range = ClientRectangle.Width - _pictureBoxBlue.Width;
                double increment = (_maximum - _minimum) / range;
                if (increment == 0)
                {
                    _pictureBoxBlue.Left = 0;
                }
                else
                {
                    _pictureBoxBlue.Left = (int)((_value - _minimum) / increment);
                }
            }
            else
            {
                int range = ClientRectangle.Height - _pictureBoxBlue.Height;
                double increment = (_maximum - _minimum) / range;
                if (increment == 0)
                {
                    _pictureBoxBlue.Top = 0;
                }
                else
                {
                    _pictureBoxBlue.Top = (int)((_value - _minimum) / increment);
                }
            }
            this.Invalidate();
        }
    }
}