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
using System.Collections.Generic;

namespace MidiApp.MidiController.Model
{
    /// <summary>
    /// interface for tone reader
    /// </summary>
    public interface IToneReader
    {
        /// <summary>
        /// read a tone from a file. implementer has to throw ToneException if required.
        /// </summary>
        /// <param name="filename">filename</param>
        /// <param name="tone">(int/out) tone object</param>
        void ReadTone(string filename, AbstractTone tone);

        /// <summary>
        /// Reads tones from a file containing several tones
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <returns></returns>
        ICollection<Tuple<string, AbstractTone>> ReadTones(string filename);
    }
}