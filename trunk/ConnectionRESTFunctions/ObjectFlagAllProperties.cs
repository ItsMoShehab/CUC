using System.Reflection;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// override that allows all writable properties on an object to be udpated to their current values - this has the effect of 
    /// filling up the changed list for that object with everything currently on this instance of the class - can be useful for 
    /// "resetting" an object back to a previous version.
    /// </summary>
    public static class ObjectFlagAllProperties
    {
        /// <summary>
        /// Set all public, writable properties on an object to be edited without changing their values - this adds each property
        /// with its current value into the changed list for that object so an update can be applied to set an object back to a previous
        /// state using a clone "holder" to keep track.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">object being updated</param>
        public static void FlagAllPropertiesForUpdate<T>(this T source)
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in properties)
            {
                // If not writable then cannot update it; if not readable then cannot check it's value
                if (!p.CanWrite || !p.CanRead) { continue; }

                MethodInfo mget = p.GetGetMethod(false);
                MethodInfo mset = p.GetSetMethod(false);

                // Get and set methods have to be public, private write properties are not to be touched
                if (mget == null) { continue; }
                if (mset == null) { continue; }

                if (p.PropertyType == typeof (string))
                {
                    string strTemp = p.GetValue(source, null) as string;
                    if (string.IsNullOrEmpty(strTemp))
                    {
                        continue;
                    }
                }

                //just add the existing value to the change list but calling the update method
                object oValue = p.GetValue(source, null);
                if (oValue == null)
                {
                    continue;
                }
                p.SetValue(source,oValue,null);
            }
        }
    }
}
