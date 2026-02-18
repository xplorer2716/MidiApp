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

namespace MidiApp.MidiController.Service
{
    /// <summary>
    /// A non fatal exception, so that catcher can identify what to do with it compared to other
    /// </summary>
    [System.Serializable]
    public class NonFatalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonFatalException"/> class.
        /// </summary>
        public NonFatalException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonFatalException" /> class.
        /// </summary>
        /// <param name="message">Error message</param>
        public NonFatalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonFatalException"/> class.
        /// </summary>
        /// <param name="bugDescription">Error message.</param>
        /// <param name="inner">The inner.</param>
        public NonFatalException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}