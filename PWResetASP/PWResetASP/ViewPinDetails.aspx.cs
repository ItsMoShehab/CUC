using System;
using Cisco.UnityConnection.RestFunctions;

namespace PWResetASP
{
    /// <summary>
    /// Page to show the PIN details (is it locked, failed attempts etc...) for a selected user.
    /// </summary>
    public partial class ViewPinDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UserBase oUser;

            //add a javascript chunk to the button to go back to the calling page
            ButtonGoBack.Attributes.Add("onClick", "javascript:history.back(); return false;");

            //check to see if the user object is loaded into the session variable as it should be.  If not then 
            //redirect back to the select user page
            if (Session["SelectedUser"]==null)
            {
                Session["ErrorNote"] = "You must first select a user before you can show their PIN details";
                Response.Redirect("~/SelectUser.aspx");
            }

            oUser = (UserBase)Session["SelectedUser"];

            LabelUserInfo.Text = "Connection User: "+oUser.ToString();

            string strPinDetails = "PIN Details:"+ Environment.NewLine+ oUser.Pin().DumpAllProps("     ");
            LabelPinDetails.Text = GlobalItems.FormatStringForHTML(strPinDetails);

            //remove the user from the session
            Session.Remove("SelectedUser");

            ButtonGoBack.Focus();
        }
    }
}