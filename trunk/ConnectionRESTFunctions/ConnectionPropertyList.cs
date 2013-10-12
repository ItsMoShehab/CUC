using System;
using System.Collections.Generic;
using System.Linq;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// List of property pairs used for passing multiple property values to object update/create functions.  These property lists are generally 
    /// constructed into HTML body strings in the form of "<PropertyName> PropertyValue </PropertyName>" as a list in the body when, say, updating
    /// values on a user object.
    /// </summary>
    [Serializable]
    public class ConnectionPropertyList : List<ConnectionObjectPropertyPair>
    {
        /// <summary>
        /// base constructor
        /// </summary>
        public ConnectionPropertyList()
        {
        }

        /// <summary>
        /// constructor that takes a property name/value pair
        /// </summary>
        public ConnectionPropertyList(string pPropertyName, string pPropertyValue)
        {
            Add(pPropertyName, pPropertyValue);
        }

        private void AddUniqueName(ConnectionObjectPropertyPair pPropertyPair)
        {
            if (pPropertyPair == null)
            {
                return;
            }

            //if the property already exists, simply update the value to the latest - last write wins here.
            foreach (var oItem in this)
            {
                if (oItem.PropertyName.Equals(pPropertyPair.PropertyName))
                {
                    oItem.PropertyValue = pPropertyPair.PropertyValue;
                    return;
                }
            }

            Add(pPropertyPair);
        }

        /// <summary>
        /// Add function allows a new name value pair to be added.
        /// </summary>
        /// <param name="pPropertyName">unique name of property for object</param>
        /// <param name="pPropertyValue">Value to set property to</param>
        public void Add(string pPropertyName, string pPropertyValue)
        {
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName, pPropertyValue);
            AddUniqueName(oPair);
        }

        /// <summary>
        /// Add function allows a new name value pair to be added.
        /// </summary>
        /// <param name="pPropertyName">unique name of property for object</param>
        /// <param name="pPropertyValue">Value to set property to</param>
        public void Add(string pPropertyName, int pPropertyValue)
        {
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName,pPropertyValue.ToString());
            AddUniqueName(oPair);
        }

        /// <summary>
        /// Add function allows a new name value pair to be added.
        /// </summary>
        /// <param name="pPropertyName">unique name of property for object</param>
        /// <param name="pPropertyValue">Value to set property to</param>
        public void Add(string pPropertyName, bool pPropertyValue)
        {
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName, ConnectionServerRest.BoolToString(pPropertyValue));
            AddUniqueName(oPair);
        }

        /// <summary>
        /// Add function allows a new name value pair to be added.
        /// </summary>
        /// <param name="pPropertyName">unique name of property for object</param>
        /// <param name="pPropertyValue">Value to set property to</param>
        public void Add(string pPropertyName, DateTime pPropertyValue)
        {
            //The Informix time/date format is a little fussy...
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName,
                                                            String.Format("{0:yyyy-MM-dd hh:mm:ss}", pPropertyValue));
            AddUniqueName(oPair);
        }

        /// <summary>
        /// Add function allows a new name value pair to be added.
        /// </summary>
        /// <param name="pPropertyName">unique name of property for object</param>
        /// <param name="pPropertyValue">Value to set property to</param>
        public void Add(string pPropertyName, DateTime? pPropertyValue)
        {
            if (pPropertyValue == null)
            {
                return;
            }
            //The Informix time/date format is a little fussy...
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName,
                                                            String.Format("{0:yyyy-MM-dd hh:mm:ss}",pPropertyValue));
            AddUniqueName(oPair);
        }

        /// <summary>
        /// Returns true if the value exists and matches the value provided, false if not
        /// </summary>
        /// <param name="pPropertyName">Name of property to check for</param>
        /// <param name="pValue">value to check for</param>
        /// <returns>true if the property exists and the value matches</returns>
        public bool ValueExists(string pPropertyName, string pValue)
        {
            return (from oPair in this where oPair.PropertyName.Equals(pPropertyName) select oPair.PropertyValue.Equals(pValue)).FirstOrDefault();
        }

        /// <summary>
        /// Returns true if the value exists and matches the value provided, false if not
        /// </summary>
        /// <param name="pPropertyName">Name of property to check for</param>
        /// <param name="pValue">value to check for</param>
        /// <returns>true if the property exists and the value matches</returns>
        public bool ValueExists(string pPropertyName, int pValue)
        {
            return (from oPair in this where oPair.PropertyName.Equals(pPropertyName) select oPair.PropertyValue.Equals(pValue.ToString())).FirstOrDefault();
        }

        /// <summary>
        /// Returns true if the value exists and matches the value provided, false if not
        /// </summary>
        /// <param name="pPropertyName">Name of property to check for</param>
        /// <param name="pValue">value to check for</param>
        /// <returns>true if the property exists and the value matches</returns>
        public bool ValueExists(string pPropertyName, bool pValue)
        {
            string strValue= ConnectionServerRest.BoolToString(pValue);
            return (from oPair in this where oPair.PropertyName.Equals(pPropertyName) select oPair.PropertyValue.Equals(strValue)).FirstOrDefault();
        }

        /// <summary>
        /// Returns true if the value exists and matches the value provided, false if not
        /// </summary>
        /// <param name="pPropertyName">Name of property to check for</param>
        /// <param name="pValue">value to check for</param>
        /// <returns>true if the property exists and the value matches</returns>
        public bool ValueExists(string pPropertyName, DateTime pValue)
        {
            string strValue= String.Format("{0:yyyy-MM-dd hh:mm:ss}", pValue);
            return (from oPair in this where oPair.PropertyName.Equals(pPropertyName) select oPair.PropertyValue.Equals(strValue)).FirstOrDefault();
        }


        /// <summary>
        /// return a simple list of all the name/value pairs in the list
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strRet = "";

            if (this.Count == 0) return "{empty}";

            foreach (ConnectionObjectPropertyPair oPair in this)
            {
                strRet += oPair + Environment.NewLine;
            }
            return strRet;
        }

        /// <summary>
        /// Simple helper function to convert the list of name/value pairs into strings in the form "name=value" as an 
        /// array for easy inclusion into URI segements and such.
        /// </summary>
        /// <returns></returns>
        public string[] ToArrayOfStrings()
        {
            List<string> oList = new List<string>();
            foreach (var oPair in this)
            {
                oList.Add(string.Format("{0}={1}", oPair.PropertyName, oPair.PropertyValue));
            }

            return oList.ToArray();
        }
    }
}
