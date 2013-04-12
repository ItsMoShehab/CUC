using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cisco.UnityConnection.RestFunctions;
using SimpleLogger;

namespace CUPIFastStart
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FormDistributionListMembers : Form
    {
        //the calling routine can pass in the ObjectId of the distribution list to work with via the public 
        public string DistributionListObjectId { get; set; }
        
        public FormDistributionListMembers()
        {
            InitializeComponent();
        }


        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// once the form is loaded (i.e. controls are drawn) check to see if the user passed in an ObjectId to work with.
        /// If not, bark at them and close out.
        /// </summary>
        private void FormDistributionListMembers_Shown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DistributionListObjectId))
            {
                MessageBox.Show("No Distribution List ObjectId provided, cannot display members.");
                this.Close();
            }

            //if we have an ObjectId to work with, load up the members and display them in the list.
            LoadMembers();

        }


        private void DisableFormControls()
        {
            this.Cursor = Cursors.WaitCursor;
            buttonAddUser.Enabled = false;
            buttonRemoveItem.Enabled = false;
        }


        private void EnableFormControls()
        {
            //allow user actions
            buttonAddUser.Enabled = true;
            buttonRemoveItem.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Armed with the ObjectId of a distribution list (passed in via a public property to this form) this routine will 
        /// get a list of members from Connection and display them in the grid on the form for review.
        /// </summary>
        private void LoadMembers()
        {
            //while we're loading members make sure the user can't try and edit anything
            DisableFormControls();

            gridMembers.DataSource = null;

            WebCallResult res;
            List<DistributionListMember> oMembers;
            
            res=DistributionList.GetMembersList(GlobalItems.CurrentConnectionServer, DistributionListObjectId, out oMembers);

            EnableFormControls();

            //if the list has no members fals is returned but the member list object will not be null - only bark an error here if it's null meaning
            //there was really a problem with the fetch.
            if (res.Success==false & oMembers==null)
            {
                MessageBox.Show("Failure fetching distribution list membership:" + res.ErrorText);
                //again, nothing can be done here without membership data so just exit the form.
                this.Close();
            }

            labelListCountValue.Text = string.Format("Members in list: {0}", oMembers.Count);

            //in a production application you'd want to rename columns to be neater and hide colums you didn't want folks to see and such - 
            //here we just show everything returned.
            gridMembers.DataSource = oMembers;
            gridMembers.Refresh();

        }


        /// <summary>
        /// remove the currently selected member in the grid (either user or distributionlist) after confirming with the user
        /// </summary>
        private void buttonRemoveItem_Click(object sender, EventArgs e)
        {
            if (gridMembers.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a distribution list member to delete");
                return;
            }

            //the objectID and alias values will always be in the result set.
            string strMemberObjectID = gridMembers.SelectedRows[0].Cells["ObjectId"].Value.ToString();
            string strDisplayName = gridMembers.SelectedRows[0].Cells["DisplayName"].Value.ToString();

            //verify with the user before deleting
            if (MessageBox.Show("Are you sure you want to delete the list member: " + strDisplayName + "?", "DELETE Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                //the user changed their minds, bail out
                return;
            }

            DisableFormControls();

            //remove the call handler
            WebCallResult res = DistributionList.RemoveMember(GlobalItems.CurrentConnectionServer, DistributionListObjectId,strMemberObjectID);

          EnableFormControls();

            if (res.Success)
            {
                MessageBox.Show("Distribution List member removed");
                Logger.Log(string.Format("Distribution list member [{0}] objectId={1} removed.", strDisplayName, strMemberObjectID));

                //need to rebuild the list without the DL member we just deleted in it.
                LoadMembers();
            }
            else
            {
                MessageBox.Show("Failure removing distribution list member:" + res.ErrorText);
                Logger.Log(string.Format("Failed to remove distribution list member with display name={0} and objectId={1}.  Error={2}", strDisplayName, strMemberObjectID, res.ToString()));
            }

        }

        /// <summary>
        /// Allow the uer to select a subscriber to add to the distribution list as a member - you'd follow the same procedure to add a list just use
        /// the AddMemberPublicList function off the DistributionList class instead of the AddMemberUser.
        /// </summary>
        private void buttonAddUser_Click(object sender, EventArgs e)
        {
            using (FormFindUser oForm = new FormFindUser())
            {
                oForm.ShowDialog();
                if (oForm.DialogResult==DialogResult.Cancel)
                {
                    //user canceled - nothing to do here
                    return;
                }

                WebCallResult res = DistributionList.AddMemberUser(GlobalItems.CurrentConnectionServer,
                                                                   DistributionListObjectId, 
                                                                   oForm.UserObjectId);

                //if the user added ok force the membership grid to update to reflect that - otherwise give the user the error message and exit.
                if (res.Success)
                {
                    MessageBox.Show("User added");
                    LoadMembers();
                }
                else
                {
                    //note, adding a duplicate member will result in an error.
                    MessageBox.Show("Error adding user:" + res.ToString());
                }
            }
        }
    }
}
