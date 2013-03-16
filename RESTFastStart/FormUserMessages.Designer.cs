namespace CUPIFastStart
{
    partial class FormUserMessages
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUserMessages));
            this.gridMessages = new System.Windows.Forms.DataGridView();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonFetchAttachments = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridMessages)).BeginInit();
            this.SuspendLayout();
            // 
            // gridMessages
            // 
            this.gridMessages.AllowUserToAddRows = false;
            this.gridMessages.AllowUserToDeleteRows = false;
            this.gridMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMessages.Location = new System.Drawing.Point(12, 12);
            this.gridMessages.Name = "gridMessages";
            this.gridMessages.ReadOnly = true;
            this.gridMessages.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMessages.Size = new System.Drawing.Size(964, 215);
            this.gridMessages.TabIndex = 0;
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonExit.Location = new System.Drawing.Point(885, 251);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(91, 36);
            this.buttonExit.TabIndex = 24;
            this.buttonExit.Text = "E&xit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonFetchAttachments
            // 
            this.buttonFetchAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFetchAttachments.Image = global::CUPIFastStart.Properties.Resources.microphone_down_32;
            this.buttonFetchAttachments.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFetchAttachments.Location = new System.Drawing.Point(788, 251);
            this.buttonFetchAttachments.Name = "buttonFetchAttachments";
            this.buttonFetchAttachments.Size = new System.Drawing.Size(91, 36);
            this.buttonFetchAttachments.TabIndex = 29;
            this.buttonFetchAttachments.Text = "Get Media";
            this.buttonFetchAttachments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonFetchAttachments.UseVisualStyleBackColor = true;
            this.buttonFetchAttachments.Click += new System.EventHandler(this.buttonFetchAttachments_Click);
            // 
            // FormUserMessages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 299);
            this.Controls.Add(this.buttonFetchAttachments);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.gridMessages);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormUserMessages";
            this.Text = "User Messages";
            this.Load += new System.EventHandler(this.FormUserMessages_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridMessages)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridMessages;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonFetchAttachments;
    }
}