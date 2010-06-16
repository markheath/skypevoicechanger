namespace SkypeFx
{
    partial class CustomerFeedbackForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.rdoOptOut = new System.Windows.Forms.RadioButton();
            this.rdoOptIn = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.risData = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.risData);
            this.groupBox1.Controls.Add(this.richTextBox1);
            this.groupBox1.Controls.Add(this.rdoOptOut);
            this.groupBox1.Controls.Add(this.rdoOptIn);
            this.groupBox1.Location = new System.Drawing.Point(5, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 189);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Help us improve our software";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(11, 33);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(264, 50);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "Would you like to anonymously participate in our customer feedback system to help" +
                " us improve our product?";
            // 
            // rdoOptOut
            // 
            this.rdoOptOut.AutoSize = true;
            this.rdoOptOut.Checked = true;
            this.rdoOptOut.Location = new System.Drawing.Point(10, 111);
            this.rdoOptOut.Name = "rdoOptOut";
            this.rdoOptOut.Size = new System.Drawing.Size(180, 17);
            this.rdoOptOut.TabIndex = 1;
            this.rdoOptOut.TabStop = true;
            this.rdoOptOut.Text = "No, I would not like to participate";
            this.rdoOptOut.UseVisualStyleBackColor = true;
            // 
            // rdoOptIn
            // 
            this.rdoOptIn.AutoSize = true;
            this.rdoOptIn.Location = new System.Drawing.Point(10, 87);
            this.rdoOptIn.Name = "rdoOptIn";
            this.rdoOptIn.Size = new System.Drawing.Size(166, 17);
            this.rdoOptIn.TabIndex = 0;
            this.rdoOptIn.Text = "Yes, I would like to participate";
            this.rdoOptIn.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(129, 198);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(210, 198);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // risData
            // 
            this.risData.AutoSize = true;
            this.risData.Location = new System.Drawing.Point(8, 144);
            this.risData.Name = "risData";
            this.risData.Size = new System.Drawing.Size(256, 13);
            this.risData.TabIndex = 3;
            this.risData.TabStop = true;
            this.risData.Text = "View collected data online at runtimeintelligence.com";
            this.risData.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.risData_LinkClicked);
            // 
            // CustomerFeedbackForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(290, 229);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomerFeedbackForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Customer Feedback Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoOptOut;
        private System.Windows.Forms.RadioButton rdoOptIn;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.LinkLabel risData;
    }
}