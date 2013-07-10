To run the integration tests for this project you must:

1. Edit the settings for the ConnectionRestFunctionsTest project - this includes the server name, login name and password for a Connection server to test with.
2. Make sure the user you are authenticating with has administration AND mailbox delegate rights - these are needed.
3. Make sure at least one port is configured to take calls.
4. Make sure there is at least one SMPP provider defined or the SMPP tests will fail.
5. You will need to configure the "ExtensionToDial" property in the setting and then answer that phone and record a message for these tests to pass and, of course, 
   you will need a configured phone system and a registered phone to work with.

NOTE: Be aware that these unit tests are NOT done with mocks or stubs, these will be created/deleting objects.  These are designed to be run on test servers, not production environments.  They do 
clean up after themselves but it's still not a good idea to run these on production servers.  Stick to the Unity tests only if you don't have a test server to use.

NOTE: Be aware that the tests are written against a Connection 10.0 server - earlier versions will have less functionality in the REST APIs and some tests will fail.  This is expected.

