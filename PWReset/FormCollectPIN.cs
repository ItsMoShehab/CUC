#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Windows.Forms;

namespace PWReset
{

    /// <summary>
    /// Simple form to collect a new PIN from the user.  The value is returned as a public property.  Basic
    /// validity checking is done up front - at least make sure the string is all digits and a minimum of 3 
    /// characters long and make the user enter the same string twice correctly.
    /// </summary>
    public partial class FormCollectPIN : Form
    {
        //values from the form are passed back via public properties
        public string NewPIN { get; private set; }
        public bool MustChange { get; private set; }
        public bool DoesNotExpire { get; private set; }
        public bool ClearHackedLockout { get; private set; }

        public FormCollectPIN()
        {
            InitializeComponent();
        }

        //check that the PIN is not blank and all digits and that both entries match one another.
        private void buttonOK_Click(object sender, EventArgs e)
        {
            long lTemp;

            if (textPIN.Text.Length < 3)
            {
                MessageBox.Show("Invalid PIN - must be at least 3 character long");
                return;
            }

            if (long.TryParse(textPIN.Text, out lTemp) == false)
            {
                MessageBox.Show("Invalid PIN - must be all digits.");
                return;
            }

            if (textPIN.Text.Equals(textPINVerify.Text) == false)
            {
                MessageBox.Show("The two PINs you entered do not match.");
                return;
            }

            //return values via public properties and close out the form.
            NewPIN = textPIN.Text;
            MustChange = checkMustChange.Checked;
            DoesNotExpire = checkDoesNotExpire.Checked;
            ClearHackedLockout = checkClearHackedLockout.Checked;

            DialogResult = DialogResult.OK;
            Close();

        }

        private void FormCollectPIN_Shown(object sender, EventArgs e)
        {
            this.textPIN.Focus();

        }

        private void FormCollectPIN_Load(object sender, EventArgs e)
        {

        }
    }
}
