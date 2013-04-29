#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Object for listing phone system associations for a media switch.  Very simple class with no constructors or 
    /// methods other than the ToStringOverride.  Currently used only in the PhoneSystem class.
    /// </summary>
    public class PhoneSystemAssociation
    {

        #region PhoneSystemAssociation Properties

        public string Alias { get; set; }
        public string DisplayName { get; set; }
        public string MediaSwitchDisplayName { get; set; }
        public string MediaSwitchObjectId { get; set; }
        public int numNoitificationDevice { get; set; }
        public int numNotificationMWI { get; set; }
        public int numPrimaryCallHandler { get; set; }

        #endregion


        #region Instance Methods

        public override string ToString()
        {
            return string.Format("User alias={0}, display name={1}, switch name={2}", Alias, DisplayName,
                                 MediaSwitchDisplayName);
        }

        #endregion

    }
}
