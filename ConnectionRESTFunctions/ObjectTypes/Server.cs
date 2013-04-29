#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System.Reflection;
using System.Text;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// simple read only class for values in a server instance that's returned as a member of a cluster
    /// </summary>
    public class Server
    {

        #region Server Properties

        public string HostName { get; set; }
        public int DatabaseReplication { get; set; }
        public string Key { get; set; }
        public string Ipv6Name { get; set; }
        public string MacAddress { get; set; }
        public string Description { get; set; }

        #endregion


        #region Instance Methods

        /// <summary>
        /// Returns a string with the text host name and key of the server
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", HostName, Key);
        }

        /// <summary>
        /// Dumps out all the properties associated with the instance of the server object in "name=value" format - each pair is on its
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
