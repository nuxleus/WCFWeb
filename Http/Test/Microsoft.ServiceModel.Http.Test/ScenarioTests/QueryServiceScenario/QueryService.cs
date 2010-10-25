//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;

// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Collections.Generic;
    using System.ServiceModel.Web;
    using System.Xml.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Xml.Serialization;
    using System.Linq;
    using Microsoft.ServiceModel.Http.Test.Mocks;

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

    [ServiceContract]
    interface IQueryService
    {
        [WebGet(UriTemplate = "CustomersNone")]
        IEnumerable<Customer> GetCustomersNone();

        [WebGet(UriTemplate = "Customers")]
        IEnumerable<Customer> GetCustomers();

        [WebGet(UriTemplate = "CustomersJson", ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Customer> GetCustomersJson();

        [WebGet(UriTemplate = "*")]
        IEnumerable<Customer> GetAll();

        [WebGet(UriTemplate = "Failed")]
        IEnumerable<Customer> GetAllFailed();
    }


    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class QueryService : IQueryService
    {
        List<Customer> customers;

        public QueryService()
        {
            customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "LastName1" },
                new Customer { Id = 2, Name = "LastName2" },
                new Customer { Id = 3, Name = "LastName3" },
                new Customer { Id = 4, Name = "LastName4" },
                new Customer { Id = 5, Name = "LastName5" }            
            };
        }

        [QueryComposition]
        public IEnumerable<Customer> GetCustomersNone()
        {
            return null;
        }

        [QueryComposition]
        public IEnumerable<Customer> GetCustomers()
        {
            return customers.AsQueryable();
        }

        [QueryComposition]
        public IEnumerable<Customer> GetCustomersJson()
        {
            return customers.AsQueryable();
        }

        [QueryComposition]
        public IEnumerable<Customer> GetAll()
        {
            return customers.AsQueryable();
        }

        [QueryComposition(typeof(MockQueryComposer))]
        public IEnumerable<Customer> GetAllFailed()
        {
            return customers.AsQueryable();
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [QueryComposition]
    class QueryService2 : IQueryService
    {
        List<Customer> customers;

        public QueryService2()
        {
            customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "LastName1" },
                new Customer { Id = 2, Name = "LastName2" },
                new Customer { Id = 3, Name = "LastName3" },
                new Customer { Id = 4, Name = "LastName4" },
                new Customer { Id = 5, Name = "LastName5" }            
            };
        }

        public IEnumerable<Customer> GetCustomersNone()
        {
            return null;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return customers.AsQueryable();
        }

        public IEnumerable<Customer> GetCustomersJson()
        {
            return customers.AsQueryable();
        }

        public IEnumerable<Customer> GetAll()
        {
            return customers.AsQueryable();
        }

        [QueryComposition(typeof(MockQueryComposer))]
        public IEnumerable<Customer> GetAllFailed()
        {
            return customers.AsQueryable();
        }
    }

}
