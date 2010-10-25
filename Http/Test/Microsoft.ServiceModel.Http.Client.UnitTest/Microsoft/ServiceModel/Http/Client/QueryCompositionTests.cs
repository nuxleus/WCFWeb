//-----------------------------------------------------------------------
// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.ServiceModel.Http.Client.Test
{
    using System;
    using System.Linq;
    using System.ServiceModel.Http.Client;
    using Microsoft.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class Customer
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public short Age { get; set; }

        public override string ToString()
        {
            return String.Format("Customer id = {0}, name = {1}", Id, Name);
        }
    }

    [TestClass]
    public class QueryCompositionTests
    {
        System.ServiceModel.Http.Client.WebQuery<Customer> customers;

        public QueryCompositionTests()
        {
            var client = new HttpClient("http://localhost/MyService1");
            customers = client.CreateQuery<Customer>("Resource1");
        }

        [TestMethod]
        public void Basic()
        {
            var query =
                from customer in customers
                select customer;

            Assert.AreEqual("http://localhost/MyService1/Resource1", GetUri(query));
        }

        [TestMethod]
        public void Orderby()
        {
            IQueryable<Customer> query;
            query =
                from customer in customers
                orderby customer.Id
                select customer;

            Assert.AreEqual("http://localhost/MyService1/Resource1?$orderby=Id", GetUri(query));

            query =
                from customer in customers
                orderby customer.Id descending
                select customer;

            Assert.AreEqual("http://localhost/MyService1/Resource1?$orderby=Id%20desc", GetUri(query));

            query =
                from customer in customers
                orderby customer.Name, customer.Id
                select customer;

            Assert.AreEqual("http://localhost/MyService1/Resource1?$orderby=Name,Id", GetUri(query));

        }

        [TestMethod]
        public void SkipAndTake()
        {
            IQueryable<Customer> query;
            query = customers.Skip(1);
            Assert.AreEqual("http://localhost/MyService1/Resource1?$skip=1", GetUri(query));

            query = customers.Skip(2).Take(3);
            Assert.AreEqual("http://localhost/MyService1/Resource1?$skip=2&$top=3", GetUri(query));

            int numToSkip = 3;
            query = customers.Skip(numToSkip);
            Assert.AreEqual("http://localhost/MyService1/Resource1?$skip=3", GetUri(query));
        }

        [TestMethod]
        public void Where()
        {
            IQueryable<Customer> query;
            query =
                from c in customers
                where c.Id > 1
                select c;

            Assert.AreEqual("http://localhost/MyService1/Resource1?$filter=Id%20gt%201", GetUri(query));

            query =
                from c in customers
                where c.Name == "Wang"
                select c;

            Assert.AreEqual("http://localhost/MyService1/Resource1?$filter=Name%20eq%20'Wang'", GetUri(query));

            // test evaluation

            int a = 3;
            query =
                from c in customers
                where c.Id + a < 8 - 2
                select c;
            Assert.AreEqual("http://localhost/MyService1/Resource1?$filter=(Id%20add%203)%20lt%206", GetUri(query));

            query =
                from c in customers
                where c.Id < 5 && c.Name.Substring(0, c.Name.Length - 1).Contains("Wang")
                select c;
            Assert.AreEqual("http://localhost/MyService1/Resource1?$filter=(Id%20lt%205)%20and%20substringof('Wang',substring(Name,0,length(Name)%20sub%201))", GetUri(query));

            // test normalization
            query =
                from c in customers
                where Object.Equals("WANG", c.Name.ToUpper())
                select c;
            Assert.AreEqual("http://localhost/MyService1/Resource1?$filter='WANG'%20eq%20toupper(Name)", GetUri(query));

            // test casting
            query =
                from c in customers
                where c.Age > 19
                select c;
            Assert.AreEqual("http://localhost/MyService1/Resource1?$filter=(cast(Age,'System.Int32'))%20gt%2019", GetUri(query));

            // test IsOf
            query =
                from c in customers
                where c.Name is string
                select c;

            Assert.AreEqual("http://localhost/MyService1/Resource1?$filter=isof(Name,%20'System.String')", GetUri(query));

            query =
                from c in customers
                where c.Name == "Wang"
                where c.Id < 2
                select c;

            Assert.AreEqual("http://localhost/MyService1/Resource1?$filter=(Name%20eq%20'Wang')%20and%20(Id%20lt%202)", GetUri(query));
        }

        [TestMethod]
        public void Integrated()
        {
            IQueryable<Customer> query;
            query =
                (from c in customers
                 where "Hello " + c.Name == "Hello Wang"
                 orderby c.Id, c.Name
                 select c).Skip(5).Take(3);

            Assert.AreEqual("http://localhost/MyService1/Resource1?$filter=concat('Hello%20',Name)%20eq%20'Hello%20Wang'&$orderby=Id,Name&$skip=5&$top=3", GetUri(query));
        }

        static string GetUri<T>(IQueryable<T> query)
        {
            return ((WebQuery<T>)query).RequestUri.AbsoluteUri;
        }
    }
}
