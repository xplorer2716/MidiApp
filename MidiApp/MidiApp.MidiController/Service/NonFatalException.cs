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
using System.Runtime.Serialization;

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

        // This constructor is needed for serialization.
        /// <summary>
        /// Initializes a new instance of the <see cref="NonFatalException"/> class.
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo"/> qui contient les données d'objet sérialisées sur l'exception levée.</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext"/> qui contient des informations contextuelles sur la source ou la destination.</param>
        /// <exception cref="T:System.ArgumentNullException">Le paramètre <paramref name="info"/> a la valeur null. </exception>
        ///
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">Le nom de la classe est null ou <see cref="P:System.Exception.HResult"/> est zéro (0). </exception>
        protected NonFatalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}