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
using System.Runtime.Serialization;

namespace MidiApp.MidiController.Model
{
    /// <summary>
    /// Exception relation to tone read/write
    /// </summary>
    public class ToneException : NonFatalException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonFatalException"/> class.
        /// </summary>
        public ToneException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonFatalException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ToneException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonFatalException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ToneException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // This constructor is needed for serialization.
        /// <summary>
        /// Initializes a new instance of the <see cref="NonFatalException"/> class.
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo"/> qui contient les donn�es d'objet s�rialis�es sur l'exception lev�e.</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext"/> qui contient des informations contextuelles sur la source ou la destination.</param>
        /// <exception cref="T:System.ArgumentNullException">Le param�tre <paramref name="info"/> a la valeur null. </exception>
        ///
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">Le nom de la classe est null ou <see cref="P:System.Exception.HResult"/> est z�ro�(0). </exception>
        protected ToneException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}