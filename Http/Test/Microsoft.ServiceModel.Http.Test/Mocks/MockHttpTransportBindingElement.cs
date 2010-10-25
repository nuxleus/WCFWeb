// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.ServiceModel.Channels;

    public class MockHttpTransportBindingElement : TransportBindingElement
    {
        public bool FoundHttpMessageEncodingBindingElement { get; private set; }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            this.FoundHttpMessageEncodingBindingElement = context.BindingParameters.Find<HttpMessageEncodingBindingElement>() != null;
            return null;
        }

        public override string Scheme
        {
            get { throw new NotImplementedException(); }
        }

        public override BindingElement Clone()
        {
            return null;
        }
    }
}
