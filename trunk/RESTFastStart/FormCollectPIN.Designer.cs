namespace CUPIFastStart
{
    partial class FormCollectPIN
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCollectPIN));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textPIN = new System.Windows.Forms.TextBox();
            this.labelEnterNewPin = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textPINVerify = new System.Windows.Forms.TextBox();
            this.checkMustChange = new System.Windows.Forms.CheckBox();
            this.checkDoesNotExpire = new System.Windows.Forms.CheckBox();
            this.checkClearHackedLockout = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Image = global::CUPIFastStart.Properties.Resources.x_24;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCancel.Location = new System.Drawing.Point(175, 141);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 31);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "&Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Image = global::CUPIFastStart.Properties.Resources.ok_24;
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonOK.Location = new System.Drawing.Point(69, 141);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 31);
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textPIN
            // 
            this.textPIN.HideSelection = false;
            this.textPIN.Location = new System.Drawing.Point(71, 12);
            this.textPIN.MaxLength = 10;
            this.textPIN.Name = "textPIN";
            this.textPIN.PasswordChar = '*';
            this.textPIN.Size = new System.Drawing.Size(204, 20);
            this.textPIN.TabIndex = 11;
            // 
            // labelEnterNewPin
            // 
            this.labelEnterNewPin.AutoSize = true;
            this.labelEnterNewPin.Location = new System.Drawing.Point(12, 15);
            this.labelEnterNewPin.Name = "labelEnterNewPin";
            this.labelEnterNewPin.Size = new System.Drawing.Size(53, 13);
            this.labelEnterNewPin.TabIndex = 12;
            this.labelEnterNewPin.Text = "New PIN:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Verify:";
            // 
            // textPINVerify
            // 
            this.textPINVerify.HideSelection = false;
            this.textPINVerify.Location = new System.Drawing.Point(71, 37);
            this.textPINVerify.MaxLength = 10;
            this.textPINVerify.Name = "textPINVerify";
            this.textPINVerify.PasswordChar = '*';
            this.textPINVerify.Size = new System.Drawing.Size(204, 20);
            this.textPINVerify.TabIndex = 14;
            // 
            // checkMustChange
            // 
            this.checkMustChange.AutoSize = true;
            this.checkMustChange.Location = new System.Drawing.Point(71, 63);
            this.checkMustChange.Name = "checkMustChange";
            this.checkMustChange.Size = new System.Drawing.Size(172, 17);
            this.checkMustChange.TabIndex = 15;
            this.checkMustChange.Text = "User must change at next login";
            this.checkMustChange.UseVisualStyleBackColor = true;
            // 
            // checkDoesNotExpire
            // 
            this.checkDoesNotExpire.AutoSize = true;
            this.checkDoesNotExpire.Location = new System.Drawing.Point(71, 86);
            this.checkDoesNotExpire.Name = "checkDoesNotExpire";
            this.checkDoesNotExpire.Size = new System.Drawing.Size(100, 17);
            this.checkDoesNotExpire.TabIndex = 16;
            this.checkDoesNotExpire.Text = "Does not expire";
            this.checkDoesNotExpire.UseVisualStyleBackColor = true;
            // 
            // checkClearHackedLockout
            // 
            this.checkClearHackedLockout.AutoSize = true;
            this.checkClearHackedLockout.Location = new System.Drawing.Point(69, 109);
            this.checkClearHackedLockout.Name = "checkClearHackedLockout";
            this.checkClearHackedLockout.Size = new System.Drawing.Size(127, 17);
            this.checkClearHackedLockout.TabIndex = 18;
            this.checkClearHackedLockout.Text = "Clear hacked lockout";
            this.checkClearHackedLockout.UseVisualStyleBackColor = true;
            // 
            // FormCollectPIN
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(287, 184);
            this.Controls.Add(this.checkClearHackedLockout);
            this.Controls.Add(this.checkDoesNotExpire);
            this.Controls.Add(this.checkMustChange);
            this.Controls.Add(this.textPINVerify);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelEnterNewPin);
            this.Controls.Add(this.textPIN);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCollectPIN";
            this.Text = "Enter new PIN";
            this.Load += new System.EventHandler(this.FormCollectPIN_Load);
            this.Shown += new System.EventHandler(this.FormCollectPIN_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button buttonCancel;
        internal System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textPIN;
        private System.Windows.Forms.Label labelEnterNewPin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textPINVerify;
        private System.Windows.Forms.CheckBox checkMustChange;
        private System.Windows.Forms.CheckBox checkDoesNotExpire;
        private System.Windows.Forms.CheckBox checkClearHackedLockout;
    }
}