To run the unit tests for this project you must:

1. Edit the settings for the ConnectionCUPIFunctionsTest project - this includes the server name, login name and password for a Connection server to test with.
2. Make sure the user you are authenticating with has administration AND mailbox delegate rights - these are needed.
3. Make sure at least one port is configured to take calls.
4. Make sure at least one interview hander is created (by default there is none).
5. Make sure there is at least one SMPP provider defined or the SMPP tests will fail.
6. By default the PhoneRecording tests are not run - to turn them on go to the PhoneRecordingTest.cs file and uncomment the "TestMethod()" line above the test and it will
   be included in the automated run.  You will need to configure the "ExtensionToDial" property in the setting and then answer that phone and record a message for these 
   tests to pass and, of course, you will need a configured phone system and a registered phone to work with.

NOTE: Be aware that these unit tests are NOT done with mocks or stubs, these will be created/deleting objects.  These are designed to be run on test servers, not production environments.

NOTE: Be aware that the tests are written against a Connection 10.0 server - earlier versions will have less functionality in the REST APIs and some tests will fail.  This is expected.

