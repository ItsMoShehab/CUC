namespace CUPIFastStart
{
    partial class FormFindUser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFindUser));
            this.lblUserSelection = new System.Windows.Forms.Label();
            this.textUserFilterText = new System.Windows.Forms.TextBox();
            this.comboUserFilterAction = new System.Windows.Forms.ComboBox();
            this.comboUserFilterElement = new System.Windows.Forms.ComboBox();
            this.gridUsers = new System.Windows.Forms.DataGridView();
            this.buttonShowUsers = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.lblShowingFirst = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).BeginInit();
            this.SuspendLayout();
            // 
            // lblUserSelection
            // 
            this.lblUserSelection.AutoSize = true;
            this.lblUserSelection.Location = new System.Drawing.Point(12, 23);
            this.lblUserSelection.Name = "lblUserSelection";
            this.lblUserSelection.Size = new System.Drawing.Size(118, 13);
            this.lblUserSelection.TabIndex = 31;
            this.lblUserSelection.Text = "User Selection Options:";
            // 
            // textUserFilterText
            // 
            this.textUserFilterText.Location = new System.Drawing.Point(337, 20);
            this.textUserFilterText.MaxLength = 32;
            this.textUserFilterText.Name = "textUserFilterText";
            this.textUserFilterText.Size = new System.Drawing.Size(153, 20);
            this.textUserFilterText.TabIndex = 30;
            // 
            // comboUserFilterAction
            // 
            this.comboUserFilterAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUserFilterAction.FormattingEnabled = true;
            this.comboUserFilterAction.Items.AddRange(new object[] {
            "Is",
            "StartsWith"});
            this.comboUserFilterAction.Location = new System.Drawing.Point(237, 19);
            this.comboUserFilterAction.Name = "comboUserFilterAction";
            this.comboUserFilterAction.Size = new System.Drawing.Size(94, 21);
            this.comboUserFilterAction.TabIndex = 29;
            // 
            // comboUserFilterElement
            // 
            this.comboUserFilterElement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUserFilterElement.FormattingEnabled = true;
            this.comboUserFilterElement.Items.AddRange(new object[] {
            "{All Users}",
            "Alias",
            "DisplayName",
            "DTMFAccessId",
            "FirstName",
            "LastName"});
            this.comboUserFilterElement.Location = new System.Drawing.Point(132, 19);
            this.comboUserFilterElement.Name = "comboUserFilterElement";
            this.comboUserFilterElement.Size = new System.Drawing.Size(99, 21);
            this.comboUserFilterElement.TabIndex = 28;
            this.comboUserFilterElement.SelectedIndexChanged += new System.EventHandler(this.comboUserFilterElement_SelectedIndexChanged);
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
            this.gridUsers.Size = new System.Drawing.Size(565, 226);
            this.gridUsers.TabIndex = 27;
            // 
            // buttonShowUsers
            // 
            this.buttonShowUsers.Image = global::CUPIFastStart.Properties.Resources.binoculars_32;
            this.buttonShowUsers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonShowUsers.Location = new System.Drawing.Point(496, 12);
            this.buttonShowUsers.Name = "buttonShowUsers";
            this.buttonShowUsers.Size = new System.Drawing.Size(82, 32);
            this.buttonShowUsers.TabIndex = 26;
            this.buttonShowUsers.Text = "Find   ";
            this.buttonShowUsers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonShowUsers.UseVisualStyleBackColor = true;
            this.buttonShowUsers.Click += new System.EventHandler(this.buttonShowUsers_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Image = global::CUPIFastStart.Properties.Resources.x_24;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCancel.Location = new System.Drawing.Point(477, 290);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 31);
            this.buttonCancel.TabIndex = 33;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Image = global::CUPIFastStart.Properties.Resources.ok_24;
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonOK.Location = new System.Drawing.Point(371, 290);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 31);
            this.buttonOK.TabIndex = 32;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // lblShowingFirst
            // 
            this.lblShowingFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblShowingFirst.AutoSize = true;
            this.lblShowingFirst.Location = new System.Drawing.Point(12, 275);
            this.lblShowingFirst.Name = "lblShowingFirst";
            this.lblShowingFirst.Size = new System.Drawing.Size(147, 13);
            this.lblShowingFirst.TabIndex = 34;
            this.lblShowingFirst.Text = "Showing first 50 matches only";
            // 
            // FormFindUser
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 332);
            this.Controls.Add(this.lblShowingFirst);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.lblUserSelection);
            this.Controls.Add(this.textUserFilterText);
            this.Controls.Add(this.comboUserFilterAction);
            this.Controls.Add(this.comboUserFilterElement);
            this.Controls.Add(this.gridUsers);
            this.Controls.Add(this.buttonShowUsers);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormFindUser";
            this.Text = "Find User";
            this.Load += new System.EventHandler(this.FormFindUser_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUserSelection;
        private System.Windows.Forms.TextBox textUserFilterText;
        private System.Windows.Forms.ComboBox comboUserFilterAction;
        private System.Windows.Forms.ComboBox comboUserFilterElement;
        private System.Windows.Forms.DataGridView gridUsers;
        private System.Windows.Forms.Button buttonShowUsers;
        internal System.Windows.Forms.Button buttonCancel;
        internal System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label lblShowingFirst;
    }
}