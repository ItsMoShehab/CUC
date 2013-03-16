namespace CUPIFastStart
{
    partial class FormDistributionListFunctions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDistributionListFunctions));
            this.buttonPreviousPage = new System.Windows.Forms.Button();
            this.buttonNextPage = new System.Windows.Forms.Button();
            this.labelListsToFetch = new System.Windows.Forms.Label();
            this.comboListsToFetch = new System.Windows.Forms.ComboBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.textListExtension = new System.Windows.Forms.TextBox();
            this.textListDisplayName = new System.Windows.Forms.TextBox();
            this.labelExtension = new System.Windows.Forms.Label();
            this.labelDisplayName = new System.Windows.Forms.Label();
            this.labelListCountValue = new System.Windows.Forms.Label();
            this.lblListSelection = new System.Windows.Forms.Label();
            this.textListFilterText = new System.Windows.Forms.TextBox();
            this.comboListFilterAction = new System.Windows.Forms.ComboBox();
            this.comboListFilterElement = new System.Windows.Forms.ComboBox();
            this.gridLists = new System.Windows.Forms.DataGridView();
            this.buttonReviewMembers = new System.Windows.Forms.Button();
            this.buttonAddItem = new System.Windows.Forms.Button();
            this.buttonRemoveItem = new System.Windows.Forms.Button();
            this.buttonUpdateItem = new System.Windows.Forms.Button();
            this.buttonShowLists = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridLists)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonPreviousPage
            // 
            this.buttonPreviousPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPreviousPage.Enabled = false;
            this.buttonPreviousPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPreviousPage.Location = new System.Drawing.Point(676, 250);
            this.buttonPreviousPage.Name = "buttonPreviousPage";
            this.buttonPreviousPage.Size = new System.Drawing.Size(30, 24);
            this.buttonPreviousPage.TabIndex = 77;
            this.buttonPreviousPage.Text = "<<";
            this.buttonPreviousPage.UseVisualStyleBackColor = true;
            this.buttonPreviousPage.Click += new System.EventHandler(this.buttonPreviousPage_Click);
            // 
            // buttonNextPage
            // 
            this.buttonNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNextPage.Enabled = false;
            this.buttonNextPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNextPage.Location = new System.Drawing.Point(710, 250);
            this.buttonNextPage.Name = "buttonNextPage";
            this.buttonNextPage.Size = new System.Drawing.Size(30, 24);
            this.buttonNextPage.TabIndex = 76;
            this.buttonNextPage.Text = ">>";
            this.buttonNextPage.UseVisualStyleBackColor = true;
            this.buttonNextPage.Click += new System.EventHandler(this.buttonNextPage_Click);
            // 
            // labelListsToFetch
            // 
            this.labelListsToFetch.AutoSize = true;
            this.labelListsToFetch.Location = new System.Drawing.Point(606, 15);
            this.labelListsToFetch.Name = "labelListsToFetch";
            this.labelListsToFetch.Size = new System.Drawing.Size(76, 13);
            this.labelListsToFetch.TabIndex = 75;
            this.labelListsToFetch.Text = "Lists per page:";
            // 
            // comboListsToFetch
            // 
            this.comboListsToFetch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboListsToFetch.FormattingEnabled = true;
            this.comboListsToFetch.Items.AddRange(new object[] {
            "25",
            "50",
            "100",
            "200"});
            this.comboListsToFetch.Location = new System.Drawing.Point(688, 11);
            this.comboListsToFetch.Name = "comboListsToFetch";
            this.comboListsToFetch.Size = new System.Drawing.Size(51, 21);
            this.comboListsToFetch.TabIndex = 74;
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonExit.Location = new System.Drawing.Point(925, 250);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(91, 36);
            this.buttonExit.TabIndex = 73;
            this.buttonExit.Text = "E&xit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // textListExtension
            // 
            this.textListExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textListExtension.Location = new System.Drawing.Point(832, 62);
            this.textListExtension.Name = "textListExtension";
            this.textListExtension.Size = new System.Drawing.Size(184, 20);
            this.textListExtension.TabIndex = 69;
            this.textListExtension.TextChanged += new System.EventHandler(this.textListExtension_TextChanged);
            // 
            // textListDisplayName
            // 
            this.textListDisplayName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textListDisplayName.Location = new System.Drawing.Point(832, 36);
            this.textListDisplayName.Name = "textListDisplayName";
            this.textListDisplayName.Size = new System.Drawing.Size(184, 20);
            this.textListDisplayName.TabIndex = 68;
            this.textListDisplayName.TextChanged += new System.EventHandler(this.textListExtension_TextChanged);
            // 
            // labelExtension
            // 
            this.labelExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelExtension.AutoSize = true;
            this.labelExtension.Location = new System.Drawing.Point(770, 65);
            this.labelExtension.Name = "labelExtension";
            this.labelExtension.Size = new System.Drawing.Size(56, 13);
            this.labelExtension.TabIndex = 67;
            this.labelExtension.Text = "Extension:";
            // 
            // labelDisplayName
            // 
            this.labelDisplayName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDisplayName.AutoSize = true;
            this.labelDisplayName.Location = new System.Drawing.Point(751, 39);
            this.labelDisplayName.Name = "labelDisplayName";
            this.labelDisplayName.Size = new System.Drawing.Size(75, 13);
            this.labelDisplayName.TabIndex = 66;
            this.labelDisplayName.Text = "Display Name:";
            // 
            // labelListCountValue
            // 
            this.labelListCountValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelListCountValue.Location = new System.Drawing.Point(591, 254);
            this.labelListCountValue.Name = "labelListCountValue";
            this.labelListCountValue.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelListCountValue.Size = new System.Drawing.Size(79, 16);
            this.labelListCountValue.TabIndex = 65;
            this.labelListCountValue.Text = "0";
            this.labelListCountValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblListSelection
            // 
            this.lblListSelection.AutoSize = true;
            this.lblListSelection.Location = new System.Drawing.Point(22, 15);
            this.lblListSelection.Name = "lblListSelection";
            this.lblListSelection.Size = new System.Drawing.Size(112, 13);
            this.lblListSelection.TabIndex = 64;
            this.lblListSelection.Text = "List Selection Options:";
            // 
            // textListFilterText
            // 
            this.textListFilterText.Location = new System.Drawing.Point(345, 12);
            this.textListFilterText.MaxLength = 32;
            this.textListFilterText.Name = "textListFilterText";
            this.textListFilterText.Size = new System.Drawing.Size(153, 20);
            this.textListFilterText.TabIndex = 63;
            // 
            // comboListFilterAction
            // 
            this.comboListFilterAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboListFilterAction.FormattingEnabled = true;
            this.comboListFilterAction.Items.AddRange(new object[] {
            "Is",
            "StartsWith"});
            this.comboListFilterAction.Location = new System.Drawing.Point(245, 11);
            this.comboListFilterAction.Name = "comboListFilterAction";
            this.comboListFilterAction.Size = new System.Drawing.Size(94, 21);
            this.comboListFilterAction.TabIndex = 62;
            // 
            // comboListFilterElement
            // 
            this.comboListFilterElement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboListFilterElement.FormattingEnabled = true;
            this.comboListFilterElement.Items.AddRange(new object[] {
            "{All Lists}",
            "Alias",
            "DisplayName",
            "DTMFAccessId"});
            this.comboListFilterElement.Location = new System.Drawing.Point(140, 11);
            this.comboListFilterElement.Name = "comboListFilterElement";
            this.comboListFilterElement.Size = new System.Drawing.Size(99, 21);
            this.comboListFilterElement.TabIndex = 61;
            this.comboListFilterElement.SelectedIndexChanged += new System.EventHandler(this.comboListFilterElement_SelectedIndexChanged);
            // 
            // gridLists
            // 
            this.gridLists.AllowUserToAddRows = false;
            this.gridLists.AllowUserToDeleteRows = false;
            this.gridLists.AllowUserToOrderColumns = true;
            this.gridLists.AllowUserToResizeRows = false;
            this.gridLists.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridLists.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gridLists.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLists.Location = new System.Drawing.Point(15, 43);
            this.gridLists.MultiSelect = false;
            this.gridLists.Name = "gridLists";
            this.gridLists.ReadOnly = true;
            this.gridLists.RowHeadersVisible = false;
            this.gridLists.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridLists.ShowEditingIcon = false;
            this.gridLists.Size = new System.Drawing.Size(725, 201);
            this.gridLists.TabIndex = 60;
            this.gridLists.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.gridLists_RowValidating);
            this.gridLists.SelectionChanged += new System.EventHandler(this.gridLists_SelectionChanged);
            // 
            // buttonReviewMembers
            // 
            this.buttonReviewMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonReviewMembers.Image = global::CUPIFastStart.Properties.Resources.group_32;
            this.buttonReviewMembers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonReviewMembers.Location = new System.Drawing.Point(209, 250);
            this.buttonReviewMembers.Name = "buttonReviewMembers";
            this.buttonReviewMembers.Size = new System.Drawing.Size(91, 36);
            this.buttonReviewMembers.TabIndex = 80;
            this.buttonReviewMembers.Text = "Members";
            this.buttonReviewMembers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonReviewMembers.UseVisualStyleBackColor = true;
            this.buttonReviewMembers.Click += new System.EventHandler(this.buttonReviewMembers_Click);
            // 
            // buttonAddItem
            // 
            this.buttonAddItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddItem.Image = global::CUPIFastStart.Properties.Resources.more_24;
            this.buttonAddItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAddItem.Location = new System.Drawing.Point(15, 250);
            this.buttonAddItem.Name = "buttonAddItem";
            this.buttonAddItem.Size = new System.Drawing.Size(91, 36);
            this.buttonAddItem.TabIndex = 70;
            this.buttonAddItem.Text = "&Add     ";
            this.buttonAddItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonAddItem.UseVisualStyleBackColor = true;
            this.buttonAddItem.Click += new System.EventHandler(this.buttonAddItem_Click);
            // 
            // buttonRemoveItem
            // 
            this.buttonRemoveItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemoveItem.Image = global::CUPIFastStart.Properties.Resources.less_24;
            this.buttonRemoveItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRemoveItem.Location = new System.Drawing.Point(112, 250);
            this.buttonRemoveItem.Name = "buttonRemoveItem";
            this.buttonRemoveItem.Size = new System.Drawing.Size(91, 36);
            this.buttonRemoveItem.TabIndex = 71;
            this.buttonRemoveItem.Text = "&Remove";
            this.buttonRemoveItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRemoveItem.UseVisualStyleBackColor = true;
            this.buttonRemoveItem.Click += new System.EventHandler(this.buttonRemoveItem_Click);
            // 
            // buttonUpdateItem
            // 
            this.buttonUpdateItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpdateItem.Enabled = false;
            this.buttonUpdateItem.Image = global::CUPIFastStart.Properties.Resources.erasure_32;
            this.buttonUpdateItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonUpdateItem.Location = new System.Drawing.Point(832, 88);
            this.buttonUpdateItem.Name = "buttonUpdateItem";
            this.buttonUpdateItem.Size = new System.Drawing.Size(91, 36);
            this.buttonUpdateItem.TabIndex = 72;
            this.buttonUpdateItem.Text = "&Update  ";
            this.buttonUpdateItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonUpdateItem.UseVisualStyleBackColor = true;
            this.buttonUpdateItem.Click += new System.EventHandler(this.buttonUpdateItem_Click);
            // 
            // buttonShowLists
            // 
            this.buttonShowLists.Image = global::CUPIFastStart.Properties.Resources.binoculars_32;
            this.buttonShowLists.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonShowLists.Location = new System.Drawing.Point(504, 5);
            this.buttonShowLists.Name = "buttonShowLists";
            this.buttonShowLists.Size = new System.Drawing.Size(82, 32);
            this.buttonShowLists.TabIndex = 59;
            this.buttonShowLists.Text = "Find   ";
            this.buttonShowLists.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonShowLists.UseVisualStyleBackColor = true;
            this.buttonShowLists.Click += new System.EventHandler(this.buttonShowLists_Click);
            // 
            // FormDistributionListFunctions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 298);
            this.Controls.Add(this.buttonReviewMembers);
            this.Controls.Add(this.buttonPreviousPage);
            this.Controls.Add(this.buttonNextPage);
            this.Controls.Add(this.labelListsToFetch);
            this.Controls.Add(this.comboListsToFetch);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonAddItem);
            this.Controls.Add(this.buttonRemoveItem);
            this.Controls.Add(this.buttonUpdateItem);
            this.Controls.Add(this.textListExtension);
            this.Controls.Add(this.textListDisplayName);
            this.Controls.Add(this.labelExtension);
            this.Controls.Add(this.labelDisplayName);
            this.Controls.Add(this.labelListCountValue);
            this.Controls.Add(this.lblListSelection);
            this.Controls.Add(this.textListFilterText);
            this.Controls.Add(this.comboListFilterAction);
            this.Controls.Add(this.comboListFilterElement);
            this.Controls.Add(this.gridLists);
            this.Controls.Add(this.buttonShowLists);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormDistributionListFunctions";
            this.Text = "Distribution List Functions";
            this.Load += new System.EventHandler(this.FormDistributionListFunctions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridLists)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonPreviousPage;
        private System.Windows.Forms.Button buttonNextPage;
        private System.Windows.Forms.Label labelListsToFetch;
        private System.Windows.Forms.ComboBox comboListsToFetch;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonAddItem;
        private System.Windows.Forms.Button buttonRemoveItem;
        private System.Windows.Forms.Button buttonUpdateItem;
        private System.Windows.Forms.TextBox textListExtension;
        private System.Windows.Forms.TextBox textListDisplayName;
        private System.Windows.Forms.Label labelExtension;
        private System.Windows.Forms.Label labelDisplayName;
        private System.Windows.Forms.Label labelListCountValue;
        private System.Windows.Forms.Label lblListSelection;
        private System.Windows.Forms.TextBox textListFilterText;
        private System.Windows.Forms.ComboBox comboListFilterAction;
        private System.Windows.Forms.ComboBox comboListFilterElement;
        private System.Windows.Forms.DataGridView gridLists;
        private System.Windows.Forms.Button buttonShowLists;
        private System.Windows.Forms.Button buttonReviewMembers;
    }
}