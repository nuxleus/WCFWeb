// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QuerySoapServiceScenarioTests
    {
        [TestMethod]
        public void TestMessageProperty()
        {
            string address = "http://localhost/soapService";

            using (ServiceHost host = new ServiceHost(typeof(QuerySoapService)))
            {
                host.AddServiceEndpoint(typeof(IQuerySoapService), new BasicHttpBinding(), address);
                host.Open();

                ChannelFactory<IQuerySoapService> cf = new ChannelFactory<IQuerySoapService>(new BasicHttpBinding());
                IQuerySoapService proxy = cf.CreateChannel(new EndpointAddress(address));
                List<Customer> result = proxy.GetCustomers(address + "?$top=3").ToList<Customer>();

                Assert.AreEqual(3, result.Count);
                Assert.AreEqual(1, result[0].Id);
                Assert.AreEqual(2, result[1].Id);
                Assert.AreEqual(3, result[2].Id);

                cf.Close();
            }
        }
    }
}
