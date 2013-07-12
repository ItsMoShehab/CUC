
namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// The ConnectionObjectPropertyPair is a very simple name/value pair construct that gets used for passing lists of property values 
    /// to functions for updating multiple items on various Connection objects.  For instance passing lists of user properties to a user 
    /// update function you create a list of these property pairs using the ConnectionPropertyList class below and pass that as a parameter
    /// to the function.
    /// We could use a prebuilt class such as a dictionary or hash table or the like but I prefer to be explicit with these classes thougout.
    /// </summary>
    public class ConnectionObjectPropertyPair
    {
        public string PropertyName;
        public string PropertyValue;

        /// <summary>
        /// Property pairing for Connection property lists - these are used to construct lists of property/name values that are passed 
        /// into methods for updating users, call handlers, menu entries etc... this allows for dyanamic property lists and easy construction
        /// and debugging routines within the class library.
        /// </summary>
        /// <param name="pPropertyName">
        /// The name of the property (case sensitive) on the Connection object you are wanting to update.
        /// </param>
        /// <param name="pPropertyValue">
        /// The value to assign to the property.
        /// </param>
        public ConnectionObjectPropertyPair(string pPropertyName, string pPropertyValue)
        {
            PropertyName = pPropertyName;
            PropertyValue = pPropertyValue;
        }

        /// <summary>
        /// Displays the property name and value for a ConnectionObjectPropertyPair object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", PropertyName, PropertyValue);
        }
    }
}
