
using System;
using System.Collections.Generic;
using System.Threading;
using Cisco.UnityConnection.RestFunctions;
using SimpleLogger;

namespace CUPIVerySimple
{

    /// <summary>
    /// This Console application is intended to show the bare minimum you need to do to attach to a Connection server via CUPI and do very basic
    /// functions using the ConnectionCUPIFunctions library.  To keep things as absolutely simple as possible it's all done in a single, long Main
    /// function with no logging, very little error handling and no UI to speak of.  
    /// Just update the server connection information found at the start of Main and adjust the user alias/extension/display name informaton to 
    /// taste and you should be good to go.
    /// </summary>
    class Program
    {
        //To keep things simple we'll just do this all in a long method called of main here.
        private static void Main()
        {
            Console.WriteLine("Starting tests");
            RunTests();
            Console.WriteLine("Hit enter to continue");
            Console.ReadLine();
        }


        private static void RunTests()
        {
            //you can attach to multiple different Connection servers an interact with them in the same program easily by just creating
            //new instances of ConnectionServer objects - all objects "know" which server they are associated with.  This example, of course, 
            //just attaches to one server.
            ConnectionServerRest  connectionServer = null;

            Logger.Log("Starting log output");

            //attach to server - insert your Connection server name/IP address and login information here.
            try
            {
                connectionServer = new ConnectionServerRest ("192.168.0.186", "CCMAdministrator", "ecsbulab");
            }

            catch (Exception ex)
            {
                //return an exit code of 1 to indicate failure exit.
                Console.WriteLine("Could not attach to Connection server: " + ex.Message);
                Console.Read();
                Environment.Exit(1);
            }

            //turn on "chatty" output to console
            connectionServer.DebugMode = true;

            //the Connection server object spits out the server name and version number in the ToString function.
            Console.WriteLine("Attached to Connection server: " + connectionServer);

            //do a version check - most things will work on older versions as well but voice name updates and some other functions will not.
            if (connectionServer.Version.IsVersionAtLeast(8, 5, 0, 0) == false)
            {
                Console.WriteLine("WARNING! The ConnectionCUPIFunctions library was written and tested against Connection 8.5 and later."
                                  + "  The version you are attached to is less than that.");
            }

            //the WebCallResult is the structure returned on most calls into the CUPIFunctions library.
            WebCallResult res;

            //fetch user with alias of "jlindborg" - we will be sending the message from his 
            //mailbox.
            UserFull oUserTestDude;

            res = UserBase.GetUser(out oUserTestDude, connectionServer, "", "jlindborg");
            if (res.Success == false)
            {
                Console.WriteLine(res);
                return;
            }

            
            List<UserMessage> oUserMessages;
            res = UserMessage.GetMessages(connectionServer, oUserTestDude.ObjectId, out oUserMessages);

            if (res.Success == false)
            {
                Console.WriteLine(res);
                return;
            }


            ////****
            ////play voice messages using the phone as a media device - aka TRAP 
            ////****
            PhoneRecording oPhone;

            try
            {
                oPhone = new PhoneRecording(connectionServer, "1001");
            }

            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to phone extension:" + ex);
                return;
            }

            List<UserMessage> oTestUserMessages;
            
            res = UserMessage.GetMessages(connectionServer, oUserTestDude.ObjectId, out oTestUserMessages);

            if (res.Success == false)
            {
                Console.WriteLine("Error fetching messages:" + res.ToString());
                Console.ReadLine();
            }

            foreach (var oMessage in oTestUserMessages)
            {
                res = oPhone.PlayMessageFile(oMessage.MsgId,100,100,0,oUserTestDude.ObjectId);

                if (res.Success == false)
                {
                    Console.WriteLine("Error playing stream:" + res.ToString());
                    Console.ReadLine();
                }
            }
            
            //hang up
            oPhone.Dispose();

