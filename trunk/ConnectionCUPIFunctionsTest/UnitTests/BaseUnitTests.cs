using System;
using Cisco.UnityConnection.RestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectionCUPIFunctionsTest.UnitTests
{
    public abstract class BaseUnitTests
    {
        // ReSharper does not handle the Assert. calls in unit test property - turn off checking for unreachable code
        // ReSharper disable HeuristicUnreachableCode

        #region Fields and Properties

        //Mock transport interface - 
        public static Mock<IConnectionRestCalls> _mockTransport { get; set; }

        //Mock REST server
        public static ConnectionServerRest _mockServer { get; set; }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion


        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            //setup mock server interface 
            _mockTransport = new Mock<IConnectionRestCalls>();

            _mockTransport.Setup(x => x.GetCupiResponse(It.IsAny<string>(), It.IsAny<MethodType>(), It.IsAny<ConnectionServerRest>(),
                                                        It.IsAny<string>(), true)).Returns(new WebCallResult
                                                            {
                                                                Success = true,
                                                                ResponseText = "{\"name\":\"vmrest\",\"version\":\"10.0.0.189\"}"
                                                            });

            try
            {
                _mockServer = new ConnectionServerRest(_mockTransport.Object, "test", "test", "test", false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed creating mock server instance:" + ex);
            }

            _mockServer.RaiseErrorEvent(It.IsAny<string>());

            _mockServer.ErrorEvents += MockServerOnErrorEvents;
        }

        private static void MockServerOnErrorEvents(object sender, ConnectionServerRest.LogEventArgs logEventArgs)
        {
            Console.WriteLine("Error event on mock server:"+logEventArgs.Line);
        }

        #endregion
    }
}
