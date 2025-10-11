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

using MidiApp.MidiController.Controller;
using MidiApp.MidiController.Service;
using MidiApp.UIControls;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MidiApp.MidiController.View
{
    /// <summary>
    /// The main application form
    /// </summary>
    public partial class AbstractControllerMainForm : Form
    {
        #region BUTTONS_HANDLERS

        /// <summary>
        /// a generic scroll event handler for all ValuedUserControl control
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void HandleControlValueChanged(string sParameterName, int Value)
        {
            try
            {
                Controller.SetParameter(sParameterName, Value);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(this, TraceLevel.Warning, BugReportFactory.CreateDetailsFromException(ex));
            }
        }

        /// <summary>
        /// To call an a MouseDown event on any ValuedUserControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HandleControlMouseDown(object sender, MouseEventArgs e)
        {
            Control current = (Control)sender;
            if (current != null)
            {
                // temporarily disable the automation input for this parameter
                int ControlChangeNumber = Controller.ControlChangeAutomationTable[(string)current.Tag];
                Controller.DisabledControlChangeNumber = ControlChangeNumber;
            }
        }

        /// <summary>
        /// To call an a MouseUp event on any ValuedUserControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HandleControlMouseUp(object sender, MouseEventArgs e)
        {
            //"reset" the disabled automation input
            Controller.DisabledControlChangeNumber = (int)ControlChangeType.None;
        }

        #endregion BUTTONS_HANDLERS

        /// <summary>
        /// This method is called whenever the form is closing
        /// Default implementation does: Controller.Stop, Controller.CloseMidiDevices,UnRegisterForControlerEvents
        /// </summary>
        protected virtual void DoCleanupBeforeClosing()
        {
            Controller.Stop();
            Controller.CloseMidiDevices();
            UnRegisterForControllerEvents();
        }

        /// <summary>
        /// OnClosing. If overriden, call base class AFTER your code <see cref="E:System.Windows.Forms.Form.Closing"/>.
        /// </summary>
        /// <param name="e"><see cref="T:System.ComponentModel.CancelEventArgs"/> args</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            DoCleanupBeforeClosing();
            base.OnClosing(e);
        }

        #region CONTROLLER_EVENTS

        /// <summary>
        /// Event handler for controller internal parameter changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        protected virtual void OnAutomationParameterChange(object sender, AbstractController.ParameterChangeEventArgs eventArg)
        {
            // retrieve the registered control from parameter name
            Control control = null;
            if (RegisteredControlsMap.TryGetValue(eventArg.ParameterName, out control))
            {
                IValuedControl current = control as IValuedControl;
                if (current != null)
                {
                    current.Value = eventArg.Value;
                    Logger.WriteLine(this, TraceLevel.Info,
                                        String.Format("OnAutomationParameterChange:  parameter:{0}, value:{1}", eventArg.ParameterName, eventArg.Value));
                }
            }
            else
            {
                Logger.WriteLine(this, TraceLevel.Warning,
                    String.Format("OnAutomationParameterChange: could not find control for parameter {0}", eventArg.ParameterName));
            }
        }

        #endregion CONTROLLER_EVENTS
    }
}