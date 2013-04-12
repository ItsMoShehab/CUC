#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Windows.Forms;
using Cisco.UnityConnection.RestFunctions;

namespace PWReset
{
    /// <summary>
    /// This form does the initial attachment to a remote Unity Connection server.  Most notably it's responsible for creating the global 
    /// instance of the ConnectionServer object that's used for all communication with the Connection server by other classes in this application.
    /// This instance is stored in GlobalItems.CurrentConnectionServer so it can be referenced, as the static class name implies, globally.
    /// Exiting out of this form without properly logging into a Connection server means the application itself exits entirely.
    /// </summary>
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        //user hit the cancel button - close the form - checks are made on form close for exiting applciation.
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Do the login and close the form out if it succeeds, put up an error dialog and give the user the chance to try again if not.
        //Failing to login either by canceling or closing this dialog without attaching properly means the application exits entirely.
        private void buttonOK_Click(object sender, EventArgs e)
        {
            //first make sure everything is filled in - if not bark at the user about it and set the focus on the offending field.
            if (this.txtServerName.Text.Length == 0)
            {
                this.txtServerName.Focus();
                MessageBox.Show("You must provide a server name");
                return;
            }

            if (this.txtUserName.Text.Length == 0)
            {
                this.txtUserName.Focus();
                MessageBox.Show("You must provide a user name");
                return;
            }

            if (this.txtPassword.Text.Length == 0)
            {
                this.txtPassword.Focus();
                MessageBox.Show("You must provide a password");
                return;
            }

            //The authentication can take a second, disable the controls on the form and show the wait cursor while this is working.
            //It's left as an exercise for the reader to do this on a background thread instead.
            DisableAllControls();

            try
            {
                GlobalItems.CurrentConnectionServer = new ConnectionServer(txtServerName.Text, txtUserName.Text, txtPassword.Text);
            }
            catch
            {
                MessageBox.Show("Login failed, make sure the server name is valid, DNS is working properly and the user name and login are valid");
                EnableAllControls();
                return;
            }

            if (GlobalItems.CurrentConnectionServer.ServerName.Length==0)
            {
              	//login failed = give the user the chance to try again.
                MessageBox.Show("Login failed, make sure the server name is valid, DNS is working properly and the user name and login are valid");
                EnableAllControls();
                return;
            }

            //if we're here then the login went through and we can close out this form.  
            //The global CurrentConnectionServer object will be used for communicating with this Connection server in the rest of 
            //the application.  Save this server name in the XML settings and default to it when we run again.  If you want to get fancy
            //you can change this to a list function and keep track of all the servers you've connected to and even the login/PW pairs for 
            //each, however that's beyond the scope of this sample application.
            Properties.Settings.Default.LastServerName = txtServerName.Text;
            Properties.Settings.Default.LastLoginName = txtUserName.Text;
            Properties.Settings.Default.Save();

            //indicate the user has hit ok and the login is valid
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        //when we login it may take a few seconds - disable the controls while we're waiting, 
        //prevents the user from playing around with controls they shouldn't be.
        private void DisableAllControls()
        {
            this.Cursor = Cursors.WaitCursor;
            this.buttonOK.Enabled = false;
            this.txtServerName.Enabled = false;
            this.txtPassword.Enabled = false;
            this.txtUserName.Enabled = false;
            Refresh();
        }

        //enable the controls on the form again.
        private void EnableAllControls()
        {
            this.Cursor = Cursors.Default;
            this.buttonOK.Enabled = true;
            this.txtServerName.Enabled = true;
            this.txtPassword.Enabled = true;
            this.txtUserName.Enabled = true;
        }

        //if there's a server name stored in the settings for the LastServerName property then default to that on the 
        //login form.   Need to check for the existence of the values before attempting to apply them, there's no "just use
        //blank if it's not there" option.
        private void FormLogin_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.LastServerName.Length>0)
            {
                this.txtServerName.Text = Properties.Settings.Default.LastServerName;
            }

            if (Properties.Settings.Default.LastServerName.Length>0)
            {
                this.txtUserName.Text = Properties.Settings.Default.LastLoginName;
            }
        }

        //when the form finishes drawing set the focus appropriately - this wont work in the load routine since the controls cannot
        //yet get focus until the form is shown.
        private void FormLogin_Shown(object sender, EventArgs e)
        {
            //set the focus on the first control from the top that doesn't have any text in it.
            if (String.IsNullOrEmpty(txtServerName.Text))
            {
                txtServerName.Focus();
            }
            else if (string.IsNullOrEmpty(txtUserName.Text))
            {
                txtUserName.Focus();
            }
            else
            {
                txtPassword.Focus();
            }
        }

        //if the user closed the form without logging in using the OK button, exit the application.
        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            //the the user hit OK the dialog is closing but the result will be set to "OK" so make sure to account 
            //for that here.
            if (this.DialogResult != DialogResult.OK)
            {
                //for this sample applicaiton canceling the login is the same as exiting the application.
                if (MessageBox.Show("Are you sure you want to abort the login?  This will exit the application.", "Cancel login",
                                    MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    //user changed their mind - cancel out and return.
                    e.Cancel = true;
                    return;
                }

                //exit the application
                GlobalItems.FinalExit();
            }
        }

    }
}
