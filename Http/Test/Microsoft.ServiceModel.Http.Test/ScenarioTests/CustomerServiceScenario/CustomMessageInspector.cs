// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.ScenarioTests
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.ServiceModel.Dispatcher;
    using System.Text;

    internal class CustomMessageInspector : HttpMessageInspector
    {
        protected override object AfterReceiveRequest(HttpRequestMessage request)
        {
            return request.Headers.Contains("NamesOnly");
        }

        protected override void BeforeSendReply(HttpResponseMessage response, object correlationState)
        {
            bool namesOnly = (bool)correlationState;

            if (namesOnly && response.Content != null)
            {
                StringBuilder builder = new StringBuilder(); 
                string[] contentSplit = response.Content.ReadAsString().Split(new string[]{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string customerText in contentSplit)
                {
                    string[] customerSplitText = customerText.Split('=', ';');
                    builder.Append(customerSplitText[3].Trim() + ", ");
                }

                if (builder.Length > 2)
                {
                    builder.Length -= 2;
                }

                response.Content = new StringContent(builder.ToString());
            }
        }
    }
}
