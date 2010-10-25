// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;

    internal class HttpMessageEncodingChannelListener : ChannelListenerBase<IReplyChannel>
    {
        private IChannelListener<IReplyChannel> innerListener;

        public HttpMessageEncodingChannelListener(IChannelListener<IReplyChannel> innerListener)
        {
            Debug.Assert(innerListener != null, "The 'innerListener' parameter should not be null.");
            this.innerListener = innerListener;
        }

        public override Uri Uri
        {
            get
            {
                return this.innerListener.Uri;
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.innerListener.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerListener.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.innerListener.EndOpen(result);
        }

        protected override IReplyChannel OnAcceptChannel(TimeSpan timeout)
        {
            IReplyChannel innerChannel = this.innerListener.AcceptChannel(timeout);
            return this.WrapInnerReplyChannel(innerChannel);
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerListener.BeginAcceptChannel(timeout, callback, state);
        }

        protected override IReplyChannel OnEndAcceptChannel(IAsyncResult result)
        {
            IReplyChannel innerChannel = this.innerListener.EndAcceptChannel(result);
            return this.WrapInnerReplyChannel(innerChannel);
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return this.innerListener.WaitForChannel(timeout);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerListener.BeginWaitForChannel(timeout, callback, state);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return this.innerListener.EndWaitForChannel(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            this.innerListener.Close(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerListener.BeginClose(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.innerListener.EndClose(result);
        }

        protected override void OnAbort()
        {
            this.innerListener.Abort();
        }

        private IReplyChannel WrapInnerReplyChannel(IReplyChannel innerChannel)
        {
            return (innerChannel != null) ?
                new HttpMessageEncodingReplyChannel(this, innerChannel) :
                (IReplyChannel)null;
        }
    }
}
