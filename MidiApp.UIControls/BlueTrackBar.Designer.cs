namespace MidiApp.UIControls
{
    partial class BlueTrackBar
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this._pictureBoxBlue = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxBlue)).BeginInit();
            this.SuspendLayout();
            // 
            // _pictureBoxBlue
            // 
            this._pictureBoxBlue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._pictureBoxBlue.BackgroundImage = MidiApp.UIControls.Properties.Resources.BTN_Thumb_Blue;
            this._pictureBoxBlue.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this._pictureBoxBlue.Location = new System.Drawing.Point(0, 70);
            this._pictureBoxBlue.Margin = new System.Windows.Forms.Padding(0);
            this._pictureBoxBlue.Name = "_pictureBoxBlue";
            this._pictureBoxBlue.Size = new System.Drawing.Size(9, 9);
            this._pictureBoxBlue.TabIndex = 3;
            this._pictureBoxBlue.TabStop = false;
            this._pictureBoxBlue.Visible = false;
            this._pictureBoxBlue.MouseMove += new System.Windows.Forms.MouseEventHandler(this.THUMB_MouseMove);
            this._pictureBoxBlue.MouseDown += new System.Windows.Forms.MouseEventHandler(this.THUMB_MouseDown);
            this._pictureBoxBlue.MouseUp += new System.Windows.Forms.MouseEventHandler(this.THUMB_MouseUp);
            // 
            // BlueTrackBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.Controls.Add(this._pictureBoxBlue);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(9, 9);
            this.Name = "BlueTrackBar";
            this.Size = new System.Drawing.Size(9, 150);
            this.Load += new System.EventHandler(this.TRACK_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.THUMB_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TRACK_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.THUMB_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxBlue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _pictureBoxBlue;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
