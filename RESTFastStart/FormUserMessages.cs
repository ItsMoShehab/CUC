using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ConnectionCUPIFunctions;

namespace CUPIFastStart
{
    public partial class FormUserMessages : Form
    {
        /// <summary>
        /// provides a way for the calling client to pass in the ObjectId of the user to fetch messages for.
        /// </summary>
        public string UserObjectId { get; set; }

        public FormUserMessages()
        {
            InitializeComponent();
        }


        /// <summary>
        /// bail out.
        /// </summary>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// grab all the messages for a the UserObjectID passed in.
        /// </summary>
        private void FormUserMessages_Load(object sender, EventArgs e)
        {
            List<UserMessage> oMessages;

            WebCallResult res= UserMessage.GetMessages(GlobalItems.CurrentConnectionServer, UserObjectId, out oMessages);

            if (res.Success==false)
            {
                MessageBox.Show("Failure fetching messages:" + res.ToString());
            }

            this.gridMessages.DataSource = oMessages;
        }


        /// <summary>
        /// Fetch all the media attachments for this voice mail and assemble them
        /// </summary>
        private void buttonFetchAttachments_Click(object sender, EventArgs e)
        {
            //check that a message is selected first
            if (gridMessages.SelectedRows.Count < 1)
            {
                MessageBox.Show("You must first select a message to fetch attachments for.");
                return;
            }

            //get the message ID off the grid selection
            string strMessageObjectId = gridMessages.SelectedRows[0].Cells["MsgId"].Value.ToString();
            
            WebCallResult res;
            int iCount;            
            
            //see how many attachments are on the message - typically this is only 1 but with forwarding scenarios it might be more.
            res = UserMessage.GetMessageAttachmentCount(GlobalItems.CurrentConnectionServer,
                                                        strMessageObjectId,
                                                        UserObjectId,
                                                        out iCount);

            if (res.Success==false)
            {
                MessageBox.Show("Failed to get attachment count:" + res.ToString());
                return;
            }

            //create a temporary folder to dump the WAV files into.
            string strLocalPath = Path.GetTempPath() + Guid.NewGuid();
            Directory.CreateDirectory(strLocalPath);

            //go fetch all the attachments in order - 0 being the most recent and getting older from there (i.e. 0 part is played first 
            //to the user).
            for (int iCounter=0;iCounter<iCount;iCounter++)
            {
                res = UserMessage.GetMessageAttachment(GlobalItems.CurrentConnectionServer,
                                                       string.Format(@"{0}\{1}_{2}.wav",strLocalPath,strMessageObjectId.Replace(":","_"),iCounter),
                                                       strMessageObjectId,
                                                       UserObjectId,
                                                       iCounter);

                if (res.Success==false)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    MessageBox.Show("Error downloading attachment media for message:" + res.ToString());
                }
            }

            //pop open the folder we created
            Process.Start("explorer.exe", strLocalPath);

        }
    }
}
