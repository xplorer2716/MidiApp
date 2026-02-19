using Sanford.Multimedia;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MidiApp.MidiController.Extensions
{
    public static class MidiDeviceExceptionExtension
    {
        // Use a Dictionary to map error codes to messages
        private static readonly Dictionary<int, string> _ErrorMessages = new();

        static MidiDeviceExceptionExtension()
        {
            // Initialize the error table
            // These error codes are taken from DeviceException and MidiDeviceException metadata
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
            _ErrorMessages.Add(20, "MMSYSERR_NODRIVERCB/MMSYSERR_LASTERROR");

            _ErrorMessages.Add(MidiDeviceException.MIDIERR_UNPREPARED, "MIDIERR_UNPREPARED, header not prepared");
            _ErrorMessages.Add(MidiDeviceException.MIDIERR_STILLPLAYING, "MIDIERR_STILLPLAYING, still something playing");
            _ErrorMessages.Add(MidiDeviceException.MIDIERR_NOMAP, "MIDIERR_NOMAP, no configured instruments");
            _ErrorMessages.Add(MidiDeviceException.MIDIERR_NOTREADY, "MIDIERR_NOTREADY, hardware is still busy");
            _ErrorMessages.Add(MidiDeviceException.MIDIERR_NODEVICE, "MIDIERR_NODEVICE, port no longer connected");
            _ErrorMessages.Add(MidiDeviceException.MIDIERR_INVALIDSETUP, "MIDIERR_INVALIDSETUP, invalid MIF");
            _ErrorMessages.Add(MidiDeviceException.MIDIERR_BADOPENMODE, "MIDIERR_BADOPENMODE, operation unsupported with open mode");
            _ErrorMessages.Add(MidiDeviceException.MIDIERR_DONT_CONTINUE, "MIDIERR_DONT_CONTINUE, thru device eating a message");
        }

        public static string GetErrorMessageForErrorCode(this DeviceException exception, int errorCode)
        {
            return _ErrorMessages.TryGetValue(errorCode, out var message)
                ? message
                : "Unknown error code.";
        }
    }
}
