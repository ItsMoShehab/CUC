namespace CUPIFastStart
{
    partial class FormNewCallHandlerInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewCallHandlerInfo));
            this.labelTemplate = new System.Windows.Forms.Label();
            this.comboTemplate = new System.Windows.Forms.ComboBox();
            this.textExtension = new System.Windows.Forms.TextBox();
            this.textDisplayName = new System.Windows.Forms.TextBox();
            this.labelExtension = new System.Windows.Forms.Label();
            this.labelDisplayName = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelTemplate
            // 
            this.labelTemplate.AutoSize = true;
            this.labelTemplate.Location = new System.Drawing.Point(31, 61);
            this.labelTemplate.Name = "labelTemplate";
            this.labelTemplate.Size = new System.Drawing.Size(54, 13);
            this.labelTemplate.TabIndex = 39;
            this.labelTemplate.Text = "Template:";
            // 
            // comboTemplate
            // 
            this.comboTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTemplate.FormattingEnabled = true;
            this.comboTemplate.Location = new System.Drawing.Point(93, 58);
            this.comboTemplate.Name = "comboTemplate";
            this.comboTemplate.Size = new System.Drawing.Size(198, 21);
            this.comboTemplate.TabIndex = 38;
            // 
            // textExtension
            // 
            this.textExtension.Location = new System.Drawing.Point(93, 32);
            this.textExtension.Name = "textExtension";
            this.textExtension.Size = new System.Drawing.Size(198, 20);
            this.textExtension.TabIndex = 37;
            // 
            // textDisplayName
            // 
            this.textDisplayName.Location = new System.Drawing.Point(93, 6);
            this.textDisplayName.Name = "textDisplayName";
            this.textDisplayName.Size = new System.Drawing.Size(198, 20);
            this.textDisplayName.TabIndex = 36;
            // 
            // labelExtension
            // 
            this.labelExtension.AutoSize = true;
            this.labelExtension.Location = new System.Drawing.Point(31, 35);
            this.labelExtension.Name = "labelExtension";
            this.labelExtension.Size = new System.Drawing.Size(56, 13);
            this.labelExtension.TabIndex = 35;
            this.labelExtension.Text = "Extension:";
            // 
            // labelDisplayName
            // 
            this.labelDisplayName.AutoSize = true;
            this.labelDisplayName.Location = new System.Drawing.Point(12, 9);
            this.labelDisplayName.Name = "labelDisplayName";
            this.labelDisplayName.Size = new System.Drawing.Size(75, 13);
            this.labelDisplayName.TabIndex = 34;
            this.labelDisplayName.Text = "Display Name:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Image = global::CUPIFastStart.Properties.Resources.x_24;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCancel.Location = new System.Drawing.Point(190, 97);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 31);
            this.buttonCancel.TabIndex = 33;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Image = global::CUPIFastStart.Properties.Resources.ok_24;
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonOK.Location = new System.Drawing.Point(84, 97);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 31);
            this.buttonOK.TabIndex = 32;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // FormNewCallHandlerInfo
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(302, 141);
            this.Controls.Add(this.labelTemplate);
            this.Controls.Add(this.comboTemplate);
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
            this.Name = "FormNewCallHandlerInfo";
            this.Text = "New Call Handler";
            this.Load += new System.EventHandler(this.FormNewCallHandlerInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTemplate;
        private System.Windows.Forms.ComboBox comboTemplate;
        private System.Windows.Forms.TextBox textExtension;
        private System.Windows.Forms.TextBox textDisplayName;
        private System.Windows.Forms.Label labelExtension;
        private System.Windows.Forms.Label labelDisplayName;
        internal System.Windows.Forms.Button buttonCancel;
        internal System.Windows.Forms.Button buttonOK;
    }
}