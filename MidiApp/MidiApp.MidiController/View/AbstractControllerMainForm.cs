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
using MidiApp.MidiController.Model;
using MidiApp.MidiController.Service;
using MidiApp.UIControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace MidiApp.MidiController.View
{
    public partial class AbstractControllerMainForm : Form
    {
        /// <summary>
        /// the <b>filename</b> of the currently edited tone sysex
        /// </summary>

        private string _toneFileName = string.Empty;

        protected virtual string ToneFileName
        {
            get { return _toneFileName; }
            set
            {
                _toneFileName = value;
            }
        }

        /// <summary>
        /// get() has to be implemented by inheriter (singleton). set() not needed
        /// </summary>
        protected virtual AbstractController Controller
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// CreateParams override (double buffering)
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                if (!ExecutionModeService.IsDesignModeActive)
                {
                    const int WS_ES_COMPOSITED = 0x02000000;
                    // Activate double buffering at the form level.  All child controls will be double buffered as well.
                    createParams.ExStyle |= WS_ES_COMPOSITED;   // WS_EX_COMPOSITED
                }
                return createParams;
            }
        }

        /// <summary>
        /// internal map to retrieve controls by parameter name
        /// updated by RegisterForControlerEvents() and UnregisterForControlerEvents()
        /// </summary>
        private Dictionary<string, Control> _RegisteredControlsMap = new Dictionary<string, Control>();

        protected virtual Dictionary<string, Control> RegisteredControlsMap
        {
            get { return _RegisteredControlsMap; }
        }

        /// <summary>
        /// register for controller events (internal parameter changes)
        ///
        /// </summary>
        protected virtual void RegisterForControllerEvents()
        {
            RecursiveRegisterValuedUserControl(this);
            Controller.AutomationParameterChangeEvent += OnAutomationParameterChange;
        }

        /// <summary>
        /// helper for RegisterForControlerEvents
        /// </summary>
        /// <param name="control"></param>
        protected virtual void RecursiveRegisterValuedUserControl(Control control)
        {
            IValuedControl current = control as IValuedControl;
            if (current != null)
            {
                string sParameterName = (string)control.Tag;

                AbstractParameter param = Controller.GetParameter(sParameterName);
                if (param == null)
                {
                    // it's a multi page parameter fader, so try with first of it
                    sParameterName = ((string)control.Tag).Replace("_X_", "_1_");
                    param = Controller.GetParameter(sParameterName);
                }
                if (param != null)
                {
                    // set min, max, value and step to
                    current.Minimum = param.MinValue;
                    current.Maximum = param.MaxValue;
                    current.Step = param.Step;
                    current.Value = param.Value;

                    RegisteredControlsMap.Add((string)control.Tag, control);
                    Logger.WriteLine(this, TraceLevel.Verbose, String.Format("Registering control [name:{0}, tag:{1}] for parameter {2}", control.Name, control.Tag, sParameterName));
                }
                else
                {
                    Logger.WriteLine(this, TraceLevel.Warning, String.Format("Could not find parameter [{0}] for registered control", sParameterName));
                }
            }

            foreach (Control subControl in control.Controls)
            {
                RecursiveRegisterValuedUserControl(subControl);
            }
        }

        /// <summary>
        /// Unregister for controller events
        /// </summary>
        protected virtual void UnRegisterForControllerEvents()
        {
            Controller.AutomationParameterChangeEvent -= this.OnAutomationParameterChange;
            RegisteredControlsMap.Clear();
        }
    }
}