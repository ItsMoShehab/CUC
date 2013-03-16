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
using ConnectionCUPIFunctions;

namespace CUPIFastStart
{
    public partial class FormNewCallHandlerInfo : Form
    {
        //return the value entered on the form to the calling party via public params
        public string DisplayName { get; private set; }
        public string Extension { get; private set; }
        public string TemplateObjectID { get; private set; }


        public FormNewCallHandlerInfo()
        {
            InitializeComponent();
        }


        //at form load time go fetch the handler templates that are on the server and load them into the combo box for selection.
        private void FormNewCallHandlerInfo_Load(object sender, EventArgs e)
        {
            List<CallHandlerTemplate> oTemplates;
            WebCallResult res;

            comboTemplate.Items.Clear();

            //fetch the templates defined on the Connection server
            res = CallHandlerTemplate.GetCallHandlerTemplates(GlobalItems.CurrentConnectionServer, out oTemplates);

            if (res.Success == false || oTemplates.Count == 0)
            {
                MessageBox.Show("Failure loading call handler templates:" + res.ErrorText);
                
                //dump the entire WebCallResult structure to the log for diagnostic purposes.
                SimpleLogger.Logger.Log(res.ToString());
                return;
            }

            //construct a simple hash table that puts the ObjectId in as the key and the Display Name in as the value.
            Hashtable ht = new Hashtable();

            foreach (CallHandlerTemplate oTemplate in oTemplates)
            {
                ht.Add(oTemplate.ObjectId, oTemplate.DisplayName);
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

        //user changed their minds - exiting application
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        //return the data on the form via public props after they pass a basic validation test.
        private void buttonOK_Click(object sender, EventArgs e)
        {
            //the user template would only not be selected if there was an error loading templates at form load.
            if (string.IsNullOrEmpty(comboTemplate.Text))
            {
                MessageBox.Show("No call handler template selected");
                return;
            }

            //make sure user has provided a display name - the extension is optional.
            if (string.IsNullOrEmpty(textDisplayName.Text))
            {
                MessageBox.Show("You must provide a display name");
                textDisplayName.Focus();
                return;
            }

            //set the public properties and close the dialog with an indication that the OK button was hit.
            DisplayName = textDisplayName.Text;
            Extension = textExtension.Text;

            //fetch the objectID off the selected user template item in the combobox.
            TemplateObjectID = comboTemplate.SelectedValue.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();

        }
    }
}
