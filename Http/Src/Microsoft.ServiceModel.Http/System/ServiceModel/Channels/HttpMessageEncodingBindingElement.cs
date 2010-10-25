// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System.Globalization;
    using System.ServiceModel;

    /// <summary>
    /// Provides an <see cref="HttpMessageEncoderFactory"/> that returns a <see cref="MessageEncoder"/> 
    /// that is able to produce and consume <see cref="HttpMessage"/> instances.
    /// </summary>
    internal sealed class HttpMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        private static readonly Type IReplyChannelType = typeof(IReplyChannel);

        public HttpMessageEncodingBindingElement()
        {
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return MessageVersion.None;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (value != MessageVersion.None)
                {
                    throw new NotSupportedException(
                        SR.OnlyMessageVersionNoneSupportedOnHttpMessageEncodingBindingElement);
                }
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            throw new NotSupportedException(
                    SR.ChannelFactoryNotSupportedByHttpMessageEncodingBindingElement);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (!IsChannelShapeSupported<TChannel>())
            {
                throw new NotSupportedException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ChannelShapeNotSupportedByHttpMessageEncodingBindingElement,
                        typeof(TChannel).Name));
            }

            context.BindingParameters.Add(this);

            IChannelListener<IReplyChannel> innerListener = context.BuildInnerChannelListener<IReplyChannel>();

            if (innerListener == null)
            {
                return null;
            }
            
            return (IChannelListener<TChannel>)new HttpMessageEncodingChannelListener(innerListener);
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.BindingParameters.Add(this);

            return IsChannelShapeSupported<TChannel>() && context.CanBuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return false;
        }

        public override BindingElement Clone()
        {
            return new HttpMessageEncodingBindingElement();
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new HttpMessageEncoderFactory();
        }

        private static bool IsChannelShapeSupported<TChannel>()
        {
            return typeof(TChannel) == IReplyChannelType;
        }
    }
}
