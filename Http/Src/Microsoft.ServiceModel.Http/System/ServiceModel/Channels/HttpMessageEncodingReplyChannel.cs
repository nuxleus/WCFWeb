// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;

    internal class HttpMessageEncodingReplyChannel : ChannelBase, IReplyChannel
    {
        private IReplyChannel innerChannel;
        private EventHandler onInnerChannelFaulted;

        public HttpMessageEncodingReplyChannel(ChannelManagerBase channelManager, IReplyChannel innerChannel)
            : base(channelManager)
        {
            Debug.Assert(channelManager != null, "The 'channelManager' parameter should not be null.");
            Debug.Assert(innerChannel != null, "The 'innerChannel' parameter should not be null.");

            this.innerChannel = innerChannel;
            this.onInnerChannelFaulted = new EventHandler(this.OnInnerChannelFaulted);
            this.innerChannel.Faulted += this.onInnerChannelFaulted;
        }

        public EndpointAddress LocalAddress
        {
            get
            {
                return this.innerChannel.LocalAddress;
            }
        }

        public override T GetProperty<T>()
        {
            T property = base.GetProperty<T>();
            if (property != null)
            {
                return property;
            }

            return this.innerChannel.GetProperty<T>();
        }

        public IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannel.BeginReceiveRequest(timeout, callback, state);
        }

        public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
        {
            return this.innerChannel.BeginReceiveRequest(callback, state);
        }

        public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannel.BeginTryReceiveRequest(timeout, callback, state);
        }

        public IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannel.BeginWaitForRequest(timeout, callback, state);
        }

        public RequestContext EndReceiveRequest(IAsyncResult result)
        {
            return WrapRequestContext(this.innerChannel.EndReceiveRequest(result));
        }

        public bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context)
        {
            bool wasSuccessful = this.innerChannel.EndTryReceiveRequest(result, out context);
            context = wasSuccessful ? WrapRequestContext(context) : null;
            return wasSuccessful;
        }

        public bool EndWaitForRequest(IAsyncResult result)
        {
            return this.innerChannel.EndWaitForRequest(result);
        }

        public RequestContext ReceiveRequest(TimeSpan timeout)
        {
            return WrapRequestContext(this.innerChannel.ReceiveRequest(timeout));
        }

        public RequestContext ReceiveRequest()
        {
            return WrapRequestContext(this.innerChannel.ReceiveRequest());
        }

        public bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
        {
            bool wasSuccessful = this.innerChannel.TryReceiveRequest(timeout, out context);
            context = wasSuccessful ? WrapRequestContext(context) : null;
            return wasSuccessful;
        }

        public bool WaitForRequest(TimeSpan timeout)
        {
            return this.innerChannel.WaitForRequest(timeout);
        }

        protected override void OnAbort()
        {
            this.innerChannel.Abort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannel.BeginClose(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannel.BeginOpen(timeout, callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            this.innerChannel.Close(timeout);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.innerChannel.EndClose(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.innerChannel.EndOpen(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.innerChannel.Open(timeout);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Caller owns the RequestContext and disposal of the RequestContext.")]
        private static RequestContext WrapRequestContext(RequestContext innerContext)
        {
            return (innerContext != null) ?
                new HttpMessageEncodingRequestContext(innerContext) :
                (RequestContext)null;
        }

        private void OnInnerChannelFaulted(object sender, EventArgs e)
        {
            this.Fault();
        }
    }
}
