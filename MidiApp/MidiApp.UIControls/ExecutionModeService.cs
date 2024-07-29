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

namespace MidiApp.UIControls
{
    /// <summary>
    /// service to know at 100% if we are in design mode
    /// </summary>
    public static class ExecutionModeService
    {
        private const string VISUAL_STUDIO_PROCESS_NAME = "devenv";

        /// <summary>
        /// Enable Design mode check
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is design mode active; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDesignModeActive
        {
            get
            {
                Process process = Process.GetCurrentProcess();
                return (process != null && process.ProcessName.StartsWith(VISUAL_STUDIO_PROCESS_NAME));
            }
        }
    }
}