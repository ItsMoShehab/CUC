
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
}