            //get the schedule details off all the schedules (both holiday and regular) that the user is associated with.  The schedule
            //assignment comes through the user's primary call handler - it references a schedule set which can contain 1 or more schedules.
            //Each schedule has 0 or more details associated with it.
            foreach (var oSchedule in oUserTestDude.PrimaryCallHandler().GetScheduleSet().Schedules())
            {
                Console.WriteLine("Schedle name=" + oSchedule.DisplayName);
                Console.WriteLine("    Details:");

                foreach (var oDetail in oSchedule.ScheduleDetails())
                {
                    Console.WriteLine(oDetail.DumpAllProps("    "));
                }
            }

            //determine if the current schedule state is ACTIVE, INACTIVE or HOLIDAY.  
            Console.WriteLine("Evaluating schedule state");
            Console.WriteLine("   Schedule state right now="+oUserTestDude.PrimaryCallHandler().GetScheduleSet().GetScheduleState(DateTime.Now).ToString());

            TransferOption oTransferAlternateSmith;
            res= oUserTestDude.PrimaryCallHandler().GetTransferOption(TransferOptionTypes.Alternate, out oTransferAlternateSmith);

            //not a lot of reasons this would fail but just in case
            if (res.Success == false)
            {
                Console.WriteLine("Could not find alternate transfer rule for jsmith");
                Console.ReadLine();
                Environment.Exit(1);
            }
            
            //update the transfer number to 12345
            oTransferAlternateSmith.Extension = "12345";
            oTransferAlternateSmith.Update();

            //enable the transfer rule with no end date
            oTransferAlternateSmith.UpdateTransferOptionEnabledStatus( true);

            //****
            //Add a new user
            //****

            //The user template name passed here is the default one created by setup and should be present on any system.  There are functions included
            //in the library that make presenting lists of templates for user selection easy - see the CUPIFastStart project for details.

            UserFull oUser;

            res = UserFull.AddUser(connectionServer, "voicemailusertemplate", "TestUserAlias", "80001", null,out oUser);

            if (res.Success == false)
            {
                //the ToString for the WebCallResult structure dumps its entire contents out which is handy for logging schenarios like this.
                Console.WriteLine("Failed creating new user:" + res.ToString());
                Console.Read();
                Environment.Exit(1);
                return;
            }
            Console.WriteLine("\n\rUser created, new ObjectId=" + oUser.ObjectId);

            //****
            //Edit that user's display name
            //****

            //We could have passed the display name, first name and any other properties we wanted into the AddUser function above if we wanted.
            //This just demonstrates how easy it is to update properties on a standing user object with a few lines of code.  The library sends 
            //only the items that have changed when sending the update.  The full list of properties send in the body is contained in the WebCallResult
            //structure passed back so if there's a problem the "ToString()" call will contain the entire URL, command type, body and everything 
            //returned by the server for easy evaluation of what went wrong.
            oUser.DisplayName = "Test User";
            oUser.FirstName = "Test";
            oUser.LastName = "User";
            res = oUser.Update();

            if (res.Success==false)
            {
                Console.WriteLine("Failed updating user:"+res.ToString());
                Console.Read();
                Environment.Exit(1);
            }

            //You can see quickly in the WebCallResults structure what values were updated for the user, which user and what their new values are.
            Console.WriteLine("\n\rUser Updated: "+res.ToString());

            //update greeting
            Greeting oMyGreeting;
            res=oUser.PrimaryCallHandler().GetGreeting(GreetingTypes.Alternate , out oMyGreeting);

            if (res.Success==false)
            {
                Console.WriteLine("Error fetching alternate greeting:"+res.ToString());
                Console.ReadLine();
                return;
            }

            res=oMyGreeting.SetGreetingWavFile(1033, @"c:\clean.wav",true);

            if (res.Success==false)
            {
                Console.WriteLine("Error applying greeting stream file:"+res.ToString());
                Console.ReadLine();
                return;
            }

            //****
            //Dump the user properties for review
            //****

            //the user "toString" shows the alias, display name and extension for the user.  The DumpAllProps does a complete list of every property 
            //and it's value for the user.  You will find all objects defined in the ConnectionCUPIFunctions library follow this design pattern.
            Console.WriteLine("\n\rUser properties for: {0}\r\n{1}",oUser, oUser.DumpAllProps("     "));

