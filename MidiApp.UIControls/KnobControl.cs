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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace MidiApp.UIControls
{
    /// <summary>
    /// knob rotating mode
    /// </summary>
    public enum EnumRotatingMode
    {
        //default
        Linear = 0,

        Rotating = 1
    }

    public enum EnumKnobStyle
    {
        //default
        Standard = 0,

        // flat knob style "à la Audulus"
        FlatStyle = 1
    }

    /// <summary>
    /// A Knob Control. Don't blame me, I took this code from codeplex.com (VB.NET) and hacked it 'til it worked
    /// </summary>
    public class KnobControl : UserControl, IValuedControl
    {
        // angle in degrees for min and max values
        private const int _START_ANGLE = 135;

        private const int _MAX_ANGLE = 270;

        // values
        private int _minimum = 0;

        private int _maximum = 255;
        private int _value = 0;

        #region Predefined values

        // up to 10 predefined values if knob is in the positive range
        private const int PREDEFINED_VALUES_UNBALANCED_COUNT = 10;

        // up to 11 predefined values if knob is in the negative/positive range (0 is mid value)
        private const int PREDEFINED_VALUES_BALANCED_COUNT = 11;

        private int[] _predefinedValues;

        // indicate if knob can receive keydown event for predefined values
        private bool _isMouseEntered = false;

        // predefined value keymap
        private static List<int> _predefinedValuesKeyValueList =
            new List<int>() { (int)Keys.D1, (int)Keys.D2, (int)Keys.D3, (int)Keys.D4, (int)Keys.D5, (int)Keys.D6, (int)Keys.D7, (int)Keys.D8, (int)Keys.D9, (int)Keys.D0, (int)Keys.OemOpenBrackets };

        #endregion Predefined values

        // to handle negative values
        private int _offsetForNegativeValues = 0;

        //colors
        private Color _knobColor = DefaultUiColors.DEFAULT_KNOB_COLOR;

        private Color _borderColor = DefaultUiColors.DEFAULT_KNOB_BORDER_COLOR;
        private Color _tickColor = DefaultUiColors.DEFAULT_KNOB_TICK_COLOR;
        private Color _ledBorderColor = DefaultUiColors.DEFAULT_KNOB_LED_COLOR;
        private const byte LIGHTCOLOR_FACTOR = 70;

        // defined with KnobStyle property
        private Color _ledBorderBackgroundColor = DefaultUiColors.DEFAULT_KNOB_LED_BACKGROUND_COLOR;

        // knob style
        private EnumKnobStyle _knobStyle = EnumKnobStyle.Standard;

        // static to improve construction time - same colors for every knobs
        private static Pen _borderPen = null;

        private static Pen _tickPen = null;

        // indicates that knox is rotating
        private bool _isKnobRotating = false;

        // memorize starting point & value when rotating mode is linear
        private Point _startingPointForLinearMode = Point.Empty;

        private int _startingValueForLinearMode = int.MinValue;

        private RectangleF _knobRect;
        private PointF _knobPoint;

        // OffScreen image and graphics for performance
        private Bitmap _knobBackgroundBitmap;

        private Bitmap _offScreenImage;
        private Graphics _offScreenGraphics;

        /// <summary>
        /// Event fired when the value is changed
        /// </summary>
        public event ValueChangedEventHandler ValueChanged;

        public delegate void ValueChangedEventHandler(object sender);

        #region " Windows Form Designer generated code "

        //Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            this.DoubleBuffered = true;
            this.ResumeLayout(false);
        }

        /// <summary>
        ///  Dispose() override
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UIService.DisposeIfNotNull(components);
                UIService.DisposeIfNotNull(_offScreenGraphics);
                UIService.DisposeIfNotNull(_offScreenImage);
                UIService.DisposeIfNotNull(_knobBackgroundBitmap);
                UIService.DisposeIfNotNull(_borderPen);
                UIService.DisposeIfNotNull(_tickPen);
            }
            base.Dispose(disposing);
        }

        #endregion " Windows Form Designer generated code "

        /// <summary>
        /// Initializes a new instance of the <see cref="KnobControl"/> class.
        /// </summary>
        public KnobControl()
            : base()
        {
            SuspendLayout();
            DoubleBuffered = true;
            SetDimensions();
            InitializeComponent();

            //To prevent flick effect
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            ResumeLayout();
        }

        #region " Public Properties "

        /// <summary>
        /// minimal value
        /// </summary>
        public int Minimum
        {
            get
            {
                if (_offsetForNegativeValues != 0)
                {
                    return -1 * _offsetForNegativeValues;
                }
                else
                {
                    return _minimum;
                }
            }
            set
            {
                if (value < 0)
                {
                    _offsetForNegativeValues = System.Math.Abs(value);
                    _minimum = 0;
                    _maximum = _offsetForNegativeValues * 2;
                }
                else
                {
                    _offsetForNegativeValues = 0;
                    _minimum = value;
                }

                UpdatePredefinedValues();
            }
        }

        /// <summary>
        /// maximal value
        /// </summary>
        public int Maximum
        {
            get
            {
                if (_offsetForNegativeValues != 0)
                {
                    return _maximum - _offsetForNegativeValues;
                }
                else
                {
                    return _maximum;
                }
            }
            set
            {
                if (_offsetForNegativeValues != 0)
                {
                    _maximum = _offsetForNegativeValues * 2;
                }
                else
                {
                    _maximum = value;
                }
                UpdatePredefinedValues();
            }
        }

        private bool isInBalancedRange()
        {
            return (Minimum < 0 && Maximum > 0 && Math.Abs(Minimum) == Maximum);
        }

        /// <summary>
        /// This method updates the prefined values when Minimum or Maximum is changed.
        /// Predefined values are reset if min/max is changed
        /// </summary>
        private void UpdatePredefinedValues()
        {
            // -x => + x
            if (isInBalancedRange())
            {
                _predefinedValues = new int[PREDEFINED_VALUES_BALANCED_COUNT];

                // split the values to always have 0 at the middle position
                int incrementCount = 5; // (0 -> 4) [5] (6->11)
                // the negative part
                double negativeIncrement = (double)Math.Abs(Minimum) / (double)incrementCount;
                int value = Minimum;
                for (int i = 0; i < incrementCount; i++)
                {
                    int newValue = value + (int)Math.Ceiling(((double)i) * negativeIncrement);
                    _predefinedValues[i] = newValue;
                }
                // the positive part
                double positiveIncrement = ((double)Maximum) / (double)(incrementCount);
                value = 0;
                for (int i = 5; i < PREDEFINED_VALUES_BALANCED_COUNT; i++)
                {
                    int newValue = value + (int)Math.Ceiling(((double)(i - 5)) * positiveIncrement);
                    _predefinedValues[i] = (newValue > Maximum) ? Maximum : newValue;
                }
            }
            else
            {
                _predefinedValues = new int[PREDEFINED_VALUES_UNBALANCED_COUNT];
                double increment = ((double)(Maximum - Minimum)) / (double)(PREDEFINED_VALUES_UNBALANCED_COUNT - 1);
                int value = Minimum;
                for (int i = 0; i < PREDEFINED_VALUES_UNBALANCED_COUNT; i++)
                {
                    int newValue = value + (int)Math.Ceiling(((double)i) * increment);
                    _predefinedValues[i] = (newValue > Maximum) ? Maximum : newValue;
                }
            }
        }

        /// <summary>
        /// step increment
        /// </summary>
        public int Step
        {
            get
            {
                // no step support yet
                return 1;
            }
            set
            {
                // no step support yet
            }
        }

        /// <summary>
        /// return the current value of the control
        /// </summary>
        public int Value
        {
            get
            {
                if (_offsetForNegativeValues != 0)
                {
                    return (_value - _offsetForNegativeValues);
                }
                else
                {
                    return _value;
                }
            }
            set
            {
                if (_offsetForNegativeValues != 0)
                {
                    _value = value + _offsetForNegativeValues;
                }
                else
                {
                    _value = value;
                }
                this.Refresh();
            }
        }

        /// <summary>
        /// Rotating mode
        /// </summary>
        public EnumRotatingMode RotatingMode { get; set; }

        /// <summary>
        /// Knob style
        /// </summary>
        public EnumKnobStyle KnobStyle
        {
            get
            {
                return _knobStyle;
            }
            set
            {
                _knobStyle = value;
                _ledBorderBackgroundColor = (value == EnumKnobStyle.Standard) ? DefaultUiColors.DEFAULT_KNOB_LED_BACKGROUND_COLOR : DefaultUiColors.ALTERNATE_KNOB_LED_BACKGROUND_COLOR;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the predefined values.
        /// </summary>
        /// <value>
        /// The predefined values.
        /// </value>
        public int[] PredefinedValues
        {
            get
            {
                return _predefinedValues;
            }
            set
            {
                if (value == null) return;
                if (isInBalancedRange() && value.Length != PREDEFINED_VALUES_BALANCED_COUNT) return;
                if (!isInBalancedRange() && value.Length != PREDEFINED_VALUES_UNBALANCED_COUNT) return;

                if (value.Max() > Maximum || value.Min() < Minimum)
                {
                    return;
                }
                _predefinedValues = value;
            }
        }

        /// <summary>
        /// Sets the internal value with changed event.
        /// </summary>
        /// <value>
        /// The internal value with changed event.
        /// </value>
        private int InternalValueWithChangedEvent
        {
            set
            {
                _value = value;
                this.Refresh();
                OnValueChanged(this);
            }
        }

        /// <summary>
        /// Knob Color
        /// </summary>
        /// <value>
        /// The color of the knob.
        /// </value>
        [DefaultValue(typeof(Color), DefaultUiColors.DEFAULT_KNOB_COLOR_STRING)]
        public Color KnobColor
        {
            get { return _knobColor; }
            set
            {
                _knobColor = value;
                //Refresh Colors
                this.Refresh();
            }
        }

        /// <summary>
        /// Led Border Color (around the knob)
        /// </summary>
        /// <value>
        /// The color of the led border.
        /// </value>
        [DefaultValue(typeof(Color), DefaultUiColors.DEFAULT_KNOB_LED_COLOR_STRING)]
        public Color LedBorderColor
        {
            get { return _ledBorderColor; }
            set
            {
                _ledBorderColor = value;
                //Refresh Colors
                this.Refresh();
            }
        }

        #endregion " Public Properties "

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

        /// <summary>
        /// no implementation, just to avoid flickering, background is drawn in OnPaint
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        /// <summary>
        /// Paint override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (ExecutionModeService.IsDesignModeActive)
            {
                using (Brush brush = new SolidBrush(_ledBorderColor))
                {
                    g.FillRectangle(brush, _knobRect.X, _knobRect.Y, _knobRect.Width, _knobRect.Height);
                }
                return;
            }

            // draw background
            if (_knobBackgroundBitmap != null)
            {
                _offScreenGraphics.DrawImage(_knobBackgroundBitmap, 0, 0, _knobBackgroundBitmap.Width, _knobBackgroundBitmap.Height);
            }

            // draw the knob and it's border
            using (
                // this one can not be static declared else FillEllipse will misbehave
                Brush knobBrush = new LinearGradientBrush(
                    _knobRect,
                   UIService.GetLightColor(KnobColor, LIGHTCOLOR_FACTOR),
                   UIService.GetDarkColor(KnobColor, LIGHTCOLOR_FACTOR), LinearGradientMode.ForwardDiagonal))
            using (Pen ledBorderPen = new Pen(_isMouseEntered ? UIService.GetLightColor(_ledBorderColor, LIGHTCOLOR_FACTOR) : _ledBorderColor))
            using (Pen ledBorderBackgroundPen = new Pen(_ledBorderBackgroundColor))
            {
                // knob and border
                if (KnobStyle == EnumKnobStyle.Standard)
                {
                    ledBorderPen.Width = 2;
                    ledBorderBackgroundPen.Width = ledBorderPen.Width + 1;
                    _offScreenGraphics.FillEllipse(knobBrush, _knobRect);
                    _offScreenGraphics.DrawEllipse(_borderPen, _knobRect);
                }
                else
                {
                    ledBorderPen.Width = 3;
                    ledBorderBackgroundPen.Width = ledBorderPen.Width;
                    ledBorderPen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
                    ledBorderBackgroundPen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
                }

                RectangleF rect = this._knobRect;
                rect.X -= ledBorderPen.Width;
                rect.Y -= ledBorderPen.Width;
                rect.Width += ledBorderPen.Width * 2;
                rect.Height += ledBorderPen.Width * 2;

                float angle = (float)(_MAX_ANGLE * Value) / (float)Math.Abs(Maximum - Minimum);
                float startAngle = (float)_START_ANGLE + (float)_MAX_ANGLE * (1F / (float)(Maximum - Minimum)) * (0F - Minimum);
                if (KnobStyle == EnumKnobStyle.Standard)
                {
                    _offScreenGraphics.DrawEllipse(ledBorderBackgroundPen, rect);
                }
                else
                {
                    _offScreenGraphics.DrawArc(ledBorderBackgroundPen, rect, _START_ANGLE, _MAX_ANGLE);
                }
                _offScreenGraphics.DrawArc(ledBorderPen, rect, startAngle, angle);
            }
            // draw the tick
            if (KnobStyle == EnumKnobStyle.Standard)
            {
                Point arrow = this.GetKnobPosition();
                _offScreenGraphics.DrawLine(_tickPen,
                    arrow,
                    new Point(
                        (int)_knobRect.X + (int)(_knobRect.Width / 2),
                        (int)_knobRect.Y + (int)(_knobRect.Height / 2)));
            }
            // Drawimage on screen
            g.DrawImage(_offScreenImage, 0, 0, _offScreenImage.Width, _offScreenImage.Height);
        }

        /// <summary>
        /// OnMouseDown event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // client rect is bigger than _knobrect and perfect for touch screens
            if ((IsPointInRectangle(new Point(e.X, e.Y), this.ClientRectangle/*_knobRect*/)))
            {
                // Start Rotation of knob
                _isKnobRotating = true;
            }
            if (RotatingMode == EnumRotatingMode.Linear)
            {
                _startingPointForLinearMode = new Point(e.X, e.Y);
                _startingValueForLinearMode = _value;
            }
        }

        /// <summary>
        /// OnMouseUp event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Stop rotation
            _isKnobRotating = false;

            if (RotatingMode == EnumRotatingMode.Rotating)
            {
                // get value and refresh to get the unactive tick repaint
                // client rect is bigger than _knobrect and perfect for touch screens
                if ((IsPointInRectangle(new Point(e.X, e.Y), this.ClientRectangle /*_knobRect*/)))
                {
                    this.InternalValueWithChangedEvent = this.GetValueFromPosition(new Point(e.X, e.Y));
                }
            }
            else
            {
                this.InternalValueWithChangedEvent = this.GetValueFromPosition(new Point(e.X, e.Y));
            }
            this.Cursor = Cursors.Default;
            this.Refresh();
        }

        // see OnMouseMove
        private int _oldOnMouseMovePosVal = int.MinValue;

        /// <summary>
        /// MouseMouve event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isKnobRotating)
            {
                this.Cursor = Cursors.Hand;
                Point p = new Point(e.X, e.Y);
                // not to send value update if the same since last mouse move update
                // moreover, it seems that if cursor is outside knob displayRect, we get
                // sometimes MouseMove event even if the mouse didn't move ?
                int posVal = this.GetValueFromPosition(p);
                if (_oldOnMouseMovePosVal != posVal)
                {
                    _oldOnMouseMovePosVal = posVal;
                    InternalValueWithChangedEvent = posVal;
                }
            }
        }

        /// <summary>
        /// OnMouseLeave override; enter predefined value mode
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isMouseEntered = true;
            // set focus to control to enable KeyDown event
            Focus();
            this.Refresh();
        }

        /// <summary>
        /// OnMouseLeave override; exit predefined value mode
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isMouseEntered = false;
            this.Refresh();
        }

        /// <summary>
        /// OnKeyDown event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_isMouseEntered)
            {
                // Determine whether the keystroke is in the keymap
                int index = _predefinedValuesKeyValueList.IndexOf(e.KeyValue);
                if (index != -1)
                {
                    if (!isInBalancedRange() && e.KeyValue == _predefinedValuesKeyValueList.Last())
                    {
                        return;
                    }
                    // select the predefined value accordingly
                    InternalValueWithChangedEvent = PredefinedValues[index] + _offsetForNegativeValues;
                }
            }
        }

        /// <summary>
        /// Sets the dimensions.
        /// </summary>
        private void SetDimensions()
        {
            // get smaller from height and width
            int size = this.Width;
            if ((this.Width > this.Height))
            {
                size = this.Height;
            }
            // allow gap on all side to determine size of knob
            float gap = 0.2F;
            _knobRect = new RectangleF((float)(size * gap), (float)(size * gap), (float)(size * (1 - (2 * gap))), (float)(size * (1 - 2 * gap)));
            _knobPoint = new PointF(_knobRect.X + _knobRect.Width / 2, _knobRect.Y + _knobRect.Height / 2);

            // create offscreen image and graphics
            UIService.DisposeIfNotNull(_offScreenImage);
            UIService.DisposeIfNotNull(_offScreenGraphics);

            _offScreenImage = new Bitmap(this.Width, this.Height);
            _offScreenGraphics = Graphics.FromImage(_offScreenImage);
            _offScreenGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _offScreenGraphics.CompositingQuality = CompositingQuality.HighQuality;
        }

        /// <summary>
        /// OnResize override
        /// </summary>
        /// <param name="e"><see cref="T:System.EventArgs"/> qui contient les données de l'événement.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetDimensions();
            InitializeBackground();
            Refresh();
        }

        /// <summary>
        /// gets knob position that is to be drawn on control (EnumRotatingMode.Rotating)
        /// </summary>
        /// <returns></returns>
        private Point GetKnobPosition()
        {
            double degree = _MAX_ANGLE * _value / (this._maximum - _minimum);
            // position  0 at - 135�
            degree = (degree + _START_ANGLE) * Math.PI / 180;

            Point Pos = (new Point(0, 0));
            Pos.X = (int)(Math.Cos(degree) * (_knobRect.Width / 2 - 2) + _knobRect.X + _knobRect.Width / 2);
            Pos.Y = (int)(Math.Sin(degree) * (_knobRect.Width / 2 - 2) + _knobRect.Y + _knobRect.Height / 2);
            return Pos;
        }

        /// <summary>
        /// converts geometrical position in to value
        /// </summary>
        /// <param name="p">Point that is to be converted</param>
        /// <returns>Value derived from position</returns>
        private int GetValueFromPosition(Point p)
        {
            int value = 0;
            // handle the rotating mode
            if (RotatingMode == EnumRotatingMode.Rotating)
            {
                double degree = 0.0;
                if ((p.X <= _knobPoint.X))
                {
                    degree = (_knobPoint.Y - p.Y) / (_knobPoint.X - p.X);
                    degree = Math.Atan(degree);
                    degree = (degree) * (180 / Math.PI) + (_START_ANGLE - 90);
                    value = (int)(degree * (this._maximum - _minimum) / _MAX_ANGLE);
                }
                else if ((p.X > _knobPoint.X))
                {
                    degree = (p.Y - _knobPoint.Y) / (p.X - _knobPoint.X);
                    degree = Math.Atan(degree);
                    degree = (360 - _START_ANGLE) + (degree) * (180 / Math.PI);
                    value = (int)(degree * (this._maximum - _minimum) / _MAX_ANGLE);
                }
            }
            else
            {
                value = _startingValueForLinearMode + (_startingPointForLinearMode.Y - p.Y);
            }

            if (value > _maximum)
            {
                value = _maximum;
            }
            if (value < _minimum)
            {
                value = _minimum;
            }
            return value;
        }

        // Method which checks is particular point is in rectangle
        // <param name="p">Point to be Chaecked</param>
        // <param name="r">Rectangle</param>
        // <returns>true is Point is in rectangle, else false</returns>
        public static bool IsPointInRectangle(Point p, RectangleF r)
        {
            bool flag = false;
            if ((p.X > r.X && p.X < r.X + r.Width && p.Y > r.Y && p.Y < r.Y + r.Height))
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// Load override
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeBackground();
            InitializeGraphicDevices();
        }

        /// <summary>
        /// Initialize static graphic devices
        /// </summary>
        private void InitializeGraphicDevices()
        {
            if (_borderPen == null)
            {
                _borderPen = new Pen(_borderColor);
            }
            if (_tickPen == null)
            {
                _tickPen = new Pen(_tickColor, 3);
                _tickPen.EndCap = LineCap.Round;
                _tickPen.StartCap = LineCap.Round;
            }
        }

        /// <summary>
        /// Initialize pseudo transparency background
        /// </summary>
        private void InitializeBackground()
        {
            Control parentControl = this.Parent;
            if (parentControl != null)
            {
                Point location = Point.Empty;

                Image backgroundImage = null;
                if (parentControl.BackgroundImage != null)
                {
                    // parent's one
                    backgroundImage = parentControl.BackgroundImage;
                    location = this.Location;
                }
                else if (parentControl.BackColor == Color.Transparent && parentControl.Parent != null && parentControl.Parent.BackgroundImage != null)
                {
                    // grandma's one
                    backgroundImage = parentControl.Parent.BackgroundImage;
                    location = new Point(parentControl.Location.X + this.Location.X, parentControl.Location.Y + this.Location.Y);
                }

                if (backgroundImage != null)
                {
                    UIService.DisposeIfNotNull(_knobBackgroundBitmap);
                    RectangleF sourceRectangle = this.ClientRectangle;
                    _knobBackgroundBitmap = new Bitmap((int)sourceRectangle.Width, (int)sourceRectangle.Height);

                    // background graphics
                    using (Graphics graphicsBackground = Graphics.FromImage(_knobBackgroundBitmap))
                    {
                        float ratioX, ratioY;

                        // Parent.BackgroundImage to get resolution
                        using (Bitmap sourceBitmap = new Bitmap(backgroundImage))
                        {
                            ratioX = sourceBitmap.HorizontalResolution / _knobBackgroundBitmap.HorizontalResolution;
                            ratioY = sourceBitmap.VerticalResolution / _knobBackgroundBitmap.VerticalResolution;
                        }
                        // compute parent stretch ratio
                        float stretchRatioHeight = 1F / (float)parentControl.ClientSize.Height * (float)backgroundImage.Height;
                        float stretchRatioWidth = 1F / (float)parentControl.ClientSize.Width * (float)backgroundImage.Width;

                        // destination rectangle
                        RectangleF destRectangle = new RectangleF(0, 0, sourceRectangle.Width, sourceRectangle.Height);

                        // resize source rectangle depending on bitmap resolution
                        sourceRectangle.X = location.X * stretchRatioWidth;
                        sourceRectangle.Y = location.Y * stretchRatioHeight;
                        sourceRectangle.Height = sourceRectangle.Height * ratioY * stretchRatioHeight;
                        sourceRectangle.Width = sourceRectangle.Width * ratioX * stretchRatioWidth;

                        //bitmap copy
                        graphicsBackground.DrawImage(backgroundImage, destRectangle, sourceRectangle, GraphicsUnit.Pixel);
                    }
                }
                else
                {
                    // fill brackground with background colorString
                    UIService.DisposeIfNotNull(_knobBackgroundBitmap);
                    RectangleF sourceRectangle = this.ClientRectangle;
                    _knobBackgroundBitmap = new Bitmap((int)sourceRectangle.Width, (int)sourceRectangle.Height);

                    // background graphics
                    using (Graphics graphicsBackground = Graphics.FromImage(_knobBackgroundBitmap))
                    using (SolidBrush brush = new SolidBrush(this.BackColor))
                    {
                        graphicsBackground.FillRectangle(brush, this.ClientRectangle);
                    }
                }
            }
        }
    }
}