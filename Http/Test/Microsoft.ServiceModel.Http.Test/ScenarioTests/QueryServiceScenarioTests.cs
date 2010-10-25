// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Http.Client;
    using System.Threading;
    using System.Web;
    using Microsoft.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryServiceScenarioTests
    {
        static Uri address;
        static QueryServiceHost host = null;
        static Uri address2;
        static QueryServiceHost host2 = null;
        static ManualResetEvent mre = new ManualResetEvent(false);

        public QueryServiceScenarioTests()
        {
            address = new Uri("http://localhost:4567/QueryService");

            if (host == null)
            {
                host = new QueryServiceHost(address, new QueryService());
                host.Open();
            }

            address2 = new Uri("http://localhost:4569/QueryService2");

            if (host2 == null)
            {
                host2 = new QueryServiceHost(address2, new QueryService2());
                host2.Open();
            }
        }

        #region OperationLevel attribute

        [TestMethod]
        public void NestedQueryTest()
        {
            HttpClient client = new HttpClient(address);
            var customers = client.CreateQuery<Customer>("Customers");

            var customersNested = (WebQuery<Customer>)
                customers.Where(customer => customer.Id > customers.Skip(2).First().Id);

            Assert.AreEqual(2, customersNested.ToList().Count());
        }

        [TestMethod]
        public void SynchronousExecution()
        {
            HttpClient client = new HttpClient(address);
            var customers = client.CreateQuery<Customer>("Customers");
            Console.WriteLine("All customers using XML deserialization");
            PrintCustomers(customers);
            Assert.AreEqual(5, customers.ToList().Count());

            var customersJson = client.CreateQuery<Customer>("CustomersJson");
            Console.WriteLine("All customers using JSON deserialization");
            PrintCustomers(customersJson);
            Assert.AreEqual(5, customers.ToList().Count());

            var customersWildcard = client.CreateQuery<Customer>("");
            Console.WriteLine("Customers returned by wildcard method");
            PrintCustomers(customersWildcard);
            Assert.AreEqual(5, customers.ToList().Count());

            Console.WriteLine("All customers whose id > 4");
            var q =
                from customer in customers
                where customer.Id > 4
                select customer;
            Assert.AreEqual(1, q.ToList().Count());
            Assert.AreEqual(1, q.ToList().Count());

            var results = ((WebQuery<Customer>)q).Execute();
            Assert.AreEqual(1, results.ToList().Count());

            var customersNone = client.CreateQuery<Customer>("CustomersNone");
            Assert.AreEqual(1, results.ToList().Count());
        }

        [TestMethod]
        public void SingleElementOperations()
        {
            HttpClient client = new HttpClient(address);
            var customers = client.CreateQuery<Customer>("Customers");

            Assert.AreEqual("LastName1", customers.First().Name);
            Assert.AreEqual("LastName1", customers.Take(1).Single().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void TestMockFailedComposer()
        {
            HttpClient client = new HttpClient(address);
            var customers = client.CreateQuery<Customer>("Failed");
            customers.Execute();
        }

        [TestMethod]
        public void AsynchronousExecution()
        {
            HttpClient client = new HttpClient(address);
            var customers = client.CreateQuery<Customer>("Customers");

            // EndX method
            IAsyncResult result = customers.BeginExecute(null, null);
            Assert.AreEqual(5, customers.EndExecute(result).Count());

            var q =
                from customer in customers
                where customer.Id > 4
                select customer;

            customers = ((WebQuery<Customer>)q);
            // Callback
            customers.BeginExecute(Callback, customers);
            mre.WaitOne();
        }

        #endregion

        #region Service Level attribute

        [TestMethod]
        public void NestedQueryTest2()
        {
            HttpClient client = new HttpClient(address2);
            var customers = client.CreateQuery<Customer>("Customers");

            var customersNested = (WebQuery<Customer>)
                customers.Where(customer => customer.Id > customers.Skip(2).First().Id);

            Assert.AreEqual(2, customersNested.ToList().Count());
        }

        [TestMethod]
        public void SynchronousExecution2()
        {
            HttpClient client = new HttpClient(address2);
            var customers = client.CreateQuery<Customer>("Customers");
            Console.WriteLine("All customers using XML deserialization");
            PrintCustomers(customers);
            Assert.AreEqual(5, customers.ToList().Count());

            var customersJson = client.CreateQuery<Customer>("CustomersJson");
            Console.WriteLine("All customers using JSON deserialization");
            PrintCustomers(customersJson);
            Assert.AreEqual(5, customers.ToList().Count());

            var customersWildcard = client.CreateQuery<Customer>("");
            Console.WriteLine("Customers returned by wildcard method");
            PrintCustomers(customersWildcard);
            Assert.AreEqual(5, customers.ToList().Count());

            Console.WriteLine("All customers whose id > 4");
            var q =
                from customer in customers
                where customer.Id > 4
                select customer;
            Assert.AreEqual(1, q.ToList().Count());
            Assert.AreEqual(1, q.ToList().Count());

            var results = ((WebQuery<Customer>)q).Execute();
            Assert.AreEqual(1, results.ToList().Count());

            var customersNone = client.CreateQuery<Customer>("CustomersNone");
            Assert.AreEqual(1, results.ToList().Count());
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void TestMockFailedComposer2()
        {
            HttpClient client = new HttpClient(address2);
            var customers = client.CreateQuery<Customer>("Failed");
            customers.Execute();
        }

        [TestMethod]
        public void SingleElementOperations2()
        {
            HttpClient client = new HttpClient(address2);
            var customers = client.CreateQuery<Customer>("Customers");

            Assert.AreEqual("LastName1", customers.First().Name);
            Assert.AreEqual("LastName1", customers.Take(1).Single().Name);
        }

        [TestMethod]
        public void AsynchronousExecution2()
        {
            HttpClient client = new HttpClient(address2);
            var customers = client.CreateQuery<Customer>("Customers");

            // EndX method
            IAsyncResult result = customers.BeginExecute(null, null);
            Assert.AreEqual(5, customers.EndExecute(result).Count());

            var q =
                from customer in customers
                where customer.Id > 4
                select customer;

            customers = ((WebQuery<Customer>)q);
            // Callback
            customers.BeginExecute(Callback, customers);
            mre.WaitOne();
        }

        #endregion

        static void PrintCustomers(IEnumerable<Customer> customers)
        {
            foreach (var customer in customers)
            {
                Console.WriteLine(customer);
            }
        }

        static void Callback(IAsyncResult result)
        {
            var q = (WebQuery<Customer>)result.AsyncState;
            Assert.AreEqual(1, q.EndExecute(result).Count());
            mre.Set();
        }
    }

}
