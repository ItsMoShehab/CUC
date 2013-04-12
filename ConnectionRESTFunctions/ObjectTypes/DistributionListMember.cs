#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// For human readable output of distribution list member types
    /// </summary>
    public enum DistributionListMemberType
    {
      	LocalUser, GlobalUser, Contact, DistributionList
    }

    /// <summary>
    /// The DistribtuionListMember class contains all the properties associated with a distribution list member in Unity Connection that can be fetched via the 
    /// CUPI interface.  This class is used only to provide a full list of members on a public distribution list - the member information cannot be edited and you
    /// cannot fetch individual member - only the full list of members associated with a list can be created and returned. 
    /// You can add/remove members from a list going through the DistributionList class.  The DistributionListMember class here is for presentation of the membership
    /// </summary>
    public class DistributionListMember
    {

        #region Properties

        
        public string Alias { get; set; }

        public bool AllowForeignMessages { get; set; }
        
        public string DisplayName { get; set; }
        public string DistributionListObjectId { get; set; }

        public bool IsUserTemplate { get; set; }

        public string LocationObjectId { get; set; }

        public string MemberContactObjectId { get; set; }
        public string MemberDistributionListObjectId { get; set; }
        public string MemberGlobalObjectId { get; set; }
        public string MemberGlobalDignetObjectId { get; set; }
        public string MemberUserObjectId { get;  set; }

        public string MemberLocationObjectId { get; set; }

        /// <summary>
        /// Not in CUPI's data, this is derived locally in the class for ease of filtering/presentation.
        /// </summary>
        public DistributionListMemberType MemberType { get; set; }

        public string ObjectId { get;  set; }

        
        #endregion


        #region Static Methods

        /// <summary>
        /// GET all the members of a public distribution list returned as a generic list of DistributionListMember objects.
        /// </summary>
        /// <param name="pDistributionListObjectId">
        /// Distribution list to fetch membership for
        /// </param>
        /// <param name="pMemberList">
        /// list of members is returned on this out param as a generic list.
        /// </param>
        /// <param name="pConnectionServer">
        /// Connection server to query against
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <returns>
        /// WebCallResult instance
        /// </returns>
        public static WebCallResult GetDistributionListMembers(ConnectionServer pConnectionServer, string pDistributionListObjectId, 
            out List<DistributionListMember> pMemberList,int pPageNumber=1, int pRowsPerPage=20)
        {
            WebCallResult res = new WebCallResult();
            pMemberList=new List<DistributionListMember>();


            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetDistributionListMembers";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(string.Format("{0}distributionlists/{1}/distributionlistmembers", pConnectionServer.BaseUrl, 
                pDistributionListObjectId), "pageNumber=" + pPageNumber, "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }


            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that's not an error, just return an empty list
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pMemberList = new List<DistributionListMember>();
                return res;
            }

            pMemberList = HTTPFunctions.GetObjectsFromJson<DistributionListMember>(res.ResponseText);

            if (pMemberList == null)
            {
                pMemberList = new List<DistributionListMember>();
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pMemberList)
            {
                //manually determine the member type here - this is a little hacky but it make life a bit easier by being able to filter
                //member types out in a way that is not provided by CUPI natively.
                if (!string.IsNullOrEmpty(oObject.MemberContactObjectId))
                {
                    oObject.MemberType = DistributionListMemberType.Contact;
                }
                else if (!string.IsNullOrEmpty(oObject.MemberDistributionListObjectId))
                {
                    oObject.MemberType = DistributionListMemberType.DistributionList;
                }
                else if (!string.IsNullOrEmpty(oObject.MemberUserObjectId))
                {
                    oObject.MemberType = DistributionListMemberType.LocalUser;
                }
                else
                {
                    oObject.MemberType = DistributionListMemberType.GlobalUser;
                }

            }

            return res;
        }


        #endregion 


        #region Instance Methods

        /// <summary>
        /// Diplays the alias, display name and type of member (list, user, contact)
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0}: {1} [{2}]", this.MemberType, this.Alias, this.DisplayName );
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the distribution list member object in "name=value" format - each pair is on its
        /// own line in the string returned.
        /// </summary>
        /// <param name="pPrefix">
        /// Optional parameter for a sting that will preceed each name/value pair as it's dumped out.  This can be useful for indenting an object's
        /// property dump when writing to a log file for instance.
        /// </param>
        /// <returns>
        /// string containing all the name value pairs defined in the call handler object instance.
        /// </returns>
        public string DumpAllProps(string pPrefix = "")
        {
            StringBuilder strBuilder = new StringBuilder();

            PropertyInfo[] oProps = this.GetType().GetProperties();

            foreach (PropertyInfo oProp in oProps)
            {
                strBuilder.AppendFormat("{0}{1} = {2}\n", pPrefix, oProp.Name, oProp.GetValue(this, BindingFlags.GetProperty, null, null, null));
            }

            return strBuilder.ToString();
        }


        #endregion 


    }
}
