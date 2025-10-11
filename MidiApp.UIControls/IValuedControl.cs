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
    /// interface that every control should implement
    /// </summary>
    public interface IValuedControl
    {
        /// <summary>
        /// return the current value of the control
        /// </summary>
        int Value { get; set; }

        /// <summary>
        /// minimal value
        /// </summary>
        int Minimum { get; set; }

        /// <summary>
        /// maximal value
        /// </summary>
        int Maximum { get; set; }

        /// <summary>
        /// step increment
        /// </summary>
        int Step { get; set; }
    }
}