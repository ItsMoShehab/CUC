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
using ConnectionCUPIFunctions;
using SimpleLogger;

namespace CUPIFastStart
{
    public partial class FormDistributionListFunctions : Form
    {

        private int _currentPage = 0;
        private int _currentTotalItems = 0;

        public FormDistributionListFunctions()
        {
            InitializeComponent();
        }


        private void FormDistributionListFunctions_Load(object sender, EventArgs e)
        {
            comboListFilterAction.SelectedIndex = 0;
            comboListFilterElement.SelectedIndex = 0;
            comboListsToFetch.SelectedIndex = 0;
        }

        private void buttonShowLists_Click(object sender, EventArgs e)
        {
            //indicate we're starting a new fetch, not moving through paged results for an existing fetch.
            _currentPage = 0;
            _currentTotalItems = 0;

            //main handler data fetching function for the form.
            UpdateDataDisplay();
        }


        /// <summary>
        /// since the HTTP fetch can take a little time, disable the input controls on the form while we are waiting
        /// </summary>
        private void DisableFormControls()
        {
            buttonShowLists.Enabled = false;
            buttonAddItem.Enabled = false;
            buttonRemoveItem.Enabled = false;
            buttonUpdateItem.Enabled = false;
            buttonShowLists.Enabled = false;

            this.Cursor = Cursors.WaitCursor;
        }

        /// <summary>
        /// re enable all controls when a long operation is complete - the UpdateItem button is left disabled since this gets
        /// enabled when a user manually changes a value in one of the bound text boxes for data on the right of the form.
        /// </summary>
        private void EnableFormControls()
        {
            buttonShowLists.Enabled = true;
            buttonAddItem.Enabled = true;
            buttonRemoveItem.Enabled = true;
            buttonUpdateItem.Enabled = false; //leave the update item disabled
            buttonShowLists.Enabled = true;

            this.Cursor = Cursors.Default;
        }


        /// <summary>
        /// Main routine that fetches the list data, dispays it in the grid and binds the textboxes for editing to the resulting
        /// data set.  All filtering of data is also handled in this routine if any filters are set.
        /// This takes an optional ObjectId parameter - if supplied it will load just that one list into the grid.  This is designed
        /// for adding new list so we can display the list just added - probably not the best UI design but you get what you pay for.
        /// </summary>
        /// <param name="pObjectID">
        /// Optional parameter if we want to display only a single call list - this gets used when creating a new list for instance.
        /// </param>
        private void UpdateDataDisplay(string pObjectID = "")
        {
            string strQuery = "";
            WebCallResult res;
            int iRowsPerPage = 0;

            //get the list data from the remote Connection server as a list of DL objects - 
            List<DistributionList> oLists;

            //fetch the number of lists to return in a query - you'll want to keep this reasonable in most cases as a very large 
            //result set can timeout on you if the server is busy.
            if (int.TryParse(comboListsToFetch.Text, out iRowsPerPage) == false)
            {
                //oops!
                MessageBox.Show("Invalid list count selection value encountered in UpdateDataDisplay:" + comboListsToFetch.Text);
                return;
            }

            //if the objectID was not passed in, fetch the list data using the filter information on the form 
            if (pObjectID.Length == 0)
            {
                //check if any filters are set - if they are we pass them in as a filter clause on the GetLists call, otherwise we pass blank 
                //which means get all lists (or the first batch in a paged fetch at any rate).
                if (comboListFilterElement.SelectedIndex > 0)
                {
                    //trim out any white space first
                    textListFilterText.Text = textListFilterText.Text.Trim();

                    if (string.IsNullOrEmpty(textListFilterText.Text))
                    {
                        MessageBox.Show("You must enter at least one character to search against for your query.");
                        textListFilterText.Focus();
                        return;
                    }

                    strQuery = string.Format("query=({0} {1} {2})", comboListFilterElement.Text, comboListFilterAction.Text,
                                      textListFilterText.Text).ToLower();
                }

                //if we're paging through a result set, incrament the count for the current page by 1 - if this is a single set or the first page the
                //_currentPage++ is set to 0 and this runs it to 1 which is the first page (it's 1 based, not zero based).
                _currentPage++;
                if (strQuery.Length > 0)
                {
                    strQuery += "&";
                }

                //limit the rows returned to what's selected on the form's drop down control 
                strQuery += string.Format("rowsPerPage={0}&pageNumber={1}", iRowsPerPage, _currentPage);
            }
            else
            {
                //single ObjectId was passed in - use a simpler query 
                strQuery = string.Format("query=({0} {1} {2})", "ObjectId", "is", pObjectID);
                _currentPage = 0;
            }

            //fetching the data via HTTP can take a bit - disable the controls on the form until the fetch returns.  A more sophisticated
            //background thread approach to fetching data is beyond the scope of this framework.
            DisableFormControls();

            res = DistributionList.GetDistributionLists(GlobalItems.CurrentConnectionServer,out oLists,strQuery);

            EnableFormControls();

            if (res.Success == false)
            {
                Logger.Log("Error fetching lists in UpdateDataDisplay on FormUSerFunctions.cs");

                //dump all the details from the return structure to the log for review
                Logger.Log(res.ToString());
                MessageBox.Show("Error encountered fetching lists:" + res.ErrorText);
                _currentPage = 0;
                _currentTotalItems = 0;
            }

            //update the total number of users returned in the query (not the number of users returned in the result set).
            _currentTotalItems = res.TotalObjectCount;

            //unbind the grid
            gridLists.DataSource = null;

            if (oLists != null)
            {
                //update the text and buttons for our paging mechanism under the grid.
                UpdatePagingDetails(iRowsPerPage, res.TotalObjectCount);

                //set the grid to bind to the list of users as it's source.  This is a read only display 
                //operation so there's no need to use a dataset or the like here.
                gridLists.DataSource = oLists;
                gridLists.AutoGenerateColumns = false;
            }
            else
            {
                labelListCountValue.Text = "0";
            }

            //bind the text controls on the form to the list - this works just like binding 
            //them to a data table in that they update automatically when you move the selected
            //row in the grid.
            textListDisplayName.DataBindings.Clear();
            textListExtension.DataBindings.Clear();

            //if data was returned bind the controls to the result sets and they get updated automatically when the currently
            //selected item is changed - if not results are returned be sure to blank out the control so they don't hold what 
            //might have been in there earlier.
            if (oLists != null)
            {
                textListDisplayName.DataBindings.Add("Text", oLists, "DisplayName", false);
                textListExtension.DataBindings.Add("Text", oLists, "DTMFAccessID", false);
            }
            else
            {
                textListDisplayName.Text = "";
                textListExtension.Text = "";
            }

            this.gridLists.Refresh();
            System.Threading.Thread.Sleep(1);

            //clear the enabled flag on the update button since it will change when we edit the text fields above.
            buttonUpdateItem.Enabled = false;

        }

