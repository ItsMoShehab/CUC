namespace CUPIFastStart
{
    partial class FormCallHandlerFunctions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCallHandlerFunctions));
            this.buttonPreviousPage = new System.Windows.Forms.Button();
            this.buttonNextPage = new System.Windows.Forms.Button();
            this.labelHandlersToFetch = new System.Windows.Forms.Label();
            this.comboHandlersToFetch = new System.Windows.Forms.ComboBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.textHandlerExtension = new System.Windows.Forms.TextBox();
            this.textHandlerDisplayName = new System.Windows.Forms.TextBox();
            this.labelExtension = new System.Windows.Forms.Label();
            this.labelDisplayName = new System.Windows.Forms.Label();
            this.labelHandlerCountValue = new System.Windows.Forms.Label();
            this.lblHandlerSelection = new System.Windows.Forms.Label();
            this.textHandlerFilterText = new System.Windows.Forms.TextBox();
            this.comboHandlerFilterAction = new System.Windows.Forms.ComboBox();
            this.comboHandlerFilterElement = new System.Windows.Forms.ComboBox();
            this.gridHandlers = new System.Windows.Forms.DataGridView();
            this.chkIsPrimary = new System.Windows.Forms.CheckBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonShowInputKeys = new System.Windows.Forms.Button();
            this.buttonSetVoiceName = new System.Windows.Forms.Button();
            this.buttonFetchVoiceName = new System.Windows.Forms.Button();
            this.buttonAddItem = new System.Windows.Forms.Button();
            this.buttonRemoveItem = new System.Windows.Forms.Button();
            this.buttonUpdateItem = new System.Windows.Forms.Button();
            this.buttonShowHandlers = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridHandlers)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonPreviousPage
            // 
            this.buttonPreviousPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPreviousPage.Enabled = false;
            this.buttonPreviousPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPreviousPage.Location = new System.Drawing.Point(682, 260);
            this.buttonPreviousPage.Name = "buttonPreviousPage";
            this.buttonPreviousPage.Size = new System.Drawing.Size(30, 24);
            this.buttonPreviousPage.TabIndex = 55;
            this.buttonPreviousPage.Text = "<<";
            this.buttonPreviousPage.UseVisualStyleBackColor = true;
            this.buttonPreviousPage.Click += new System.EventHandler(this.buttonPreviousPage_Click);
            // 
            // buttonNextPage
            // 
            this.buttonNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNextPage.Enabled = false;
            this.buttonNextPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNextPage.Location = new System.Drawing.Point(716, 260);
            this.buttonNextPage.Name = "buttonNextPage";
            this.buttonNextPage.Size = new System.Drawing.Size(30, 24);
            this.buttonNextPage.TabIndex = 54;
            this.buttonNextPage.Text = ">>";
            this.buttonNextPage.UseVisualStyleBackColor = true;
            this.buttonNextPage.Click += new System.EventHandler(this.buttonNextPage_Click);
            // 
            // labelHandlersToFetch
            // 
            this.labelHandlersToFetch.AutoSize = true;
            this.labelHandlersToFetch.Location = new System.Drawing.Point(597, 23);
            this.labelHandlersToFetch.Name = "labelHandlersToFetch";
            this.labelHandlersToFetch.Size = new System.Drawing.Size(97, 13);
            this.labelHandlersToFetch.TabIndex = 53;
            this.labelHandlersToFetch.Text = "Handlers per page:";
            // 
            // comboHandlersToFetch
            // 
            this.comboHandlersToFetch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboHandlersToFetch.FormattingEnabled = true;
            this.comboHandlersToFetch.Items.AddRange(new object[] {
            "5",
            "25",
            "50",
            "100",
            "200"});
            this.comboHandlersToFetch.Location = new System.Drawing.Point(695, 19);
            this.comboHandlersToFetch.Name = "comboHandlersToFetch";
            this.comboHandlersToFetch.Size = new System.Drawing.Size(51, 21);
            this.comboHandlersToFetch.TabIndex = 52;
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonExit.Location = new System.Drawing.Point(931, 260);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(91, 36);
            this.buttonExit.TabIndex = 51;
            this.buttonExit.Text = "E&xit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // textHandlerExtension
            // 
            this.textHandlerExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textHandlerExtension.Location = new System.Drawing.Point(838, 72);
            this.textHandlerExtension.Name = "textHandlerExtension";
            this.textHandlerExtension.Size = new System.Drawing.Size(184, 20);
            this.textHandlerExtension.TabIndex = 47;
            this.textHandlerExtension.TextChanged += new System.EventHandler(this.textHandlerExtension_TextChanged);
            // 
            // textHandlerDisplayName
            // 
            this.textHandlerDisplayName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textHandlerDisplayName.Location = new System.Drawing.Point(838, 46);
            this.textHandlerDisplayName.Name = "textHandlerDisplayName";
            this.textHandlerDisplayName.Size = new System.Drawing.Size(184, 20);
            this.textHandlerDisplayName.TabIndex = 46;
            this.textHandlerDisplayName.TextChanged += new System.EventHandler(this.textHandlerDisplayName_TextChanged);
            // 
            // labelExtension
            // 
            this.labelExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelExtension.AutoSize = true;
            this.labelExtension.Location = new System.Drawing.Point(776, 75);
            this.labelExtension.Name = "labelExtension";
            this.labelExtension.Size = new System.Drawing.Size(56, 13);
            this.labelExtension.TabIndex = 43;
            this.labelExtension.Text = "Extension:";
            // 
            // labelDisplayName
            // 
            this.labelDisplayName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDisplayName.AutoSize = true;
            this.labelDisplayName.Location = new System.Drawing.Point(757, 49);
            this.labelDisplayName.Name = "labelDisplayName";
            this.labelDisplayName.Size = new System.Drawing.Size(75, 13);
            this.labelDisplayName.TabIndex = 42;
            this.labelDisplayName.Text = "Display Name:";
            // 
            // labelHandlerCountValue
            // 
            this.labelHandlerCountValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHandlerCountValue.Location = new System.Drawing.Point(597, 264);
            this.labelHandlerCountValue.Name = "labelHandlerCountValue";
            this.labelHandlerCountValue.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelHandlerCountValue.Size = new System.Drawing.Size(79, 16);
            this.labelHandlerCountValue.TabIndex = 37;
            this.labelHandlerCountValue.Text = "0";
            this.labelHandlerCountValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHandlerSelection
            // 
            this.lblHandlerSelection.AutoSize = true;
            this.lblHandlerSelection.Location = new System.Drawing.Point(13, 23);
            this.lblHandlerSelection.Name = "lblHandlerSelection";
            this.lblHandlerSelection.Size = new System.Drawing.Size(133, 13);
            this.lblHandlerSelection.TabIndex = 36;
            this.lblHandlerSelection.Text = "Handler Selection Options:";
            // 
            // textHandlerFilterText
            // 
            this.textHandlerFilterText.Location = new System.Drawing.Point(352, 20);
            this.textHandlerFilterText.MaxLength = 32;
            this.textHandlerFilterText.Name = "textHandlerFilterText";
            this.textHandlerFilterText.Size = new System.Drawing.Size(153, 20);
            this.textHandlerFilterText.TabIndex = 35;
            // 
            // comboHandlerFilterAction
            // 
            this.comboHandlerFilterAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboHandlerFilterAction.FormattingEnabled = true;
            this.comboHandlerFilterAction.Items.AddRange(new object[] {
            "Is",
            "StartsWith"});
            this.comboHandlerFilterAction.Location = new System.Drawing.Point(252, 19);
            this.comboHandlerFilterAction.Name = "comboHandlerFilterAction";
            this.comboHandlerFilterAction.Size = new System.Drawing.Size(94, 21);
            this.comboHandlerFilterAction.TabIndex = 34;
            // 
            // comboHandlerFilterElement
            // 
            this.comboHandlerFilterElement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboHandlerFilterElement.FormattingEnabled = true;
            this.comboHandlerFilterElement.Items.AddRange(new object[] {
            "{All Handlers}",
            "DisplayName",
            "DTMFAccessId"});
            this.comboHandlerFilterElement.Location = new System.Drawing.Point(147, 19);
            this.comboHandlerFilterElement.Name = "comboHandlerFilterElement";
            this.comboHandlerFilterElement.Size = new System.Drawing.Size(99, 21);
            this.comboHandlerFilterElement.TabIndex = 33;
            this.comboHandlerFilterElement.SelectedIndexChanged += new System.EventHandler(this.comboHandlerFilterElement_SelectedIndexChanged);
            // 
            // gridHandlers
            // 
            this.gridHandlers.AllowUserToAddRows = false;
            this.gridHandlers.AllowUserToDeleteRows = false;
            this.gridHandlers.AllowUserToOrderColumns = true;
            this.gridHandlers.AllowUserToResizeRows = false;
            this.gridHandlers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridHandlers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gridHandlers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridHandlers.Location = new System.Drawing.Point(13, 46);
            this.gridHandlers.MultiSelect = false;
            this.gridHandlers.Name = "gridHandlers";
            this.gridHandlers.ReadOnly = true;
            this.gridHandlers.RowHeadersVisible = false;
            this.gridHandlers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridHandlers.ShowEditingIcon = false;
            this.gridHandlers.Size = new System.Drawing.Size(733, 208);
            this.gridHandlers.TabIndex = 32;
            this.gridHandlers.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.gridHandlers_RowValidating);
            this.gridHandlers.SelectionChanged += new System.EventHandler(this.gridHandlers_SelectionChanged);
            // 
            // chkIsPrimary
            // 
            this.chkIsPrimary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkIsPrimary.AutoSize = true;
            this.chkIsPrimary.Enabled = false;
            this.chkIsPrimary.Location = new System.Drawing.Point(838, 98);
            this.chkIsPrimary.Name = "chkIsPrimary";
            this.chkIsPrimary.Size = new System.Drawing.Size(175, 17);
            this.chkIsPrimary.TabIndex = 58;
            this.chkIsPrimary.Text = "Is primary (assoicated with user)";
            this.chkIsPrimary.UseVisualStyleBackColor = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "WAV files | *.wav";
            this.saveFileDialog.Title = "Select a location to save the WAV file to.";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "WAV Files|*.wav";
            this.openFileDialog.Title = "Select voice name WAV file";
            // 
            // buttonShowInputKeys
            // 
            this.buttonShowInputKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonShowInputKeys.Image = global::CUPIFastStart.Properties.Resources.hang_up_32;
            this.buttonShowInputKeys.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonShowInputKeys.Location = new System.Drawing.Point(400, 260);
            this.buttonShowInputKeys.Name = "buttonShowInputKeys";
            this.buttonShowInputKeys.Size = new System.Drawing.Size(96, 36);
            this.buttonShowInputKeys.TabIndex = 59;
            this.buttonShowInputKeys.Text = "        Input Keys";
            this.buttonShowInputKeys.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonShowInputKeys.UseVisualStyleBackColor = true;
            this.buttonShowInputKeys.Click += new System.EventHandler(this.buttonShowInputKeys_Click);
            // 
            // buttonSetVoiceName
            // 
            this.buttonSetVoiceName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSetVoiceName.Image = global::CUPIFastStart.Properties.Resources.microphone_up_32;
            this.buttonSetVoiceName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSetVoiceName.Location = new System.Drawing.Point(303, 260);
            this.buttonSetVoiceName.Name = "buttonSetVoiceName";
            this.buttonSetVoiceName.Size = new System.Drawing.Size(91, 36);
            this.buttonSetVoiceName.TabIndex = 57;
            this.buttonSetVoiceName.Text = "Put Name";
            this.buttonSetVoiceName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonSetVoiceName.UseVisualStyleBackColor = true;
            this.buttonSetVoiceName.Click += new System.EventHandler(this.buttonSetVoiceName_Click);
            // 
            // buttonFetchVoiceName
            // 
            this.buttonFetchVoiceName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonFetchVoiceName.Image = global::CUPIFastStart.Properties.Resources.microphone_down_32;
            this.buttonFetchVoiceName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFetchVoiceName.Location = new System.Drawing.Point(206, 260);
            this.buttonFetchVoiceName.Name = "buttonFetchVoiceName";
            this.buttonFetchVoiceName.Size = new System.Drawing.Size(91, 36);
            this.buttonFetchVoiceName.TabIndex = 56;
            this.buttonFetchVoiceName.Text = "Get Name";
            this.buttonFetchVoiceName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonFetchVoiceName.UseVisualStyleBackColor = true;
            this.buttonFetchVoiceName.Click += new System.EventHandler(this.buttonFetchVoiceName_Click);
            // 
            // buttonAddItem
            // 
            this.buttonAddItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddItem.Image = global::CUPIFastStart.Properties.Resources.more_24;
            this.buttonAddItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAddItem.Location = new System.Drawing.Point(12, 260);
            this.buttonAddItem.Name = "buttonAddItem";
            this.buttonAddItem.Size = new System.Drawing.Size(91, 36);
            this.buttonAddItem.TabIndex = 48;
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
            this.buttonRemoveItem.Location = new System.Drawing.Point(109, 260);
            this.buttonRemoveItem.Name = "buttonRemoveItem";
            this.buttonRemoveItem.Size = new System.Drawing.Size(91, 36);
            this.buttonRemoveItem.TabIndex = 49;
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
            this.buttonUpdateItem.Location = new System.Drawing.Point(838, 122);
            this.buttonUpdateItem.Name = "buttonUpdateItem";
            this.buttonUpdateItem.Size = new System.Drawing.Size(91, 36);
            this.buttonUpdateItem.TabIndex = 50;
            this.buttonUpdateItem.Text = "&Update  ";
            this.buttonUpdateItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonUpdateItem.UseVisualStyleBackColor = true;
            this.buttonUpdateItem.Click += new System.EventHandler(this.buttonUpdateItem_Click);
            // 
            // buttonShowHandlers
            // 
            this.buttonShowHandlers.Image = global::CUPIFastStart.Properties.Resources.binoculars_32;
            this.buttonShowHandlers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonShowHandlers.Location = new System.Drawing.Point(511, 12);
            this.buttonShowHandlers.Name = "buttonShowHandlers";
            this.buttonShowHandlers.Size = new System.Drawing.Size(82, 32);
            this.buttonShowHandlers.TabIndex = 31;
            this.buttonShowHandlers.Text = "Find   ";
            this.buttonShowHandlers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonShowHandlers.UseVisualStyleBackColor = true;
            this.buttonShowHandlers.Click += new System.EventHandler(this.buttonShowHandlers_Click);
            // 
            // FormCallHandlerFunctions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 308);
            this.Controls.Add(this.buttonShowInputKeys);
            this.Controls.Add(this.chkIsPrimary);
            this.Controls.Add(this.buttonSetVoiceName);
            this.Controls.Add(this.buttonFetchVoiceName);
            this.Controls.Add(this.buttonPreviousPage);
            this.Controls.Add(this.buttonNextPage);
            this.Controls.Add(this.labelHandlersToFetch);
            this.Controls.Add(this.comboHandlersToFetch);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonAddItem);
            this.Controls.Add(this.buttonRemoveItem);
            this.Controls.Add(this.buttonUpdateItem);
            this.Controls.Add(this.textHandlerExtension);
            this.Controls.Add(this.textHandlerDisplayName);
            this.Controls.Add(this.labelExtension);
            this.Controls.Add(this.labelDisplayName);
            this.Controls.Add(this.labelHandlerCountValue);
            this.Controls.Add(this.lblHandlerSelection);
            this.Controls.Add(this.textHandlerFilterText);
            this.Controls.Add(this.comboHandlerFilterAction);
            this.Controls.Add(this.comboHandlerFilterElement);
            this.Controls.Add(this.gridHandlers);
            this.Controls.Add(this.buttonShowHandlers);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1050, 284);
            this.Name = "FormCallHandlerFunctions";
            this.Text = "Call Handler Functions";
            this.Load += new System.EventHandler(this.FormCallHandlerFunctions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridHandlers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSetVoiceName;
        private System.Windows.Forms.Button buttonFetchVoiceName;
        private System.Windows.Forms.Button buttonPreviousPage;
        private System.Windows.Forms.Button buttonNextPage;
        private System.Windows.Forms.Label labelHandlersToFetch;
        private System.Windows.Forms.ComboBox comboHandlersToFetch;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonAddItem;
        private System.Windows.Forms.Button buttonRemoveItem;
        private System.Windows.Forms.Button buttonUpdateItem;
        private System.Windows.Forms.TextBox textHandlerExtension;
        private System.Windows.Forms.TextBox textHandlerDisplayName;
        private System.Windows.Forms.Label labelExtension;
        private System.Windows.Forms.Label labelDisplayName;
        private System.Windows.Forms.Label labelHandlerCountValue;
        private System.Windows.Forms.Label lblHandlerSelection;
        private System.Windows.Forms.TextBox textHandlerFilterText;
        private System.Windows.Forms.ComboBox comboHandlerFilterAction;
        private System.Windows.Forms.ComboBox comboHandlerFilterElement;
        private System.Windows.Forms.DataGridView gridHandlers;
        private System.Windows.Forms.Button buttonShowHandlers;
        private System.Windows.Forms.CheckBox chkIsPrimary;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button buttonShowInputKeys;
    }
}