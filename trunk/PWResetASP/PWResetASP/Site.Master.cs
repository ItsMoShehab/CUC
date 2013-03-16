using System.Web.UI;

namespace PWResetASP
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        /// <summary>
        /// If the user is currently logged in/authenticated the session will have a 
        /// CurrentconnectionServer object in it - if not, it will be null - update the 
        /// text of the login button accordingly.
        /// </summary>
        protected override void Render(HtmlTextWriter writer)
        {
            if (Session["CurrentConnectionServer"]==null)
            {
                LinkButtonLogin.Text = "[Log In]";
            }
            else
            {
                LinkButtonLogin.Text = "[Log Out]";
            }

            base.Render(writer);
        }


        /// <summary>
        /// For either log in or log out the action is the same - clear the session and redirect to the 
        /// login page.
        /// </summary>
        protected void LinkButtonLogin_Click(object sender, System.EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Default.aspx");
        }
    }
}
