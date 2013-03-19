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
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
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

        public string Alias { get; private set; }

        public bool AllowForeignMessages { get; private set; }
        
        public string DisplayName { get; private set; }
        public string DistributionListObjectId { get; private set; }

        public bool IsUserTemplate { get; private set; }

        public string LocationObjectId { get; private set; }

        public string MemberContactObjectId { get; private set; }
        public string MemberDistributionListObjectId { get; private set; }
        public string MemberGlobalObjectId { get; private set; }
        public string MemberGlobalDignetObjectId { get; private set; }
        public string MemberUserObjectId { get; private set; }

        public string MemberLocationObjectId { get; private set; }

        /// <summary>
        /// Not in CUPI's data, this is derived locally in the class for ease of filtering/presentation.
        /// </summary>
        public DistributionListMemberType MemberType { get; private set; }

        public string ObjectId { get; private set; }

        
        #endregion


        #region Static Methods

        /// <summary>
        /// Get all the members of a public distribution list returned as a generic list of DistributionListMember objects.
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
        /// <returns>
        /// WebCallResult instance
        /// </returns>
        public static WebCallResult GetDistributionListMembers(ConnectionServer pConnectionServer, string pDistributionListObjectId, 
            out List<DistributionListMember> pMemberList)
        {
            WebCallResult res = new WebCallResult();
            pMemberList=new List<DistributionListMember>();


            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetDistributionListMembers";
                return res;
            }

            string strUrl = string.Format("{0}distributionlists/{1}/distributionlistmembers", pConnectionServer.BaseUrl,pDistributionListObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.Get, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements may be empty - that's legal - return an empty list here
            if (res.XmlElement == null || res.XmlElement.HasElements == false)
            {
                pMemberList = new List<DistributionListMember>();
                return res;
            }

            pMemberList = GetDistributionListMembersFromXElements(pConnectionServer, res.XmlElement);
            return res;
        }


        /// <summary>
        ///Helper function to take an XML blob returned from the REST interface for a list (or listss) return and convert it into an generic
        ///list of DistributionList class objects. 
        /// </summary>
        private static List<DistributionListMember> GetDistributionListMembersFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {
            List<DistributionListMember> oDListMember = new List<DistributionListMember>();

            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced passed to GetDistributionListMembersFromXElements");
            }

            //pull out a set of XMLElements for each CallHandler object returned using the power of LINQ
            var listMembers = from e in pXElement.Elements()
                           where e.Name.LocalName == "DistributionListMember"
                           select e;

            //for each handler returned in the list of handlers from the XML, construct a CallHandler object using the elements associated with that 
            //handler.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlListMembers in listMembers)
            {
                DistributionListMember oDistributionListMember = new DistributionListMember();
                foreach (XElement oElement in oXmlListMembers.Elements())
                {
                    //adds the XML property to the CallHandler object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXmlFetch(oDistributionListMember, oElement);
                }

                //manually determine the member type here - this is a little hacky but it make life a bit easier by being able to filter
                //member types out in a way that is not provided by CUPI natively.
                if (!string.IsNullOrEmpty(oDistributionListMember.MemberContactObjectId))
                {
                    oDistributionListMember.MemberType = DistributionListMemberType.Contact;
                }
                else if (!string.IsNullOrEmpty(oDistributionListMember.MemberDistributionListObjectId))
                {
                    oDistributionListMember.MemberType = DistributionListMemberType.DistributionList;
                }
                else if (!string.IsNullOrEmpty(oDistributionListMember.MemberUserObjectId))
                {
                    oDistributionListMember.MemberType = DistributionListMemberType.LocalUser;
                }
                else
                {
                    oDistributionListMember.MemberType = DistributionListMemberType.GlobalUser;
                }

                //add the fully populated CallHandler object to the list that will be returned to the calling routine.
                oDListMember.Add(oDistributionListMember);
            }

            return oDListMember;
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
