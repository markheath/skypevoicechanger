namespace SkypeVoiceChanger.Effects
{
    partial class EffectSliderPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelDescription = new System.Windows.Forms.Label();
            this.metroTrackBar1 = new MetroFramework.Controls.MetroTrackBar();
            this.textBoxValue = new MetroFramework.Controls.MetroTextBox();
            this.SuspendLayout();
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(4, 9);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(89, 13);
            this.labelDescription.TabIndex = 2;
            this.labelDescription.Text = "Slider Description";
            // 
            // metroTrackBar1
            // 
            this.metroTrackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metroTrackBar1.BackColor = System.Drawing.Color.Transparent;
            this.metroTrackBar1.Location = new System.Drawing.Point(131, 1);
            this.metroTrackBar1.Name = "metroTrackBar1";
            this.metroTrackBar1.Size = new System.Drawing.Size(274, 23);
            this.metroTrackBar1.TabIndex = 3;
            this.metroTrackBar1.Text = "metroTrackBar1";
            // 
            // textBoxValue
            // 
            this.textBoxValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxValue.Lines = new string[0];
            this.textBoxValue.Location = new System.Drawing.Point(411, 3);
            this.textBoxValue.MaxLength = 32767;
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.PasswordChar = '\0';
            this.textBoxValue.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxValue.SelectedText = "";
            this.textBoxValue.Size = new System.Drawing.Size(61, 23);
            this.textBoxValue.TabIndex = 4;
            this.textBoxValue.UseSelectable = true;
            // 
            // EffectSliderPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.metroTrackBar1);
            this.Controls.Add(this.labelDescription);
            this.Name = "EffectSliderPanel";
            this.Size = new System.Drawing.Size(476, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDescription;
        private MetroFramework.Controls.MetroTrackBar metroTrackBar1;
        private MetroFramework.Controls.MetroTextBox textBoxValue;
    }
}
