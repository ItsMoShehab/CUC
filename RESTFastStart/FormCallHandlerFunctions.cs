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
using System.Text;
using System.Windows.Forms;
using ConnectionCUPIFunctions;
using SimpleLogger;

namespace CUPIFastStart
{

    /// <summary>
    /// This form is designed to demonstrate the use of the ConnectionCUPIFunctions class library for doing the most common (and sometimes
    /// tricky) operations for call handlers on the Unity Connection directory.  It shows fetching handlers, filtering that search, editing top level
    /// handler data, deleting handlers, creating new handlers and fetching and setting voice names for handlers.
    /// You can, of course, do all these things with raw HTTP calls which the library lets you do however the wrapped functions provided in 
    /// the CallHandler class makes this much easier and quicker if you're working in .NET which this project intends to demonstrate.
    /// </summary>
    public partial class FormCallHandlerFunctions : Form
    {
        private int _currentPage = 0;
        private int _currentTotalItems = 0;

        public FormCallHandlerFunctions()
        {
            InitializeComponent();
        }

        /// <summary>
        ///at form load time force the filtering combo boxes to have their first item (index 0) selected and showing.  By default
        ///these show blank.
        /// </summary>
        private void FormCallHandlerFunctions_Load(object sender, EventArgs e)
        {
            comboHandlerFilterAction.SelectedIndex = 0;
            comboHandlerFilterElement.SelectedIndex = 0;
            comboHandlersToFetch.SelectedIndex = 0;
        }

        /// <summary>
        /// since the HTTP fetch can take a little time, disable the input controls on the form while we are waiting
        /// </summary>
        private void DisableFormControls()
        {
            buttonShowHandlers.Enabled = false;
            buttonAddItem.Enabled = false;
            buttonRemoveItem.Enabled = false;
            buttonUpdateItem.Enabled = false;
            buttonFetchVoiceName.Enabled = false;
            buttonSetVoiceName.Enabled = false;

            this.Cursor = Cursors.WaitCursor;
        }

        /// <summary>
        /// re enable all controls when a long operation is complete - the UpdateItem button is left disabled since this gets
        /// enabled when a user manually changes a value in one of the bound text boxes for user data on the right of the form.
        /// </summary>
        private void EnableFormControls()
        {
            buttonShowHandlers.Enabled = true;
            buttonAddItem.Enabled = true;
            buttonRemoveItem.Enabled = true;
            buttonUpdateItem.Enabled = false; //leave the update item disabled
            buttonFetchVoiceName.Enabled = true;
            buttonSetVoiceName.Enabled = true;

            this.Cursor = Cursors.Default;
        }

        //user issues another call handler filter request.
        private void buttonShowHandlers_Click(object sender, EventArgs e)
        {
            //indicate we're starting a new fetch, not moving through paged results for an existing fetch.
            _currentPage = 0;
            _currentTotalItems = 0;

            //main handler data fetching function for the form.
            UpdateDataDisplay();
        }


