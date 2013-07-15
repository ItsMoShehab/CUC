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
    /// Class that contains methods for finding, fetching, adding, updating and deleting search space object in Connection
    /// </summary>
    public class SearchSpace :IUnityDisplayInterface
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires ConnectionServer object where SearchSpace is homed.  Can optionally pass ObjectId or name
        /// of search space to load data for.
        /// </summary>
        public SearchSpace(ConnectionServerRest pConnectionServer, string pObjectId = "", string pName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to SearchSpace construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pName))
            {
                return;
            }

            WebCallResult res = GetSearchSpace(pObjectId, pName);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Unable to find search space by objectId={0}, name={1}. Error={2}", 
                    pObjectId, pName, res));
            }
        }

        /// <summary>
        /// General constructor for Json parsing library
        /// </summary>
        public SearchSpace()
        {
        }

        #endregion


        #region Fields and Properties 

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return Name; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }


        //reference to the ConnectionServer object used to create this instance.
        public ConnectionServerRest HomeServer { get; private set; }

        private List<Partition> _partitions;
        /// <summary>
        /// Lazy fetch method to return list of member partitions for the search space instance.  Impelmented as a method isntead of a property
        /// so it doesn't get triggered when a generic list of search space objects is bound to a grid or executed in LINQ function.
        /// </summary>
        /// <param name="pForceRefetchOfData">
        /// Pass as true to force the partitions to be refetched from the database even if they've already been populated
        /// </param>
        /// <returns>
        /// Generic list of Partition objects in order
        /// </returns>
        public List<Partition> GetSearchSpaceMembers(bool pForceRefetchOfData = false)
        {
            if (pForceRefetchOfData)
            {
                _partitions = null;
            }

            if (_partitions == null)
            {
                GetPartitions(out _partitions);
            }

            return _partitions;
        }

        #endregion


        #region SearchSpace Properties

        [JsonProperty]
        public string Description { get; private set;}

        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string LocationObjectId { get; private set; }

        [JsonProperty]
        public DateTime TimeOwnershipChanged { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the partition
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", Description, ObjectId);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the SearchSpace object in "name=value" format - each pair is on its
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


        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchSearchSpaceData()
        {
            return GetSearchSpace(this.ObjectId,"");
        }

        /// <summary>
        /// Fills current instance of class with details of search space for objectId passed in if found.
        /// </summary>
        /// <param name="pObjectId">
        /// Unique Id for search space to load
        /// </param>
        /// <param name="pName">
        /// Optional name of the search space to look for if an ObjectId is not provided - not case sensitive
        /// </param>
        /// <returns>
        /// Instance of WebCallResult class
        /// </returns>
        private WebCallResult GetSearchSpace(string pObjectId, string pName)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            string strObjectId = pObjectId;
            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdFromName(pName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    res.ErrorText = "Unable to find search space by name=" + pName;
                    return res;
                }
            }

            string strUrl = HomeServer.BaseUrl + "searchspaces/" + strObjectId;

            //issue the command to the CUPI interface
            res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this, RestTransportFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }
            return res;
        }


        /// <summary>
        /// Fetch the ObjectId of a search space by it's name.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pName">
        /// Name of the search space to find
        /// </param>
        /// <returns>
        /// ObjectId of search space if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromName(string pName)
        {
            string strUrl = HomeServer.BaseUrl + string.Format("searchspaces/?query=(Name is {0})", pName.UriSafe());

            //issue the command to the CUPI interface
            WebCallResult res = HomeServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false || res.TotalObjectCount == 0)
            {
                return "";
            }

            List<SearchSpace> oSearchSpaces = HomeServer.GetObjectsFromJson<SearchSpace>(res.ResponseText);

            foreach (var oSpace in oSearchSpaces)
            {
                if (oSpace.Name.Equals(pName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oSpace.ObjectId;
                }
            }

            return "";
        }


        /// <summary>
        /// Returns a list of all partitions that are members of the search spaces.
        /// </summary>
        /// <param name="pPartitions"></param>
        /// <returns></returns>
        private WebCallResult GetPartitions(out List<Partition> pPartitions)
        {
            return GetPartitions(HomeServer, this.ObjectId, out pPartitions);
        }


        /// <summary>
        /// Remove a search space from the Connection directory.  If this search space is being referenced the removal will fail.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public WebCallResult Delete()
        {
            return DeleteSearchSpace(HomeServer, this.ObjectId);
        }


        /// <summary>
        /// Update a search space - the only items you're allowed to change on a standing space are its name and description so those are 
        /// provided as optional parameters instead of the more generic name/value pair structure followed by some other classes.
        /// Use the seperate methods for adding/removing partitions as members of the search space.
        /// </summary>
        /// <param name="pName">
        /// Updated name - optional
        /// </param>
        /// <param name="pDescription">
        /// Updated description - optional
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public WebCallResult Update(string pName = "", string pDescription = "")
        {
            return UpdateSearchSpace(HomeServer, this.ObjectId, pName, pDescription);
        }

        /// <summary>
        /// Add a partition as member of a search space - the partition is added at the end of the list in priority order
        /// </summary>
        /// <param name="pPartitionObjectId">
        /// Partition to be added
        /// </param>
        /// <param name="pSortOrder">
        /// 1 based sort order.  1 is first, 2 is 2nd etc... the numbers you use for partition sequence don't matter and they do not have to 
        /// be contiguous, but they do have to be unique - best to use the member count +1 mechanism to add new partitions to the end.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult AddSearchSpaceMember(string pPartitionObjectId, int pSortOrder)
        {
            return AddSearchSpaceMember(this.HomeServer, this.ObjectId, pPartitionObjectId,pSortOrder);
        }

        /// <summary>
        /// remove a partition as member of a search space
        /// </summary>
        /// <param name="pPartitionObjectId">
        /// Partition to be removed
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult DeleteSearchSpaceMember(string pPartitionObjectId)
        {
            return DeleteSearchSpaceMember(this.HomeServer, this.ObjectId, pPartitionObjectId);
        }

        #endregion


        #region Static Methods


        /// <summary>
        /// Gets the list of all search spaces and resturns them as a generic list of SearchSpace objects. 
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that the search space should be pulled from
        /// </param>
        /// <param name="pSearchSpaces">
        /// Out parameter that is used to return the list of SearchSpace objects defined on Connection - there must be at least one.
        /// </param>
        /// <param name="pPageNumber">
        /// Results page to fetch - defaults to 1
        /// </param>
        /// <param name="pRowsPerPage">
        /// Results to return per page, defaults to 20
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        /// <param name="pClauses">
        /// Zero or more strings can be passed for clauses (filters, sorts, page directives).  Only one query and one sort parameter at a time
        /// are currently supported by CUPI - in other words you can't have "query=(alias startswith ab)" and "query=(FirstName startswith a)" in
        /// the same call.  Also if you have a sort and a query clause they must both reference the same column.
        /// </param>        
        public static WebCallResult GetSearchSpaces(ConnectionServerRest pConnectionServer, out List<SearchSpace> pSearchSpaces, int pPageNumber = 1,
            int pRowsPerPage = 20, params string[] pClauses)
        {
            WebCallResult res = new WebCallResult();
            pSearchSpaces = null;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to GetSearchSpaces";
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

            string strUrl = ConnectionServerRest.AddClausesToUri(pConnectionServer.BaseUrl + "searchspaces", temp.ToArray());

            //issue the command to the CUPI interface
            res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                pSearchSpaces = new List<SearchSpace>();
                res.Success = false;
                return res;
            }

            //not an error, just return empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                pSearchSpaces= new List<SearchSpace>();
                return res;
            }

            pSearchSpaces = pConnectionServer.GetObjectsFromJson<SearchSpace>(res.ResponseText);

            if (pSearchSpaces == null)
            {
                pSearchSpaces = new List<SearchSpace>();
                res.ErrorText = "Could not parse response text into SearchSpaces:" + res.ResponseText;
                res.Success = false;
                return res;
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pSearchSpaces)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        /// Returns a generic list of partitions that are members of the search space
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being updated
        /// </param>
        /// <param name="pSearchSpaceObjectId">
        /// ObjectId of the search space to fetch partitions for
        /// </param>
        /// <param name="pPartitions">
        /// Genreic list of Partition objects associated with the search space - this list can be empty.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        private static WebCallResult GetPartitions(ConnectionServerRest pConnectionServer, string pSearchSpaceObjectId, out List<Partition> pPartitions )
        {
            pPartitions= new List<Partition>();

            string strUrl = pConnectionServer.BaseUrl + string.Format("searchspaces/{0}/searchspacemembers",pSearchSpaceObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false)
            {
                return res;
            }


            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case
            if (string.IsNullOrEmpty(res.ResponseText))
            {
                res.Success = false;
                return res;
            }

            //no error, just return an empty list
            if (res.TotalObjectCount == 0 | res.ResponseText.Length < 25)
            {
                return res;
            }

            List<SearchSpaceMember> oMembers = pConnectionServer.GetObjectsFromJson<SearchSpaceMember>(res.ResponseText);

            if (oMembers == null)
            {
                return res;
            }

            //create an instance of each partition found in the membership list
            foreach (var oMember in oMembers)
            {
                try
                {
                    Partition oPartition = new Partition(pConnectionServer, oMember.PartitionObjectId);
                    pPartitions.Add(oPartition);
                }
                catch (UnityConnectionRestException ex)
                {
                    return ex.WebCallResult;
                }
            }

            //the ConnectionServer property is not filled in in the default class constructor used by the Json parser - 
            //run through here and assign it for all instances.
            foreach (var oObject in pPartitions)
            {
                oObject.HomeServer = pConnectionServer;
            }

            return res;
        }


        /// <summary>
        /// Create a new search space in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pSearchSpace">
        /// Newly created search space object is passed back on this out param
        /// </param>
        /// <param name="pName">
        /// Name of the new search space - must be unique.
        /// </param>
        /// <param name="pDescription">
        /// Optional description of new search space.
        /// </param>
        /// <param name="pLocationObjectId">
        /// Optional location ObjectId to create the search space in - if not provided it will default to the primary location of the Connection server 
        /// its being created in
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddSearchSpace(ConnectionServerRest pConnectionServer,
                                                   out SearchSpace pSearchSpace,
                                                   string pName,
                                                   string pDescription = "",
                                                   string pLocationObjectId = "")
        {
            pSearchSpace = null;
            WebCallResult res = AddSearchSpace(pConnectionServer, pName, pDescription, pLocationObjectId);
            
            if (res.Success)
            {
                //fetc the instance of the partition just created.
                try
                {
                    pSearchSpace = new SearchSpace(pConnectionServer, res.ReturnedObjectId);
                }
                catch (Exception)
                {
                    res.Success = false;
                    res.ErrorText = "Could not find newly created search space by objectId:" + res;
                }
            }

            return res;
        }

        /// <summary>
        /// Create a new search space in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pName">
        /// Name of the new search space - must be unique.
        /// </param>
        /// <param name="pDescription">
        /// Optional description of new search space.
        /// </param>
        /// <param name="pLocationObjectId">
        /// Optional location ObjectId to create the search space in - if not provided it will default to the primary location of the Connection server 
        /// its being created in
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddSearchSpace(ConnectionServerRest pConnectionServer,
                                                    string pName,
                                                    string pDescription = "",
                                                    string pLocationObjectId = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddSearchSpace";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pName))
            {
                res.ErrorText = "Empty value passed for partition name in AddSearchSpace";
                return res;
            }

            string strBody = "<SearchSpace>";

            strBody += string.Format("<{0}>{1}</{0}>", "Name", pName);

            if (!string.IsNullOrEmpty(pDescription))
            {
                strBody += string.Format("<{0}>{1}</{0}>", "Description", pDescription);
            }

            if (!string.IsNullOrEmpty(pLocationObjectId))
            {
                strBody += string.Format("<{0}>{1}</{0}>", "LocationObjectId", pLocationObjectId);
            }

            strBody += "</SearchSpace>";

            res = pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "searchspaces", MethodType.POST, strBody, false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                if (res.ResponseText.Contains(@"/vmrest/searchspaces/"))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/searchspaces/", "").Trim();
                }
            }

            return res;
        }

        /// <summary>
        /// Remove a search space from the Connection directory.  If this search space is being referenced the removal will fail.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pSearchSpaceObjectId">
        /// ObjectId of the search space to delete
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public static WebCallResult DeleteSearchSpace(ConnectionServerRest pConnectionServer, string pSearchSpaceObjectId)
        {
            WebCallResult res;
            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Null ConnectionServer reference passed to DeleteSearchSpace";
                return res;
            }

            if (string.IsNullOrEmpty(pSearchSpaceObjectId))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Empty objectId passed to DeleteSearchSpace";
                return res;
            }

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "searchspaces/" + pSearchSpaceObjectId,
                                            MethodType.DELETE, "");
        }


        /// <summary>
        /// Update a search space - the only items you're allowed to change on a standing search are its name and description so those are 
        /// provided as optional parameters instead of the more generic name/value pair structure followed by some other classes.
        /// Use the methods provided for adding/removing partitions as members.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that object is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// Unique identifier for the search space
        /// </param>
        /// <param name="pName">
        /// Updated name - optional
        /// </param>
        /// <param name="pDescription">
        /// Updated description - optional
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public static WebCallResult UpdateSearchSpace(ConnectionServerRest pConnectionServer, string pObjectId, string pName = "", string pDescription = "")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdateSearchSpace";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty objectId passed to UpdateSearchSpace";
                return res;
            }

            string strBody = "<SearchSpace>";

            //tack on the property value pair with appropriate tags
            strBody += string.Format("<{0}>{1}</{0}>", "ObjectId", pObjectId);

            if (!string.IsNullOrEmpty(pName))
            {
                strBody += string.Format("<{0}>{1}</{0}>", "Name", pName);
            }

            if (!string.IsNullOrEmpty(pDescription))
            {
                strBody += string.Format("<{0}>{1}</{0}>", "Description", pDescription);
            }

            strBody += "</SearchSpace>";

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + "searchspaces/" + pObjectId,
                                            MethodType.PUT,strBody,false);

        }

        /// <summary>
        /// Add a partition as member of a search space - the partition is added at the end of the list in priority order
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pSearchSpaceObjectId">
        /// Search space to be edited
        /// </param>
        /// <param name="pPartitionObjectId">
        /// Partition to be added
        /// </param>
        /// <param name="pSortOrder">
        /// 1 based sort order.  1 is first, 2 is 2nd etc... the numbers you use for partition sequence don't matter and they do not have to 
        /// be contiguous, but they do have to be unique - best to use the member count +1 mechanism to add new partitions to the end.
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddSearchSpaceMember(ConnectionServerRest pConnectionServer,
                                                    string pSearchSpaceObjectId,
                                                    string pPartitionObjectId,
                                                    int pSortOrder)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to AddSearchSpaceMember";
                return res;
            }

            //make sure that something is passed in for the 2 required params - the extension is optional.
            if (String.IsNullOrEmpty(pSearchSpaceObjectId) | string.IsNullOrEmpty(pPartitionObjectId))
            {
                res.ErrorText = "Empty value passed for partition or search space identifiers name in AddSearchSpaceMember";
                return res;
            }

            string strBody = "<SearchSpaceMember>";

            strBody += string.Format("<{0}>{1}</{0}>", "PartitionObjectId", pPartitionObjectId);
            strBody += string.Format("<{0}>{1}</{0}>", "SortOrder", pSortOrder);
            strBody += "</SearchSpaceMember>";

            res = pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl + string.Format("searchspaces/{0}/searchspacemembers", 
                pSearchSpaceObjectId), MethodType.POST,strBody,false);

            //if the call went through then the ObjectId will be returned in the URI form.
            if (res.Success)
            {
                string strPrefix = string.Format(@"/vmrest/searchspaces/{0}/searchspacemembers/", pSearchSpaceObjectId);
                if (res.ResponseText.Contains(strPrefix))
                {
                    res.ReturnedObjectId = res.ResponseText.Replace(strPrefix, "").Trim();
                }
            }

            return res;
        }


        /// <summary>
        /// remove a partition as member of a search space
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pSearchSpaceObjectId">
        /// Search space to be edited
        /// </param>
        /// <param name="pPartitionObjectId">
        /// Partition to be removed
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult DeleteSearchSpaceMember(ConnectionServerRest pConnectionServer,
                                                    string pSearchSpaceObjectId,
                                                    string pPartitionObjectId)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to DeleteSearchSpaceMember";
                return res;
            }

            //make sure that something is passed in for the 2 required params 
            if (String.IsNullOrEmpty(pSearchSpaceObjectId) | string.IsNullOrEmpty(pPartitionObjectId))
            {
                res.ErrorText = "Empty value passed for partition or search space identifiers name in DeleteSearchSpaceMember";
                return res;
            }

            //get the ObjectId of the searchspace member based on the partition objectId passed in
            string strObjectId = GetSearchSpaceMemberObjectIdFromPartitionObjectId(pConnectionServer,pPartitionObjectId,pSearchSpaceObjectId);
            if (string.IsNullOrEmpty(strObjectId))
            {
                res.ErrorText = "Could not find search space member by partition objectId=" + pPartitionObjectId;
                return res;
            }

            return pConnectionServer.GetCupiResponse(pConnectionServer.BaseUrl +
                      string.Format("searchspaces/{0}/searchspacemembers/{1}", pSearchSpaceObjectId, strObjectId),MethodType.DELETE,"");
        }


        /// <summary>
        /// Find the SearchSpaceMemberObjectId from the partitionObjectId value - the partition objectId is unique for the collection associated 
        /// with a search space and this is much easier than having the user have to go fetch the associated objectId when removing.  
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pPartitionObjectId">
        /// Partition to remove from the search space member list
        /// </param>
        /// <param name="pSearchSpaceObjectId">
        /// Search space to edit
        /// </param>
        /// <returns>
        /// SearchSpaceMember ObjectId value if found, empty string if not.
        /// </returns>
        private static string GetSearchSpaceMemberObjectIdFromPartitionObjectId(ConnectionServerRest pConnectionServer, 
                                                                                string pPartitionObjectId, string pSearchSpaceObjectId)
        {

            string strUrl = string.Format("{0}searchspaces/{1}/searchspacemembers/?query=(PartitionObjectId is {2})", 
                pConnectionServer.BaseUrl, pSearchSpaceObjectId, pPartitionObjectId);

            //issue the command to the CUPI interface
            WebCallResult res = pConnectionServer.GetCupiResponse(strUrl, MethodType.GET, "");

            if (res.Success == false || res.TotalObjectCount ==0)
            {
                return "";
            }

            List<SearchSpaceMember> oMembers = pConnectionServer.GetObjectsFromJson<SearchSpaceMember>(res.ResponseText);

            foreach (var oMap in oMembers)
            {
                if (oMap.PartitionObjectId.Equals(pPartitionObjectId, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oMap.ObjectId;
                }
            }

            return "";
        }

       

        #endregion

    }
}
