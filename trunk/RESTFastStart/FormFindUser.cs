using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ConnectionCUPIFunctions;
using SimpleLogger;

namespace CUPIFastStart
{
    /// <summary>
    /// form that allows the user to search/select a single user and return that ID to the calling routine.
    /// </summary>
    public partial class FormFindUser : Form
    {
        //return the selected user's ObjectId via this public property
        public string UserObjectId { get; private set; }

        public FormFindUser()
        {
            InitializeComponent();
        }

        private void FormFindUser_Load(object sender, EventArgs e)
        {
            //force first items in comboboxes to be selected at form load time.
            comboUserFilterAction.SelectedIndex = 0;
            comboUserFilterElement.SelectedIndex = 0;
        }

        /// <summary>
        /// Web fetches can take a bit, disable the form controls while it's busy
        /// </summary>
        private void DisableFormControls()
        {
            this.Cursor = Cursors.WaitCursor;
            buttonCancel.Enabled = false;
            buttonOK.Enabled = false;
            buttonShowUsers.Enabled = false;
        }

        private void EnableFormControls()
        {
            this.Cursor = Cursors.Default;
            buttonCancel.Enabled = true;
            buttonOK.Enabled = true;
            buttonShowUsers.Enabled = true;
        }


        /// <summary>
        /// If all users option is selected in the Filterelement combo then disable the other user selection controls as they have no impact
        /// </summary>
        private void comboUserFilterElement_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboUserFilterElement.SelectedIndex == 0)
            {
                comboUserFilterAction.Enabled = false;
                textUserFilterText.Enabled = false;
            }
            else
            {
                comboUserFilterAction.Enabled = true;
                textUserFilterText.Enabled = true;
            }
        }


        //Main routine that fetches the user data, dispays it in the grid 
        // All filtering of data is also handled in this routine if any filters are set.
        private void UpdateDataDisplay()
        {
            string strQuery = "";
            WebCallResult res;

            //get the user data from the remote Connection server as a list of UserBase objects - remember these are "light" users 
            //that contain a smaller set of data than a "UserFull" - this is designed for list presentation and the like, although
            //in this sample application we're just showing everything, typically you'd hide all the ObjectId values.
            List<UserBase> oUsers;

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
            if (strQuery.Length > 0)
            {
                strQuery += "&";
            }

            //Hard coded to fetch first 50 no matter what - there's examples of how to do proper paging of results on the main users functions
            //page, call handler page and distribution list page.  
            strQuery += string.Format("rowsPerPage={0}&pageNumber={1}", 50, 1);

            //fetching the data via HTTP can take a bit - disable the controls on the form until the fetch returns.  A more sophisticated
            //background thread approach to fetching data is beyond the scope of this framework.
            DisableFormControls();

            res = UserBase.GetUsers(GlobalItems.CurrentConnectionServer, out oUsers, strQuery);

            EnableFormControls();

            if (res.Success == false)
            {
                Logger.Log("Error fetching users in UpdateDataDisplay on FormFindUser.cs");

                //dump all the details from the return structure to the log for review
                Logger.Log(res.ToString());
                MessageBox.Show("Error encountered fetching users:" + res.ErrorText);
            }

            //unbind the grid
            this.gridUsers.DataSource = null;

            if (oUsers != null)
            {
                //set the grid to bind to the list of users as it's source.  This is a read only display 
                //operation so there's no need to use a dataset or the like here.
                this.gridUsers.DataSource = oUsers;
                this.gridUsers.AutoGenerateColumns = false;
            }

            this.gridUsers.Refresh();
        }

        /// <summary>
        /// fill the search grid with users based on the search selection provided by the user.
        /// </summary>
        private void buttonShowUsers_Click(object sender, EventArgs e)
        {
            UpdateDataDisplay();
        }

        /// <summary>
        /// Make sure the user has selected a user in the grid before closing out the dialog.  If the user wishes to skip selecting 
        /// the user they must use the cancel button instead.
        /// </summary>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (gridUsers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a user");
                return;
            }

            //the objectID and alias values will always be in the result set.
            UserObjectId = gridUsers.SelectedRows[0].Cells["ObjectId"].Value.ToString();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Bail out with a cancel dialog result so the calling routine knows the user changed their mind.
        /// </summary>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }

}
