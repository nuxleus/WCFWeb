// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel.Dispatcher;
    using System.Reflection;

    public class MockOperationInvoker : IOperationInvoker
    {
        MethodInfo methodInfo;

        public MockOperationInvoker(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
        }

        public object[] AllocateInputs()
        {
            return new object[this.methodInfo.GetParameters().Count<ParameterInfo>()];
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = null;
            return this.methodInfo.Invoke(instance, inputs);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();            
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronous
        {
            get { return true; }
        }
    }
}
