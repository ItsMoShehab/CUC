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
    /// simple read only class for providing lists of schedule set members
    /// </summary>
    [Serializable]
    public class ScheduleSetMember
    {

        #region Constructors and Destructors

        /// <summary>
        /// empty constructor for JSON serializer
        /// </summary>
        public ScheduleSetMember()
        {

        }
        
        /// <summary>
        /// Simple constructor with scheduleset objectId value passed in
        /// </summary>
        public ScheduleSetMember(string pScheduleSetObjectId)
        {
            ScheduleSetObjectId = pScheduleSetObjectId;
        }

        #endregion


        #region ScheduleSetMember Properties

        [JsonProperty]
        public string ScheduleSetObjectId { get; private set; }

        [JsonProperty]
        public string ScheduleObjectId { get; private set; }

        [JsonProperty]
        public bool Exclude { get; private set; }

        #endregion

    }
}
