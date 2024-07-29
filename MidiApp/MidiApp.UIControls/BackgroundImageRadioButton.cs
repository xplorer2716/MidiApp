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
namespace MidiApp.UIControls
{
    /// <summary>
    /// Radio button with background bitmap
    /// </summary>
    public class BackgroundImageRadioButton : System.Windows.Forms.RadioButton
    {
        /// <summary>
        /// ctor
        /// </summary>
        public BackgroundImageRadioButton()
            : base()
        {
            SuspendLayout();
            DoubleBuffered = true;
            ResumeLayout();
        }
    }
}