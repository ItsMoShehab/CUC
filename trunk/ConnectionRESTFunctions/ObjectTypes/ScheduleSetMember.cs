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
    /// simple read only class for providing lists of schedule set members
    /// </summary>
    public class ScheduleSetMember
    {
        public string ScheduleSetObjectId { get; set; }
        public string ScheduleObjectId { get; set; }
        public bool Exclude { get; set; }


        public ScheduleSetMember(string pScheduleSetObjectId)
        {
            ScheduleSetObjectId = pScheduleSetObjectId;
        }

    }
}
