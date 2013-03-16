#region Legal Disclaimer

//This code and samples are provided “as-is”, responsibility for testing and supporting it lies with you.  In lawyer-ese:

//THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE PROGRAM “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. 
//SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

#endregion

using System;
using ConnectionCUPIFunctions;
using SimpleLogger;

namespace PWReset
{
    /// <summary>
    /// Simple mechanism to approximate global variables in C# - the Connection Server object instance (and anything else deemed worth of accessing
    /// globally) can be referenced here from anywhere in the solution.
    /// </summary>
    public static class GlobalItems
    {
        /// <summary>
        /// Public instance of a ConnectionServer object - for this demo we will be supporting a conneciton to a single server only but the ConnectionRESTFunctions
        /// class is designed to allow you to have mutliple server instances in an application if this is what you want.  The instance is held here for easy 
        /// access from different parts of the application without having to pass an instance object around in method calls or the like.
        /// </summary>
        public static ConnectionServer CurrentConnectionServer;

        /// <summary>
        /// Simple helper function to call when exiting the application - generally used to make sure open resoruces like log files, databases and such are 
        /// properly closed out before leaving.
        /// For this sample application there isn't much to do when exiting, however the logger can be closed before exiting if it's open at any rate.
        /// </summary>
        public static void FinalExit()
        {
            Logger.StopLogging();
            Environment.Exit(0);  //issue an code of 0 - if the app is called via the CLI this is what's returned.
        }
    }
}
