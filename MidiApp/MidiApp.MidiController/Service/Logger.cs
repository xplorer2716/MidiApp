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
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MidiApp.MidiController.Service
{
    /// <summary>
    /// A simple logger
    /// </summary>
    public static class Logger
    {
        // the listener
        private static readonly TextWriterTraceListener _textWriterListener;

        private static readonly FileStream _filestream;
        private const string _logFileName = "applog.txt";
        private static bool _autoFlush = true;

        private static TraceSwitch _traceSwitch;

        private static readonly object _mutex = new object();

        /// <summary>
        /// Flush automatically after write op.
        /// </summary>
        public static bool AutoFlush
        {
            get { return Logger._autoFlush; }
            set { Logger._autoFlush = value; }
        }

        /// <summary>
        /// TraceSwitch
        /// </summary>
        public static TraceSwitch Switch
        {
            get { return _traceSwitch; }
        }

        static Logger()
        {
            try
            {
                lock (_mutex)
                {
                    _traceSwitch = new TraceSwitch(typeof(Logger).FullName, typeof(Logger).FullName);

                    // don't use executable path since we need write access to it, use common app data path instead (Vista, W7)
                    string path = Path.GetDirectoryName(Application.CommonAppDataPath);
                    _filestream = new FileStream(Path.Combine(path, _logFileName), FileMode.Create);
                    _textWriterListener = new TextWriterTraceListener(_filestream);
                    Trace.Listeners.Add(_textWriterListener);
                }
            }
            catch (Exception ex)
            {
                // EPIC FAIL. logger initialisation error will not hang the app
                Debug.Assert(false, "Logger init failed: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        /// <summary>
        /// generates a prefix for log lines
        /// </summary>
        /// <returns></returns>
        private static string Prefix(TraceLevel level)
        {
            DateTime now = DateTime.Now;
            return string.Format("[{0}:{1:D3}] {2}: ", now.ToLongTimeString(), now.Millisecond, level.ToString().Substring(0, 3).ToUpper());
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
        {
            Write(null, message);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        public static void Write(object source, string message)
        {
            lock (_mutex)
            {
                message = source != null ? source.GetType().Name + ": " + message : message;
                Trace.Write(string.Format("{0}{1}", Prefix(TraceLevel.Off), message));
                if (AutoFlush) { Trace.Flush(); }
            }
        }

        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(string message)
        {
            WriteLine(null, message);
        }

        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        public static void WriteLine(object source, string message)
        {
            lock (_mutex)
            {
                message = (source != null) ? (source.GetType().Name + ": " + message) : message;
                Trace.WriteLine(string.Format("{0}{1}", Prefix(TraceLevel.Off), message));
                if (AutoFlush) { Trace.Flush(); }
            }
        }

        /// <summary>
        /// WritLine with condition
        /// </summary>
        /// <param name="condition">condition / trace level</param>
        /// <param name="message"></param>
        public static void WriteLineIf(bool condition, string message)
        {
            WriteLineIf(null, condition, message);
        }

        /// <summary>
        /// WriteLine with condition
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="condition">condition / trace level</param>
        /// <param name="message">The message.</param>
        public static void WriteLineIf(object source, bool condition, string message)
        {
            lock (_mutex)
            {
                message = (source != null) ? (source.GetType().Name + ": " + message) : message;
                Trace.WriteLineIf(condition, string.Format("{0}{1}", Prefix(TraceLevel.Off), message));
                if (AutoFlush) { Trace.Flush(); }
            }
        }

        /// <summary>
        /// WritLine with TraceSwitch
        /// </summary>
        /// <param name="condition"> trace level</param>
        /// <param name="message"></param>
        public static void WriteLine(TraceLevel level, string message)
        {
            WriteLine(null, level, message);
        }

        /// <summary>
        /// WritLine with TraceSwitch
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        public static void WriteLine(object source, TraceLevel level, string message)
        {
            lock (_mutex)
            {
                message = source != null ? source.GetType().Name + ": " + message : message;
                Trace.WriteLineIf(level <= _traceSwitch.Level, string.Format("{0}{1}", Prefix(level), message));
                if (AutoFlush) { Trace.Flush(); }
            }
        }

        /// <summary>
        /// Flush
        /// </summary>
        public static void Flush()
        {
            lock (_mutex)
            {
                Trace.Flush();
            }
        }
    }
}