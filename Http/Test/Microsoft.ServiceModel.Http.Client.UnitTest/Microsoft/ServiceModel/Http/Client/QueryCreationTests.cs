//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.ServiceModel.Http.Client.Test
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.ServiceModel.Http.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryCreationTests
    {
        List<string> baseAddresses;
        List<string> resourceAddresses;
        List<string> baseAddressesIncorrect;

        public QueryCreationTests()
        {
            baseAddresses = new List<string> 
            { 
                "http://localhost/MyService1",
                "http://localhost/MyService1/",
                "http://localhost:1234/MyService1/MyService2",
                "http://www.hello.com:1234/MyService1/MyService2/"
            };

            resourceAddresses = new List<string>
            {
                "",
                "Resource1",
                "/Resource1",
                "/Resource1/",
                "Resource1/Resource2/Resource3/",
                "/Resource1/Resource2/Resource3?x=2&y=3/",
                "Resource with space",
                "Resource%20without%20space"
            };

            baseAddressesIncorrect = new List<string>
            {
                "localhost",
                "http://",
            };
        }

        [TestMethod]
        public void Basic()
        {
            foreach (var baseAddress in baseAddresses)
            {
                HttpClient client = new HttpClient(baseAddress);
                client.Channel = new WebRequestChannel();

                HttpClient client2 = new HttpClient(new Uri(baseAddress));
                client2.Channel = new WebRequestChannel();

                foreach (var relativeAddress in resourceAddresses)
                {
                    WebQuery<Customer> q = client.CreateQuery<Customer>(relativeAddress);
                    WebQuery<Customer> q2 = client2.CreateQuery<Customer>(new Uri(relativeAddress, UriKind.Relative));
                    string s = q.RequestUri.AbsoluteUri;
                    string s2 = q2.RequestUri.AbsoluteUri;

                    Assert.AreEqual(s, s2);                    
                }
            }
        }

        [TestMethod]
        public void EmptyQuery()
        {
            HttpClient client = new HttpClient(baseAddresses[0]);
            WebQuery<Customer> emptyQuery = client.CreateQuery<Customer>();
            Console.WriteLine("Empty query resource at: {0}", emptyQuery.RequestUri.AbsoluteUri);
        }

        [ExpectedException(typeof(UriFormatException))]
        public void InvalidBaseAddressTests1()
        {
            HttpClient client = new HttpClient(baseAddressesIncorrect[0]);
        }

        [ExpectedException(typeof(UriFormatException))]
        public void InvalidBaseAddressTests2()
        {
            HttpClient client = new HttpClient(baseAddressesIncorrect[1]);
        }
    }
}
