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
using System.Reflection;

namespace MidiApp.MidiController.Service
{
    /// <summary>
    /// Give some formatted information about an assembly.
    /// </summary>
    public static class AssemblyService
    {
        /// <summary>
        /// returns the name of the products
        /// </summary>
        /// <returns></returns>
        public static string GetProductName()
        {
            object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            return attributes.Length > 0 ? ((AssemblyProductAttribute)attributes[0]).Product : "PRODUCT_NAME";
        }

        /// <summary>
        /// Returns the software name and version
        /// </summary>
        /// <returns></returns>
        public static string GetProductNameAndVersionAsString()
        {
            // build the message
            return string.Format(@"{0} Version {1}", GetProductName(), GetVersionAsString());
        }

        /// <summary>
        /// Returns the software version as a string
        /// </summary>
        /// <returns></returns>
        public static string GetVersionAsString()
        {
            AssemblyName name = Assembly.GetEntryAssembly().GetName();
            return string.Format("{0}.{1}.{2}.{3}", name.Version.Major, name.Version.Minor, name.Version.Build, name.Version.Revision);
        }
    }
}