namespace MidiApp.MidiController.View
{
    partial class TopLevelExceptionForm
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
            this._pictureBox = new System.Windows.Forms.PictureBox();
            this._btClose = new System.Windows.Forms.Button();
            this._supportAddressLabel = new System.Windows.Forms.LinkLabel();
            this._labelErrorMessage = new System.Windows.Forms.Label();
            this._textBoxDetails = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _pictureBox
            // 
            this._pictureBox.Location = new System.Drawing.Point(13, 12);
            this._pictureBox.Name = "_pictureBox";
            this._pictureBox.Size = new System.Drawing.Size(32, 32);
            this._pictureBox.TabIndex = 0;
            this._pictureBox.TabStop = false;
            // 
            // _btClose
            // 
            this._btClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btClose.BackColor = System.Drawing.SystemColors.Control;
            this._btClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btClose.ForeColor = System.Drawing.SystemColors.ControlText;
            this._btClose.Location = new System.Drawing.Point(476, 333);
            this._btClose.Name = "_btClose";
            this._btClose.Size = new System.Drawing.Size(75, 23);
            this._btClose.TabIndex = 4;
            this._btClose.Text = "Close";
            this._btClose.UseVisualStyleBackColor = false;
            this._btClose.Click += new System.EventHandler(this._btClose_Click);
            // 
            // _supportAddressLabel
            // 
            this._supportAddressLabel.Location = new System.Drawing.Point(0, 0);
            this._supportAddressLabel.Name = "_supportAddressLabel";
            this._supportAddressLabel.Size = new System.Drawing.Size(100, 23);
            this._supportAddressLabel.TabIndex = 10;
            // 
            // _labelErrorMessage
            // 
            this._labelErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._labelErrorMessage.ForeColor = System.Drawing.Color.Black;
            this._labelErrorMessage.Location = new System.Drawing.Point(51, 12);
            this._labelErrorMessage.Name = "_labelErrorMessage";
            this._labelErrorMessage.Size = new System.Drawing.Size(500, 45);
            this._labelErrorMessage.TabIndex = 9;
            this._labelErrorMessage.Text = "This is a default error message.";
            // 
            // _textBoxDetails
            // 
            this._textBoxDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxDetails.BackColor = System.Drawing.Color.White;
            this._textBoxDetails.ForeColor = System.Drawing.Color.Black;
            this._textBoxDetails.Location = new System.Drawing.Point(13, 79);
            this._textBoxDetails.Multiline = true;
            this._textBoxDetails.Name = "_textBoxDetails";
            this._textBoxDetails.ReadOnly = true;
            this._textBoxDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._textBoxDetails.Size = new System.Drawing.Size(538, 248);
            this._textBoxDetails.TabIndex = 2;
            // 
            // TopLevelExceptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(563, 369);
            this.Controls.Add(this._labelErrorMessage);
            this.Controls.Add(this._supportAddressLabel);
            this.Controls.Add(this._btClose);
            this.Controls.Add(this._textBoxDetails);
            this.Controls.Add(this._pictureBox);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(218)))), ((int)(((byte)(218)))));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(571, 396);
            this.Name = "TopLevelExceptionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Guru meditation...";
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.Button _btClose;
        private System.Windows.Forms.LinkLabel _supportAddressLabel;
        private System.Windows.Forms.Label _labelErrorMessage;
        private System.Windows.Forms.TextBox _textBoxDetails;
    }
}
