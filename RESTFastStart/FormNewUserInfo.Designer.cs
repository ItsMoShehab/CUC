namespace CUPIFastStart
{
    partial class FormNewUserInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewUserInfo));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textUserExtension = new System.Windows.Forms.TextBox();
            this.textUserDisplayName = new System.Windows.Forms.TextBox();
            this.textUserLastName = new System.Windows.Forms.TextBox();
            this.textUserFirstName = new System.Windows.Forms.TextBox();
            this.labelExtension = new System.Windows.Forms.Label();
            this.labelDisplayName = new System.Windows.Forms.Label();
            this.labelLastName = new System.Windows.Forms.Label();
            this.labelFirstname = new System.Windows.Forms.Label();
            this.labelAlias = new System.Windows.Forms.Label();
            this.textUserAlias = new System.Windows.Forms.TextBox();
            this.comboTemplate = new System.Windows.Forms.ComboBox();
            this.labelTemplate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Image = global::CUPIFastStart.Properties.Resources.x_24;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCancel.Location = new System.Drawing.Point(186, 191);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 31);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Image = global::CUPIFastStart.Properties.Resources.ok_24;
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonOK.Location = new System.Drawing.Point(80, 191);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 31);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textUserExtension
            // 
            this.textUserExtension.Location = new System.Drawing.Point(88, 116);
            this.textUserExtension.Name = "textUserExtension";
            this.textUserExtension.Size = new System.Drawing.Size(198, 20);
            this.textUserExtension.TabIndex = 29;
            // 
            // textUserDisplayName
            // 
            this.textUserDisplayName.Location = new System.Drawing.Point(88, 90);
            this.textUserDisplayName.Name = "textUserDisplayName";
            this.textUserDisplayName.Size = new System.Drawing.Size(198, 20);
            this.textUserDisplayName.TabIndex = 28;
            // 
            // textUserLastName
            // 
            this.textUserLastName.Location = new System.Drawing.Point(88, 64);
            this.textUserLastName.Name = "textUserLastName";
            this.textUserLastName.Size = new System.Drawing.Size(198, 20);
            this.textUserLastName.TabIndex = 27;
            // 
            // textUserFirstName
            // 
            this.textUserFirstName.Location = new System.Drawing.Point(88, 38);
            this.textUserFirstName.Name = "textUserFirstName";
            this.textUserFirstName.Size = new System.Drawing.Size(198, 20);
            this.textUserFirstName.TabIndex = 26;
            // 
            // labelExtension
            // 
            this.labelExtension.AutoSize = true;
            this.labelExtension.Location = new System.Drawing.Point(26, 119);
            this.labelExtension.Name = "labelExtension";
            this.labelExtension.Size = new System.Drawing.Size(56, 13);
            this.labelExtension.TabIndex = 25;
            this.labelExtension.Text = "Extension:";
            // 
            // labelDisplayName
            // 
            this.labelDisplayName.AutoSize = true;
            this.labelDisplayName.Location = new System.Drawing.Point(7, 93);
            this.labelDisplayName.Name = "labelDisplayName";
            this.labelDisplayName.Size = new System.Drawing.Size(75, 13);
            this.labelDisplayName.TabIndex = 24;
            this.labelDisplayName.Text = "Display Name:";
            // 
            // labelLastName
            // 
            this.labelLastName.AutoSize = true;
            this.labelLastName.Location = new System.Drawing.Point(21, 67);
            this.labelLastName.Name = "labelLastName";
            this.labelLastName.Size = new System.Drawing.Size(61, 13);
            this.labelLastName.TabIndex = 23;
            this.labelLastName.Text = "Last Name:";
            // 
            // labelFirstname
            // 
            this.labelFirstname.AutoSize = true;
            this.labelFirstname.Location = new System.Drawing.Point(22, 41);
            this.labelFirstname.Name = "labelFirstname";
            this.labelFirstname.Size = new System.Drawing.Size(60, 13);
            this.labelFirstname.TabIndex = 22;
            this.labelFirstname.Text = "First Name:";
            // 
            // labelAlias
            // 
            this.labelAlias.AutoSize = true;
            this.labelAlias.Location = new System.Drawing.Point(50, 15);
            this.labelAlias.Name = "labelAlias";
            this.labelAlias.Size = new System.Drawing.Size(32, 13);
            this.labelAlias.TabIndex = 21;
            this.labelAlias.Text = "Alias:";
            // 
            // textUserAlias
            // 
            this.textUserAlias.Location = new System.Drawing.Point(88, 12);
            this.textUserAlias.Name = "textUserAlias";
            this.textUserAlias.Size = new System.Drawing.Size(198, 20);
            this.textUserAlias.TabIndex = 20;
            // 
            // comboTemplate
            // 
            this.comboTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTemplate.FormattingEnabled = true;
            this.comboTemplate.Location = new System.Drawing.Point(88, 142);
            this.comboTemplate.Name = "comboTemplate";
            this.comboTemplate.Size = new System.Drawing.Size(198, 21);
            this.comboTemplate.TabIndex = 30;
            // 
            // labelTemplate
            // 
            this.labelTemplate.AutoSize = true;
            this.labelTemplate.Location = new System.Drawing.Point(26, 145);
            this.labelTemplate.Name = "labelTemplate";
            this.labelTemplate.Size = new System.Drawing.Size(54, 13);
            this.labelTemplate.TabIndex = 31;
            this.labelTemplate.Text = "Template:";
            // 
            // FormNewUserInfo
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 234);
            this.Controls.Add(this.labelTemplate);
            this.Controls.Add(this.comboTemplate);
            this.Controls.Add(this.textUserExtension);
            this.Controls.Add(this.textUserDisplayName);
            this.Controls.Add(this.textUserLastName);
            this.Controls.Add(this.textUserFirstName);
            this.Controls.Add(this.labelExtension);
            this.Controls.Add(this.labelDisplayName);
            this.Controls.Add(this.labelLastName);
            this.Controls.Add(this.labelFirstname);
            this.Controls.Add(this.labelAlias);
            this.Controls.Add(this.textUserAlias);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNewUserInfo";
            this.Text = "New User";
            this.Load += new System.EventHandler(this.FormNewUserInfo_Load);
            this.Shown += new System.EventHandler(this.FormNewUserInfo_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button buttonCancel;
        internal System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textUserExtension;
        private System.Windows.Forms.TextBox textUserDisplayName;
        private System.Windows.Forms.TextBox textUserLastName;
        private System.Windows.Forms.TextBox textUserFirstName;
        private System.Windows.Forms.Label labelExtension;
        private System.Windows.Forms.Label labelDisplayName;
        private System.Windows.Forms.Label labelLastName;
        private System.Windows.Forms.Label labelFirstname;
        private System.Windows.Forms.Label labelAlias;
        private System.Windows.Forms.TextBox textUserAlias;
        private System.Windows.Forms.ComboBox comboTemplate;
        private System.Windows.Forms.Label labelTemplate;
    }
}