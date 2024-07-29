#region License

/* Copyright (c) 2005 Leslie Sanford
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

#endregion License

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion Contact

using System;
using System.Collections;

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    /// Defines constants representing various system exclusive message types.
    /// </summary>
    public enum SysExType
    {
        /// <summary>
        /// Represents the start of system exclusive message type.
        /// </summary>
        Start = 0xF0,

        /// <summary>
        /// Represents the continuation of a system exclusive message.
        /// </summary>
        Continuation = 0xF7
    }

    /// <summary>
    /// Represents MIDI system exclusive messages.
    /// </summary>
    public sealed class SysExMessage : IMidiMessage, IEnumerable, ICloneable
    {
        #region SysExEventMessage Members

        /// <summary>
        /// lock object for the [] operator (set/get)
        /// </summary>
        private object _LockObject = new object();

        #region Constants

        /// <summary>
        /// Maximum value for system exclusive channels.
        /// </summary>
        public const int SysExChannelMaxValue = 127;

        #endregion Constants

        #region Fields

        // The system exclusive data.
        private byte[] data;

        #endregion Fields

        #region Construction

        /// <summary>
        /// Initializes a new instance of the SysExMessageEventArgs class with the
        /// specified system exclusive data.
        /// </summary>
        /// <param name="data">
        /// The system exclusive data.
        /// </param>
        /// <remarks>
        /// The system exclusive data's status byte, the first byte in the
        /// data, must have a value of 0xF0 or 0xF7.
        /// </remarks>
        public SysExMessage(byte[] data)
        {
            #region Require

            if (data.Length < 1)
            {
                throw new ArgumentException(
                    "System exclusive data is too short.", "data");
            }
            else if (data[0] != (byte)SysExType.Start &&
                data[0] != (byte)SysExType.Continuation)
            {
                throw new ArgumentException(
                    "Unknown status value.", "data");
            }

            #endregion Require

            this.data = new byte[data.Length];
            data.CopyTo(this.data, 0);
        }

        #endregion Construction

        #region Methods

        public byte[] GetBytes()
        {
            byte[] clone = new byte[data.Length];

            data.CopyTo(clone, 0);

            return clone;
        }

        public void CopyTo(byte[] buffer, int index)
        {
            data.CopyTo(buffer, index);
        }

        public override bool Equals(object obj)
        {
            #region Guard

            if (!(obj is SysExMessage))
            {
                return false;
            }

            #endregion Guard

            SysExMessage message = (SysExMessage)obj;

            bool equals = true;

            if (this.Length != message.Length)
            {
                equals = false;
            }

            for (int i = 0; i < this.Length && equals; i++)
            {
                if (this[i] != message[i])
                {
                    equals = false;
                }
            }

            return equals;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If index is less than zero or greater than or equal to the length
        /// of the message.
        /// </exception>
        public byte this[int index]
        {
            get
            {
                lock (_LockObject)
                {
                    #region Require

                    if (index < 0 || index >= Length)
                    {
                        throw new ArgumentOutOfRangeException("index", index,
                            "Index into system exclusive message out of range.");
                    }

                    #endregion Require

                    return data[index];
                }
            }
            set
            {
                lock (_LockObject)
                {
                    #region Require

                    if (index < 0 || index >= Length)
                    {
                        throw new ArgumentOutOfRangeException("index", index,
                            "Index into system exclusive message out of range.");
                    }

                    #endregion Require

                    data[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets the length of the system exclusive data.
        /// </summary>
        public int Length
        {
            get
            {
                return data.Length;
            }
        }

        /// <summary>
        /// Gets the system exclusive type.
        /// </summary>
        public SysExType SysExType
        {
            get
            {
                return (SysExType)this[0];
            }
        }

        #endregion Properties

        #endregion SysExEventMessage Members

        /// <summary>
        /// Gets the status value.
        /// </summary>
        public int Status
        {
            get
            {
                return (int)this[0];
            }
        }

        /// <summary>
        /// Gets the MessageType.
        /// </summary>
        public MessageType MessageType
        {
            get
            {
                return MessageType.SystemExclusive;
            }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(data);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents a raw sysex byte array
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public static string ToString(byte[] rawData)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            for (int i = 0; i < rawData.Length; i++)
            {
                if ((i % 16) == 0) { builder.Append(Environment.NewLine); }
                string comma;
                if (i < rawData.Length - 1) { comma = ","; } else { comma = string.Empty; }
                builder.AppendFormat("{0:X2}{1}", rawData[i], comma);
            }
            return builder.ToString();
        }

        #endregion IEnumerable Members

        #region ICloneable Membres

        public object Clone()
        {
            return new SysExMessage(this.data);
        }

        #endregion ICloneable Membres
    }
}