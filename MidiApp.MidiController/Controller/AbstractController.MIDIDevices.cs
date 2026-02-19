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

using MidiApp.MidiController.Model;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.UI.Windows;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MidiApp.MidiController.Controller
{
    public abstract partial class AbstractController
    {
        /// <summary>
        /// The Midi output device for the synth. use SendDataToSynthOutputDevice instead of using it directly
        /// </summary>
        protected OutputDevice _synthOutputDevice = null;

        /// <summary>
        /// Synth input
        /// </summary>
        protected InputDevice _synthInputDevice = null;

        /// <summary>
        /// The MIDI input device for automation (DAW CC#)
        /// </summary>
        protected InputDevice _automationInputDevice = null;

        /// <summary>
        /// the Sysex Transmit delay (delay in ms between each sysex message sent to synth)
        /// </summary>
        private int _parameterTransmitDelay = 20; //default value

        /// <summary>
        /// Change the Automation MIDI input device by specifying its name
        /// </summary>
        /// <param name="deviceName">name of device</param>
        /// <returns>false if device is not found</returns>
        public virtual bool SetAutomationInputDevice(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName)) return false;

            Action deactivateAutomationInputDevice = () =>
            {
                _automationInputDevice.StopRecording();
                _automationInputDevice.ChannelMessageReceived -= AutomationInputDeviceChannelMessageReceived;
                _automationInputDevice.SysCommonMessageReceived -= AutomationInputDeviceSysCommonMessageReceived;
                _automationInputDevice.SysExMessageReceived -= AutomationInputDeviceSysExMessageReceived;
                _automationInputDevice.SysRealtimeMessageReceived -= AutomationInputDeviceSysRealtimeMessageReceived;
                _automationInputDevice.Error -= new EventHandler<Sanford.Multimedia.ErrorEventArgs>(AutomationInputDeviceError);
                _automationInputDevice.Close();
                _automationInputDevice.Dispose();
            };

            for (int iDevice = 0; iDevice < InputDevice.DeviceCount; iDevice++)
            {
                MidiInCaps capabilities = InputDevice.GetDeviceCapabilities(iDevice);
                if (deviceName.CompareTo(capabilities.name) == 0)
                {
                    if (VerifyAutomationInputDevice())
                    {
                        deactivateAutomationInputDevice();
                    }
                    _automationInputDevice = new InputDevice(iDevice);
                    _automationInputDevice.ChannelMessageReceived += AutomationInputDeviceChannelMessageReceived;
                    _automationInputDevice.SysCommonMessageReceived += AutomationInputDeviceSysCommonMessageReceived;
                    _automationInputDevice.SysExMessageReceived += AutomationInputDeviceSysExMessageReceived;
                    _automationInputDevice.SysRealtimeMessageReceived += AutomationInputDeviceSysRealtimeMessageReceived;
                    _automationInputDevice.Error += new EventHandler<Sanford.Multimedia.ErrorEventArgs>(AutomationInputDeviceError);
                    _automationInputDevice.StartRecording();
                    return true;
                }
            }
            // device not found
            if (VerifyAutomationInputDevice())
            {
                deactivateAutomationInputDevice();
            }
            _automationInputDevice = null;
            return false;
        }

        #region AUTOMATION_INPUT

        /// <summary>
        /// Handler for InputDevice error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AutomationInputDeviceError(object sender, Sanford.Multimedia.ErrorEventArgs e)
        {
        }

        protected virtual void AutomationInputDeviceChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            // if CC# message is registered as automation, update the corresponding parameter.
            // else redirect to synth as is
            if (e.Message.Command == ChannelCommand.Controller)
            {
                // check if this control change number is not temporarily disabled
                if (e.Message.Data1 != DisabledControlChangeNumber)
                {
                    List<string> ParameterNames = _controlChangeAutomationTable[e.Message.Data1];
                    if (ParameterNames != null)
                    {
                        for (int i = 0; i < ParameterNames.Count; i++)
                        {
                            string sParameterName = ParameterNames[i];
                            // change parameter, and autoscale
                            AbstractParameter param = GetParameter(sParameterName);

                            double dInterleave = (param.MaxValue + System.Math.Abs(param.MinValue));
                            //special case handling for "1" interleave. fill the gap when we get more that 64 in order to have mid range switching
                            double dValue = (double)e.Message.Data2;
                            if ((dInterleave == 1))
                            {
                                if (dValue > 63) dValue = 127;
                            }
                            double dControllerRatio = (dValue / (double)127);
                            int Value = (int)(dControllerRatio * dInterleave) - System.Math.Abs(param.MinValue);
                            SetParameter(sParameterName, Value);
                            // fire the parameter change event
                            NotifyAutomationParameterChangeEvent(new ParameterChangeEventArgs(sParameterName, Value));
                        }
                    }
                    else
                    {
                        //this  CC does not automate anything, forward
                        if (VerifySynthOutputDevice())
                        {
                            ChannelMessage forwardedMessage = new ChannelMessage(e.Message.Command, Tone.MIDIChannel, e.Message.Data1, e.Message.Data2);
                            _synthOutputDevice.Send(forwardedMessage);
                        }
                    }
                }
            }
            else
            {
                if (VerifySynthOutputDevice())
                {
                    ChannelMessage forwardedMessage = new ChannelMessage(e.Message.Command, Tone.MIDIChannel, e.Message.Data1, e.Message.Data2);
                    _synthOutputDevice.Send(forwardedMessage);
                }
            }
        }

        /// <summary>
        /// defaut behavior redirects to synth output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AutomationInputDeviceSysExMessageReceived(object sender, SysExMessageEventArgs e)
        {
            SynchronizationContext.Current.Post(delegate (object dummy)
            {
                // redirect received message to synth
                if (VerifySynthOutputDevice())
                {
                    _synthOutputDevice.Send(e.Message);
                }
            }, null);
        }

        /// <summary>
        /// defaut behavior redirects to synth output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AutomationInputDeviceSysCommonMessageReceived(object sender, SysCommonMessageEventArgs e)
        {
            SynchronizationContext.Current.Post(delegate (object dummy)
            {
                // redirect received message to synth
                if (VerifySynthOutputDevice())
                {
                    _synthOutputDevice.Send(e.Message);
                }
            }, null);
        }

        /// <summary>
        /// defaut behavior redirects to synth output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AutomationInputDeviceSysRealtimeMessageReceived(object sender, SysRealtimeMessageEventArgs e)
        {
            SynchronizationContext.Current.Post(delegate (object dummy)
            {
                // redirect received message to synth
                if (VerifySynthOutputDevice())
                {
                    _synthOutputDevice.Send(e.Message);
                }
            }, null);
        }

        #endregion AUTOMATION_INPUT

        #region SYNTH_INPUT

        /// <summary>
        /// Handler for InputDevice error (default: do nothing)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SynthInputDeviceError(object sender, Sanford.Multimedia.ErrorEventArgs e)
        {
        }

        /// <summary>
        /// default behavior: do nothing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SynthInputDeviceChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            //do nothing
        }

        /// <summary>
        /// default behavior: do nothing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SynthInputDeviceSysExMessageReceived(object sender, SysExMessageEventArgs e)
        {
            //do nothing
        }

        /// <summary>
        /// default behavior: do nothing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SynthInputDeviceSysCommonMessageReceived(object sender, SysCommonMessageEventArgs e)
        {
            //do nothing
        }

        /// <summary>
        /// default behavior: do nothing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SynthInputDeviceSysRealtimeMessageReceived(object sender, SysRealtimeMessageEventArgs e)
        {
            //do nothing
        }

        public virtual bool SetSynthInputDevice(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName)) return false;

            Action deactivatSynthInputDevice = () =>
              {
                  _synthInputDevice.StopRecording();
                  _synthInputDevice.ChannelMessageReceived -= SynthInputDeviceChannelMessageReceived;
                  _synthInputDevice.SysCommonMessageReceived -= SynthInputDeviceSysCommonMessageReceived;
                  _synthInputDevice.SysExMessageReceived -= SynthInputDeviceSysExMessageReceived;
                  _synthInputDevice.SysRealtimeMessageReceived -= SynthInputDeviceSysRealtimeMessageReceived;
                  _synthInputDevice.Error -= new EventHandler<Sanford.Multimedia.ErrorEventArgs>(SynthInputDeviceError);
                  _synthInputDevice.Close();
                  _synthInputDevice.Dispose();
              };

            for (int iDevice = 0; iDevice < InputDevice.DeviceCount; iDevice++)
            {
                MidiInCaps capabilities = InputDevice.GetDeviceCapabilities(iDevice);
                if (deviceName.CompareTo(capabilities.name) == 0)
                {
                    if (VerifySynthInputDevice())
                    {
                        deactivatSynthInputDevice();
                    }
                    _synthInputDevice = new InputDevice(iDevice);
                    _synthInputDevice.ChannelMessageReceived += SynthInputDeviceChannelMessageReceived;
                    _synthInputDevice.SysCommonMessageReceived += SynthInputDeviceSysCommonMessageReceived;
                    _synthInputDevice.SysExMessageReceived += SynthInputDeviceSysExMessageReceived;
                    _synthInputDevice.SysRealtimeMessageReceived += SynthInputDeviceSysRealtimeMessageReceived;
                    _synthInputDevice.Error += new EventHandler<Sanford.Multimedia.ErrorEventArgs>(SynthInputDeviceError);
                    _synthInputDevice.StartRecording();
                    return true;
                }
            }
            // device not found
            if (VerifySynthInputDevice())
            {
                deactivatSynthInputDevice();
            }
            _synthInputDevice = null;
            return false;
        }

        #endregion SYNTH_INPUT

        public virtual bool SetSynthOutputDevice(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName)) return false;

            for (int iDevice = 0; iDevice < OutputDevice.DeviceCount; iDevice++)
            {
                MidiOutCaps capabilities = OutputDevice.GetDeviceCapabilities(iDevice);
                if (deviceName.CompareTo(capabilities.name) == 0)
                {
                    if (VerifySynthOutputDevice())
                    {
                        _synthOutputDevice.Close();
                        _synthOutputDevice.Dispose();
                    }
                    _synthOutputDevice = new OutputDevice(iDevice);
                    return true;
                }
            }
            //device not found
            if (VerifySynthOutputDevice())
            {
                _synthOutputDevice.Close();
                _synthOutputDevice.Dispose();
            }
            _synthOutputDevice = null;
            return false;
        }

        /// <summary>
        /// Close all the opened MIDI Devices. to call when the application ends
        /// </summary>
        public virtual void CloseMidiDevices()
        {
            if (_synthOutputDevice != null)
            {
                _synthOutputDevice.Close();
                _synthOutputDevice.Dispose();
                _synthOutputDevice = null;
            }
            if (_synthInputDevice != null)
            {
                _synthInputDevice.StopRecording();
                // unregister the handlers
                _synthInputDevice.ChannelMessageReceived -= SynthInputDeviceChannelMessageReceived;
                _synthInputDevice.SysCommonMessageReceived -= SynthInputDeviceSysCommonMessageReceived;
                _synthInputDevice.SysExMessageReceived -= SynthInputDeviceSysExMessageReceived;
                _synthInputDevice.SysRealtimeMessageReceived -= SynthInputDeviceSysRealtimeMessageReceived;
                _synthInputDevice.Error -= new EventHandler<Sanford.Multimedia.ErrorEventArgs>(SynthInputDeviceError);

                _synthInputDevice.Close();
                _synthInputDevice.Dispose();
                _synthInputDevice = null;
            }
            if (_automationInputDevice != null)
            {
                _automationInputDevice.StopRecording();
                // unregister the handlers
                _automationInputDevice.ChannelMessageReceived -= AutomationInputDeviceChannelMessageReceived;
                _automationInputDevice.SysCommonMessageReceived -= AutomationInputDeviceSysCommonMessageReceived;
                _automationInputDevice.SysExMessageReceived -= AutomationInputDeviceSysExMessageReceived;
                _automationInputDevice.SysRealtimeMessageReceived -= AutomationInputDeviceSysRealtimeMessageReceived;
                _automationInputDevice.Error -= new EventHandler<Sanford.Multimedia.ErrorEventArgs>(AutomationInputDeviceError);

                _automationInputDevice.Close();
                _automationInputDevice.Dispose();
                _automationInputDevice = null;
            }
        }

        /// <summary>
        /// set the Sysex Transmit delay (delay between each sysex message sent to synth)
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public virtual int ParameterTransmitDelay
        {
            get
            {
                return _parameterTransmitDelay;
            }
            set
            {
                if (value < 0)
                {
                    throw new OverflowException("delay can not be <0");
                }
                _parameterTransmitDelay = value;
            }
        }

        /// <summary>
        /// play a note with the synth
        /// </summary>
        /// <param name="e"></param>
        public virtual void PlayNote(bool isNoteOn, PianoKeyEventArgs e)
        {
            if (VerifySynthOutputDevice())
            {
                ChannelCommand command;
                int Velocity;
                if (isNoteOn) { command = ChannelCommand.NoteOn; Velocity = 127; } else { command = ChannelCommand.NoteOff; Velocity = 0; }
                SynchronizationContext.Current.Post(delegate (object dummy)
                {
                    // redirect received message to synth
                    _synthOutputDevice.Send(new ChannelMessage(command, Tone.MIDIChannel, e.NoteID, Velocity));
                }, null);
            }
        }
    }
}