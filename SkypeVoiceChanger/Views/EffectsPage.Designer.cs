namespace SkypeVoiceChanger.Views
{
    partial class EffectsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EffectsPage));
            this.effectPanel1 = new SkypeVoiceChanger.Effects.EffectPanel();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonOpen = new System.Windows.Forms.ToolStripButton();
            this.buttonRewind = new System.Windows.Forms.ToolStripButton();
            this.buttonPlay = new System.Windows.Forms.ToolStripButton();
            this.buttonPause = new System.Windows.Forms.ToolStripButton();
            this.buttonStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonAddEffect = new System.Windows.Forms.ToolStripButton();
            this.buttonRemoveEffect = new System.Windows.Forms.ToolStripButton();
            this.buttonMoveEffectUp = new System.Windows.Forms.ToolStripButton();
            this.buttonMoveEffectDown = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // effectPanel1
            // 
            this.effectPanel1.AutoScroll = true;
            this.effectPanel1.BackColor = System.Drawing.Color.White;
            this.effectPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.effectPanel1.Location = new System.Drawing.Point(140, 39);
            this.effectPanel1.Name = "effectPanel1";
            this.effectPanel1.Size = new System.Drawing.Size(507, 390);
            this.effectPanel1.TabIndex = 11;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(0, 39);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(140, 390);
            this.checkedListBox1.TabIndex = 10;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonOpen,
            this.buttonRewind,
            this.buttonPlay,
            this.buttonPause,
            this.buttonStop,
            this.toolStripSeparator1,
            this.buttonAddEffect,
            this.buttonRemoveEffect,
            this.buttonMoveEffectUp,
            this.buttonMoveEffectDown});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(647, 39);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonOpen
            // 
            this.buttonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOpen.Image = ((System.Drawing.Image)(resources.GetObject("buttonOpen.Image")));
            this.buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(36, 36);
            this.buttonOpen.Text = "Open Sound File...";
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonRewind
            // 
            this.buttonRewind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRewind.Image = ((System.Drawing.Image)(resources.GetObject("buttonRewind.Image")));
            this.buttonRewind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRewind.Name = "buttonRewind";
            this.buttonRewind.Size = new System.Drawing.Size(36, 36);
            this.buttonRewind.Text = "Rewind";
            this.buttonRewind.Click += new System.EventHandler(this.buttonRewind_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPlay.Image = ((System.Drawing.Image)(resources.GetObject("buttonPlay.Image")));
            this.buttonPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(36, 36);
            this.buttonPlay.Text = "Play";
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPause.Image = ((System.Drawing.Image)(resources.GetObject("buttonPause.Image")));
            this.buttonPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(36, 36);
            this.buttonPause.Text = "Pause";
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonStop.Image = ((System.Drawing.Image)(resources.GetObject("buttonStop.Image")));
            this.buttonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(36, 36);
            this.buttonStop.Text = "Stop";
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // buttonAddEffect
            // 
            this.buttonAddEffect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddEffect.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddEffect.Image")));
            this.buttonAddEffect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddEffect.Name = "buttonAddEffect";
            this.buttonAddEffect.Size = new System.Drawing.Size(36, 36);
            this.buttonAddEffect.Text = "Add Effect";
            this.buttonAddEffect.Click += new System.EventHandler(this.buttonAddEffect_Click);
            // 
            // buttonRemoveEffect
            // 
            this.buttonRemoveEffect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemoveEffect.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemoveEffect.Image")));
            this.buttonRemoveEffect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemoveEffect.Name = "buttonRemoveEffect";
            this.buttonRemoveEffect.Size = new System.Drawing.Size(36, 36);
            this.buttonRemoveEffect.Text = "Remove Effect";
            this.buttonRemoveEffect.Click += new System.EventHandler(this.buttonRemoveEffect_Click);
            // 
            // buttonMoveEffectUp
            // 
            this.buttonMoveEffectUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveEffectUp.Image = ((System.Drawing.Image)(resources.GetObject("buttonMoveEffectUp.Image")));
            this.buttonMoveEffectUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveEffectUp.Name = "buttonMoveEffectUp";
            this.buttonMoveEffectUp.Size = new System.Drawing.Size(36, 36);
            this.buttonMoveEffectUp.Text = "Move Selected Effect Up";
            this.buttonMoveEffectUp.Click += new System.EventHandler(this.buttonMoveEffectUp_Click);
            // 
            // buttonMoveEffectDown
            // 
            this.buttonMoveEffectDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveEffectDown.Image = ((System.Drawing.Image)(resources.GetObject("buttonMoveEffectDown.Image")));
            this.buttonMoveEffectDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveEffectDown.Name = "buttonMoveEffectDown";
            this.buttonMoveEffectDown.Size = new System.Drawing.Size(36, 36);
            this.buttonMoveEffectDown.Text = "Move Selected Effect Down";
            this.buttonMoveEffectDown.Click += new System.EventHandler(this.buttonMoveEffectDown_Click);
            // 
            // EffectsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.effectPanel1);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "EffectsPage";
            this.Size = new System.Drawing.Size(647, 429);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Effects.EffectPanel effectPanel1;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonOpen;
        private System.Windows.Forms.ToolStripButton buttonRewind;
        private System.Windows.Forms.ToolStripButton buttonPlay;
        private System.Windows.Forms.ToolStripButton buttonPause;
        private System.Windows.Forms.ToolStripButton buttonStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonAddEffect;
        private System.Windows.Forms.ToolStripButton buttonRemoveEffect;
        private System.Windows.Forms.ToolStripButton buttonMoveEffectUp;
        private System.Windows.Forms.ToolStripButton buttonMoveEffectDown;
    }
}
