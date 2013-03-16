using System;
using ConnectionCUPIFunctions;

namespace PWResetASP.Account
{
 /// <summary>
 /// This is the main entry point page for the web application - as with most CUPI applications you'll want your entry point to be 
 /// the login page and not allow the user access to the rest of your application if they are not an admin or the server isn't valid.
 /// For the web app here we'll be using a session variable to cart around our (rather small) ConnectionServer object that is used in
 /// other pages of the application to ensure the user is authenticated.
 /// </summary>
    public partial class Login : System.Web.UI.Page
    {

        /// <summary>
        /// Park the input cursor at the server name text box.
        /// Note that the ASP attributes in the mark up page have the login button as the default recipient of the 
        /// enter key for this form.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            //if the user is redirected to the login page for invalid access or the like, an error message will be in 
            //the session - check for it here.
            if (Session["ErrorNote"] != null )
            {
                LabelStatus.Text = Session["ErrorNote"].ToString();
                Session.Remove("ErrorNote");
            }

            TextBoxServerName.Focus();

        }


        /// <summary>
        /// Verify that the user's account exists on the Unity Connection server and that they have administrator rights.  The login
        /// to CUPI for admins will fail otherwise.
        /// If the login goes through add the user information to the session variable - this doesn't get used for anything other than 
        /// as verification that the currently logged in user is authenticated - the credentials have to be resent with each CUPI command
        /// regardless.  This just makes sure the user isn't getting cute and going directly to the user selection page.
        /// </summary>
        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            ConnectionServer currentConnectionServer;

            //check for empty strings - yes, you can use ASP form control checks for these but then you have difficulty with the Java Script
            //method of enabling/disabling the login button on the client side so I do the checks manually here and just use a single status
            //label for feedback - not quite as slick but the trade off is worth it.
            if (string.IsNullOrEmpty(TextBoxServerName.Text.Trim()))
            {
                LabelStatus.Text = "You must provide a Connection server name";
                TextBoxServerName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(TextBoxName.Text.Trim()))
            {
                LabelStatus.Text = "You must provide a login name";
                TextBoxName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(TextBoxPassword.Text.Trim()))
            {
                LabelStatus.Text = "You must provide a password";
                TextBoxPassword.Focus();
                return;
            }

            LabelStatus.Text = "";

            try
            {
                currentConnectionServer = new ConnectionServer(TextBoxServerName.Text.Trim(), TextBoxName.Text.Trim(), TextBoxPassword.Text.Trim());
            }
            catch
            {
                LabelStatus.Text="Login failed, make sure the server name is valid, DNS is working properly and the user name and login are valid";
                ButtonLogin.Enabled = true;
                return;
            }

            //if the class creation failed but the server login still didn't complete (this should really never happen, but just in case)
            if (currentConnectionServer.ServerName.Length == 0)
            {
                //login failed = give the user the chance to try again.
                LabelStatus.Text="Login failed, make sure the server name is valid, DNS is working properly and the user name and login are valid";
                ButtonLogin.Enabled = true;
                return;
            }

            //stuff the current connection server object into the session state where we can pull it out later.  Then redirect to the select user 
            //page which is where all the action takes place.
            Session["CurrentConnectionServer"] = currentConnectionServer;
            Response.Redirect("~/SelectUser.aspx");
        }
    }
}
