#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Cisco.UnityConnection.RestFunctions;
using SimpleLogger;

namespace PWReset
{
    /// <summary>
    /// This form allows a help desk admin to select a user and reset their PIN - this is a minimalistic approach to providing limited 
    /// access to CUPI for task-specific purposes.
    /// </summary>
    public partial class FormUserFunctions : Form
    {
        //used to keep track of paging through result sets of users
        private int _currentPage=0;
        private int _currentTotalItems = 0;

        public FormUserFunctions()
        {
            InitializeComponent();
        }

        //user issues a new user filter request.
        private void buttonShowUsers_Click(object sender, EventArgs e)
        {
            //indicate we're starting a new fetch, not moving through paged results for an existing fetch.
            _currentPage = 0;
            _currentTotalItems = 0;

            //main user data fetching function for the form.
            UpdateDataDisplay();
        }

        //Main routine that fetches the user data, dispays it in the grid and binds the textboxes for editing to the resulting
        //data set.  All filtering of data is also handled in this routine if any filters are set.
        //This takes an optional ObjectId parameter - if supplied it will load just that one user into the grid.  This is designed
        //for adding new users so we can display the user just added - probably not the best UI design but you get what you pay for.
        private void UpdateDataDisplay(string pObjectID="")
        {
            string strQuery="";
            WebCallResult res;
            int iRowsPerPage = 0;

            //get the user data from the remote Connection server as a list of User objects - remember these are "light" users 
            //that contain a smaller set of data than a "UserFull" - this is designed for list presentation and the like, although
            //in this sample application we're just showing everything, typically you'd hide all the ObjectId values.
            List<Cisco.UnityConnection.RestFunctions.UserBase> oUsers;

            //fetch the number of users to return in a query - you'll want to keep this reasonable in most cases as a very large 
            //result set can timeout on you if the server is busy.
            if (int.TryParse(comboUsersToFetch.Text,out iRowsPerPage)==false)
            {
                //oops!
                MessageBox.Show("Invalid user count selection value encountered in UpdateDataDisplay:" +
                                comboUsersToFetch.Text);
                return;
            }

            //if the objectID was not passed in, fetch the user data using the filter information on the form (or all users)
            if (pObjectID.Length == 0)
            {
                //check if any filters are set - if they are we pass them in as a filter clause on the GetUsers call, otherwise we pass blank 
                //which means get all users (or the first batch in a paged fetch at any rate.
                if (comboUserFilterElement.SelectedIndex > 0)
                {
                    //trim out any white space first
                    textUserFilterText.Text = textUserFilterText.Text.Trim();
                    
                    if (string.IsNullOrEmpty(textUserFilterText.Text))
                    {
                        MessageBox.Show("You must enter at least one character to search against for your query.");
                        textUserFilterText.Focus();
                        return;
                    }

                    strQuery = string.Format("query=({0} {1} {2})",GetSearchClauseFromPrettyName(comboUserFilterElement.Text), comboUserFilterAction.Text,
                                      textUserFilterText.Text).ToLower();

                }

                //if we're paging through a result set, incrament the count for the current page by 1 - if this is a single set or the first page the
                //_currentPage++ is set to 0 and this runs it to 1 which is the first page (it's 1 based, not zero based).
                _currentPage++;
                if (strQuery.Length>0)
                {
                    strQuery += "&";
                }

                //limit the rows returned to what's selected on the form's drop down control
                strQuery += string.Format("rowsPerPage={0}&pageNumber={1}", iRowsPerPage, _currentPage);
            }
            else
            {
                //single ObjectId was passed in - use a simpler query - yes, you can also do this by constructing a URI in the form of:
                //"{server name}\vmrest\users\{objectid}" - however the XML that's returned contains much more data than we need for our 
                //display grid here.
                strQuery = string.Format("query=({0} {1} {2})", "ObjectId", "is", pObjectID);
                _currentPage = 0;
            }

            //fetching the data via HTTP can take a bit - disable the controls on the form until the fetch returns.  A more sophisticated
            //background thread approach to fetching data is beyond the scope of this framework.
            DisableFormControls();

            res = UserBase.GetUsers(GlobalItems.CurrentConnectionServer, out oUsers, strQuery);

            EnableFormControls();

            if (res.Success == false)
            {
                Logger.Log("Error fetching users in UpdateDataDisplay on FormUSerFunctions.cs");
                
                //dump all the details from the return structure to the log for review, however don't put anything up on the 
                //screen since if no matches are found on a search this can return an error
                Logger.Log(res.ToString());
                //MessageBox.Show("Error encountered fetching users:" + res.ErrorText);
                _currentPage = 0;
                _currentTotalItems = 0;
            }

            //update the total number of users returned in the query (not the number of users returned in the result set).
            _currentTotalItems = res.TotalObjectCount;

            //unbind the grid
            this.gridUsers.DataSource = null;

            if (oUsers !=null)
            {
                //update the text and buttons for our paging mechanism under the grid.
                UpdatePagingDetails(iRowsPerPage, res.TotalObjectCount);

                //set the grid to bind to the list of users as it's source.  This is a read only display 
                //operation so there's no need to use a dataset or the like here.
                this.gridUsers.DataSource = oUsers;
                this.gridUsers.AutoGenerateColumns = false;
                UpdateGridDisplay();
            }
            else
            {
                this.LabelUserCountValue.Text = "0";
            }
                
            //bind the text controls on the form to the list - this works just like binding 
            //them to a data table in that they update automatically when you move the selected
            //row in the grid.
            textUserAlias.DataBindings.Clear();
            textUserDisplayName.DataBindings.Clear();
            textUserExtension.DataBindings.Clear();
            textUserFirstName.DataBindings.Clear();
            textUserLastName.DataBindings.Clear();

            //if data was returned bind the controls to the result sets and they get updated automatically when the currently
            //selected item is changed - if not results are returned be sure to blank out the control so they don't hold what 
            //might have been in there earlier.
            if (oUsers != null)
            {
                textUserAlias.DataBindings.Add("Text", oUsers, "Alias", false);
                textUserDisplayName.DataBindings.Add("Text", oUsers, "DisplayName", false);
                textUserExtension.DataBindings.Add("Text", oUsers, "DTMFAccessID", false);
                textUserFirstName.DataBindings.Add("Text", oUsers, "FirstName", false);
                textUserLastName.DataBindings.Add("Text", oUsers, "LastName", false);
            }
            else
            {
                textUserAlias.Text = "";
                textUserDisplayName.Text = "";
                textUserExtension.Text = "";
                textUserFirstName.Text = "";
                textUserLastName.Text = "";
            }

            this.gridUsers.Refresh();
            System.Threading.Thread.Sleep(1);

        }


