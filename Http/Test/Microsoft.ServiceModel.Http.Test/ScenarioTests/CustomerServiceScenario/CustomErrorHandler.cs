// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.ScenarioTests
{
    using System;
    using Microsoft.Http;
    using System.ServiceModel.Dispatcher;

    internal class CustomErrorHandler : HttpMessageErrorHandler
    {
        public override bool HandleError(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            return error is HttpResponseMessageException;
        }

        protected override void ProvideResponse(Exception error, HttpResponseMessage response)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            HttpResponseMessageException httpError = error as HttpResponseMessageException;
            if (httpError != null)
            {
                httpError.Response.CopyTo(response);
            }
        }
    }
}
