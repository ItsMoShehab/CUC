#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// The DistribtuionListMember class contains all the properties associated with a distribution list member in Unity Connection that can be fetched via the 
    /// CUPI interface.  This class is used only to provide a full list of members on a public distribution list - the member information cannot be edited and you
    /// cannot fetch individual member - only the full list of members associated with a list can be created and returned. 
    /// You can add/remove members from a list going through the DistributionList class.  The DistributionListMember class here is for presentation of the membership
    /// </summary>
    public class DistributionListMember : IUnityDisplayInterface
    {

        #region Fields and Properties

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return DisplayName; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        /// <summary>
        /// Not in CUPI's data, this is derived locally in the class for ease of filtering/presentation.
        /// </summary>
        public DistributionListMemberType MemberType { get; private set; }

        #endregion


        #region DistributionListMember Properties

        [JsonProperty]
        public string Alias { get; private set; }

        [JsonProperty]
        public bool AllowForeignMessage { get; private set; }

        [JsonProperty]
        public string DisplayName { get; private set; }

        [JsonProperty]
        public string DistributionListObjectId { get; private set; }

        [JsonProperty]
        public bool IsUserTemplate { get; private set; }

        [JsonProperty]
        public string LocationObjectId { get; private set; }

        [JsonProperty]
        public string MemberContactObjectId { get; private set; }

        [JsonProperty]
        public string MemberDistributionListObjectId { get; private set; }

        [JsonProperty]
        public string MemberGlobalUserObjectId { get; private set; }

        [JsonProperty]
        public string MemberGlobalUserDignetObjectId { get; private set; }

        [JsonProperty]
        public string MemberUserObjectId { get;  set; }

        [JsonProperty]
        public string MemberLocationObjectId { get; private set; }

        [JsonProperty]
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
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>        
        /// <returns>
        /// WebCallResult instance
        /// </returns>
        public static WebCallResult GetDistributionListMembers(ConnectionServer pConnectionServer, string pDistributionListObjectId,
            out List<DistributionListMember> pMemberList, int pPageNumber = 1, int pRowsPerPage = 20, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            pMemberList=new List<DistributionListMember>();


            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetDistributionListMembers";
                return res;
            }

            //tack on the paging items to the parameters list
            List<string> temp;
            if (pClauses == null)
            {
                temp = new List<string>();
            }
            else
            {
                temp = pClauses.ToList();
            }

            temp.Add("pageNumber=" + pPageNumber);
            temp.Add("rowsPerPage=" + pRowsPerPage);

            string strUrl = ConnectionServer.AddClausesToUri(string.Format("{0}distributionlists/{1}/distributionlistmembers", pConnectionServer.BaseUrl, 
                pDistributionListObjectId), temp.ToArray());

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }


            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty thats an error
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                pMemberList = new List<DistributionListMember>();
                return res;
            }

            //not an error, just return an empty list
            if (res.TotalObjectCount == 0)
            {
                pMemberList=new List<DistributionListMember>();
                return res;
            }

            pMemberList = pConnectionServer.GetObjectsFromJson<DistributionListMember>(res.ResponseText);

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
        /// string containing all the name value pairs defined in the object instance.
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
