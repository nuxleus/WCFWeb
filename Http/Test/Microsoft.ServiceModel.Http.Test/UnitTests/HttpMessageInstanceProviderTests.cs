// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpMessageInstanceProviderTests
    {
        [TestMethod]
        [Description("HttpMessageInstanceProvider.CreateInstance returns custom instance")]
        public void CreateInstance_Returns_Custom_Instance()
        {
            MockService1 instance = new MockService1();
            InstanceContext context = new InstanceContext(instance);
            Message message = new HttpRequestMessage().ToMessage();
            IInstanceProvider provider = new MockHttpMessageInstanceProvider()
            {
                OnGetInstance = (ctx, msg) =>
                {
                    return instance;
                }
            };

            object returnedInstance = provider.GetInstance(context, message);
            Assert.AreEqual(instance, returnedInstance, "Instance provider should have returned the same instance we provided to the mock");
            Assert.IsTrue(((MockHttpMessageInstanceProvider)provider).WasGetInstanceCalled, "GetInstance in derived class was not called");
        }

        // TODO: review -- is this the contract we want to allow?
        [TestMethod]
        [Description("HttpMessageInstanceProvider.CreateInstance allows null instance")]
        public void CreateInstance_Allows_Null_Instance()
        {
            MockService1 instance = new MockService1();
            InstanceContext context = new InstanceContext(instance);
            Message message = new HttpRequestMessage().ToMessage();

            IInstanceProvider provider = new MockHttpMessageInstanceProvider()
            {
                OnGetInstance = (ctx, msg) => { return null; }
            };

            object returnedInstance = provider.GetInstance(context, message);
            Assert.IsNull(returnedInstance, "Instance provider should have returned a null instance");
            Assert.IsTrue(((MockHttpMessageInstanceProvider)provider).WasGetInstanceCalled, "GetInstance in derived class was not called");
        }

        [TestMethod]
        [Description("HttpMessageInstanceProvider.CreateInstance with null InstanceContext throws")]
        public void CreateInstance_Null_InstanceContext_Throws()
        {
            MockService1 instance = new MockService1();
            InstanceContext context = null;
            IInstanceProvider provider = new MockHttpMessageInstanceProvider();
            Message message = new HttpRequestMessage().ToMessage();
            ExceptionAssert.ThrowsArgumentNull(
                "Null instance context should throw",
                "instanceContext",
                () =>
                {
                    provider.GetInstance(context, message);
                });
        }

        [TestMethod]
        [Description("HttpMessageInstanceProvider.CreateInstance with null Message throws")]
        public void CreateInstance_Null_Message_Throws()
        {
            MockService1 instance = new MockService1();
            InstanceContext context = new InstanceContext(instance);
            IInstanceProvider provider = new MockHttpMessageInstanceProvider();
            Message message = null;
            ExceptionAssert.ThrowsArgumentNull(
                "Null message should throw",
                "message",
                () =>
                {
                    provider.GetInstance(context, message);
                });
        }

        [TestMethod]
        [Description("HttpMessageInstanceProvider.CreateInstance with non-http message throws")]
        public void CreateInstance_Non_Http_Message_Throws()
        {
            MockService1 instance = new MockService1();
            InstanceContext context = new InstanceContext(instance);
            IInstanceProvider provider = new MockHttpMessageInstanceProvider();
            Message message = Message.CreateMessage(MessageVersion.None, "notUsed");
            ExceptionAssert.ThrowsInvalidOperation(
                "WCF message without inner http message should throw",
                () =>
                {
                    provider.GetInstance(context, message);
                });
        }
    }
}
