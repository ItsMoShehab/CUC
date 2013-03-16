using System;
using System.Windows.Forms;

namespace CUPIFastStart
{
    public partial class FormNewDistributionListInfo : Form
    {

        //return the value entered on the form to the calling party via public params
        public string Alias { get; private set; }
        public string DisplayName { get; private set; }
        public string Extension { get; private set; }

        public FormNewDistributionListInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// cancel out - be sure to return cancel as the dialog result here
        /// </summary>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        /// <summary>
        /// User wants to 
        /// </summary>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            //make sure user has provided a display name and alias - the extension is optional.
            if (string.IsNullOrEmpty(textAlias.Text))
            {
                MessageBox.Show("You must provide an alias name");
                textAlias.Focus();
                return;
            }
            
            if (string.IsNullOrEmpty(textDisplayName.Text))
            {
                MessageBox.Show("You must provide a display name");
                textDisplayName.Focus();
                return;
            }



            //set the public properties and close the dialog with an indication that the OK button was hit.
            DisplayName = textDisplayName.Text;
            Alias = textAlias.Text;
            Extension = textExtension.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
