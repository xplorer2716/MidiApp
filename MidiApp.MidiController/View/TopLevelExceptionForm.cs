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

using MidiApp.MidiController.Service;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MidiApp.MidiController.View
{
    /// <summary>
    /// Top level exception handling form
    /// </summary>
    public partial class TopLevelExceptionForm : Form
    {
        // the exception
        private Exception _exception;

        private string _additionalMessage;

        //
        /// <summary>
        /// Only for design mode. Initializes a new instance of the <see cref="TopLevelExceptionForm"/> class.
        /// </summary>
        public TopLevelExceptionForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="exception">the exception</param>
        /// <param name="supportAddress">The support address (website, email...)</param>
        /// <param name="additionalMessage">an additional bugDescription to display (null/empty if not)</param>
        /// <param name="?">The ?.</param>
        public TopLevelExceptionForm(Exception exception,

            string additionalMessage
            )
        {
            InitializeComponent();

            Debug.Assert(exception != null);
            _exception = exception;
            _additionalMessage = additionalMessage;
        }

        /// <summary>
        /// Handles the Click event of the _btClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// OnLoad <see cref="E:System.Windows.Forms.Form.Load"/>.
        /// </summary>
        /// <param name="e"><see cref="T:System.EventArgs"/>arg.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Bitmap errorBitmap = null;
            errorBitmap = SystemIcons.Error.ToBitmap();
            try
            {
                errorBitmap.MakeTransparent(Color.Black);
            }
            catch { }

            this._pictureBox.Image = errorBitmap;

            string errorUniqueId = null;
            string report = BugReportFactory.CreateBugReport(_exception, _additionalMessage, ref errorUniqueId);

            // Description
            this._labelErrorMessage.Text = BugReportFactory.CreateMessage(_exception, _additionalMessage, ref errorUniqueId);
            this._textBoxDetails.Text = report;
        }
    }
}