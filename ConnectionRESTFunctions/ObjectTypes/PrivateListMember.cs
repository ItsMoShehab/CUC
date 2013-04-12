﻿#region Legal Disclaimer

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
    /// Make the private list member type enum more human readable in output.
    /// </summary>
    public enum PrivateListMemberType
    {
      	LocalUser, RemoteContact, DistributionList, PrivateList
    }

    /// <summary>
    /// Class for fetching and enumerating private list members
    /// </summary>
    public class PrivateListMember
    {

        #region Properties

        public string Alias { get; set; }
        public string DisplayName { get; set; }
        public string PersonalVoiceMailListObjectId { get; set; }
        public string Extension { get; set; }

        public string MemberContactObjectId { get; set; }
        public string MemberDistributionListObjectId { get; set; }
        public string MemberSubscriberObjectId { get; set; }
        public string MemberPersonalVoiceMailListObjectId { get; set; }

        public string ObjectId { get; set; }

        /// <summary>
        /// Not in CUPI's data, this is derived locally in the class for ease of filtering/presentation.
        /// </summary>
        public PrivateListMemberType MemberType { get; private set; }

        
        #endregion


        #region Static Methods

        /// <summary>
        /// GET the list of members of a private distribution list
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the private list is homed on.
        /// </param>
        /// <param name="pPrivateListObjectId">
        /// The Unique identifier for the private list
        /// </param>
        /// <param name="pOwnerUserObjectId">
        /// The user that owns the private list
        /// </param>
        /// <param name="pMemberList">
        /// The list of members is returned as a generic list of PrivateListMember classes on this out param
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>        
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult GetPrivateListMembers(ConnectionServer pConnectionServer, string pPrivateListObjectId, string pOwnerUserObjectId,
            out List<PrivateListMember> pMemberList,int pPageNumber=1, int pRowsPerPage=20)
        {
            WebCallResult res = new WebCallResult();
            pMemberList = null;


            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPrivateListMembers";
                return res;
            }

            string strUrl = HTTPFunctions.AddClausesToUri(string.Format("{0}users/{1}/privatelists/{2}/privatelistmembers", pConnectionServer.BaseUrl, pOwnerUserObjectId, 
                pPrivateListObjectId), "pageNumber=" + pPageNumber, "rowsPerPage=" + pRowsPerPage);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one template
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pMemberList = new List<PrivateListMember>();
                res.Success = false;
                return res;
            }

            pMemberList = HTTPFunctions.GetObjectsFromJson<PrivateListMember>(res.ResponseText);

            if (pMemberList == null)
            {
                pMemberList = new List<PrivateListMember>();
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
                    oObject.MemberType = PrivateListMemberType.RemoteContact;
                }
                else if (!string.IsNullOrEmpty(oObject.MemberDistributionListObjectId))
                {
                    oObject.MemberType = PrivateListMemberType.DistributionList;
                }
                else if (!string.IsNullOrEmpty(oObject.MemberPersonalVoiceMailListObjectId))
                {
                    oObject.MemberType = PrivateListMemberType.PrivateList;
                }
                else
                {
                    oObject.MemberType = PrivateListMemberType.LocalUser;
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