        //Returns the search item from the pretty name used in the drop down list for search selection.  For instance 
        //'Primary Extension' is replaced with 'DTMFAccessId' for searching purposes.
        private string GetSearchClauseFromPrettyName(string pPrettyName)
        {
            switch (pPrettyName.ToUpper()) 
            {
                case "PRIMARY EXTENSION":
                    return "DTMFAccessId";
                case "FIRST NAME":
                    return "FirstName";
                case "LAST NAME":
                    return "LastName";
                case "DISPLAY NAME":
                    return "DisplayName";
                case "ALIAS":
                    return "Alias";
                default:
                    MessageBox.Show("Invalid search string passed to GetSearchClauseFromPrettyName:" + pPrettyName);
                    return "";
            }
        }


        //helper function to update the text and button controls for our paging display on the form.  It makes sure the next/previous
        //buttons are enabled/disabled as necessary and the the current position is properly reflected in the label at the bottom of 
        //the grid
        private void UpdatePagingDetails(int pRowsPerPage, int pTotalObjectCount)
        {
            //show the user range we're currently showing in the grid.  Make sure to enable/disable the next/previous
            //buttons as appropriate to be sure we don't fly off the ends of the list - the CUPI interface will throw an 
            //error if you send an invalid page number.
            int iStart = pRowsPerPage * (_currentPage - 1);
            if (iStart < 1) iStart = 1;
            buttonPreviousPage.Enabled = iStart != 1;

            //calculate the end - making sure to not advertise past the max count.  Be sure to disable the next button
            //if we're at the end of the list of results.
            int iEnd = (pRowsPerPage * _currentPage);

            if (iEnd >= pTotalObjectCount)
            {
                iEnd = pTotalObjectCount;
                buttonNextPage.Enabled = false;
            }
            else
            {
                buttonNextPage.Enabled = true;
            }

            if (iEnd < 1) iEnd = 1;

            this.LabelUserCountValue.Text = string.Format("{0}-{1} of {2}", iStart, iEnd, pTotalObjectCount);
        }

        //Updates the display of the data in the grid - mostly this just hides the unnecessary columns in the grid such as the 
        //ObjectId columns and the like - it can be expanded for coloration, making the column headers pretty etc... but for now 
        //it just limits what data is shown to the more useful items.
        private void UpdateGridDisplay()
        {
            //if the grid is not currently bound or populated, exit out
            if (gridUsers.DataSource==null || gridUsers.Rows.Count<1)
            {
                return;
            }

            gridUsers.Columns["CallHandlerObjectId"].Visible = false;
            gridUsers.Columns["COSObjectId"].Visible = false;
            gridUsers.Columns["ObjectId"].Visible = false;
            gridUsers.Columns["LocationObjectId"].Visible = false;
            gridUsers.Columns["MediaSwitchObjectId"].Visible = false;
            gridUsers.Columns["PartitionObjectId"].Visible = false;
        }


        //since the HTTP fetch can take a little time, disable the input controls on the form while we are waiting
        private void DisableFormControls()
        {
            buttonShowUsers.Enabled = false;
            buttonResetPIN.Enabled = false;

            this.Cursor = Cursors.WaitCursor;
        }

