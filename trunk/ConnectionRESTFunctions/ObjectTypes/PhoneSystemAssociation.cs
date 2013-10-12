#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Object for listing phone system associations for a media switch.  Very simple class with no constructors or 
    /// methods other than the ToStringOverride.  Currently used only in the PhoneSystem class.
    /// </summary>
    [Serializable]
    public class PhoneSystemAssociation
    {

        #region PhoneSystemAssociation Properties

        [JsonProperty]
        public string Alias { get; private set; }

        [JsonProperty]
        public string DisplayName { get; private set; }

        [JsonProperty]
        public string MediaSwitchDisplayName { get; private set; }

        [JsonProperty]
        public string MediaSwitchObjectId { get; private set; }

        [JsonProperty]
        public int NumNoitificationDevice { get; private set; }

        [JsonProperty]
        public int NumNotificationMwi { get; private set; }

        [JsonProperty]
        public int NumPrimaryCallHandler { get; private set; }

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
