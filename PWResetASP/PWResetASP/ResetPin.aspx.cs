using System;
using Cisco.UnityConnection.RestFunctions;

namespace PWResetASP
{
    public partial class ResetPin : System.Web.UI.Page
    {

        private UserBase _user;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            //check to see if the user object is loaded into the session variable as it should be.  If not then 
            //redirect back to the select user page
            if (Session["SelectedUser"] == null)
            {
                Session["ErrorNote"] = "You must first select a user before you can reset their PIN";
                Response.Redirect("~/SelectUser.aspx");
            }

            _user = (UserBase)Session["SelectedUser"];

            LabelUserInfo.Text = "Connection User: " + _user.ToString();

            TextBoxNewPin.Focus();

        }


        /// <summary>
        /// Clear the user object from the session, stick a note that the PIN reset was canceled and redirect
        /// back to the user's page
        /// </summary>
        protected void buttonCancel_Click(object sender, EventArgs e)
        {
            Session["ErrorNote"] = "PIN reset canceled for: "+_user.ToString();
            try
            {
                Session.Remove("SelectedUser");
            }
            catch{}

            Response.Redirect("~/SelectUser.aspx");

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void buttonOK_Click(object sender, EventArgs e)
        {
            long lTemp;

            //check PINs
            if (TextBoxNewPin.Text.Trim().Length < 3)
            {
                LabelStatus.Text="Invalid PIN - must be at least 3 digits long";
                return;
            }

            if (long.TryParse(TextBoxNewPin.Text, out lTemp) == false)
            {
                LabelStatus.Text="Invalid PIN - must be all digits.";
                return;
            }

            if (TextBoxNewPin.Text.Equals(TextBoxVerifyNewPin.Text) == false)
            {
                LabelStatus.Text = "The two PINs you entered do not match.";
                return;
            }

            //reset the PIN - pass in null's for the flag values we aren't allowing the user to fiddle with so their current
            //values will be left alone.  Easy enough to extend the PIN collection form to allow passing the "can't change" and
            //"locked" values if you want.
            WebCallResult res = _user.ResetPin(TextBoxNewPin.Text, false, checkMustChange.Checked, null,
                                               checkDoesNotExpire.Checked, checkClearHackedLockout.Checked);
            if (res.Success==false)
            {
                LabelStatus.Text=string.Format("PIN reset failed for user: {0}/nError={1}", _user.Alias, GlobalItems.FormatStringForHTML(res.ToString()));
                return;
            }
            
            Session["ErrorNote"] = "PIN reset for: "+_user.ToString();

            //remove user object from session
            try
            {
                Session.Remove("SelectedUser");
            }
            catch { }

            Response.Redirect("~/SelectUser.aspx");
        }
    }
}