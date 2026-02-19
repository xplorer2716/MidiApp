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
using System.Text;
using System.Web;

namespace MidiApp.MidiController.Service
{
    /// <summary>
    /// Extensions for the String class.
    /// </summary>
    /// <remarks>see http://www.blackbeltcoder.com/Articles/strings/converting-text-to-html</remarks>
    public static class StringExtensions
    {
        private static string _paraBreak = Environment.NewLine + Environment.NewLine;

        /// <summary>
        /// Returns a copy of this string converted to HTML markup.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ToHtml(this string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");

            int position = 0;
            while (position < text.Length)
            {
                // Extract next paragraph
                int start = position;
                position = text.IndexOf(_paraBreak, start);
                if (position < 0)
                    position = text.Length;
                string paragraph = text.Substring(start, position - start).Trim();

                // Encode non-empty paragraph
                if (paragraph.Length > 0)
                    EncodeParagraph(paragraph, sb);

                // Skip over paragraph break
                position += _paraBreak.Length;
            }

            sb.Append("</html>");
            // return result
            return sb.ToString();
        }

        /// <summary>
        /// Encodes a single paragraph to HTML.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="sb">StringBuilder to write results</param>
        private static void EncodeParagraph(string text, StringBuilder sb)
        {
            // Start new paragraph
            sb.AppendLine("<p>");

            // HTML encode text
            text = HttpUtility.HtmlEncode(text);

            // Convert single newlines to <br>
            text = text.Replace(Environment.NewLine, "<br/>\r\n");

            sb.Append(text);
            // Close paragraph
            sb.AppendLine("\r\n</p>");
        }
    }
}