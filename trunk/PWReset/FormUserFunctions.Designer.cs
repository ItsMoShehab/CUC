namespace PWReset
{
    partial class FormUserFunctions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUserFunctions));
            this.gridUsers = new System.Windows.Forms.DataGridView();
            this.comboUserFilterElement = new System.Windows.Forms.ComboBox();
            this.comboUserFilterAction = new System.Windows.Forms.ComboBox();
            this.textUserFilterText = new System.Windows.Forms.TextBox();
            this.lblUserSelection = new System.Windows.Forms.Label();
            this.LabelUserCountValue = new System.Windows.Forms.Label();
            this.textUserAlias = new System.Windows.Forms.TextBox();
            this.labelAlias = new System.Windows.Forms.Label();
            this.labelFirstname = new System.Windows.Forms.Label();
            this.labelLastName = new System.Windows.Forms.Label();
            this.labelDisplayName = new System.Windows.Forms.Label();
            this.labelExtension = new System.Windows.Forms.Label();
            this.textUserFirstName = new System.Windows.Forms.TextBox();
            this.textUserLastName = new System.Windows.Forms.TextBox();
            this.textUserDisplayName = new System.Windows.Forms.TextBox();
            this.textUserExtension = new System.Windows.Forms.TextBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.comboUsersToFetch = new System.Windows.Forms.ComboBox();
            this.labelUsersToFetch = new System.Windows.Forms.Label();
            this.buttonNextPage = new System.Windows.Forms.Button();
            this.buttonPreviousPage = new System.Windows.Forms.Button();
            this.buttonShowUsers = new System.Windows.Forms.Button();
            this.buttonResetPIN = new System.Windows.Forms.Button();
            this.buttonShowPin = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).BeginInit();
            this.SuspendLayout();
            // 
            // gridUsers
            // 
            this.gridUsers.AllowUserToAddRows = false;
            this.gridUsers.AllowUserToDeleteRows = false;
            this.gridUsers.AllowUserToOrderColumns = true;
            this.gridUsers.AllowUserToResizeRows = false;
            this.gridUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gridUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUsers.Location = new System.Drawing.Point(12, 46);
            this.gridUsers.MultiSelect = false;
            this.gridUsers.Name = "gridUsers";
            this.gridUsers.ReadOnly = true;
            this.gridUsers.RowHeadersVisible = false;
            this.gridUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridUsers.ShowEditingIcon = false;
            this.gridUsers.Size = new System.Drawing.Size(707, 278);
            this.gridUsers.TabIndex = 3;
            // 
            // comboUserFilterElement
            // 
            this.comboUserFilterElement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUserFilterElement.FormattingEnabled = true;
            this.comboUserFilterElement.Items.AddRange(new object[] {
            "{All Users}",
            "Alias",
            "Display Name",
            "Primary Extension",
            "First Name",
            "Last Name"});
            this.comboUserFilterElement.Location = new System.Drawing.Point(132, 19);
            this.comboUserFilterElement.Name = "comboUserFilterElement";
            this.comboUserFilterElement.Size = new System.Drawing.Size(109, 21);
            this.comboUserFilterElement.TabIndex = 4;
            this.comboUserFilterElement.SelectedIndexChanged += new System.EventHandler(this.comboUserFilterElement_SelectedIndexChanged);
            // 
            // comboUserFilterAction
            // 
            this.comboUserFilterAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUserFilterAction.FormattingEnabled = true;
            this.comboUserFilterAction.Items.AddRange(new object[] {
            "Is",
            "StartsWith"});
            this.comboUserFilterAction.Location = new System.Drawing.Point(247, 19);
            this.comboUserFilterAction.Name = "comboUserFilterAction";
            this.comboUserFilterAction.Size = new System.Drawing.Size(94, 21);
            this.comboUserFilterAction.TabIndex = 5;
            // 
            // textUserFilterText
            // 
            this.textUserFilterText.Location = new System.Drawing.Point(347, 20);
            this.textUserFilterText.MaxLength = 32;
            this.textUserFilterText.Name = "textUserFilterText";
            this.textUserFilterText.Size = new System.Drawing.Size(143, 20);
            this.textUserFilterText.TabIndex = 6;
            // 
            // lblUserSelection
            // 
            this.lblUserSelection.AutoSize = true;
            this.lblUserSelection.Location = new System.Drawing.Point(12, 23);
            this.lblUserSelection.Name = "lblUserSelection";
            this.lblUserSelection.Size = new System.Drawing.Size(118, 13);
            this.lblUserSelection.TabIndex = 7;
            this.lblUserSelection.Text = "User Selection Options:";
            // 
            // LabelUserCountValue
            // 
            this.LabelUserCountValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelUserCountValue.Location = new System.Drawing.Point(572, 330);
            this.LabelUserCountValue.Name = "LabelUserCountValue";
            this.LabelUserCountValue.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelUserCountValue.Size = new System.Drawing.Size(79, 16);
            this.LabelUserCountValue.TabIndex = 9;
            this.LabelUserCountValue.Text = "0";
            this.LabelUserCountValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textUserAlias
            // 
            this.textUserAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textUserAlias.Location = new System.Drawing.Point(806, 63);
            this.textUserAlias.Name = "textUserAlias";
            this.textUserAlias.ReadOnly = true;
            this.textUserAlias.Size = new System.Drawing.Size(190, 20);
            this.textUserAlias.TabIndex = 10;
            // 
            // labelAlias
            // 
            this.labelAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAlias.AutoSize = true;
            this.labelAlias.Location = new System.Drawing.Point(768, 66);
            this.labelAlias.Name = "labelAlias";
            this.labelAlias.Size = new System.Drawing.Size(32, 13);
            this.labelAlias.TabIndex = 11;
            this.labelAlias.Text = "Alias:";
            // 
            // labelFirstname
            // 
            this.labelFirstname.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFirstname.AutoSize = true;
            this.labelFirstname.Location = new System.Drawing.Point(740, 92);
            this.labelFirstname.Name = "labelFirstname";
            this.labelFirstname.Size = new System.Drawing.Size(60, 13);
            this.labelFirstname.TabIndex = 12;
            this.labelFirstname.Text = "First Name:";
            // 
            // labelLastName
            // 
            this.labelLastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLastName.AutoSize = true;
            this.labelLastName.Location = new System.Drawing.Point(739, 118);
            this.labelLastName.Name = "labelLastName";
            this.labelLastName.Size = new System.Drawing.Size(61, 13);
            this.labelLastName.TabIndex = 13;
            this.labelLastName.Text = "Last Name:";
            // 
            // labelDisplayName
            // 
            this.labelDisplayName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDisplayName.AutoSize = true;
            this.labelDisplayName.Location = new System.Drawing.Point(725, 144);
            this.labelDisplayName.Name = "labelDisplayName";
            this.labelDisplayName.Size = new System.Drawing.Size(75, 13);
            this.labelDisplayName.TabIndex = 14;
            this.labelDisplayName.Text = "Display Name:";
            // 
            // labelExtension
            // 
            this.labelExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelExtension.AutoSize = true;
            this.labelExtension.Location = new System.Drawing.Point(744, 170);
            this.labelExtension.Name = "labelExtension";
            this.labelExtension.Size = new System.Drawing.Size(56, 13);
            this.labelExtension.TabIndex = 15;
            this.labelExtension.Text = "Extension:";
            // 
            // textUserFirstName
            // 
            this.textUserFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textUserFirstName.Location = new System.Drawing.Point(806, 89);
            this.textUserFirstName.Name = "textUserFirstName";
            this.textUserFirstName.ReadOnly = true;
            this.textUserFirstName.Size = new System.Drawing.Size(190, 20);
            this.textUserFirstName.TabIndex = 16;
            // 
            // textUserLastName
            // 
            this.textUserLastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textUserLastName.Location = new System.Drawing.Point(806, 115);
            this.textUserLastName.Name = "textUserLastName";
            this.textUserLastName.ReadOnly = true;
            this.textUserLastName.Size = new System.Drawing.Size(190, 20);
            this.textUserLastName.TabIndex = 17;
            // 
            // textUserDisplayName
            // 
            this.textUserDisplayName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textUserDisplayName.Location = new System.Drawing.Point(806, 141);
            this.textUserDisplayName.Name = "textUserDisplayName";
            this.textUserDisplayName.ReadOnly = true;
            this.textUserDisplayName.Size = new System.Drawing.Size(190, 20);
            this.textUserDisplayName.TabIndex = 18;
            // 
            // textUserExtension
            // 
            this.textUserExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textUserExtension.Location = new System.Drawing.Point(806, 167);
            this.textUserExtension.Name = "textUserExtension";
            this.textUserExtension.ReadOnly = true;
            this.textUserExtension.Size = new System.Drawing.Size(190, 20);
            this.textUserExtension.TabIndex = 19;
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonExit.Location = new System.Drawing.Point(899, 340);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(91, 36);
            this.buttonExit.TabIndex = 23;
            this.buttonExit.Text = "E&xit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // comboUsersToFetch
            // 
            this.comboUsersToFetch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUsersToFetch.FormattingEnabled = true;
            this.comboUsersToFetch.Items.AddRange(new object[] {
            "25",
            "50",
            "100",
            "200"});
            this.comboUsersToFetch.Location = new System.Drawing.Point(670, 19);
            this.comboUsersToFetch.Name = "comboUsersToFetch";
            this.comboUsersToFetch.Size = new System.Drawing.Size(51, 21);
            this.comboUsersToFetch.TabIndex = 24;
            // 
            // labelUsersToFetch
            // 
            this.labelUsersToFetch.AutoSize = true;
            this.labelUsersToFetch.Location = new System.Drawing.Point(582, 23);
            this.labelUsersToFetch.Name = "labelUsersToFetch";
            this.labelUsersToFetch.Size = new System.Drawing.Size(82, 13);
            this.labelUsersToFetch.TabIndex = 25;
            this.labelUsersToFetch.Text = "Users per page:";
            // 
            // buttonNextPage
            // 
            this.buttonNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNextPage.Enabled = false;
            this.buttonNextPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNextPage.Location = new System.Drawing.Point(691, 327);
            this.buttonNextPage.Name = "buttonNextPage";
            this.buttonNextPage.Size = new System.Drawing.Size(30, 24);
            this.buttonNextPage.TabIndex = 26;
            this.buttonNextPage.Text = ">>";
            this.buttonNextPage.UseVisualStyleBackColor = true;
            this.buttonNextPage.Click += new System.EventHandler(this.buttonNextPage_Click);
            // 
            // buttonPreviousPage
            // 
            this.buttonPreviousPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPreviousPage.Enabled = false;
            this.buttonPreviousPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPreviousPage.Location = new System.Drawing.Point(657, 327);
            this.buttonPreviousPage.Name = "buttonPreviousPage";
            this.buttonPreviousPage.Size = new System.Drawing.Size(30, 24);
            this.buttonPreviousPage.TabIndex = 27;
            this.buttonPreviousPage.Text = "<<";
            this.buttonPreviousPage.UseVisualStyleBackColor = true;
            this.buttonPreviousPage.Click += new System.EventHandler(this.buttonPreviousPage_Click);
            // 
            // buttonShowUsers
            // 
            this.buttonShowUsers.Image = global::PasswordReset.Properties.Resources.binoculars_32;
            this.buttonShowUsers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonShowUsers.Location = new System.Drawing.Point(496, 12);
            this.buttonShowUsers.Name = "buttonShowUsers";
            this.buttonShowUsers.Size = new System.Drawing.Size(82, 32);
            this.buttonShowUsers.TabIndex = 2;
            this.buttonShowUsers.Text = "Find   ";
            this.buttonShowUsers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonShowUsers.UseVisualStyleBackColor = true;
            this.buttonShowUsers.Click += new System.EventHandler(this.buttonShowUsers_Click);
            // 
            // buttonResetPIN
            // 
            this.buttonResetPIN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonResetPIN.Image = global::PasswordReset.Properties.Resources.primary_key_refresh_32;
            this.buttonResetPIN.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonResetPIN.Location = new System.Drawing.Point(12, 340);
            this.buttonResetPIN.Name = "buttonResetPIN";
            this.buttonResetPIN.Size = new System.Drawing.Size(93, 36);
            this.buttonResetPIN.TabIndex = 30;
            this.buttonResetPIN.Text = "&Reset PIN";
            this.buttonResetPIN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonResetPIN.UseVisualStyleBackColor = true;
            this.buttonResetPIN.Click += new System.EventHandler(this.buttonResetPIN_Click);
            // 
            // buttonShowPin
            // 
            this.buttonShowPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonShowPin.Image = global::PasswordReset.Properties.Resources.loupe_24;
            this.buttonShowPin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonShowPin.Location = new System.Drawing.Point(111, 340);
            this.buttonShowPin.Name = "buttonShowPin";
            this.buttonShowPin.Size = new System.Drawing.Size(93, 36);
            this.buttonShowPin.TabIndex = 33;
            this.buttonShowPin.Text = "&Show PIN";
            this.buttonShowPin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonShowPin.UseVisualStyleBackColor = true;
            this.buttonShowPin.Click += new System.EventHandler(this.buttonShowPin_Click);
            // 
            // FormUserFunctions
            // 
            this.AcceptButton = this.buttonShowUsers;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 384);
            this.Controls.Add(this.buttonShowPin);
            this.Controls.Add(this.buttonResetPIN);
            this.Controls.Add(this.buttonPreviousPage);
            this.Controls.Add(this.buttonNextPage);
            this.Controls.Add(this.labelUsersToFetch);
            this.Controls.Add(this.comboUsersToFetch);
            this.Controls.Add(this.buttonExit);
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
            this.Controls.Add(this.LabelUserCountValue);
            this.Controls.Add(this.lblUserSelection);
            this.Controls.Add(this.textUserFilterText);
            this.Controls.Add(this.comboUserFilterAction);
            this.Controls.Add(this.comboUserFilterElement);
            this.Controls.Add(this.gridUsers);
            this.Controls.Add(this.buttonShowUsers);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1018, 422);
            this.Name = "FormUserFunctions";
            this.Text = "User Functions";
            this.Load += new System.EventHandler(this.FormUserFunctions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridUsers;
        private System.Windows.Forms.Button buttonShowUsers;
        private System.Windows.Forms.ComboBox comboUserFilterElement;
        private System.Windows.Forms.ComboBox comboUserFilterAction;
        private System.Windows.Forms.TextBox textUserFilterText;
        private System.Windows.Forms.Label lblUserSelection;
        private System.Windows.Forms.Label LabelUserCountValue;
        private System.Windows.Forms.TextBox textUserAlias;
        private System.Windows.Forms.Label labelAlias;
        private System.Windows.Forms.Label labelFirstname;
        private System.Windows.Forms.Label labelLastName;
        private System.Windows.Forms.Label labelDisplayName;
        private System.Windows.Forms.Label labelExtension;
        private System.Windows.Forms.TextBox textUserFirstName;
        private System.Windows.Forms.TextBox textUserLastName;
        private System.Windows.Forms.TextBox textUserDisplayName;
        private System.Windows.Forms.TextBox textUserExtension;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.ComboBox comboUsersToFetch;
        private System.Windows.Forms.Label labelUsersToFetch;
        private System.Windows.Forms.Button buttonNextPage;
        private System.Windows.Forms.Button buttonPreviousPage;
        private System.Windows.Forms.Button buttonResetPIN;
        private System.Windows.Forms.Button buttonShowPin;
    }
}