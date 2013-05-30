using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Cisco.UnityConnection.RestFunctions;

namespace PWResetASP
{
    /// <summary>
    /// Form for allowing the user to search for and review users and then review or reset PINs for those users.
    /// </summary>
    public partial class SelectUser : System.Web.UI.Page
    {
        #region Fields and Properties

        /// <summary>
        /// ConnectionServer object that is passed to this page from the login page via the Session.  Used for displaying info 
        /// about the Connection server and executing queries and functions against it via REST.
        /// </summary>
        private ConnectionServerRest _connectionServer;
        
        /// <summary>
        /// Holds the current page in a multiple page scenario when searching and reviewing users.  This gets stored in the ViewState.
        /// </summary>
        private int _currentPage=0;

        /// <summary>
        /// Column order for the grid - the grid in ASP does not handle conversion from named columns to their index so you need to do
        /// it yourself if you're not using a seperate data source like a dataTable to bind your grid to which we're not here.
        /// </summary>
        public enum GridColumnNames
        {
            Alias = 2,
            FirstName = 3,
            LastName = 4,
            DisplayName = 5,
            Extension = 6,
            ObjectId = 7,

        }

        #endregion

        /// <summary>
        /// Check that the ConnectionServer object is in the session (meaning the user has authenticated).  Otherwise redirect to 
        /// the main login page without comment.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            _connectionServer = (ConnectionServerRest)Session["CurrentConnectionServer"];

            if (_connectionServer == null)
            {
                //stick a note that the page has been accessed without authorization into the session - the login page
                //will display this.
                Session.Add("ErrorNote", "You must first log into a Connection server before searching for users");

                //Logger.Write(LoggingLevel.WARN, "SelectUser page loaded with no authenticated user in the session.");
                Response.Redirect("~/Default.aspx");
                return;
            }

            //if the user is redirected to the user selection page for invalid access or the like, an error message will be in 
            //the session - check for it here.
            if (Session["ErrorNote"] != null)
            {
                labelStatus.Text = Session["ErrorNote"].ToString();
                Session.Remove("ErrorNote");
            }

            //remember what page we were on between page refreshes
            if (ViewState["currentPage"] != null)
            {
                _currentPage = (int)ViewState["currentPage"];
            }

            //display server info at the top.
            labelServerInfo.Text = string.Format("Connection server:{0}, version: {1}", _connectionServer.ServerName,_connectionServer.Version.ToString());

            textUserFilterText.Focus();

        }


        /// <summary>
        /// Main routine that fetches the user data, dispays it in the grid and binds the textboxes for editing to the resulting
        /// data set.  All filtering of data is also handled in this routine if any filters are set.
        /// </summary>
        private void UpdateDataDisplay()
        {
            string strQuery = "";
            WebCallResult res;
            int iRowsPerPage = 0;

            labelStatus.Text = "";

            //get the user data from the remote Connection server as a list of User objects - remember these are "light" users 
            //that contain a smaller set of data than a "UserFull" - this is designed for list presentation and the like, although
            //in this sample application we're just showing everything, typically you'd hide all the ObjectId values.
            List<Cisco.UnityConnection.RestFunctions.UserBase> oUsers;

            //fetch the number of users to return in a query - you'll want to keep this reasonable in most cases as a very large 
            //result set can timeout on you if the server is busy.
            if (int.TryParse(comboUsersToFetch.Text, out iRowsPerPage) == false)
            {
                //oops!
                labelStatus.Text="Invalid user count selection value encountered in UpdateDataDisplay:" +comboUsersToFetch.Text;
                return;
            }

            ////check if any filters are set - if they are we pass them in as a filter clause on the GetUsers call, otherwise we pass blank 
            ////which means get all users (or the first batch in a paged fetch at any rate.
            if (comboUserFilterElement.SelectedIndex > 0)
            {
                //trim out any white space first
                textUserFilterText.Text = textUserFilterText.Text.Trim();

                if (string.IsNullOrEmpty(textUserFilterText.Text))
                {
                    labelStatus.Text="You must enter at least one character to search against for your query.";
                    textUserFilterText.Focus();
                    return;
                }

                strQuery = string.Format("query=({0} {1} {2})", comboUserFilterElement.SelectedValue, comboUserFilterAction.SelectedValue,
                                    textUserFilterText.Text).ToLower();

            }

            //if we're paging through a result set, incrament the count for the current page by 1 - if this is a single set or the first page the
            //_currentPage++ is set to 0 and this runs it to 1 which is the first page (it's 1 based, not zero based).
            _currentPage++;
            ViewState["currentPage"] = _currentPage;

            if (strQuery.Length > 0)
            {
                strQuery += "&";
            }

            //limit the rows returned to what's selected on the form's drop down control
            strQuery += string.Format("rowsPerPage={0}&pageNumber={1}", iRowsPerPage, _currentPage);
            
            //the sorting is a bit weak in CUPI - if you're filtering by an element then you must sort by that element if you wish the results
            //to be sorted - a little annoying.  If we're getting all users then sort by alias.
            string strSort;
            if (comboUserFilterElement.SelectedIndex>0)
            {
                strSort = string.Format("sort=({0} asc)", comboUserFilterElement.SelectedValue.ToString());
            }
            else
            {
                //default to alias if we're getting all users
                strSort = "sort=(alias asc)";
            }

            res = Cisco.UnityConnection.RestFunctions.UserBase.GetUsers(_connectionServer, out oUsers, strQuery, strSort);

            if (res.Success == false)
            {
                //dump all the details from the return structure to the log for review, however don't put anything up on the 
                //screen since if no matches are found on a search this can return an error
                //Logger.Log(res.ToString());
                labelStatus.Text="Error encountered fetching users:" + res.ErrorText;
                _currentPage = 0;
                ViewState["currentPage"] = _currentPage;
            }

            //update the total number of users returned in the query (not the number of users returned in the result set).

            //unbind the grid
            gridUsers.DataSource = null;
            gridUsers.DataBind();
            
            if (oUsers != null)
            {
                //update the text and buttons for our paging mechanism under the grid.
                UpdatePagingDetails(iRowsPerPage, res.TotalObjectCount);
                
                //set the grid to bind to the list of users as it's source.  This is a read only display 
                //operation so there's no need to use a dataset or the like here.
                gridUsers.DataSource = oUsers;
                gridUsers.AutoGenerateColumns = false;
                gridUsers.DataBind();
            }
            else
            {
                LabelUserCountValue.Text = "0";
            }

        }

