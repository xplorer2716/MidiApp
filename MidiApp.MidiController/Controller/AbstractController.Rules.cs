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

namespace MidiApp.MidiController.Controller
{
    public abstract partial class AbstractController
    {
        /// <summary>
        /// internal helper - verify availability of the parameter in the sysex map
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        private bool VerifyParameterName(string parameterName)
        {
            if (!Tone.ParameterMap.Contains(parameterName))
            {
                System.Diagnostics.Debug.Assert(false, "No sysex message found for: " + parameterName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// internal helper - verify availability of MIDI OutputDevice
        /// </summary>
        /// <returns></returns>
        protected bool VerifySynthOutputDevice()
        {
            if ((_synthOutputDevice == null) || ((_synthOutputDevice != null) && (_synthOutputDevice.IsDisposed)))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// internal helper - verify availability of MIDI OutputDevice
        /// </summary>
        /// <returns></returns>
        protected bool VerifySynthInputDevice()
        {
            if ((_synthInputDevice == null) || ((_synthInputDevice != null) && (_synthInputDevice.IsDisposed)))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// internal helper - verify availability of MIDI InputDevice
        /// </summary>
        /// <returns></returns>
        private bool VerifyAutomationInputDevice()
        {
            if ((_automationInputDevice == null) || ((_automationInputDevice != null) && (_automationInputDevice.IsDisposed)))
            {
                return false;
            }

            return true;
        }
    }
}