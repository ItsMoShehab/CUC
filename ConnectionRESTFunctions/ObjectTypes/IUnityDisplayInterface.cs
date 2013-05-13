
using System.Collections.Generic;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Gurantees that the object type in question has a unique identifier and a display name which most do.  Allows for easy
    /// handling in lists, drop downs etc... 
    /// </summary>
    public interface IUnityDisplayInterface
    {
        string SelectionDisplayString { get; }
        string UniqueIdentifier { get; }
    }

    /// <summary>
    /// Comparison class used for sorting lists of objects that impelement the IunityDisplayInterface
    /// </summary>
    public class UnityDisplayObjectCompare : IComparer<IUnityDisplayInterface>
    {
        /// <summary>
        /// Compares two object that implement the IUnityDisplayInterfce based on their display string 
        /// </summary>
        /// <returns>
        /// 0 if items are equal (two null values are considered equal), -1 if item 1 is less than item 2 and 1 if item 1
        /// is greater than item 2
        /// </returns>
        public int Compare(IUnityDisplayInterface pObject1, IUnityDisplayInterface pObject2)
        {
            return pObject1.SelectionDisplayString.CompareTo(pObject2.SelectionDisplayString);
        }
    }
}
