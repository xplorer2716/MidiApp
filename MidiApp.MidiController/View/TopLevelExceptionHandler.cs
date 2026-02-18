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

using MidiApp.MidiController.Service;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MidiApp.MidiController.View
{
    public static class TopLevelExceptionHandler
    {
        /// <summary>
        /// Exception handler for NON UI Thread exception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void NonUIThreadExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // Houston, we've got a serious problem here
            Exception exception = (Exception)e.ExceptionObject;

            if (exception != null)
            {
                try
                {
                    Logger.WriteLine(TraceLevel.Error, BugReportFactory.CreateDetailsFromException(exception));
                    TopLevelExceptionForm form = new TopLevelExceptionForm(
                        exception,
                        "The application will close immediatly");

                    form.ShowDialog();
                }
                catch
                {
                    MessageBox.Show("An Error occured while handling the error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // we need to call a cleanup code, since mainform's OnClosing will not be called
                    Logger.Flush();

                    foreach (Form form in Application.OpenForms)
                    {
                        if (form != null && !form.IsDisposed && form.IsHandleCreated)
                        {
                            form.Close();
                            form.Dispose();
                        }
                    }
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Exception Handler for UI thread exception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void UIThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // Houston, we've got a (perhaps not too) serious problem here
            Exception exception = e.Exception;

            if (exception != null)
            {
                try
                {
                    Logger.WriteLine(TraceLevel.Error, BugReportFactory.CreateDetailsFromException(exception));
                    TopLevelExceptionForm form = new TopLevelExceptionForm(
                        exception, "Please save your work and close the application.");
                    form.ShowDialog();
                }
                catch
                {
                    MessageBox.Show("An Error occured while handling the error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // if the user closes the app, cleanup will be already done, because mainform's OnClosing will be called
                    Logger.Flush();
                }
            }
        }
    }
}