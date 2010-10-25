// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System.ServiceModel;

    [ServiceContract]
    public class MockService3
    {
        [OperationContract]
        public string SampleMethod(int x) 
        { 
            return null; 
        }

        [OperationContract]
        public string SampleInOutMethod(int inParameter1, char inParameter2, out double outParameter) 
        { 
            outParameter = 0.0;  
            return null; 
        }
    }
}
