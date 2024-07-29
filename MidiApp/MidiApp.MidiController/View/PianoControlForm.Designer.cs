namespace MidiApp.MidiController.View
{
     public partial class PianoControlForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pianoControl = new Sanford.Multimedia.Midi.UI.PianoControl();
            this.SuspendLayout();
            // 
            // pianoControl
            // 
            this.pianoControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pianoControl.HighNoteID = 108;
            this.pianoControl.Location = new System.Drawing.Point(12, 5);
            this.pianoControl.LowNoteID = 24;
            this.pianoControl.Name = "pianoControl";
            this.pianoControl.NoteOnColor = System.Drawing.Color.Red;
            this.pianoControl.Size = new System.Drawing.Size(688, 79);
            this.pianoControl.TabIndex = 0;
            this.pianoControl.PianoKeyDown += new System.EventHandler<Sanford.Multimedia.Midi.UI.PianoKeyEventArgs>(this.pianoControl1_PianoKeyDown);
            this.pianoControl.PianoKeyUp += new System.EventHandler<Sanford.Multimedia.Midi.UI.PianoKeyEventArgs>(this.pianoControl1_PianoKeyUp);
            // 
            // PianoControlForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(56)))), ((int)(((byte)(62)))));
            this.ClientSize = new System.Drawing.Size(712, 91);
            this.ControlBox = false;
            this.Controls.Add(this.pianoControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PianoControlForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Keyboard";
            this.ResumeLayout(false);

        }

        #endregion

        private Sanford.Multimedia.Midi.UI.PianoControl pianoControl;

    }
}
