/*
Xplorer - A real-time editor for the Oberheim Xpander and Matrix-12 synths
Copyright (C) 2012-2026 Pascal Schmitt

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

namespace MidiApp.MidiController.Controller
{
    public abstract partial class AbstractController
    {
        /// <summary>
        /// arguments for a EventHandler
        /// </summary>
        public class ParameterChangeEventArgs : EventArgs
        {
            public string ParameterName { get; set; }

            public int Value { get; set; }

            public ParameterChangeEventArgs(string parameterName, int value)
            {
                this.ParameterName = parameterName;
                this.Value = value;
            }
        }

        /// <summary>
        /// event fired when a AbstractController parameter is changed internally (CC automation, internal randomizer)
        /// this can be used for GUI update
        /// </summary>
        public event EventHandler<ParameterChangeEventArgs> AutomationParameterChangeEvent = null;
    }
}