        //re enable all controls when a long operation is complete - the UpdateItem button is left disabled since this gets
        //enabled when a user manually changes a value in one of the bound text boxes for user data on the right of the form.
        private void EnableFormControls()
        {
            buttonShowUsers.Enabled = true;
            buttonResetPIN.Enabled = true;

            this.Cursor = Cursors.Default;
        }

        //at form load time force the filtering combo boxes to have their first item (index 0) selected and showing.  By default
        //these show blank.
        private void FormUserFunctions_Load(object sender, EventArgs e)
        {
            Logger.Log("Starting REST Fast Start application.");

            //the login form authenticates against a remote Connection server and gets the version information and a few other items
            //off of it for us.  It will not return if there is not a successful attachment, it will terminate the applciation instead so 
            //there's no need to check for that here - if you plan to connect to multiple Connection servers in one application you'll of 
            //course need to rework the connection logic here - this sample application is intended to keep this simple and allow for only
            //one connection.
            using (FormLogin oLogin = new FormLogin())
            {
                oLogin.ShowDialog();
            }

            //populate server info on the forms title
            this.Text = "Password Reset for " + GlobalItems.CurrentConnectionServer.ServerName;

            Logger.Log("Attached to Connection server=" + GlobalItems.CurrentConnectionServer.ServerName);
            Logger.Log("Connection server version=" + GlobalItems.CurrentConnectionServer.Version.ToString());

            if (GlobalItems.CurrentConnectionServer.Version.IsVersionAtLeast(8, 5, 0, 0) == false)
            {
                MessageBox.Show("This test application was written and tested against Connection 8.5 and later, an earlier version is detected."
                                + "  Some funtions may not work properly, it's suggested you test against 8.5 or later.");
            }

            
            comboUserFilterAction.SelectedIndex = 0;
            comboUserFilterElement.SelectedIndex = 0;
            comboUsersToFetch.SelectedIndex = 0;

        }

        //adjust the user filter options based on the first element selection combo box value.
        //Index 0 is "all users" and so the other two items can be disabled.  Otherwise it's 
        //a property name to filter against for users so the other two items need to be enabled
        //to allow for user input.
        private void comboUserFilterElement_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboUserFilterElement.SelectedIndex ==0)
            {
                this.comboUserFilterAction.Enabled = false;
                this.textUserFilterText.Enabled = false;
            }
            else
            {
                this.comboUserFilterAction.Enabled = true;
                this.textUserFilterText.Enabled = true;
            }
        }

        //close the dialog - the form object will be disposed by the calling method.
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        //load the next set of results in a multiple page result set of users.
        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            //UpdateDataDisplay already incraments the current page by 1 each time through.
            UpdateDataDisplay();
        }

        //load the previous set of results in a multiple page result set of users.
        private void buttonPreviousPage_Click(object sender, EventArgs e)
        {
            //current page is incramented by 1 when it goes through the UpdateDataDisplay routine - to go back a 
            //page we need to go back 2.
            _currentPage = _currentPage - 2;
            UpdateDataDisplay();
        }

        
        //User has selected to reset the PIN for the user currently selected in the grid.
        private void buttonResetPIN_Click(object sender, EventArgs e)
        {
            if (gridUsers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a user to set the PIN for.");
                return;
            }

            //the objectID and alias values will always be in the result set.
            string strObjectID = gridUsers.SelectedRows[0].Cells["ObjectId"].Value.ToString();
            string strAlias = gridUsers.SelectedRows[0].Cells["Alias"].Value.ToString();

            using (FormCollectPIN oForm = new FormCollectPIN())
            {
                if (oForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                //reset the PIN - pass in null's for the flag values we aren't allowing the user to fiddle with so their current
                //values will be left alone.  Easy enough to extend the PIN collection form to allow passing the "can't change" and
                //"locked" values if you want.
                WebCallResult res = UserBase.ResetUserPin(GlobalItems.CurrentConnectionServer,
                    strObjectID, 
                    oForm.NewPIN,
                    false,
                    oForm.MustChange,
                    null,
                    oForm.DoesNotExpire,
                    oForm.ClearHackedLockout);

                if (res.Success)
                {
                    MessageBox.Show("PIN reset for user: " + strAlias);
                }
                else
                {
                    MessageBox.Show(String.Format("PIN reset failed for user: {0}/nError={1}", strAlias,res.ErrorText));
                }
            }

        }


        /// <summary>
        /// Just dump the credential information we have for the user's PIN to a text string and show it in a message box - nothing
        /// fancy here.
        /// </summary>
        private void buttonShowPin_Click(object sender, EventArgs e)
        {
            if (gridUsers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a user to show PIN credentials information for.");
                return;
            }

            string strObjectID = gridUsers.SelectedRows[0].Cells["ObjectId"].Value.ToString();

            UserBase oUser;

            WebCallResult res = UserBase.GetUser(out oUser, GlobalItems.CurrentConnectionServer, strObjectID);
            if (res.Success==false)
            {
                MessageBox.Show("Failed to fetch selected user:" + res.ToString());
                return;
            }

            MessageBox.Show("PIN Credential info:"+Environment.NewLine+ oUser.Pin().DumpAllProps());
        }

    }
}