        /// <summary>
        /// helper function to update the text and button controls for our paging display on the form.  It makes sure the next/previous
        /// buttons are enabled/disabled as necessary and the the current position is properly reflected in the label at the bottom of 
        /// the grid
        /// </summary>
        /// <param name="pRowsPerPage">
        /// Indicates how many rows per page we're fetching - this is controlled on the form's comboHandlerstoFetch control and the count
        /// is passed in here.
        /// </param>
        /// <param name="pTotalObjectCount">
        /// How many objects were reported total by CUPI for the query.
        /// </param>
        private void UpdatePagingDetails(int pRowsPerPage, int pTotalObjectCount)
        {
            //show the handler range we're currently showing in the grid.  Make sure to enable/disable the next/previous
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

            this.labelListCountValue.Text = string.Format("{0}-{1} of {2}", iStart, iEnd, pTotalObjectCount);
        }


        /// <summary>
        /// If all ists are selected for display, disable the other filter selection controls
        /// </summary>
        private void comboListFilterElement_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboListFilterElement.SelectedIndex == 0)
            {
                comboListFilterAction.Enabled = false;
                textListFilterText.Enabled = false;
            }
            else
            {
                comboListFilterAction.Enabled = true;
                textListFilterText.Enabled = true;
            }
        }


        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        ///user wants to update one of the top level items for a list - they may have edited the display name and/or extension.
        ///All the values will be sent over regardless of if they've changed - a more sophisticated "dirty flag" approach or the like is left as an exercize for the reader.
        /// </summary>
        private void buttonUpdateItem_Click(object sender, EventArgs e)
        {
            DisableFormControls();

            ConnectionPropertyList oPropList = new ConnectionPropertyList();

            string strObjectID = gridLists.SelectedRows[0].Cells["ObjectId"].Value.ToString();

            //the property names are case sensitive
            oPropList.Add("DisplayName", textListDisplayName.Text);

            //currently CUPI does not like you setting DtmfAccessId to blank - even though for lists this is legal, so for now screen this out
            if (textListExtension.Text.Length > 0)
            {
                oPropList.Add("DtmfAccessId", textListExtension.Text);
            }

            //do the call to update the items via CUPI
            WebCallResult res = DistributionList.UpdateDistributionList(GlobalItems.CurrentConnectionServer, strObjectID, oPropList);

            EnableFormControls();

            if (res.Success)
            {
                MessageBox.Show("List updated");
                Logger.Log(string.Format("List updated:{0}\r\n{1}", strObjectID, oPropList));
                gridLists.Refresh();
            }
            else
            {
                MessageBox.Show("List failed to update:" + res.ErrorText);
                Logger.Log(string.Format("List failed to update:{0}, error=[{1}]\r\n{2}", strObjectID, res.ErrorText, oPropList));

                //dump out all the results information send from the server
                Logger.Log(res.ToString());
            }
        }

        /// <summary>
        /// In a paging results scenario this moves back a page - if that moves "before 0" the UpdateDataDisplay routine takes 
        /// care of it.  The back button is disabled once the first page is reached - we need to go back 2 because the UpdateDataDisplay
        /// routine adds one each time it goes through.
        /// </summary>
        private void buttonPreviousPage_Click(object sender, EventArgs e)
        {
            _currentPage = _currentPage - 2;
            UpdateDataDisplay();
        }

        /// <summary>
        /// The UpdateDataDisplay routine updates the _currentPage counter by adding 1 to it each time it goes through so this will 
        /// go to the next page automatically in a multiple page results scenario.
        /// The next button is disabled when the last page is reached.
        /// </summary>
        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            UpdateDataDisplay();
        }

        /// <summary>
        /// User wishes to add a new distribution list to Connection - this routine shows a modal form for gathering the name, alias and optionally
        /// an extension number and then adds the new list and shows it in the grid by itself.
        /// </summary>
        private void buttonAddItem_Click(object sender, EventArgs e)
        {

            using (FormNewDistributionListInfo oForm = new FormNewDistributionListInfo())
            {
                if (oForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                //the call can take a minute to return - disable controls on the form to indicate we're busy.
                DisableFormControls();

                WebCallResult res = DistributionList.AddDistributionList(GlobalItems.CurrentConnectionServer,
                                                               oForm.DisplayName,
                                                               oForm.Alias,
                                                               oForm.Extension,
                                                               null);
                EnableFormControls();

                if (res.Success)
                {
                    //force a refresh of the grid and then select the list you just added - the ObjectId of the newly added list is returned 
                    //in the WebCallResult structure as the ReturnObjectID.
                    _currentPage = 0;
                    UpdateDataDisplay(res.ReturnedObjectId);
                    Logger.Log(string.Format("New distribution list added:{0},{1}", oForm.DisplayName, res.ReturnedObjectId));
                }
                else
                {
                    Logger.Log(string.Format(string.Format("New distribution list add failed:{0}\r\n{1}", oForm.DisplayName, res.ToString())));
                    MessageBox.Show(String.Format("Error adding new distribution list: {0}\r\nresponse from CUPI: {1}", res.ErrorText, res.ResponseText));
                }
            }
        }


        /// <summary>
        /// remove the currently selected distribution list in the grid after confirming with the user that's what they wish to do.
        /// </summary>
        private void buttonRemoveItem_Click(object sender, EventArgs e)
        {
            if (gridLists.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a distribution list to delete");
                return;
            }

            //the objectID and alias values will always be in the result set.
            string strObjectID = gridLists.SelectedRows[0].Cells["ObjectId"].Value.ToString();
            string strDisplayName = gridLists.SelectedRows[0].Cells["DisplayName"].Value.ToString();

            //verify with the user before deleting
            if (MessageBox.Show("Are you sure you want to delete the list: " + strDisplayName + "?", "Delete Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                //the user changed their minds, bail out
                return;
            }

            DisableFormControls();

            //remove the call handler
            WebCallResult res = DistributionList.DeleteDistributionList(GlobalItems.CurrentConnectionServer, strObjectID);

            EnableFormControls();

            if (res.Success)
            {
                MessageBox.Show("Distribution List removed");
                Logger.Log(string.Format("Distribution list [{0}] objectId={1} removed.", strDisplayName, strObjectID));

                //rebuild the grid - if we were traversing a list of lists before, be sure to reset the current page to 0 here because we 
                //need to rebuild the list without the DL we just deleted in it.
                _currentPage = 0;
                UpdateDataDisplay();
            }
            else
            {
                MessageBox.Show("Failure removing distribution list:" + res.ErrorText);
                Logger.Log(string.Format("Failed to remove distribution list with display name={0} and objectId={1}.  Error={2}", strDisplayName, strObjectID, res.ToString()));
            }
        }

        /// <summary>
        /// when either the display name or extension change, light up the update button
        /// </summary>
        private void textListExtension_TextChanged(object sender, EventArgs e)
        {
            this.buttonUpdateItem.Enabled = true;
        }


        /// <summary>
        /// If the user choose to change the list selected and there are pending changes, warn the user they will lose those changes and give them the 
        /// chance to update or discard before moving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridLists_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (buttonUpdateItem.Enabled)
            {
                if (MessageBox.Show("Pending edits will be lost, are you sure you want to move to a new record before saving your edits?",
                    "Cancel Pending Edits", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    //cancel the move and bail out
                    e.Cancel = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Once the new row is selected make sure the "enabled" state of the update items button is disabled.
        /// </summary>
        private void gridLists_SelectionChanged(object sender, EventArgs e)
        {
            this.buttonUpdateItem.Enabled = false;
        }

        /// <summary>
        /// show membership information for selected distribution list in the grid.
        /// </summary>
        private void buttonReviewMembers_Click(object sender, EventArgs e)
        {
            //check to be sure a list is selected first
            if (gridLists.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a distribution list to show members for");
                return;
            }

            //the objectID and alias values will always be in the result set.
            string strObjectID = gridLists.SelectedRows[0].Cells["ObjectId"].Value.ToString();
            
            using (FormDistributionListMembers oForm = new FormDistributionListMembers())
            {
                oForm.DistributionListObjectId = strObjectID;
                oForm.ShowDialog();
            }

        }

    }
}
