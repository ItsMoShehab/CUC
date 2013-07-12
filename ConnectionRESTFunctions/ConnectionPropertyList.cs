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
    public class ConnectionPropertyList : List<ConnectionObjectPropertyPair>
    {
        //base constructor
        public ConnectionPropertyList()
        {
        }

        //constructor that takes a property name/value pair
        public ConnectionPropertyList(string pPropertyName, string pPropertyValue)
        {
            Add(pPropertyName, pPropertyValue);
        }

        //Add function allows a new name value pair to be added.
        public void Add(string pPropertyName, string pPropertyValue)
        {
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName, pPropertyValue);
            Add(oPair);
        }

        //for adding an integer value
        public void Add(string pPropertyName, int pPropertyValue)
        {
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName,pPropertyValue.ToString());
            Add(oPair);
        }

        //for adding a boolean value - CUPI needs 0/1 passed instead of "true" or "false" here
        public void Add(string pPropertyName, bool pPropertyValue)
        {
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName, ConnectionServerRest.BoolToString(pPropertyValue));
            Add(oPair);
        }

        //for adding a date - Informix needs special formatting
        public void Add(string pPropertyName, DateTime pPropertyValue)
        {
            //The Informix time/date format is a little fussy...
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName,
                                                            String.Format("{0:yyyy-MM-dd hh:mm:ss}", pPropertyValue));
            Add(oPair);
        }

        //adding a nullable date - don't add if it's null
        public void Add(string pPropertyName, DateTime? pPropertyValue)
        {
            if (pPropertyValue == null)
            {
                return;
            }
            //The Informix time/date format is a little fussy...
            ConnectionObjectPropertyPair oPair = new ConnectionObjectPropertyPair(pPropertyName,
                                                            String.Format("{0:yyyy-MM-dd hh:mm:ss}",pPropertyValue));
            Add(oPair);
        }

        //Returns true if the value exists and matches the value provided, false if not
        public bool ValueExists(string pPropertyName, string pValue)
        {
            return (from oPair in this where oPair.PropertyName.Equals(pPropertyName) select oPair.PropertyValue.Equals(pValue)).FirstOrDefault();
        }

        //Returns true if the value exists and matches the value provided, false if not
        public bool ValueExists(string pPropertyName, int pValue)
        {
            return (from oPair in this where oPair.PropertyName.Equals(pPropertyName) select oPair.PropertyValue.Equals(pValue.ToString())).FirstOrDefault();
        }

        //Returns true if the value exists and matches the value provided, false if not
        public bool ValueExists(string pPropertyName, bool pValue)
        {
            string strValue= ConnectionServerRest.BoolToString(pValue);
            return (from oPair in this where oPair.PropertyName.Equals(pPropertyName) select oPair.PropertyValue.Equals(strValue)).FirstOrDefault();
        }

        //Returns true if the value exists and matches the value provided, false if not
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