        /// <summary>
        /// Main routine that fetches the handler data, dispays it in the grid and binds the textboxes for editing to the resulting
        /// data set.  All filtering of data is also handled in this routine if any filters are set.
        /// This takes an optional ObjectId parameter - if supplied it will load just that one handler into the grid.  This is designed
        /// for adding new handlers so we can display the handler just added - probably not the best UI design but you get what you pay for.
        /// </summary>
        /// <param name="pObjectID">
        /// Optional parameter if we want to display only a single call handler - this gets used when creating a new handler for instance.
        /// </param>
        private void UpdateDataDisplay(string pObjectID = "")
        {
            string strQuery = "";
            WebCallResult res;
            int iRowsPerPage = 0;

            //get the user data from the remote Connection server as a list of CallHandler objects
            List<CallHandler> oHandlers;

            //fetch the number of handlers to return in a query - you'll want to keep this reasonable in most cases as a very large 
            //result set can timeout on you if the server is busy.
            if (int.TryParse(comboHandlersToFetch.Text, out iRowsPerPage) == false)
            {
                //oops!
                MessageBox.Show("Invalid handler count selection value encountered in UpdateDataDisplay:" + comboHandlersToFetch.Text);
                return;
            }

            //if the objectID was not passed in, fetch the handler data using the filter information on the form 
            if (pObjectID.Length == 0)
            {
                //check if any filters are set - if they are we pass them in as a filter clause on the GetHandlers call, otherwise we pass blank 
                //which means get all handlers (or the first batch in a paged fetch at any rate).
                if (comboHandlerFilterElement.SelectedIndex > 0)
                {
                    //trim out any white space first
                    textHandlerFilterText.Text = textHandlerFilterText.Text.Trim();

                    if (string.IsNullOrEmpty(textHandlerFilterText.Text))
                    {
                        MessageBox.Show("You must enter at least one character to search against for your query.");
                        textHandlerFilterText.Focus();
                        return;
                    }

                    strQuery = string.Format("query=({0} {1} {2})", comboHandlerFilterElement.Text, comboHandlerFilterAction.Text,
                                      textHandlerFilterText.Text).ToLower();
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

            res = CallHandler.GetCallHandlers(GlobalItems.CurrentConnectionServer, out oHandlers, strQuery);

            EnableFormControls();

            if (res.Success == false)
            {
                Logger.Log("Error fetching handlers in UpdateDataDisplay on FormUSerFunctions.cs");

                //dump all the details from the return structure to the log for review
                Logger.Log(res.ToString());
                MessageBox.Show("Error encountered fetching handlers:" + res.ErrorText);
                _currentPage = 0;
                _currentTotalItems = 0;
            }

            //update the total number of handlers returned in the query (not the number of handlers returned in the result set).
            _currentTotalItems = res.TotalObjectCount;

            //unbind the grid
            this.gridHandlers.DataSource = null;

            if (oHandlers != null)
            {
                //update the text and buttons for our paging mechanism under the grid.
                UpdatePagingDetails(iRowsPerPage, res.TotalObjectCount);

                //set the grid to bind to the list of handlers as it's source.  This is a read only display 
                //operation so there's no need to use a dataset or the like here.
                this.gridHandlers.DataSource = oHandlers;
                this.gridHandlers.AutoGenerateColumns = false;
            }
            else
            {
                this.labelHandlerCountValue.Text = "0";
            }

            //bind the text controls on the form to the list - this works just like binding 
            //them to a data table in that they update automatically when you move the selected
            //row in the grid.
            textHandlerDisplayName.DataBindings.Clear();
            textHandlerExtension.DataBindings.Clear();
            chkIsPrimary.DataBindings.Clear();

            //if data was returned bind the controls to the result sets and they get updated automatically when the currently
            //selected item is changed - if not results are returned be sure to blank out the control so they don't hold what 
            //might have been in there earlier.
            if (oHandlers != null)
            {
                textHandlerDisplayName.DataBindings.Add("Text", oHandlers, "DisplayName", false);
                textHandlerExtension.DataBindings.Add("Text", oHandlers, "DTMFAccessID", false);
                chkIsPrimary.DataBindings.Add("Checked", oHandlers, "IsPrimary", false);
            }
            else
            {
                textHandlerDisplayName.Text = "";
                textHandlerExtension.Text = "";
                chkIsPrimary.Checked = false;
            }

            this.gridHandlers.Refresh();
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

            this.labelHandlerCountValue.Text = string.Format("{0}-{1} of {2}", iStart, iEnd, pTotalObjectCount);
        }

        /// <summary>
        /// Close out the window entirely
        /// </summary>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// UpdateDataDisplay already incraments the current page by 1 each time through.
        /// </summary>
        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            UpdateDataDisplay();
        }

        /// <summary>
        ///current page is incramented by 1 when it goes through the UpdateDataDisplay routine - to go back a 
        ///page we need to go back 2.
        /// </summary>
        private void buttonPreviousPage_Click(object sender, EventArgs e)
        {
            _currentPage = _currentPage - 2;
            UpdateDataDisplay();
        }

        /// <summary>
        /// user edited the display name text - light up the save changes button
        /// </summary>
        private void textHandlerDisplayName_TextChanged(object sender, EventArgs e)
        {
            buttonUpdateItem.Enabled = true;
        }

        
        /// <summary>
        /// user edited the Extension text - light up the save changes button
        /// </summary>
        private void textHandlerExtension_TextChanged(object sender, EventArgs e)
        {
            buttonUpdateItem.Enabled = true;
        }

        /// <summary>
        ///user wants to update one of the top level items for a handler - they may have edited the display name and/or extension.
        ///All the values will be sent over regardless of if they've changed - a more sophisticated "dirty flag" approach or the like is left as an exercize for reader.
        /// </summary>
        private void buttonUpdateItem_Click(object sender, EventArgs e)
        {
            DisableFormControls();

            ConnectionPropertyList oPropList = new ConnectionPropertyList();

            string strObjectID = gridHandlers.SelectedRows[0].Cells["ObjectId"].Value.ToString();

            //the property names are case sensitive
            oPropList.Add("DisplayName", textHandlerDisplayName.Text);
            
            //currently CUPI does not like you setting DtmfAccessId to blank - even though for handlers this is legal, so for now screen this out
            if (textHandlerExtension.Text.Length>0)
            {
                oPropList.Add("DtmfAccessId", textHandlerExtension.Text);
            }

            //do the call to update the items via CUPI
            WebCallResult res = CallHandler.UpdateCallHandler(GlobalItems.CurrentConnectionServer, strObjectID, oPropList);

            EnableFormControls();

            if (res.Success)
            {
                MessageBox.Show("Handler updated");
                Logger.Log(string.Format("Handler updated:{0}\r\n{1}",strObjectID, oPropList));
                gridHandlers.Refresh();
            }
            else
            {
                MessageBox.Show("Handler failed to update:" + res.ErrorText);
                Logger.Log(string.Format("Handler failed to update:{0}, error=[{1}]\r\n{2}", strObjectID, res.ErrorText,oPropList));

                //dump out all the results information send from the server
                Logger.Log(res.ToString());
            }
        }

        
        /// <summary>
        /// adjust the handler filter options based on the first element selection combo box value.
        /// Index 0 is "all handlers" and so the other two items can be disabled.  Otherwise it's 
        /// a property name to filter against for handlers so the other two items need to be enabled
        /// to allow for user input.
        /// </summary>
        private void comboHandlerFilterElement_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboHandlerFilterElement.SelectedIndex == 0)
            {
                this.comboHandlerFilterAction.Enabled = false;
                this.textHandlerFilterText.Enabled = false;
            }
            else
            {
                this.comboHandlerFilterAction.Enabled = true;
                this.textHandlerFilterText.Enabled = true;
            }
        }

