#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Cisco.UnityConnection.RestFunctions;
using SimpleLogger;

namespace CUPIFastStart
{
    /// <summary>
    /// This form is designed to demonstrate the use of the ConnectionCUPIFunctions class library for doing the most common (and sometimes
    /// tricky) operations for users on the Unity Connection directory.  It shows fetching users, filtering that search, editing top level
    /// user data, deleting users, creating new users, fetching and setting voice names for users and updating PINs.
    /// You can, of course, do all these things with raw HTTP calls which the library lets you do however the wrapped functions provided in 
    /// the User class makes this much easier and quicker if you're working in .NET which this project intends to demonstrate.
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

                    strQuery = string.Format("query=({0} {1} {2})", comboUserFilterElement.Text, comboUserFilterAction.Text,
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
                
                //dump all the details from the return structure to the log for review
                Logger.Log(res.ToString());
                MessageBox.Show("Error encountered fetching users:" + res.ErrorText);
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

            //clear the enabled flag on the update button since it will change when we edit the text fields above.
            buttonUpdateItem.Enabled = false;

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

        //since the HTTP fetch can take a little time, disable the input controls on the form while we are waiting
        private void DisableFormControls()
        {
            buttonShowUsers.Enabled = false;
            buttonAddItem.Enabled = false;
            buttonRemoveItem.Enabled = false;
            buttonUpdateItem.Enabled = false;
            buttonFetchVoiceName.Enabled = false;
            buttonSetVoiceName.Enabled = false;
            buttonResetPIN.Enabled = false;
            buttonShowMessages.Enabled = false;

            this.Cursor = Cursors.WaitCursor;
        }

        //re enable all controls when a long operation is complete - the UpdateItem button is left disabled since this gets
        //enabled when a user manually changes a value in one of the bound text boxes for user data on the right of the form.
        private void EnableFormControls()
        {
            buttonShowUsers.Enabled = true;
            buttonAddItem.Enabled = true;
            buttonRemoveItem.Enabled = true;
            buttonUpdateItem.Enabled = false; //leave the update item disabled
            buttonFetchVoiceName.Enabled = true;
            buttonSetVoiceName.Enabled = true;
            buttonResetPIN.Enabled = true;
            buttonShowMessages.Enabled = true;

            this.Cursor = Cursors.Default;
        }

        //at form load time force the filtering combo boxes to have their first item (index 0) selected and showing.  By default
        //these show blank.
        private void FormUserFunctions_Load(object sender, EventArgs e)
        {
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

        //user edited one of the properties text boxes on the form - enable the update data button to indicate this can
        //update that user's data at this point.
        private void textUserFirstName_TextChanged(object sender, EventArgs e)
        {
            buttonUpdateItem.Enabled = true;
        }

        
        //after the user moves to a new record, disable the update data button.
        private void gridUsers_SelectionChanged(object sender, EventArgs e)
        {
            this.buttonUpdateItem.Enabled = false;
        }

        //when the user tries to move to a new record but there are pending edits outstanding, warn them they will lose this information
        //and give them a chance to cancel out.
        private void gridUsers_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (buttonUpdateItem.Enabled)
            {
                if (MessageBox.Show("Pending edits will be lost, are you sure you want to move to a new record before saving your edits?",
                    "Cancel Pending Edits",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.No)
                {
                    //cancel the move and bail out
                    e.Cancel = true;
                    return;
                }
            }
        }

        //remove the currently selected user in the grid.  If there is no user selected in the grid (i.e. it's empty) then warn the 
        //user and exit.
        private void buttonRemoveItem_Click(object sender, EventArgs e)
        {
            if (gridUsers.SelectedRows.Count<1)
            {
                MessageBox.Show("You must first select a user to delete");
                return;
            }

            //the objectID and alias values will always be in the result set.
            string strObjectID = gridUsers.SelectedRows[0].Cells["ObjectId"].Value.ToString();
            string strAlias = gridUsers.SelectedRows[0].Cells["Alias"].Value.ToString();

            //verify with the user before deleting
            if (MessageBox.Show("Are you sure you want to delete the user:"+strAlias+"?","DELETE Confirmation",
                MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.No)
            {
                //the user changed their minds, bail out
                return;
            }

            DisableFormControls();

            //remove the user
            WebCallResult res = UserBase.DeleteUser(GlobalItems.CurrentConnectionServer, strObjectID);
            
            EnableFormControls();

            if (res.Success)
            {
                MessageBox.Show("User removed");
                Logger.Log(string.Format("User with alias={0} and objectId={1} removed.",strAlias,strObjectID));
                
                //rebuild the grid - if we were traversing a list of users before, be sure to reset the current page to 0 here because we 
                //need to rebuild the list without the guy we just deleted in it.
                _currentPage = 0;
                UpdateDataDisplay();
            }
            else
            {
                MessageBox.Show("Failure removing user: "+res.ErrorText);
                Logger.Log(string.Format("Failed to remove user with alias={0} and objectId={1}.  Error={2}", strAlias, strObjectID,res.ToString()));
            }
        }


        //user wants to update one of the top level items for a user - they may have edited the first/last name, display name, alias or extension (any or all of them).
        //All the values will be sent over regardless of if they've changed - a more sophisticated "dirty flag" approach or the like is left as an exercize for reader.
        private void buttonUpdateItem_Click(object sender, EventArgs e)
        {
            DisableFormControls();
            
            ConnectionPropertyList oPropList = new ConnectionPropertyList();

            string strObjectID = gridUsers.SelectedRows[0].Cells["ObjectId"].Value.ToString();

            //the property names are case sensitive
            oPropList.Add("Alias",textUserAlias.Text);
            oPropList.Add("FirstName", textUserFirstName.Text);
            oPropList.Add("LastName",textUserLastName.Text);
            oPropList.Add("DisplayName",textUserDisplayName.Text);
            oPropList.Add("DtmfAccessId", textUserExtension.Text);

            //do the call to update the items via CUPI
            WebCallResult res = UserBase.UpdateUser(GlobalItems.CurrentConnectionServer, strObjectID, oPropList);

            EnableFormControls();

            if (res.Success)
            {
                MessageBox.Show("User updated");
                Logger.Log(string.Format("User updated:{0}\r\n{1}", strObjectID, oPropList));
                gridUsers.Refresh();
            }
            else
            {
                MessageBox.Show("User failed to update:" + res.ErrorText);
                Logger.Log(string.Format("User failed to update:{0}, error=[{1}]\r\n{2}", strObjectID, res.ErrorText, oPropList));
                
                 //dump out all the results information send from the server for diag purposes
                Logger.Log(res.ToString());
            }

        }


        //helper function that changes the currently selected row in the user grid to match the user ObjectId passed in.  If no user is found with 
        //that ID in the grid then no change is made, otherwise the currently selected row is moved to that value.
        private void MoveGridToUserObjectIDRow(string pObjectID)
        {
            foreach (DataGridViewRow oRow in gridUsers.Rows)
            {
              	if (oRow.Cells["ObjectId"].Value.ToString().Equals(pObjectID))
              	{
                    //the data grid view control isn't the greatest - you have to set the selected property on the row (to get it to highlight) and
                    //also force the current cell to be in the row to get the data update event to trigger (which populates the text controls based
                    //on the current index of the List of User objects the grid is bound to).
                    oRow.Selected = true;
              	    this.gridUsers.CurrentCell = this.gridUsers[1, oRow.Index];
                    gridUsers.Refresh();
              	    
                    //the edit button will enable since the text controls updated - clear it here before returning.
                    this.buttonUpdateItem.Enabled = false;
              	    return;
              	}
            }
        }

        //create a new user - pop a dialog that allows the user to provide an alias, first name, last name, display name and extension - if at least
        //the alias and extension are provided this routine will then attempt to create that user on the Connection server.
        private void buttonAddItem_Click(object sender, EventArgs e)
        {
            //gather the new subscriber data from the user.
            using (FormNewUserInfo oForm = new FormNewUserInfo())
            {
                if (oForm.ShowDialog() != DialogResult.OK)
                {
                    //user canceled or closed the dialog without going through OK - bail out.
                    return;
                }

                //the call can take a minute to return - disable controls on the form to indicate we're busy.
                DisableFormControls();

                ConnectionPropertyList oProps = new ConnectionPropertyList();

                //fill up the props with the other items (if any) that are not blank
                if (oForm.FirstName.Length > 0) oProps.Add("FirstName", oForm.FirstName);
                if (oForm.LastName.Length > 0) oProps.Add("LastName", oForm.LastName);
                if (oForm.DisplayName.Length > 0) oProps.Add("DisplayName", oForm.DisplayName);

                WebCallResult res = UserBase.AddUser(GlobalItems.CurrentConnectionServer, oForm.TemplateAlias, oForm.Alias, oForm.Extension, oProps);

                EnableFormControls();

                if (res.Success)
                {
                  	//force a refresh of the grid and then select the user you just added - the ObjectId of the newly added user is returned 
                    //in the WebCallResult structure as the ReturnObjectID.
                    _currentPage = 0;
                    UpdateDataDisplay(res.ReturnedObjectId);
                    Logger.Log(string.Format("New user added:{0},{1}",oForm.Alias,res.ReturnedObjectId));
                }
                else
                {
                    Logger.Log(string.Format(string.Format("New user add failed:{0}\r\n{1}",oForm.Alias,res.ToString())));
                    MessageBox.Show(String.Format("Error adding user: {0}\nresponse from CUPI: {1}",res.ErrorText,res.ResponseText));
                }
            }

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


        //download the wav file for the users vocie name if they have one recorded.
        private void buttonFetchVoiceName_Click(object sender, EventArgs e)
        {
            WebCallResult res;

            if (gridUsers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a user to fetch the voice name for.");
                return;
            }

            //the objectID and alias values will always be in the result set.
            string strObjectID = gridUsers.SelectedRows[0].Cells["ObjectId"].Value.ToString();
            string strAlias = gridUsers.SelectedRows[0].Cells["Alias"].Value.ToString();

            //make the user pick a location to store the voice name - pre populate the file name with the user's alias
            //to make it unique if they're getting a few into the same location.
            saveFileDialog.FileName = string.Format("{0}_VoiceName.wav", strAlias);
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (saveFileDialog.ShowDialog()==DialogResult.Cancel)
            {
                //user changed their minds
                return;
            }

            //downloading a wav file via HTTP can take a bit - disable the controls to let the user know we're busy.
            DisableFormControls();

            //fetch the voice name off the Connection server and store it at the path provided.
            res = UserBase.GetUserVoiceName(GlobalItems.CurrentConnectionServer, saveFileDialog.FileName, strObjectID);

            EnableFormControls();

            if (res.Success)
            {
                //open the folder in file explorer
                Process.Start("explorer.exe", Directory.GetParent(saveFileDialog.FileName).ToString());
            }
            else
            {
                MessageBox.Show("Failure fetching WAV file for user: " + res.ErrorText);
            }

        }

        
        /// <summary>
        /// upload a WAV file to the voice name of a user.
        /// </summary>
        private void buttonSetVoiceName_Click(object sender, EventArgs e)
        {
            WebCallResult res;

            if (gridUsers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a user to set the voice name for.");
                return;
            }

            //the objectID and alias values will always be in the result set.
            string strObjectID = gridUsers.SelectedRows[0].Cells["ObjectId"].Value.ToString();

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                //user changed their minds
                return;
            }

            //it can take a little time to upload a large wav file via HTTP, disable the controls to let the user know we're busy.
            DisableFormControls();

            //upload the voice name to the Connection server.
            res = UserBase.SetUserVoiceName(GlobalItems.CurrentConnectionServer, openFileDialog.FileName, strObjectID, false);

            EnableFormControls();

            if (res.Success)
            {
                MessageBox.Show("Voice name updated");
            }
            else
            {
                MessageBox.Show("Error updating voice name:" + res.ErrorText);
            }

        }

        /// <summary>
        /// Show the exit destination of the user in a human readable description.
        /// </summary>
        private void buttonShowExitDestination_Click(object sender, EventArgs e)
        {
            WebCallResult res;

            if (gridUsers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a user to show the exit details for.");
                return;
            }

            //the objectID and alias values will always be in the result set.
            string strObjectID = gridUsers.SelectedRows[0].Cells["ObjectId"].Value.ToString();

            UserFull oUser;
            res = UserFull.GetUser(GlobalItems.CurrentConnectionServer, strObjectID, out oUser);

            if (res.Success==false)
            {
                MessageBox.Show("Failed fetching user:" + res.ToString());
                return;
            }

            string strExitInfo = string.Format("Exit action={0}",
                                               GlobalItems.CurrentConnectionServer.GetActionDescription(
                                                   oUser.ExitAction, oUser.ExitTargetConversation,
                                                   oUser.ExitTargetHandlerObjectId));

            MessageBox.Show(strExitInfo);
        }


        /// <summary>
        /// prototype to show how to get messages via CUMI interface
        /// </summary>
        private void buttonShowMessages_Click(object sender, EventArgs e)
        {
            if (gridUsers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a user to show messages for.");
                return;
            }

            string strObjectID = gridUsers.SelectedRows[0].Cells["ObjectId"].Value.ToString();

            using (FormUserMessages oMessages = new FormUserMessages())
            {
                oMessages.UserObjectId = strObjectID;
                oMessages.ShowDialog();
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
