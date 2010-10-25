// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.ServiceModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This service exposes different operation signatures that probe different 
    /// code paths and combinations of inputs, outputs, and returns
    /// </summary>
    [ServiceContract]
    public class MockService2
    {
        [OperationContract]
        public string GetAStringFromInt(int parameter1) 
        { 
            return parameter1.ToString(); 
        }

        [OperationContract]
        [Description("This is MethodWithAttribute")]
        public string MethodWithAttribute(int parameter1) 
        { 
            return parameter1.ToString(); 
        }

        [OperationContract]
        public void NoParametersReturnsVoid() 
        { 
        }

        [OperationContract]
        public double NoParametersReturnsDouble() 
        { 
            return 5.0; 
        }

        [OperationContract]
        public string GetAStringFromMultiple(int parameter1, double parameter2, string parameter3)
        { 
            return parameter1.ToString() + parameter2.ToString() + parameter3; 
        }

        [OperationContract]
        public void OneInOneOutReturnsVoid(int parameter1, out double parameter2) 
        { 
            parameter2 = parameter1; 
        }

        [OperationContract]
        public string OneInOneOutReturnsString(char parameter1, out double parameter2) 
        { 
            parameter2 = 0.0; 
            return "hello"; 
        }

        [OperationContract]
        public string OneInMultipleOutReturnsString(int parameter1, out double parameter2, out char parameter3) 
        { 
            parameter2 = parameter1; 
            parameter3 = 'c';  
            return "hello"; 
        }

        [OperationContract]
        public IAsyncResult BeginAsyncMethod(AsyncCallback callback, object state) 
        { 
            return null; 
        }
    }
}
