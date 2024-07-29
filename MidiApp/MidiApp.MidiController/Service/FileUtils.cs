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
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace MidiApp.MidiController.Service
{
    /// <summary>
    /// Some file utility functions not available in the .NET framework
    /// </summary>
    public static class FileUtils
    {
        public const string SYSEX_FILE_EXTENSION = "syx";
        public const string SYSEX_FILE_EXTENSION_WITH_DOT = ".syx";
        public const string SYSEX_FILE_FILTER = "sysex files (*.syx)|*.syx|All files (*.*)|*.*";

        /// <summary>
        /// Make a real usable filename from an arbitrary string for a given directory
        /// Invalid chars are replaced by "_"
        /// If file already exist, number is added until the name is unique
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="directoryName">Name of the directory.</param>
        /// <returns></returns>
        public static string MakeUniqueFilenameFromString(string name, string extension, string directoryName)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + ":.)&");
            string invalidReStr = string.Format(@"[{0}]+", invalidChars);
            string sanitisized = Regex.Replace(name, invalidReStr, string.Empty);
            string originalSanitisized = sanitisized;
            int number = 1;
            while (File.Exists(Path.Combine(directoryName, sanitisized + extension)))
            {
                sanitisized = originalSanitisized + number.ToString(CultureInfo.InvariantCulture);
                number++;
            }
            return sanitisized + extension;
        }
    }
}