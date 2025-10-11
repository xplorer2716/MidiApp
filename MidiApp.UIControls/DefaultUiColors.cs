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

using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace MidiApp.UIControls
{
    /// <summary>
    /// Some predefined colors
    /// </summary>
    public class DefaultUiColors
    {
        // opponents colors (see wikipedia)
        public const string GREEN_STRING = "0, 146, 70";

        public const string RED_STRING = "188, 30, 71";
        public const string YELLOW_STRING = "254, 194, 0";
        public const string BLUE_STRING = "0, 129, 205";

        public static readonly Color WHITE = Color.White;
        public static readonly Color BLACK = Color.Black;
        public static readonly Color GREEN = ColorFromString(GREEN_STRING);
        public static readonly Color RED = ColorFromString(RED_STRING);
        public static readonly Color YELLOW = ColorFromString(YELLOW_STRING);
        public static readonly Color BLUE = ColorFromString(BLUE_STRING);

        public const string DEFAULT_KNOB_TICK_COLOR_STRING = "50, 50, 50";
        public const string DEFAULT_KNOB_LED_COLOR_STRING = "102, 181, 227";
        public const string DEFAULT_KNOB_LED_BACKGROUND_COLOR_STRING = "5,255,255,255";
        public const string ALTERNATE_KNOB_LED_BACKGROUND_COLOR_STRING = "10,255,255,255";
        public const string DEFAULT_KNOB_COLOR_STRING = "96,96,96";
        public const string DEFAULT_KNOB_BORDER_COLOR_STRING = "0,0,0";

        // BUTTON DEFAULT COLORS
        public const string DEFAULT_BUTTON_FOREGROUND_COLOR_STRING = "102, 181, 255";

        // KNOB DEFAULT COLORS
        public static readonly Color DEFAULT_KNOB_TICK_COLOR = ColorFromString(DEFAULT_KNOB_TICK_COLOR_STRING);

        public static readonly Color DEFAULT_KNOB_LED_COLOR = ColorFromString(DEFAULT_KNOB_LED_COLOR_STRING);
        public static readonly Color DEFAULT_KNOB_LED_BACKGROUND_COLOR = ColorFromString(DEFAULT_KNOB_LED_BACKGROUND_COLOR_STRING);
        public static readonly Color ALTERNATE_KNOB_LED_BACKGROUND_COLOR = ColorFromString(ALTERNATE_KNOB_LED_BACKGROUND_COLOR_STRING);

        public static readonly Color DEFAULT_KNOB_COLOR = ColorFromString(DEFAULT_KNOB_COLOR_STRING);
        public static readonly Color DEFAULT_KNOB_BORDER_COLOR = ColorFromString(DEFAULT_KNOB_BORDER_COLOR_STRING);

        // BUTTON DEFAULT COLORS
        public static readonly Color DEFAULT_BUTTON_FOREGROUND_COLOR = ColorFromString(DEFAULT_BUTTON_FOREGROUND_COLOR_STRING);

        /// <summary>
        /// Buttons the colorString of the button foreground colorString from led.
        /// </summary>
        /// <param name="ledColor">Color of the led.</param>
        /// <returns></returns>
        public static Color ButtonForegroundColorFromLedColor(Color ledColor)
        {
            int blue = ledColor.B + 28 <= 255 ? ledColor.B + 28 : 255;
            return Color.FromArgb(ledColor.A, ledColor.R, ledColor.G, blue);
        }

        /// <summary>
        /// Create a colorString from string
        /// </summary>
        /// <param name="colorString">string colorString as "A,R,G,B" or "R,G,B" </param>
        /// <returns> System.Drawing.Color </returns>
        public static Color ColorFromString(string colorString)
        {
            // default values
            byte alpha = 255, red = 0, green = 0, blue = 0;
            char[] sep = new char[] { ',' };
            string[] strings = colorString.Split(sep);
            byte[] values = new byte[strings.Length];

            int index = 0;
            foreach (string valueString in strings)
            {
                try
                {
                    byte value = byte.Parse(valueString.Trim(), CultureInfo.InvariantCulture);
                    values[index] = value;
                    index++;
                }
                catch (System.Exception e)
                {
                    Debug.Fail(e.Message);
                }
            }

            Debug.Assert((values.Length == 3) || (values.Length == 4));
            index = 0;
            if (values.Length > 3)
            {
                alpha = values[index]; index++;
            }
            if (values.Length > 2)
            {
                red = values[index]; index++;
                green = values[index]; index++;
                blue = values[index];
            }
            return Color.FromArgb(alpha, red, green, blue);
        }
    }
}