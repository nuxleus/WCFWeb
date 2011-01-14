//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.ServiceModel;
//using System.ServiceModel.Web;
//using Microsoft.ServiceModel.Http;

//namespace Microsoft.Net.Http.Extensions.Test.UnitTests
//{

//    [ServiceContract]
//    public class Service
//    {
//        public Service()
//        {
            
//        }

//        [WebGet(UriTemplate ="")]
//        public string HandleMessage(HttpRequestMessage request, HttpResponseMessage response)
//        {
//            RequestMessage = request;
//            return "Foo";
//        }

//        public HttpRequestMessage RequestMessage { get; set; }
//    }

//    [TestClass]
//    public class HttpClientExtensionsTest
//    {
//        private WebHttpServiceHost Host;
//        private const string HostUri = "http://localhost:8300/Test/";
//        private Service service;

//        [TestInitialize]
//        public void Setup() {
//            Host = new WebHttpServiceHost(typeof(Service), new Uri(HostUri));
//            Host.Open();

//        }

//        [TestCleanup]
//        public void Teardown()
//        {
//            Host.Close();
//        }


//        [TestMethod]
//        public void WhenCreateRequestWithDefaultsIsCalledThenMessageWithDefaultHeadersSetIsReturned()
//        {
//            var client = new HttpClient();
//            var acceptItem = new MediaTypeWithQualityHeaderValue("application/xml");
//            client.DefaultRequestHeaders.Accept.Add(acceptItem);
//            HttpRequestMessage message = client.CreateRequestWithDefaults();
//            Assert.AreEqual(acceptItem, message.Headers.Accept.First());
//        }
 
//        [TestMethod]
//        public void WhenGetIsCalledPassingAMessageThenMethodIsSetToGet()
//        {
//            var client = new HttpClient();
//            var request = new HttpRequestMessage();
//            client.Get(new Uri(HostUri), request);
//            Assert.AreEqual(HttpMethod.Get, this.service.RequestMessage.Method);
//        }

//        [TestMethod]
//        public void WhenGetIsCalledPassingAMessageThenMessageIsSent()
//        {
//        }

//        [TestMethod]
//        public void WhenPostIsCalledPassingAMessageThenMethodIsSetToPost()
//        {
//        }

//        [TestMethod]
//        public void WhenPostIsCalledPassingAMessageThenMessageIsSent()
//        {
//        }

//        [TestMethod]
//        public void WhenDeleteIsCalledPassingAMessageThenMethodIsSetToDelete()
//        {
//        }

//        [TestMethod]
//        public void WhenDeleteIsCalledPassingAMessageThenMessageIsSent()
//        {
//        }

//        [TestMethod]
//        public void WhenPutIsCalledPassingMessageThenMessageIsSent()
//        {
//        }

//        [TestMethod]
//        public void WhenPutIsCalledPassingAMessageThenMessageIsSent()
//        {
//        }


//    }
//}

