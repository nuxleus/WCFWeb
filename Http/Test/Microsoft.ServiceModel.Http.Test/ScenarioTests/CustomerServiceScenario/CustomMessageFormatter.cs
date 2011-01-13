// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.ScenarioTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    internal class CustomMessageFormatter : HttpMessageFormatter
    {
        private bool hasRequestMessage;
        private bool hasResponseMessage;

        public CustomMessageFormatter(HttpOperationDescription httpOperation)
        {
            if (httpOperation == null)
            {
                throw new ArgumentNullException("httpOperation");
            }

            if (httpOperation.InputParameters.Count == 1 &&
                 httpOperation.InputParameters[0].ParameterType == typeof(HttpRequestMessage))
            {
                this.hasRequestMessage = true;
            }
            else if (httpOperation.InputParameters.Count != 0)
            {
                throw new NotSupportedException(
                    "The MessageFormatter only supports a single optional input parameter of type 'HttpRequestMessage'.");
            }

            if (httpOperation.OutputParameters.Count > 0)
            {
                throw new NotSupportedException(
                    "The MessageFormatter only does not support outputt parameters.");
            }

            if (httpOperation.ReturnValue.ParameterType == typeof(HttpResponseMessage))
            {
                this.hasResponseMessage = true;
            }
            else if (httpOperation.ReturnValue.ParameterType != typeof(void))
            {
                throw new NotSupportedException(
                    "The MessageFormatter only supports an optional return type of 'HttpResponseMessage'.");
            }
        }

        protected override void DeserializeRequest(HttpRequestMessage message, object[] parameters)
        {
            if (this.hasRequestMessage)
            {
                parameters[0] = message;
            }
        }

        protected override void SerializeReply(object[] parameters, object result, HttpResponseMessage response)
        {
            if (this.hasResponseMessage)
            {
                HttpResponseMessage responseFromOperation = result as HttpResponseMessage;
                if (responseFromOperation == null)
                {
                    throw new InvalidOperationException("The result should have been an HttpResponseMessage instance.");
                }

                responseFromOperation.CopyTo(response);
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new ByteArrayContent(new byte[0]);
            }
        }
    }
}
