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
using Sanford.Multimedia;
using Sanford.Multimedia.Midi;
using System;
using System.Collections;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MidiApp.MidiController.Service
{
    /// <summary>
    /// A bug report factory
    /// </summary>
    public static class BugReportFactory
    {
        private static string _line = string.Empty.PadLeft(40, '-');

        /// <summary>
        /// Creates the bug report.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="additionalMessage">The additional bugDescription.</param>
        /// <returns></returns>
        public static string CreateBugReport(Exception exception, string additionalMessage, ref string errorUniqueId)
        {
            StringBuilder report = new StringBuilder();

            report.AppendLine(CreateMessage(exception, additionalMessage, ref errorUniqueId));
            report.AppendLine(_line);
            report.AppendLine("DETAILS:");
            report.AppendLine(CreateDetailsFromException(exception));
            report.AppendLine(_line);
            report.AppendLine("ENVIRONMENT:");
            report.AppendLine(CreateEnvironmentInfo());
            report.AppendLine(_line);
            report.AppendLine("CONFIG:");
            report.AppendLine(CreateUserConfigInfo());
            report.AppendLine(_line);
            report.AppendLine("MIDI:");
            report.AppendLine(CreateMidiDevicesInfo());

            return report.ToString();
        }

        /// <summary>
        /// try to get the user.config file content
        /// this will not work in a clickonce deployement
        /// </summary>
        /// <returns></returns>
        private static string CreateUserConfigInfo()
        {
            string content = string.Empty;
            Configuration config;
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                string path = config.FilePath;
                if (File.Exists(path))
                {
                    content = File.ReadAllText(path);
                }
                else
                {
                    content = "User configuration file does not exists yet.";
                }
            }
            catch
            {
                //no op
            }
            return content;
        }

        /// <summary>
        /// Creates screen informations
        /// </summary>
        /// <param name="control">The mainform control, or null</param>
        /// <returns></returns>
        public static string CreateScreensInfo(Control control)
        {
            StringBuilder sb = new StringBuilder();

            Screen mainformScreen = (control != null ? Screen.FromControl(control) : null);
            Screen[] screens = Screen.AllScreens;
            foreach (Screen screen in screens)
            {
                // device name can have bullshit in it that can even not be displayed as Unicode string.
                // do a cleanup to avoid EncoderFallbackException when writing to log file
                string deviceName = Regex.Replace(screen.DeviceName, @"[^A-Za-z0-9_]", string.Empty);

                sb.AppendFormat(CultureInfo.InvariantCulture,
                    "\r\nScreen: {0}\r\n  Primary:{1}\r\n  BitsPerPixel: {2}\r\n  Bounds: {3}\r\n  WorkingArea: {4}\r\n",
                    deviceName + (screen.Equals(mainformScreen) ? " (showing app)" : string.Empty),
                    screen.Primary,
                    screen.BitsPerPixel,
                    screen.Bounds,
                    screen.WorkingArea);
            }
            if (control != null)
            {
                using (Graphics g = control.CreateGraphics())
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "Screen resolution: DpiX: {0}, DpiY: {1}", g.DpiX, g.DpiY);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Create environment informations
        /// </summary>
        /// <returns></returns>
        private static string CreateEnvironmentInfo()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                //System
                sb.AppendFormat("Environment.OSVersion: {0}\r\n", Environment.OSVersion.ToString());
                sb.AppendFormat("Environment.ProcessorCount: {0}\r\n", Environment.ProcessorCount);

                sb.AppendFormat("SystemInfo.PrimaryMonitorSize: {0}\r\n", SystemInformation.PrimaryMonitorSize.ToString());
                sb.AppendFormat("SystemInfo.MonitorCount: {0}\r\n", SystemInformation.MonitorCount.ToString());
                sb.AppendFormat(CreateScreensInfo(null));
                sb.AppendFormat("SystemInfo.UIEffectsEnabled: {0}\r\n", SystemInformation.UIEffectsEnabled.ToString());
                sb.AppendFormat("SystemInfo.Network: {0}\r\n", SystemInformation.Network);

                // CLR
                sb.AppendFormat("Environment.Version(CLR): {0}\r\n", Environment.Version.ToString());

                // Environment variables
                sb.AppendLine("Environment.GetEnvironmentVariables");
                foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
                {
                    sb.AppendFormat("{0}:{1}\r\n", entry.Key, entry.Value);
                }

                // Executing Application
                Assembly assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    sb.AppendFormat("Assembly.FullName: {0}\r\n", assembly.FullName);
                    sb.AppendFormat("Assembly.Codebase: {0}\r\n", assembly.CodeBase);
                }
                try
                {
                    sb.AppendFormat("Configuration.FilePath: {0}\r\n", ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);
                }
                catch
                {
                }
            }
            catch
            {
            }

            return sb.ToString();
        }

        /// <summary>
        /// create a list off all the midi devices
        /// </summary>
        /// <returns></returns>
        public static string CreateMidiDevicesInfo()
        {
            StringBuilder sb = new StringBuilder("Input Midi devices:\r\n");

            try
            {
                for (int i = 0; i < InputDevice.DeviceCount; i++)
                {
                    MidiInCaps inCaps = InputDevice.GetDeviceCapabilities(i);
                    ushort version = Convert.ToUInt16(inCaps.driverVersion);
                    sb.AppendFormat("{0} (v{1}.{2}) \r\n", inCaps.name, (version & 0xFF00) >> 8, (version & 0x00FF));
                }
                sb.AppendLine("\r\nOutput Midi devices:");
                for (int i = 0; i < OutputDevice.DeviceCount; i++)
                {
                    MidiOutCaps outCaps = OutputDevice.GetDeviceCapabilities(i);
                    ushort version = Convert.ToUInt16(outCaps.driverVersion);
                    sb.AppendFormat("{0} (v{1}.{2}) \r\n", outCaps.name, (version & 0xFF00) >> 8, (version & 0x00FF));
                }
            }
            catch
            {
            }
            finally
            {
            }
            return sb.ToString();
        }

        /// <summary>
        /// Creates the details part out of an Exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static string CreateDetailsFromException(Exception exception)
        {
            // details: stacktrace, base exception (if defined), base exception stack trace
            string details = string.Format("{0}:{1}\r\n{2}", exception.GetType().FullName, exception.Message, exception.StackTrace);

            try
            {
                Exception baseException = exception.GetBaseException();
                if (baseException != null)
                {
                    string midiDetails = string.Empty;
                    DeviceException deviceException = baseException as DeviceException;
                    if (deviceException != null)
                    {
                        // retrieve midi details if base exception is a MIDI Device related one
                        midiDetails = string.Format("MIDI error code is: {0} - {1}",
                            deviceException.ErrorCode,
                            MidiDeviceException.GetErrorMessageForErrorCode(deviceException.ErrorCode).ToUpper());
                    }
                    details = string.Format("{0}\r\n**** Exception.GetBaseException: {1}:{2}\r\n{3}{4}",
                        details,
                        baseException.GetType().FullName,
                        baseException.Message,
                        (!string.IsNullOrEmpty(midiDetails)) ? midiDetails + "\r\n" : midiDetails,
                        baseException.StackTrace);
                }
            }
            catch
            {
                // don't throw exception when getting exception details
            }
            return details;
        }

        /// <summary>
        /// Creates a detailled message about the exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="additionalMessage">The additional bugDescription.</param>
        /// <param name="errorUniqueId">(out) The error unique identifier.</param>
        /// <returns></returns>
        internal static string CreateMessage(Exception exception, string additionalMessage, ref string errorUniqueId)
        {
            if (errorUniqueId == null)
            {
                errorUniqueId = GetUniqueIdForException(exception);
            }
            StringBuilder builder = new StringBuilder(string.Format("An unhandled error occured. The error was: {0}.{2}Id={1}{2}", exception.Message, errorUniqueId, Environment.NewLine));

            if (additionalMessage != null && additionalMessage.Length != 0)
            {
                builder.AppendFormat("{0}{1}", additionalMessage, Environment.NewLine);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Generates a "unique" id from an Exception
        /// We generates an MD5 key from the stacktrace, so we can identify the same exception location for multiple instances of the same software release.
        /// Far better than generating a guid.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static string GetUniqueIdForException(Exception e)
        {
            string uniqueId = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(e.StackTrace));

                // Create a new Stringbuilder to collect the bytes and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed dataand format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                // return a guid-like formating
                uniqueId = sBuilder.ToString().ToUpperInvariant();
                uniqueId = Regex.Replace(uniqueId, @"(\w{8})(\w{8})(\w{8})(\w{8})", "$1-$2-$3-$4");
            }
            return uniqueId;
        }
    }
}