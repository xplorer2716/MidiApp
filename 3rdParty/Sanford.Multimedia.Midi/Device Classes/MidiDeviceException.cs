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

using System.Collections.Generic;

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    /// The base class for all MIDI device exception classes.
    /// </summary>
    public class MidiDeviceException : DeviceException
    {
        #region Error Codes

        public const int MIDIERR_UNPREPARED = 64; /* header not prepared */
        public const int MIDIERR_STILLPLAYING = 65; /* still something playing */
        public const int MIDIERR_NOMAP = 66; /* no configured instruments */
        public const int MIDIERR_NOTREADY = 67; /* hardware is still busy */
        public const int MIDIERR_NODEVICE = 68; /* port no longer connected */
        public const int MIDIERR_INVALIDSETUP = 69; /* invalid MIF */
        public const int MIDIERR_BADOPENMODE = 70; /* operation unsupported w/ open mode */
        public const int MIDIERR_DONT_CONTINUE = 71; /* thru device 'eating' a message */
        public const int MIDIERR_LASTERROR = 71; /* last error in range */

        private static Dictionary<int, string> _ErrorMessages = new Dictionary<int, string>();

        /// <summary>
        /// helper function to get an error message from error code. handles the DeviceException errors too.
        /// </summary>
        /// <param name="ErrorCode"></param>
        /// <returns>null if not defined</returns>
        public static string GetErrorMessageForErrorCode(int ErrorCode)
        {
            string message = "Undefined error";
            _ErrorMessages.TryGetValue(ErrorCode, out message);
            return message;
        }

        #endregion Error Codes

        #region Construction

        static MidiDeviceException()
        {
            // init the error table
            // these error codes are taken from DeviceException metadata
            _ErrorMessages.Add(0, "MMSYSERR_NOERROR");
            _ErrorMessages.Add(1, "MMSYSERR_ERROR");
            _ErrorMessages.Add(2, "MMSYSERR_BADDEVICEID");
            _ErrorMessages.Add(3, "MMSYSERR_NOTENABLED");
            _ErrorMessages.Add(4, "MMSYSERR_ALLOCATED");
            _ErrorMessages.Add(5, "MMSYSERR_INVALHANDLE");
            _ErrorMessages.Add(6, "MMSYSERR_NODRIVER");
            _ErrorMessages.Add(7, "MMSYSERR_NOMEM");
            _ErrorMessages.Add(8, "MMSYSERR_NOTSUPPORTED");
            _ErrorMessages.Add(9, "MMSYSERR_BADERRNUM");

            _ErrorMessages.Add(10, "MMSYSERR_INVALFLAG");
            _ErrorMessages.Add(11, "MMSYSERR_INVALPARAM");
            _ErrorMessages.Add(12, "MMSYSERR_HANDLEBUSY");
            _ErrorMessages.Add(13, "MMSYSERR_INVALIDALIAS");
            _ErrorMessages.Add(14, "MMSYSERR_BADDB");
            _ErrorMessages.Add(15, "MMSYSERR_KEYNOTFOUND");
            _ErrorMessages.Add(16, "MMSYSERR_READERROR");
            _ErrorMessages.Add(17, "MMSYSERR_WRITEERROR");
            _ErrorMessages.Add(18, "MMSYSERR_DELETEERROR");
            _ErrorMessages.Add(19, "MMSYSERR_VALNOTFOUND");
            _ErrorMessages.Add(20, "MMSYSERR_NODRIVERCB");
            //_ErrorMessages.Add(20, "MMSYSERR_LASTERROR");

            _ErrorMessages.Add(MIDIERR_UNPREPARED, "MIDIERR_UNPREPARED,header not prepared");
            _ErrorMessages.Add(MIDIERR_STILLPLAYING, "MIDIERR_STILLPLAYING,still something playing");
            _ErrorMessages.Add(MIDIERR_NOMAP, "MIDIERR_NOMAP,no configured instruments");
            _ErrorMessages.Add(MIDIERR_NOTREADY, "MIDIERR_NOTREADY,hardware is still busy");
            _ErrorMessages.Add(MIDIERR_NODEVICE, "MIDIERR_NODEVICE,port no longer connected");
            _ErrorMessages.Add(MIDIERR_INVALIDSETUP, "MIDIERR_INVALIDSETUP,invalid MIF");
            _ErrorMessages.Add(MIDIERR_BADOPENMODE, "MIDIERR_BADOPENMODE,operation unsupported w/ open mode");
            _ErrorMessages.Add(MIDIERR_DONT_CONTINUE, "MIDIERR_DONT_CONTINUE,thru device eating a message");
        }

        /// <summary>
        /// Initializes a new instance of the DeviceException class with the
        /// specified error code.
        /// </summary>
        /// <param name="errCode">
        /// The error code.
        /// </param>
        public MidiDeviceException(int errCode)
            : base(errCode)
        {
        }

        #endregion Construction
    }
}