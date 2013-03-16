namespace CUPIFastStart
{
    partial class FormDistributionListMembers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDistributionListMembers));
            this.gridMembers = new System.Windows.Forms.DataGridView();
            this.buttonExit = new System.Windows.Forms.Button();
            this.labelListCountValue = new System.Windows.Forms.Label();
            this.buttonAddUser = new System.Windows.Forms.Button();
            this.buttonRemoveItem = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridMembers)).BeginInit();
            this.SuspendLayout();
            // 
            // gridMembers
            // 
            this.gridMembers.AllowUserToAddRows = false;
            this.gridMembers.AllowUserToDeleteRows = false;
            this.gridMembers.AllowUserToOrderColumns = true;
            this.gridMembers.AllowUserToResizeRows = false;
            this.gridMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridMembers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gridMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMembers.Location = new System.Drawing.Point(12, 12);
            this.gridMembers.MultiSelect = false;
            this.gridMembers.Name = "gridMembers";
            this.gridMembers.ReadOnly = true;
            this.gridMembers.RowHeadersVisible = false;
            this.gridMembers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMembers.ShowEditingIcon = false;
            this.gridMembers.Size = new System.Drawing.Size(881, 249);
            this.gridMembers.TabIndex = 72;
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonExit.Location = new System.Drawing.Point(802, 267);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(91, 36);
            this.buttonExit.TabIndex = 75;
            this.buttonExit.Text = "E&xit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // labelListCountValue
            // 
            this.labelListCountValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelListCountValue.Location = new System.Drawing.Point(650, 267);
            this.labelListCountValue.Name = "labelListCountValue";
            this.labelListCountValue.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelListCountValue.Size = new System.Drawing.Size(146, 17);
            this.labelListCountValue.TabIndex = 76;
            this.labelListCountValue.Text = "0";
            this.labelListCountValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonAddUser
            // 
            this.buttonAddUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddUser.Image = global::CUPIFastStart.Properties.Resources.alias_32;
            this.buttonAddUser.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAddUser.Location = new System.Drawing.Point(9, 267);
            this.buttonAddUser.Name = "buttonAddUser";
            this.buttonAddUser.Size = new System.Drawing.Size(91, 36);
            this.buttonAddUser.TabIndex = 73;
            this.buttonAddUser.Text = "&Add User";
            this.buttonAddUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonAddUser.UseVisualStyleBackColor = true;
            this.buttonAddUser.Click += new System.EventHandler(this.buttonAddUser_Click);
            // 
            // buttonRemoveItem
            // 
            this.buttonRemoveItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemoveItem.Image = global::CUPIFastStart.Properties.Resources.less_24;
            this.buttonRemoveItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRemoveItem.Location = new System.Drawing.Point(106, 267);
            this.buttonRemoveItem.Name = "buttonRemoveItem";
            this.buttonRemoveItem.Size = new System.Drawing.Size(91, 36);
            this.buttonRemoveItem.TabIndex = 74;
            this.buttonRemoveItem.Text = "&Remove";
            this.buttonRemoveItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRemoveItem.UseVisualStyleBackColor = true;
            this.buttonRemoveItem.Click += new System.EventHandler(this.buttonRemoveItem_Click);
            // 
            // FormDistributionListMembers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 315);
            this.Controls.Add(this.labelListCountValue);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonAddUser);
            this.Controls.Add(this.buttonRemoveItem);
            this.Controls.Add(this.gridMembers);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormDistributionListMembers";
            this.Text = "Distribution List Members";
            this.Shown += new System.EventHandler(this.FormDistributionListMembers_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.gridMembers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAddUser;
        private System.Windows.Forms.Button buttonRemoveItem;
        private System.Windows.Forms.DataGridView gridMembers;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Label labelListCountValue;
    }
}