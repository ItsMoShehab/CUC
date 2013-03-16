using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ConnectionCUPIFunctions
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

        public string Alias { get; private set; }
        public string DisplayName { get; private set; }
        public string PersonalVoiceMailListObjectId { get; private set; }
        public string Extension { get; private set; }

        public string MemberContactObjectId { get; private set; }
        public string MemberDistributionListObjectId { get; private set; }
        public string MemberSubscriberObjectId { get; private set; }
        public string MemberPersonalVoiceMailListObjectId { get; private set; }

        /// <summary>
        /// Not in CUPI's data, this is derived locally in the class for ease of filtering/presentation.
        /// </summary>
        public PrivateListMemberType MemberType { get; private set; }

        public string ObjectId { get; private set; }

        
        #endregion


        #region Static Methods

        /// <summary>
        /// Get the list of members of a private distribution list
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
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult GetPrivateListMembers(ConnectionServer pConnectionServer, string pPrivateListObjectId, string pOwnerUserObjectId,
            out List<PrivateListMember> pMemberList)
        {
            WebCallResult res = new WebCallResult();
            pMemberList = null;


            if (pConnectionServer == null)
            {
                res.ErrorText = "Null Connection server object passed to GetPrivateListMembers";
                return res;
            }

            string strUrl = string.Format("{0}users/{1}/privatelists/{2}/privatelistmembers", pConnectionServer.BaseUrl,pOwnerUserObjectId, pPrivateListObjectId);

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCUPIResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the XML elements can be empty, that's legal
            if (res.XMLElement == null || res.XMLElement.HasElements == false)
            {
                pMemberList = new List<PrivateListMember>();
                return res;
            }

            pMemberList = GetPrivateListMembersFromXElements(pConnectionServer, res.XMLElement);
            return res;
        }


        /// <summary>
        ///Helper function to take an XML blob returned from the REST interface for a list (or listss) return and convert it into an generic
        ///list of DistributionList class objects. 
        /// </summary>
        private static List<PrivateListMember> GetPrivateListMembersFromXElements(ConnectionServer pConnectionServer, XElement pXElement)
        {
            List<PrivateListMember> oDListMember = new List<PrivateListMember>();

            //pull out a set of XMLElements for each CallHandler object returned using the power of LINQ
            var listMembers = from e in pXElement.Elements()
                           where e.Name.LocalName == "PrivateListMember"
                           select e;

            //for each handler returned in the list of handlers from the XML, construct a CallHandler object using the elements associated with that 
            //handler.  This is done using the SafeXMLFetch routine which is a general purpose mechanism for deserializing XML data into strongly
            //types objects.
            foreach (var oXmlListMembers in listMembers)
            {
                PrivateListMember oPrivateListMember = new PrivateListMember();
                foreach (XElement oElement in oXmlListMembers.Elements())
                {
                    //adds the XML property to the CallHandler object if the proeprty name is found as a property on the object.
                    pConnectionServer.SafeXMLFetch(oPrivateListMember, oElement);
                }

                //manually determine the member type here - this is a little hacky but it make life a bit easier by being able to filter
                //member types out in a way that is not provided by CUPI natively.
                if (!string.IsNullOrEmpty(oPrivateListMember.MemberContactObjectId))
                {
                    oPrivateListMember.MemberType = PrivateListMemberType.RemoteContact;
                }
                else if (!string.IsNullOrEmpty(oPrivateListMember.MemberDistributionListObjectId))
                {
                    oPrivateListMember.MemberType = PrivateListMemberType.DistributionList;
                }
                else if (!string.IsNullOrEmpty(oPrivateListMember.MemberPersonalVoiceMailListObjectId))
                {
                    oPrivateListMember.MemberType = PrivateListMemberType.PrivateList;
                }
                else 
                {
                    oPrivateListMember.MemberType = PrivateListMemberType.LocalUser;
                }

                //add the fully populated CallHandler object to the list that will be returned to the calling routine.
                oDListMember.Add(oPrivateListMember);
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