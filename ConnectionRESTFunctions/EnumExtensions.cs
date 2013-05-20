using System;
using System.ComponentModel;

namespace Cisco.UnityConnection.RestFunctions
{
    /// <summary>
    /// Need to extend the Enum class so we can get a description property - this is necessary to handle the 
    /// OffHours vs Off Hours value in the enum construction.
    /// </summary>
    public static class EnumExtensions
    {
        public static string Description(this Enum value)
        {
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute),false);
            return attributes.Length == 0
                ? value.ToString()
                : ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}
