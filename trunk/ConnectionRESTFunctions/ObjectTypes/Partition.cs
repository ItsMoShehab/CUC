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
    /// The partition class is used only to provide an interface for user to select a partition from those configured on the Connection server.  You
    /// cannot create/edit/delete partitions through the ConnectionCUPIFunctions library. 
    /// </summary>
    public class Partition :IUnityDisplayInterface
    {

        #region Constructors and Destructors


        /// <summary>
        /// Constructor requires ConnectionServer the partition lives on - you can optionally pass the ObjectId or the display name
        /// of the partition to load data for.
        /// </summary>
        public Partition(ConnectionServer pConnectionServer, string pObjectId = "", string pName = "")
        {
            if (pConnectionServer == null)
            {
                throw new ArgumentException("Null ConnectionServer referenced pasted to Partition construtor");
            }

            HomeServer = pConnectionServer;

            if (string.IsNullOrEmpty(pObjectId) & string.IsNullOrEmpty(pName))
            {
                return;
            }

            WebCallResult res = GetPartition(pObjectId, pName);

            if (res.Success == false)
            {
                throw new UnityConnectionRestException(res,string.Format("Unable to find partition by objectId={0}, name={1}. Error={2}", 
                    pObjectId, pName, res));
            }
        }

        /// <summary>
        /// General constructor for Json parsing library
        /// </summary>
        public Partition()
        {
        }

        #endregion


        #region Fields and Properties 

        //used for displaying in grids and drop downs
        public string SelectionDisplayString { get { return Name; } }

        //used for displaying/selecting in grids/dropdowns
        public string UniqueIdentifier { get { return ObjectId; } }

        //reference to the ConnectionServer object used to create this instance.
        public ConnectionServer HomeServer { get; set; }

        #endregion


        #region Partition Properties

        [JsonProperty]
        public string Description { get; private set; }

        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public string ObjectId { get; private set; }

        [JsonProperty]
        public string LocationObjectId { get; private set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text name and objectID of the partition
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", Description, ObjectId);
        }


        /// <summary>
        /// Dumps out all the properties associated with the instance of the call handler object in "name=value" format - each pair is on its
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


        /// <summary>
        /// Pull the data from the Connection server for this object again - if changes have been made external this will 
        /// "refresh" the object
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public WebCallResult RefetchPartitionData()
        {
            return GetPartition(this.ObjectId,"");
        }

        /// <summary>
        /// Fills current instance of class with details of partition for objectId passed in if found.
        /// </summary>
        /// <param name="pObjectId">
        /// Unique Id for partition to load
        /// </param>
        /// <param name="pName">
        /// Optional name of partition to find
        /// </param>
        /// <returns>
        /// Instance of WebCallResult class
        /// </returns>
        private WebCallResult GetPartition(string pObjectId, string pName)
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            string strObjectId = pObjectId;

            if (string.IsNullOrEmpty(pObjectId))
            {
                strObjectId = GetObjectIdFromName(pName);
                if (string.IsNullOrEmpty(strObjectId))
                {
                    res.ErrorText = "Could not find partition by name=" + pName;
                    return res;
                }
            }

            string strUrl = HomeServer.BaseUrl + "partitions/" + strObjectId;

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false)
            {
                return res;
            }

            try
            {
                JsonConvert.PopulateObject(res.ResponseText, this, HTTPFunctions.JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failure populating class instance form JSON response:" + ex;
                res.Success = false;
            }

            
            return res;
        }


        /// <summary>
        /// Fetch the ObjectId of a partition by it's name.  Empty string returned if not match is found.
        /// </summary>
        /// <param name="pName">
        /// Name of the partition to find
        /// </param>
        /// <returns>
        /// ObjectId of partition if found or empty string if not.
        /// </returns>
        private string GetObjectIdFromName(string pName)
        {
            string strUrl = HomeServer.BaseUrl + string.Format("partitions/?query=(Name is {0})", pName);

            //issue the command to the CUPI interface
            WebCallResult res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, HomeServer, "");

            if (res.Success == false || res.TotalObjectCount==0)
            {
                return "";
            }

            List<Partition> oPartitions = HTTPFunctions.GetObjectsFromJson<Partition>(res.ResponseText);

            foreach (var oPartition in oPartitions)
            {
                if (oPartition.Name.Equals(pName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return oPartition.ObjectId;
                }
            }

            return "";
        }

        /// <summary>
        /// Remove a partition from the Connection directory.  If this partition is being referenced the removal will fail.
        /// </summary>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public WebCallResult Delete()
        {
            return DeletePartition(HomeServer, this.ObjectId);
        }


        /// <summary>
        /// Update a partition - the only items you're allowed to change on a standing partition are its name and description so those are 
        /// provided as optional parameters instead of the more generic name/value pair structure followed by some other classes.
        /// </summary>
        /// <param name="pName">
        /// Updated name - optional
        /// </param>
        /// <param name="pDescription">
        /// Updated description - optional
        /// </param>
        /// <returns>
        /// Instance of the WEbCallResult class
        /// </returns>
        public WebCallResult Update(string pName = "", string pDescription ="")
        {
            return UpdatePartition(HomeServer, this.ObjectId, pName, pDescription);
        }


        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the list of all partitions and resturns them as a generic list of Partition objects.  This
        /// list can be used for providing drop down list selection for handler creation purposes or the like.
        /// </summary>
        /// <param name="pConnectionServer">
        /// The Connection server object that the partition should be pulled from
        /// </param>
        /// <param name="pPartitions">
        /// Out parameter that is used to return the list of Partition objects defined on Connection - there must be at least one.
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
        /// Instance of the WebCallResults class containing details of the items sent and recieved from the CUPI interface.
        /// </returns>
        public static WebCallResult GetPartitions(ConnectionServer pConnectionServer, out List<Partition> pPartitions,
            int pPageNumber = 1, int pRowsPerPage = 20, params string[] pClauses)
        {
            WebCallResult res;
            pPartitions = null;

            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.ErrorText = "Null ConnectionServer referenced passed to GetPartitions";
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

            string strUrl = HTTPFunctions.AddClausesToUri(pConnectionServer.BaseUrl + "partitions", temp.ToArray());

            //issue the command to the CUPI interface
            res = HTTPFunctions.GetCupiResponse(strUrl, MethodType.GET, pConnectionServer, "");

            if (res.Success == false)
            {
                return res;
            }

            //if the call was successful the JSON dictionary should always be populated with something, but just in case do a check here.
            //if this is empty that means an error in this case - should always be at least one partition
            if (string.IsNullOrEmpty(res.ResponseText) || res.TotalObjectCount == 0)
            {
                pPartitions = new List<Partition>();
                res.Success = false;
                return res;
            }

            pPartitions = HTTPFunctions.GetObjectsFromJson<Partition>(res.ResponseText);

            //special case - Json.Net always creates an object even when there's no data for it.
            if (pPartitions == null || (pPartitions.Count == 1 && string.IsNullOrEmpty(pPartitions[0].ObjectId)))
            {
                pPartitions = new List<Partition>();
                return res;
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
        /// Create a new Partition in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pPartition">
        /// Newly created partition object is passed back on this out param
        /// </param>
        /// <param name="pName">
        /// Name of the new partition - must be unique.
        /// </param>
        /// <param name="pDescription">
        /// Optional description of new partition.
        /// </param>
        /// <param name="pLocationObjectId">
        /// Optional location ObjectId to create the partition in - if not provided it will default to the primary location of the Connection server 
        /// its being created in
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddPartition(ConnectionServer pConnectionServer,
                                                         out Partition pPartition,
                                                         string pName,
                                                         string pDescription = "",
                                                         string pLocationObjectId = "")
                {
                    pPartition = null;

                    WebCallResult res = AddPartition(pConnectionServer, pName, pDescription, pLocationObjectId);

                    if (res.Success)
                    {
                        //fetch the instance of the partition just created.
                        try
                        {
                            pPartition = new Partition(pConnectionServer, res.ReturnedObjectId);
                        }
                        catch (Exception)
                        {
                            res.Success = false;
                            res.ErrorText = "Could not find newly created partition by objectId:" + res;
                        }
                    }

            return res;
            }

        /// <summary>
        /// Create a new Partition in the Connection directory 
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server being edited
        /// </param>
        /// <param name="pName">
        /// Name of the new partition - must be unique.
        /// </param>
        /// <param name="pDescription">
        /// Optional description of new partition.
        /// </param>
        /// <param name="pLocationObjectId">
        /// Optional location ObjectId to create the partition in - if not provided it will default to the primary location of the Connection server 
        /// its being created in
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class.
        /// </returns>
        public static WebCallResult AddPartition(ConnectionServer pConnectionServer,
                                                    string pName,
                                                    string pDescription ="",
                                                    string pLocationObjectId="")
        {
            
             WebCallResult res = new WebCallResult();
             res.Success = false;

             if (pConnectionServer == null)
             {
                 res.ErrorText = "Null ConnectionServer referenced passed to AddPartition";
                 return res;
             }

             //make sure that something is passed in for the 2 required params - the extension is optional.
             if (String.IsNullOrEmpty(pName))
             {
                 res.ErrorText = "Empty value passed for partition name in AddPartition";
                 return res;
             }

             string strBody = "<Partition>";

             strBody += string.Format("<{0}>{1}</{0}>", "Name", pName);

             if (!string.IsNullOrEmpty(pDescription))
             {
                 strBody += string.Format("<{0}>{1}</{0}>", "Description", pDescription);
             }

             if (!string.IsNullOrEmpty(pLocationObjectId))
             {
                 strBody += string.Format("<{0}>{1}</{0}>", "LocationObjectId", pLocationObjectId);
             }

             strBody += "</Partition>";

             res = HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "partitions", MethodType.POST,pConnectionServer,strBody,false);

             //if the call went through then the ObjectId will be returned in the URI form.
             if (res.Success)
             {
                 if (res.ResponseText.Contains(@"/vmrest/partitions/"))
                 {
                     res.ReturnedObjectId = res.ResponseText.Replace(@"/vmrest/partitions/", "").Trim();
                 }
             }

             return res;
         }


        /// <summary>
        /// Remove a partition from the Connection directory.  If this partition is being referenced the removal will fail.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server the partition is homed on.
        /// </param>
        /// <param name="pPartitionObjectId">
        /// ObjectId of the partition to delete
        /// </param>
        /// <returns>
        /// Instance of the WebCallResult class
        /// </returns>
        public static WebCallResult DeletePartition(ConnectionServer pConnectionServer, string pPartitionObjectId)
        {
            WebCallResult res;
            if (pConnectionServer == null)
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Null ConnectionServer reference passed to DeletePartition";
                return res;
            }

            if (string.IsNullOrEmpty(pPartitionObjectId))
            {
                res = new WebCallResult();
                res.Success = false;
                res.ErrorText = "Empty objectId passed to DeletePartition";
                return res;
            }

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "partitions/" + pPartitionObjectId,
                                            MethodType.DELETE,pConnectionServer, "");
        }


        /// <summary>
        /// Update a partition - the only items you're allowed to change on a standing partition are its name and description so those are 
        /// provided as optional parameters instead of the more generic name/value pair structure followed by some other classes.
        /// </summary>
        /// <param name="pConnectionServer">
        /// Connection server that object is homed on.
        /// </param>
        /// <param name="pObjectId">
        /// Unique identifier for the partition
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
        public static WebCallResult UpdatePartition(ConnectionServer pConnectionServer, string pObjectId, string pName="", string pDescription="")
        {
            WebCallResult res = new WebCallResult();
            res.Success = false;

            if (pConnectionServer == null)
            {
                res.ErrorText = "Null ConnectionServer referenced passed to UpdatePartition";
                return res;
            }

            if (string.IsNullOrEmpty(pObjectId))
            {
                res.ErrorText = "Empty objectId passed to UpdatePartition";
                return res;
            }

            string strBody = "<Partition>";

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

            strBody += "</Partition>";

            return HTTPFunctions.GetCupiResponse(pConnectionServer.BaseUrl + "partitions/" + pObjectId,MethodType.PUT,pConnectionServer,strBody,false);
        }

        #endregion

    }
}
