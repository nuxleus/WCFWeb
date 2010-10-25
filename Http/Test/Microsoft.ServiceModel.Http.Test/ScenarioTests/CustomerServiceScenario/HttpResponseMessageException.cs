// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.ScenarioTests
{
    using System;
    using Microsoft.Http;

    public class HttpResponseMessageException : Exception
    {
        public HttpResponseMessageException(HttpResponseMessage response) 
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            this.Response = response;
        }

        public HttpResponseMessage Response { get; private set; }
    }
}