        /// <summary>
        /// helper function to update the text and button controls for our paging display on the form.  It makes sure the next/previous
        /// buttons are enabled/disabled as necessary and the the current position is properly reflected in the label at the bottom of 
        /// the grid
        /// </summary>
        /// <param name="pRowsPerPage">
        /// How many rows (users) we're fetching at a time from Connection.
        /// </param>
        /// <param name="pTotalObjectCount">
        /// How many users, total, are included in the currently active user query against Connection
        /// </param>
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

            LabelUserCountValue.Text = string.Format("{0}-{1} of {2}", iStart, iEnd, pTotalObjectCount);
        }


        /// <summary>
        /// Issue a new query search for users against Connection using the filter options set by the user on the form.  This resets
        /// the current page value and dumps all the current content from the user search grid.
        /// </summary>
        protected void ButtonFindUsers_Click(object sender, EventArgs e)
        {
            _currentPage = 0;
            ViewState["currentPage"] = _currentPage;            
            UpdateDataDisplay();
        }


        /// <summary>
        /// Override the render method to add the ability to have the row select in the grid when you click on any part of the row.
        /// Not sure why such behavior is not supported 
        /// </summary>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            foreach (GridViewRow row in gridUsers.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Set the last parameter to True to register for event validation. 
                    row.Attributes["onclick"] =ClientScript.GetPostBackClientHyperlink(gridUsers,"Select$" + row.DataItemIndex, true);
                }
            }
            base.Render(writer);
        }


        /// <summary>
        /// Handle the "button" clicks for the view and pin reset items in the grid control
        /// </summary>
        protected void gridUsers_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            // Retrieve the row index stored in the 
            // CommandArgument property.
            int index = Convert.ToInt32(e.CommandArgument);

            // Retrieve the row that contains the button 
            // from the Rows collection.
            GridViewRow row = gridUsers.Rows[index];

            //clear the currently selected user from the session variable if it exists
            if (Session["CurrentUser"]!=null)
            {
                Session.Remove("CurrentUser");
            }

            //fetch the user based on their ObjectId - we'll then stick this user in the session so we can perform operations
            //on it later.
            UserBase oUser;

            WebCallResult res = Cisco.UnityConnection.RestFunctions.UserBase.GetUser(out oUser, _connectionServer, row.Cells[(int)GridColumnNames.ObjectId].Text);
            
            if (res.Success == false)
            {
                labelStatus.Text="Failed to fetch selected user:" + res.ToString();
                return;
            }

            //stuff the user into the session so we can access it on other pages in the site
            Session.Add("SelectedUser",oUser);

            //redirect to the appropriate page based on which link/button was clicked
            if (e.CommandName == "ViewPin")
            {
                Response.Redirect("~/ViewPinDetails.aspx");                
            }

            if (e.CommandName=="ResetPin")
            {
                Response.Redirect("~/ResetPin.aspx");                
            }

        }

        
        //advance through multiple pages of users
        protected void buttonNextPage_Click(object sender, EventArgs e)
        {
            //UpdateDataDisplay already incraments the current page by 1 each time through.
            UpdateDataDisplay();
        }

        
        //go back a page when multiple pages of users are returned
        protected void buttonPreviousPage_Click(object sender, EventArgs e)
        {
            //current page is incramented by 1 when it goes through the UpdateDataDisplay routine - to go back a 
            //page we need to go back 2.
            _currentPage = _currentPage - 2;
            ViewState["currentPage"] = _currentPage;
            UpdateDataDisplay();
        }

        /// <summary>
        /// Strange quirk of the grid control from MS - if you hide the column in the grid then you can't fetch data from it - so if 
        /// you want data around in the grid that the user doesn't see but you still want to fetch it you have to hide the cell on each
        /// row in the RowDataBound method here - very odd, but it works.
        /// </summary>
        protected void gridUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[(int)GridColumnNames.ObjectId].Visible = false;
        }


    }
}