            //****
            //Add an alternate extension to that User
            //****

            //this adds the alternate extension "800012" as the first administrator added alternate extension.
            res = AlternateExtension.AddAlternateExtension(connectionServer, oUser.ObjectId, 1, "800012");

            if (res.Success==false)
            {
                Console.WriteLine("Failed adding alternate extension:"+res.ToString());
                Console.Read();
                Environment.Exit(1);
            }

            //whenever adding a new object using a method that does not return an instance of that object (as we do with the alternate extension above) the ObjectId
            //of the newly created object (if the call suceeds) can be pulled from the WebCallResult structure's ReturnedObjectId property.
            Console.WriteLine("\n\rAlternate Extension added, ObjectId returned= "+res.ReturnedObjectId);

            //you can always turn around and now fetch an object instance of that alternate extension like this - remember that when fetching or editing alternate
            //extensions you have to provide the user's objectId as well.
            AlternateExtension oAltExt;
            res=AlternateExtension.GetAlternateExtension(connectionServer, oUser.ObjectId, res.ReturnedObjectId,out oAltExt);

            if (res.Success==false)
            {
                Console.WriteLine("Failed fetching new alternate extension object: "+res.ToString());
                Environment.Exit(1);
            }

            //now we can update the alternate extension easily - you'll find this same type of pattern on all objects in the library (when they are completed).
            oAltExt.DtmfAccessId = "800013";
            res=oAltExt.Update();

            if (res.Success==false)
            {
                Console.WriteLine("Failed to update the alternate extension: "+res.ToString());
                Environment.Exit(1);
            }

            Console.WriteLine("\n\rAlternate extension updated: "+oAltExt);

            //****
            //List the user's notification devices
            //****
            Console.WriteLine("\n\rNotification devices for: "+oUser.Alias);

            foreach (NotificationDevice oDevice in oUser.NotificationDevices())
            {
                Console.WriteLine(oDevice.ToString());
                Console.WriteLine(oDevice.DumpAllProps("     "));
            }

            //******
            //List the users menu entry keys
            //******
            Console.WriteLine("\n\rMenu entry keys for: " + oUser.Alias);

            foreach (MenuEntry oMenu in oUser.PrimaryCallHandler().GetMenuEntries())
            {
                //use the GetActionDescription method on the ConnectionServer object to produce a bit more readable output for the 
                //actions menu keys are assigned to.  You can still use the oMenu.ToString() here as well to dump out the raw data instead.
                Console.WriteLine("{0}:{1}", oMenu.TouchtoneKey,connectionServer.GetActionDescription(oMenu.Action, oMenu.TargetConversation,
                                                                         oMenu.TargetHandlerObjectId));
            }

            //****
            //List the first 5 system call handlers found in the system.  
            //****
            
            ////Pass the query for any object's "get(object name)s" by passing in a list of strings at the end - the library makes sure these are 
            ////put onto the URL with proper escape codes and "?" and "&" symbols.  It does not check the syntax of the items, however, and remember 
            ////that they ARE case sensitive.
            ////Remember "IsPrimary" is set to 1 when it's a special handler associated with a user.
            List<CallHandler> oCallHandlers;
            res = CallHandler.GetCallHandlers(connectionServer, out oCallHandlers, "query=(IsPrimary is 0)", "rowsPerPage=5", "pageNumber=1");

            if (res.Success == false)
            {
                Console.WriteLine("Failed fetching system call handlers: " + res.ToString());
                Console.Read();
                Environment.Exit(1);
            }

            Console.WriteLine("\n\rUp to the first 5 Call Handlers in the system:");

            foreach (CallHandler oHandler in oCallHandlers)
            {
                Console.WriteLine(oHandler.ToString());
            }

            //****
            //Reset the users PIN
            //****

