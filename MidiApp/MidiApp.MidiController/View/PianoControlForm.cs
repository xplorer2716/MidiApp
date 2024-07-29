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

using MidiApp.MidiController.Controller;
using System.Drawing;
using System.Windows.Forms;

namespace MidiApp.MidiController.View
{
    public partial class PianoControlForm : Form
    {
        private AbstractController _controller = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PianoControlForm"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="noteOnColor">Color of the note on.</param>
        public PianoControlForm(AbstractController controller, Color noteOnColor)
        {
            InitializeComponent();
            _controller = controller;
            NoteOnColor = noteOnColor;
        }

        /// <summary>
        /// Gets or sets the color of the note on.
        /// </summary>
        /// <value>
        /// The color of the note on.
        /// </value>
        public Color NoteOnColor
        {
            get
            {
                return pianoControl.NoteOnColor;
            }
            set
            {
                pianoControl.NoteOnColor = value;
            }
        }

        private void pianoControl1_PianoKeyUp(object sender, Sanford.Multimedia.Midi.UI.PianoKeyEventArgs e)
        {
            _controller.PlayNote(false, e);
        }

        private void pianoControl1_PianoKeyDown(object sender, Sanford.Multimedia.Midi.UI.PianoKeyEventArgs e)
        {
            _controller.PlayNote(true, e);
        }
    }
}