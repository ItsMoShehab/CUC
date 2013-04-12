#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Cisco.UnityConnection.RestFunctions;

namespace CUPIFastStart
{
    /// <summary>
    /// Simple form that allows the user to provide basic data for creating a new user.  The alias and extension are required, the other
    /// values may be blank.
    /// </summary>
    public partial class FormNewUserInfo : Form
    {
        //form returns values via public properties
        public string Alias { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string DisplayName { get; private set; }
        public string Extension { get; private set; }
        public string TemplateAlias { get; private set; }

        public FormNewUserInfo()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        //user presses ok button - verify that at least the alias and extension values are present.
        //For a production application you'd probalby want to do some business logic validation on the extension value, check for illegal
        //characters in fields etc... before sending it off to the CUPI interface.
        private void buttonOK_Click(object sender, EventArgs e)
        {
            //the user template would only not be selected if there was an error loading templates at form load.
            if (string.IsNullOrEmpty(comboTemplate.Text))
            {
                MessageBox.Show("No user template selected");
                return;
            }

            //make sure user has provided required parameters for user creation.
            if (string.IsNullOrEmpty(textUserAlias.Text))
            {
                MessageBox.Show("You must provide an alias");
                textUserAlias.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textUserExtension.Text))
            {
                MessageBox.Show("You must provide an extension");
                textUserExtension.Focus();
                return;
            }

            //set the public properties and close the dialog with an indication that the OK button was hit.
            Alias = textUserAlias.Text;
            FirstName = textUserFirstName.Text;
            LastName = textUserLastName.Text;
            DisplayName = textUserDisplayName.Text;
            Extension = textUserExtension.Text;

            //fetch the alias off the selected user template item in the combobox.
            TemplateAlias = comboTemplate.SelectedValue.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();

        }


        //at form load time go fetch the user templates that are on the server and load them into the combo box for selection.
        private void FormNewUserInfo_Load(object sender, EventArgs e)
        {
            List<UserTemplate> oTemplates;
            WebCallResult res;
            
            comboTemplate.Items.Clear();

            //fetch the templates defined on the Connection server
            res = UserTemplate.GetUserTemplates(GlobalItems.CurrentConnectionServer,out oTemplates);

            if (res.Success==false || oTemplates.Count==0)
            {
                MessageBox.Show("Failure loading user templates:" + res.ErrorText);
                SimpleLogger.Logger.Log(res.ToString());
                return;
            }

            //construct a simple hash table that puts the ObjectId in as the key and the Display Name in as the value.
            Hashtable ht = new Hashtable();

            foreach (UserTemplate oTemplate in oTemplates)
            {
                ht.Add(oTemplate.Alias, oTemplate.DisplayName);
            }

            //bind the combobox to the hash table
            BindingSource bs = new BindingSource();
            bs.DataSource = ht;
            
            comboTemplate.DataSource = bs;
            comboTemplate.DisplayMember = "value";
            comboTemplate.ValueMember = "key";
            
            //force the first template to be selected
            comboTemplate.SelectedIndex = 0;

        }

        //have the cursor focus on the first field that needs to be filled in.
        private void FormNewUserInfo_Shown(object sender, EventArgs e)
        {
            textUserAlias.Focus();
        }
    }
}
