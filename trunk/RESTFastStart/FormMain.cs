#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using SimpleLogger;

namespace CUPIFastStart
{
    public partial class FormMain : Form
    {

        public FormMain()
        {
            InitializeComponent();
        }

        //at load up time start the logging function which will create a new log file in the temporary folder for the currently
        //logged in UserControl.
        private void FormMain_Load(object sender, EventArgs e)
        {
            Logger.Log("Starting REST Fast Start application.");

            //the login form authenticates against a remote Connection server and gets the version information and a few other items
            //off of it for us.  It will not return if there is not a successful attachment, it will terminate the applciation instead so 
            //there's no need to check for that here - if you plan to connect to multiple Connection servers in one application you'll of 
            //course need to rework the connection logic here - this sample application is intended to keep this simple and allow for only
            //one connection.
            using(FormLogin oLogin=new FormLogin())
            {
                oLogin.ShowDialog();
            }

            //populate server info on the form
            textServerName.Text = GlobalItems.CurrentConnectionServer.ServerName;
            textVersion.Text = GlobalItems.CurrentConnectionServer.Version.ToString();

            Logger.Log("Attached to Connection server="+GlobalItems.CurrentConnectionServer.ServerName);
            Logger.Log("Connection server version=" + GlobalItems.CurrentConnectionServer.Version.ToString() );

            if (GlobalItems.CurrentConnectionServer.Version.IsVersionAtLeast(8,5,0,0)==false)
            {
                MessageBox.Show("This test application was written and tested against Connection 8.5 and later, an earlier version is detected."
                                +"  Some funtions may not work properly, it's suggested you test against 8.5 or later.");
            }

        }

        
        //show the user functions form as a dialog
        private void buttonUserFunctions_Click(object sender, EventArgs e)
        {
            FormUserFunctions oForm = new FormUserFunctions();
            oForm.Show();
        }

        //user wants to exit - confirm before unloading
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?","Confirm Exit",
                MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.No)
            {
                //user changed their minds
                return;
            }

            //exit application
            GlobalItems.FinalExit();

        }

        //if the log file is open/exists then force it to flush and pop it open in notepad for review.
        private void viewCurrentLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(Logger.GetCurrentLogFilePath()))
            {
              	Logger.FlushLog();
                Process.Start("notepad.exe", Logger.GetCurrentLogFilePath());
            }
            else
            {
                //this should never happen, by the time the main form is loaded, logging is active.
                MessageBox.Show("No log file found.");
            }
        }

        
        /// <summary>
        /// open the folder where log files are stored using file exporer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openLogFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(Logger.GetCurrentLogFilePath()))
            {
                Logger.FlushLog();
                Process.Start("explorer.exe", Directory.GetParent(Logger.GetCurrentLogFilePath()).ToString());
            }
            else
            {
                //this should never happen, by the time the main form is loaded, logging is active.
                MessageBox.Show("No log file folder found.");
            }

        }

        /// <summary>
        /// pop simple about box
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox oAbout = new AboutBox())
            {
                oAbout.Show();
            }
        }

        
        /// <summary>
        /// enable or disable debug output for the logger
        /// </summary>
        private void enableDebugOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableDebugOutputToolStripMenuItem.Checked = !enableDebugOutputToolStripMenuItem.Checked;

            Logger.DebugEnabled = enableDebugOutputToolStripMenuItem.Checked;
        }

        
        /// <summary>
        /// Show call handler functions example form.
        /// </summary>
        private void buttonCallHandlerFunctions_Click(object sender, EventArgs e)
        {
            FormCallHandlerFunctions oForm = new FormCallHandlerFunctions();
            oForm.Show();
        }

        
        /// <summary>
        /// Show distribution list functions example form
        /// </summary>
        private void buttonSystemListFunctions_Click(object sender, EventArgs e)
        {
            FormDistributionListFunctions oForm = new FormDistributionListFunctions();
            oForm.Show();

        }

        /// <summary>
        /// If the user wants to monitor HTTP traffic on the fly then open a floating window for them to do it with a rich text box control on it 
        /// and pass a handle to that RTE to the HTTPFunctions class so it knows to dump inbound and outbound data to that control.
        /// </summary>
        private void showHTTPTrafficToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHttpTraffic oTraffic = new FormHttpTraffic();
            oTraffic.Show();
            Cisco.UnityConnection.RestFunctions.HTTPFunctions.RichTextControlToOutputTo = oTraffic.RichTextBoxOutput;
        }

    }
}
