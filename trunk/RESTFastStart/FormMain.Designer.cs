namespace CUPIFastStart
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonUserFunctions = new System.Windows.Forms.Button();
            this.buttonCallHandlerFunctions = new System.Windows.Forms.Button();
            this.buttonSystemListFunctions = new System.Windows.Forms.Button();
            this.labelServerName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.textServerName = new System.Windows.Forms.TextBox();
            this.textVersion = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewCurrentLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableDebugOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHTTPTrafficToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonUserFunctions
            // 
            this.buttonUserFunctions.Location = new System.Drawing.Point(12, 87);
            this.buttonUserFunctions.Name = "buttonUserFunctions";
            this.buttonUserFunctions.Size = new System.Drawing.Size(108, 33);
            this.buttonUserFunctions.TabIndex = 0;
            this.buttonUserFunctions.Text = "Users";
            this.buttonUserFunctions.UseVisualStyleBackColor = true;
            this.buttonUserFunctions.Click += new System.EventHandler(this.buttonUserFunctions_Click);
            // 
            // buttonCallHandlerFunctions
            // 
            this.buttonCallHandlerFunctions.Location = new System.Drawing.Point(12, 126);
            this.buttonCallHandlerFunctions.Name = "buttonCallHandlerFunctions";
            this.buttonCallHandlerFunctions.Size = new System.Drawing.Size(108, 33);
            this.buttonCallHandlerFunctions.TabIndex = 1;
            this.buttonCallHandlerFunctions.Text = "Call Handlers";
            this.buttonCallHandlerFunctions.UseVisualStyleBackColor = true;
            this.buttonCallHandlerFunctions.Click += new System.EventHandler(this.buttonCallHandlerFunctions_Click);
            // 
            // buttonSystemListFunctions
            // 
            this.buttonSystemListFunctions.Location = new System.Drawing.Point(12, 165);
            this.buttonSystemListFunctions.Name = "buttonSystemListFunctions";
            this.buttonSystemListFunctions.Size = new System.Drawing.Size(108, 33);
            this.buttonSystemListFunctions.TabIndex = 2;
            this.buttonSystemListFunctions.Text = "System Lists";
            this.buttonSystemListFunctions.UseVisualStyleBackColor = true;
            this.buttonSystemListFunctions.Click += new System.EventHandler(this.buttonSystemListFunctions_Click);
            // 
            // labelServerName
            // 
            this.labelServerName.AutoSize = true;
            this.labelServerName.Location = new System.Drawing.Point(12, 36);
            this.labelServerName.Name = "labelServerName";
            this.labelServerName.Size = new System.Drawing.Size(96, 13);
            this.labelServerName.TabIndex = 4;
            this.labelServerName.Text = "Connection server:";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(60, 57);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(45, 13);
            this.labelVersion.TabIndex = 5;
            this.labelVersion.Text = "Version:";
            // 
            // textServerName
            // 
            this.textServerName.Location = new System.Drawing.Point(111, 33);
            this.textServerName.Name = "textServerName";
            this.textServerName.ReadOnly = true;
            this.textServerName.Size = new System.Drawing.Size(188, 20);
            this.textServerName.TabIndex = 6;
            // 
            // textVersion
            // 
            this.textVersion.Location = new System.Drawing.Point(111, 54);
            this.textVersion.Name = "textVersion";
            this.textVersion.ReadOnly = true;
            this.textVersion.Size = new System.Drawing.Size(188, 20);
            this.textVersion.TabIndex = 7;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(303, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openLogFolderToolStripMenuItem,
            this.viewCurrentLogFileToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openLogFolderToolStripMenuItem
            // 
            this.openLogFolderToolStripMenuItem.Name = "openLogFolderToolStripMenuItem";
            this.openLogFolderToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.openLogFolderToolStripMenuItem.Text = "Open Log Folder";
            this.openLogFolderToolStripMenuItem.Click += new System.EventHandler(this.openLogFolderToolStripMenuItem_Click);
            // 
            // viewCurrentLogFileToolStripMenuItem
            // 
            this.viewCurrentLogFileToolStripMenuItem.Name = "viewCurrentLogFileToolStripMenuItem";
            this.viewCurrentLogFileToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.viewCurrentLogFileToolStripMenuItem.Text = "View Current Log File";
            this.viewCurrentLogFileToolStripMenuItem.Click += new System.EventHandler(this.viewCurrentLogFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(183, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableDebugOutputToolStripMenuItem,
            this.showHTTPTrafficToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // enableDebugOutputToolStripMenuItem
            // 
            this.enableDebugOutputToolStripMenuItem.Name = "enableDebugOutputToolStripMenuItem";
            this.enableDebugOutputToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.enableDebugOutputToolStripMenuItem.Text = "Enable Debug Output";
            this.enableDebugOutputToolStripMenuItem.Click += new System.EventHandler(this.enableDebugOutputToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // showHTTPTrafficToolStripMenuItem
            // 
            this.showHTTPTrafficToolStripMenuItem.Name = "showHTTPTrafficToolStripMenuItem";
            this.showHTTPTrafficToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.showHTTPTrafficToolStripMenuItem.Text = "Show HTTP Traffic";
            this.showHTTPTrafficToolStripMenuItem.Click += new System.EventHandler(this.showHTTPTrafficToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 205);
            this.Controls.Add(this.textVersion);
            this.Controls.Add(this.textServerName);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelServerName);
            this.Controls.Add(this.buttonSystemListFunctions);
            this.Controls.Add(this.buttonCallHandlerFunctions);
            this.Controls.Add(this.buttonUserFunctions);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "CUPI Fast Start";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonUserFunctions;
        private System.Windows.Forms.Button buttonCallHandlerFunctions;
        private System.Windows.Forms.Button buttonSystemListFunctions;
        private System.Windows.Forms.Label labelServerName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TextBox textServerName;
        private System.Windows.Forms.TextBox textVersion;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewCurrentLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableDebugOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showHTTPTrafficToolStripMenuItem;

    }
}