        /// <summary>
        /// add a new call handler using the FormNewCallHandler to gather details for it.
        /// </summary>
        private void buttonAddItem_Click(object sender, EventArgs e)
        {
            using (FormNewCallHandlerInfo oForm = new FormNewCallHandlerInfo())
            {
                if (oForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                //the call can take a minute to return - disable controls on the form to indicate we're busy.
                DisableFormControls();

                WebCallResult res = CallHandler.AddCallHandler(GlobalItems.CurrentConnectionServer,
                                                               oForm.TemplateObjectID,
                                                               oForm.DisplayName,
                                                               oForm.Extension,
                                                               null);
                EnableFormControls();

                if (res.Success)
                {
                    //force a refresh of the grid and then select the handler you just added - the ObjectId of the newly added handler is returned 
                    //in the WebCallResult structure as the ReturnObjectID.
                    _currentPage = 0;
                    UpdateDataDisplay(res.ReturnedObjectId);
                    Logger.Log(string.Format("New call handler added:{0},{1}", oForm.DisplayName, res.ReturnedObjectId));
                }
                else
                {
                    Logger.Log(string.Format(string.Format("New call handler add failed:{0}\r\n{1}", oForm.DisplayName, res.ToString())));
                    MessageBox.Show(String.Format("Error adding new call handler: {0}\r\nresponse from CUPI: {1}", res.ErrorText, res.ResponseText));
                }
            }

            
        }


        /// <summary>
        ///remove the currently selected handler in the grid.  If there is no handler selected in the grid (i.e. it's empty) then warn the 
        ///user and exit.
        /// </summary>
        private void buttonRemoveItem_Click(object sender, EventArgs e)
        {
            if (gridHandlers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a call handler to delete");
                return;
            }

            //the objectID and alias values will always be in the result set.
            string strObjectID = gridHandlers.SelectedRows[0].Cells["ObjectId"].Value.ToString();
            string strDisplayName = gridHandlers.SelectedRows[0].Cells["DisplayName"].Value.ToString();

            //verify with the user before deleting
            if (MessageBox.Show("Are you sure you want to delete the call handler: " + strDisplayName + "?", "Delete Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                //the user changed their minds, bail out
                return;
            }

            DisableFormControls();

            //remove the call handler
            WebCallResult res = CallHandler.DeleteCallHandler(GlobalItems.CurrentConnectionServer, strObjectID);

            EnableFormControls();

            if (res.Success)
            {
                MessageBox.Show("Call Handler removed");
                Logger.Log(string.Format("Call handler [{0}] objectId={1} removed.", strDisplayName, strObjectID));

                //rebuild the grid - if we were traversing a list of handlers before, be sure to reset the current page to 0 here because we 
                //need to rebuild the list without the handler we just deleted in it.
                _currentPage = 0;
                UpdateDataDisplay();
            }
            else
            {
                MessageBox.Show("Failure removing call hanlder:"+res.ErrorText);
                Logger.Log(string.Format("Failed to remove call handler with display name={0} and objectId={1}.  Error={2}", strDisplayName, strObjectID, res.ToString()));
            }
        }


        /// <summary>
        /// Demonstrate how to fetch the voice name off a call handler
        /// </summary>
        private void buttonFetchVoiceName_Click(object sender, EventArgs e)
        {
            WebCallResult res;

            if (gridHandlers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a call handler to fetch the voice name for.");
                return;
            }

            //the objectID and alias values will always be in the result set.
            string strObjectID = gridHandlers.SelectedRows[0].Cells["ObjectId"].Value.ToString();
            string strDisplayName = gridHandlers.SelectedRows[0].Cells["DisplayName"].Value.ToString();

            //make the user pick a location to store the voice name - pre populate the file name with the handlers's display
            //name to make it unique if they're getting a few into the same location.
            saveFileDialog.FileName = string.Format("{0}_VoiceName.wav", strDisplayName);
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                //user changed their minds
                return;
            }

            //downloading a wav file via HTTP can take a bit - disable the controls to let the user know we're busy.
            DisableFormControls();

            //fetch the voice name off the Connection server and store it at the path provided.
            res = CallHandler.GetCallHandlerVoiceName(GlobalItems.CurrentConnectionServer, saveFileDialog.FileName, strObjectID);

            EnableFormControls();

            if (res.Success)
            {
                //open the folder in file explorer
                Process.Start("explorer.exe", Directory.GetParent(saveFileDialog.FileName).ToString());
            }
            else
            {
                MessageBox.Show("Failure fetching WAV file for call handler: " + res.ErrorText);
            }
        }

        /// <summary>
        /// Demonstrate how to upload a WAV file as a voice name for a call handler
        /// </summary>
        private void buttonSetVoiceName_Click(object sender, EventArgs e)
        {
            WebCallResult res;

            if (gridHandlers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a call handler to set the voice name for.");
                return;
            }

            //the objectID values will always be in the result set.
            string strObjectID = gridHandlers.SelectedRows[0].Cells["ObjectId"].Value.ToString();

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                //user changed their minds
                return;
            }

            //it can take a little time to upload a large wav file via HTTP, disable the controls to let the user know we're busy.
            DisableFormControls();

            //Upload the voice name to the Connection server.
            res = CallHandler.SetCallHandlerVoiceName(GlobalItems.CurrentConnectionServer, openFileDialog.FileName, strObjectID, false);

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
        /// When a new handler is selected make sure it disables the update item button
        /// </summary>
        private void gridHandlers_SelectionChanged(object sender, EventArgs e)
        {
            this.buttonUpdateItem.Enabled = false;
        }


        /// <summary>
        /// If the user changes the currently selected handler and there are pending edits that have not been saved warn the user and give them a chance
        /// to save or discard them.
        /// </summary>
        private void gridHandlers_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
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
        /// Show all the menu entry keys (0-0, * and #) in a human readable format in a simple dialog box.  Not the most sophisticated display 
        /// mechanisms but meant to show how to easily iterate over the menu keys for a handler and use the action string helper function on the 
        /// ConnectionServer class.
        /// </summary>
        private void buttonShowInputKeys_Click(object sender, EventArgs e)
        {
            WebCallResult res;

             if (gridHandlers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a call handler to fetch the menu keys for.");
                return;
            }

            //grab the current call handler from the grid.
            string strObjectId = gridHandlers.SelectedRows[0].Cells["ObjectId"].Value.ToString();

            CallHandler oHandler;
            res=CallHandler.GetCallHandler(out oHandler, GlobalItems.CurrentConnectionServer, strObjectId);

            if (res.Success==false)
            {
                MessageBox.Show("Error fetching call handler:" + res.ToString());
                return;
            }

            StringBuilder strKeys= new StringBuilder("Menu entry keys:\n\r");
            foreach (MenuEntry oMenuKey in oHandler.GetMenuEntries())
            {
                strKeys.Append(oMenuKey.TouchtoneKey);
                strKeys.Append(": ");
                strKeys.AppendLine(GlobalItems.CurrentConnectionServer.GetActionDescription(oMenuKey.Action,
                                                                                        oMenuKey.TargetConversation,
                                                                                        oMenuKey.TargetHandlerObjectId));
            }

            //show the resulting output.
            MessageBox.Show(strKeys.ToString());
        }


    }
}
