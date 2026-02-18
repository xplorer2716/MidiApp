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

using System.Collections.Generic;

namespace MidiApp.MidiController.Controller.Arguments
{
    /// <summary>
    /// Argument for Tone randomization
    /// </summary>
    public class RandomizeToneArgument
    {
        /// <summary>
        /// List of parameters that are not to be changed for a randomization
        /// </summary>
        public HashSet<string> ExcludedParameters { get; private set; }

        /// <summary>
        /// Humanize toggle and factor
        /// Humanize in a radomization of a parameter starting from it's current value at a given ratio.
        /// Ex: value is 50, humanize is 10%, final value will be 50 +/- 10%.
        /// If final value exceeds the min or max value, it will have the min or the max.
        /// </summary>

        private float? _humanizeRatio;

        /// <summary>
        /// Humanize ratio (0..1.0F)
        /// </summary>
        public float? HumanizeRatio
        {
            get
            {
                return _humanizeRatio;
            }
            set
            {
                if (value.HasValue)
                {
                    if (value > 1.0F) _humanizeRatio = 1.0F;
                    else if (value < 0.0F) _humanizeRatio = 0.0F;
                    else _humanizeRatio = value;
                }
                else _humanizeRatio = value;
            }
        }

        /// <summary>
        /// Do not use this ctor
        /// </summary>
        protected RandomizeToneArgument()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomizeToneArgument" /> class.
        /// </summary>
        /// <param name="excludedParameters">The excluded parameters.</param>
        /// <param name="humanizeRatio">The humanize ratio.</param>
        public RandomizeToneArgument(HashSet<string> excludedParameters, float? humanizeRatio)
        {
            if (excludedParameters != null)
            {
                ExcludedParameters = excludedParameters;
            }
            else
            {
                ExcludedParameters = new HashSet<string>();
            }

            _humanizeRatio = humanizeRatio;
        }
    }
}