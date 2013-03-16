namespace CUPIFastStart
{
    partial class FormNewDistributionListInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewDistributionListInfo));
            this.textExtension = new System.Windows.Forms.TextBox();
            this.textDisplayName = new System.Windows.Forms.TextBox();
            this.labelExtension = new System.Windows.Forms.Label();
            this.labelDisplayName = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textAlias = new System.Windows.Forms.TextBox();
            this.labelAlias = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textExtension
            // 
            this.textExtension.Location = new System.Drawing.Point(93, 64);
            this.textExtension.Name = "textExtension";
            this.textExtension.Size = new System.Drawing.Size(198, 20);
            this.textExtension.TabIndex = 2;
            // 
            // textDisplayName
            // 
            this.textDisplayName.Location = new System.Drawing.Point(93, 38);
            this.textDisplayName.Name = "textDisplayName";
            this.textDisplayName.Size = new System.Drawing.Size(198, 20);
            this.textDisplayName.TabIndex = 1;
            // 
            // labelExtension
            // 
            this.labelExtension.AutoSize = true;
            this.labelExtension.Location = new System.Drawing.Point(31, 67);
            this.labelExtension.Name = "labelExtension";
            this.labelExtension.Size = new System.Drawing.Size(56, 13);
            this.labelExtension.TabIndex = 43;
            this.labelExtension.Text = "Extension:";
            // 
            // labelDisplayName
            // 
            this.labelDisplayName.AutoSize = true;
            this.labelDisplayName.Location = new System.Drawing.Point(12, 41);
            this.labelDisplayName.Name = "labelDisplayName";
            this.labelDisplayName.Size = new System.Drawing.Size(75, 13);
            this.labelDisplayName.TabIndex = 42;
            this.labelDisplayName.Text = "Display Name:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Image = global::CUPIFastStart.Properties.Resources.x_24;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCancel.Location = new System.Drawing.Point(191, 102);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 31);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Image = global::CUPIFastStart.Properties.Resources.ok_24;
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonOK.Location = new System.Drawing.Point(85, 102);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 31);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textAlias
            // 
            this.textAlias.Location = new System.Drawing.Point(93, 12);
            this.textAlias.Name = "textAlias";
            this.textAlias.Size = new System.Drawing.Size(198, 20);
            this.textAlias.TabIndex = 0;
            // 
            // labelAlias
            // 
            this.labelAlias.AutoSize = true;
            this.labelAlias.Location = new System.Drawing.Point(55, 15);
            this.labelAlias.Name = "labelAlias";
            this.labelAlias.Size = new System.Drawing.Size(32, 13);
            this.labelAlias.TabIndex = 50;
            this.labelAlias.Text = "Alias:";
            // 
            // FormNewDistributionListInfo
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 146);
            this.Controls.Add(this.textAlias);
            this.Controls.Add(this.labelAlias);
            this.Controls.Add(this.textExtension);
            this.Controls.Add(this.textDisplayName);
            this.Controls.Add(this.labelExtension);
            this.Controls.Add(this.labelDisplayName);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNewDistributionListInfo";
            this.Text = "New Distribution List";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textExtension;
        private System.Windows.Forms.TextBox textDisplayName;
        private System.Windows.Forms.Label labelExtension;
        private System.Windows.Forms.Label labelDisplayName;
        internal System.Windows.Forms.Button buttonCancel;
        internal System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textAlias;
        private System.Windows.Forms.Label labelAlias;
    }
}