            //pass the optional flags for forcing change, not expiring etc... if these are not passed the current values for the credential settings
            //will be left alone.  You can pass just these flags and a blank PIN string if you want - the PIN will NOT be set to blank (that is not 
            //allowed in this class library).  If you wish to force a blank password (assuming such a thing is allowed in your configuration which is
            //not advisable) then you will have to do so manually calling the credential update on your own via the RestTransportFunctions library.
            res=oUser.ResetPin("123454321", false, false, false, true);

            if (res.Success==false)
            {
                Console.WriteLine("Failure updating PIN for user: "+res.ToString());
            }

            //****
            //Update the voice name for the user via local wav file
            //****

            //The TestGuyVoiceName.wav file is part of the CUPIVerySimple project and is marked to copy into the binary folder on each run so you do 
            //not need to provide the full path here.
            //I HIGHLY recommend you pass "True" to convert to PCM - it does more than just change the codec, it also forces it into 16 Khz, 8 bit mono which
            //Connection is happy with - even PCM with a different sample rate can cause it to send the dreded "invalid media format" error back.
            res = oUser.SetVoiceName(@"WAVFiles\TestGuyVoiceName.wav",true);

            if (res.Success==false)
            {
                Console.WriteLine("Failure updating voice name for user: "+res.ToString());
            }

            //****
            //Update the users alternate greeting to be active and update it's recording to play US English greeting WAV file we uplaod.
            //****
            Greeting oGreeting;
            res = oUser.PrimaryCallHandler().GetGreeting(GreetingTypes.Alternate, out oGreeting);

            if (res.Success == false)
            {
                Console.WriteLine("Error fetching the alternate greeting for the user: " + res.ToString());
            }

            //update the recording for the alterate greeting and set it to play that custom greeting (instead of the system generated 
            //greeting or blank for the greeting).
            res = oGreeting.SetGreetingWavFile((int)LanguageCodes.EnglishUnitedStates, @"WAVFiles\TestGuyGreeting.wav", true);

            if (res.Success == false)
            {
                Console.WriteLine("Error setting alternate greeting recording: " + res.ToString());
            }

            //By default the greeting is set to play the system generated greeting prompts - to play the custom recorded greeting we just 
            //uploaded you need to set the "PlayWhat" to "1" (which is wrapped in the PlayWhatTypes enum here for readability.
            oGreeting.PlayWhat = PlayWhatTypes.RecordedGreeting;
            res = oGreeting.Update();

            if (res.Success == false)
            {
                Console.WriteLine("Error setting alternate greeting playwhat property: " + res.ToString());
            }

            //use the helper function to enable the alternate greeting (it's disabled by default).  This call sets it to be enabled forever.
            res = oGreeting.UpdateGreetingEnabledStatus(true);

            if (res.Success == false)
            {
                Console.WriteLine("Error alternate greeting enabled property: " + res.ToString());
            }

            //GET the user's mailbox store stats (quotas, size etc...)
            MailboxInfo oMailboxInfo= new MailboxInfo(connectionServer,oUser.ObjectId);
            Console.WriteLine(oMailboxInfo.DumpAllProps());
            
            //get messages for user - get the first 50 messages, sorted by urgent first and restrict the list 
            //to only unread voice messages.
            List<UserMessage> oMessages;
            res = UserMessage.GetMessages(connectionServer, oUser.ObjectId, out oMessages, 1, 50,MessageSortOrder.URGENT_FIRST,
                                        MessageFilter.Type_Voice | MessageFilter.Read_False);
            
            //list subject, sender and time sent for each message returned
            if (res.Success)
            {

                foreach (UserMessage oMessage in oMessages)
                {
                    Console.WriteLine(oMessage.ToString());
                }
            }

            //remove test user
            res=oUser.Delete();

            if (res.Success)
            {
                Console.WriteLine("User deleted...");
            }

            //give you time to review the console output before exiting.
            Console.WriteLine("\n\rPress enter to exit...");
            Console.ReadLine();

            //indicate success exit code
            Environment.Exit(0);
        }


       
    }
}
