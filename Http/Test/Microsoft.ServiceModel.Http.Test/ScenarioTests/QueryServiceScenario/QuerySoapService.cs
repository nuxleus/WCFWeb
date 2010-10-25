// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    public class QuerySoapService : IQuerySoapService
    {
        IEnumerable<Customer> customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "LastName1" },
            new Customer { Id = 2, Name = "LastName2" },
            new Customer { Id = 3, Name = "LastName3" },
            new Customer { Id = 4, Name = "LastName4" },
            new Customer { Id = 5, Name = "LastName5" }            
        };

        public IEnumerable<Customer> GetCustomers(string query)
        {
            return customers;
        }
    }

    [ServiceContract]
    interface IQuerySoapService
    {
        [OperationContract]
        [QueryOperationBehavior]
        [QueryComposition]
        IEnumerable<Customer> GetCustomers(string query);       
    }

    public class QueryParameterInspector : IParameterInspector
    {
        void IParameterInspector.AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
        }

        object IParameterInspector.BeforeCall(string operationName, object[] inputs)
        {
            if (operationName == "GetCustomers")
            {
                string s = (string)inputs[0];
                OperationContext.Current.IncomingMessageProperties.Add(QueryCompositionMessageProperty.Name, new QueryCompositionMessageProperty(s));
            }

            return null;
        }
    }

    public class QueryOperationBehavior : Attribute, IOperationBehavior
    {

        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.Add(new QueryParameterInspector());
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
        }
    }
}
