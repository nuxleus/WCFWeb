// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Client
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    // AsyncResult starts acquired; Complete releases.
    internal abstract class AsyncResult : IAsyncResult
    {
        private static AsyncCallback asyncCompletionWrapperCallback;
        private AsyncCallback callback;
        private bool completedSynchronously;
        private bool endCalled;
        private Exception exception;
        private bool isCompleted;
        private AsyncCompletion nextAsyncCompletion;
        private object state;
        private Action beforePrepareAsyncCompletionAction;
        private Func<IAsyncResult, bool> checkSyncValidationFunc;
        private ManualResetEvent manualResetEvent;
        private object thisLock;

#if DEBUG
        private StackTrace endStack;
        private StackTrace completeStack;
        private UncompletedAsyncResultMarker marker;
#endif

        protected AsyncResult(AsyncCallback callback, object state)
        {
            this.callback = callback;
            this.state = state;
            this.thisLock = new object();

#if DEBUG
            this.marker = new UncompletedAsyncResultMarker(this);
#endif
        }

        // can be utilized by subclasses to write core completion code for both the sync and async paths
        // in one location, signalling chainable synchronous completion with the boolean result,
        // and leveraging PrepareAsyncCompletion for conversion to an AsyncCallback.
        // NOTE: requires that "this" is passed in as the state object to the asynchronous sub-call being used with a completion routine.
        protected delegate bool AsyncCompletion(IAsyncResult result);

        public object AsyncState
        {
            get
            {
                return this.state;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (this.manualResetEvent != null)
                {
                    return this.manualResetEvent;
                }

                lock (this.ThisLock)
                {
                    if (this.manualResetEvent == null)
                    {
                        this.manualResetEvent = new ManualResetEvent(this.isCompleted);
                    }
                }

                return this.manualResetEvent;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                return this.completedSynchronously;
            }
        }

        public bool HasCallback
        {
            get
            {
                return this.callback != null;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return this.isCompleted;
            }
        }

        // used in conjunction with PrepareAsyncCompletion to allow for finally blocks
        protected Action<AsyncResult, Exception> OnCompleting { get; set; }

        // subclasses like TraceAsyncResult can use this to wrap the callback functionality in a scope
        protected Action<AsyncCallback, IAsyncResult> VirtualCallback
        {
            get;
            set;
        }

        private object ThisLock
        {
            get
            {
                return this.thisLock;
            }
        }

        protected static void ThrowInvalidAsyncResult(IAsyncResult result)
        {
            throw new InvalidOperationException(String.Format(SR.InvalidAsyncResultImplementation, result.GetType()));
        }

        protected static void ThrowInvalidAsyncResult(string debugText)
        {
            string message = SR.InvalidAsyncResultImplementation;
            if (debugText != null)
            {
#if DEBUG
                message += " " + debugText;
#endif
            }

            throw new InvalidOperationException(message);
        }

        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
            where TAsyncResult : AsyncResult
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            TAsyncResult asyncResult = result as TAsyncResult;

            if (asyncResult == null)
            {
                throw new ArgumentException(SR.InvalidAsyncResultImplementation);
            }

            if (asyncResult.endCalled)
            {
                throw new InvalidOperationException(SR.AsyncResultAlreadyEnded);
            }

#if DEBUG
            if (asyncResult.endStack == null)
            {
                asyncResult.endStack = new StackTrace();
            }
#endif

            asyncResult.endCalled = true;

            if (!asyncResult.isCompleted)
            {
                asyncResult.AsyncWaitHandle.WaitOne();
            }

            if (asyncResult.manualResetEvent != null)
            {
                asyncResult.manualResetEvent.Close();
            }

            if (asyncResult.exception != null)
            {
                throw asyncResult.exception;
            }

            return asyncResult;
        }

        protected void Complete(bool completedSynchronously)
        {
            if (this.isCompleted)
            {
                throw new InvalidOperationException(string.Format(SR.AsyncResultCompletedTwice, GetType()));
            }

#if DEBUG
            this.marker.AsyncResult = null;
            this.marker = null;
            if (this.completeStack == null)
            {
                this.completeStack = new StackTrace();
            }
#endif

            this.completedSynchronously = completedSynchronously;
            if (this.OnCompleting != null)
            {
                // Allow exception replacement, like a catch/throw pattern.
                try
                {
                    this.OnCompleting(this, this.exception);
                }
                catch (Exception exception)
                {
                    if (Utility.IsFatal(exception))
                    {
                        throw;
                    }

                    this.exception = exception;
                }
            }

            if (completedSynchronously)
            {
                // If we completedSynchronously, then there's no chance that the manualResetEvent was created so
                // we don't need to worry about a threading issue
                this.isCompleted = true;
            }
            else
            {
                lock (this.ThisLock)
                {
                    this.isCompleted = true;
                    if (this.manualResetEvent != null)
                    {
                        this.manualResetEvent.Set();
                    }
                }
            }

            if (this.callback != null)
            {
                try
                {
                    if (this.VirtualCallback != null)
                    {
                        this.VirtualCallback(this.callback, this);
                    }
                    else
                    {
                        this.callback(this);
                    }
                }
#pragma warning disable 1634
#pragma warning suppress 56500 // transferring exception to another thread
                catch (Exception e)
                {
                    if (Utility.IsFatal(e))
                    {
                        throw;
                    }

                    throw new InvalidOperationException(); // CallbackException(InternalSR.AsyncCallbackThrewException, e));
                }
#pragma warning restore 1634
            }
        }

        protected void Complete(bool completedSynchronously, Exception exception)
        {
            this.exception = exception;
            this.Complete(completedSynchronously);
        }

        // Note: this should be only derived by the TransactedAsyncResult
        protected virtual bool OnContinueAsyncCompletion(IAsyncResult result)
        {
            return true;
        }

        // Note: this should be used only by the TransactedAsyncResult
        protected void SetBeforePrepareAsyncCompletionAction(Action beforePrepareAsyncCompletionAction)
        {
            this.beforePrepareAsyncCompletionAction = beforePrepareAsyncCompletionAction;
        }

        // Note: this should be used only by the TransactedAsyncResult
        protected void SetCheckSyncValidationFunc(Func<IAsyncResult, bool> checkSyncValidationFunc)
        {
            this.checkSyncValidationFunc = checkSyncValidationFunc;
        }

        protected AsyncCallback PrepareAsyncCompletion(AsyncCompletion callback)
        {
            if (this.beforePrepareAsyncCompletionAction != null)
            {
                this.beforePrepareAsyncCompletionAction();
            }

            this.nextAsyncCompletion = callback;
            
            if (AsyncResult.asyncCompletionWrapperCallback == null)
            {
                AsyncResult.asyncCompletionWrapperCallback = new AsyncCallback(AsyncCompletionWrapperCallback);
            }

            return AsyncResult.asyncCompletionWrapperCallback;
        }

        protected bool CheckSyncContinue(IAsyncResult result)
        {
            AsyncCompletion dummy;
            return this.TryContinueHelper(result, out dummy);
        }

        protected bool SyncContinue(IAsyncResult result)
        {
            AsyncCompletion callback;
            if (this.TryContinueHelper(result, out callback))
            {
                return callback(result);
            }
            else
            {
                return false;
            }
        }

        private static void AsyncCompletionWrapperCallback(IAsyncResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("null");
            }

            if (result.CompletedSynchronously)
            {
                return;
            }

            AsyncResult thisPtr = (AsyncResult)result.AsyncState;
            if (!thisPtr.OnContinueAsyncCompletion(result))
            {
                return;
            }

            AsyncCompletion callback = thisPtr.GetNextCompletion();
            if (callback == null)
            {
                ThrowInvalidAsyncResult(result);
            }

            bool completeSelf = false;
            Exception completionException = null;
            try
            {
                completeSelf = callback(result);
            }
            catch (Exception e)
            {
                if (Utility.IsFatal(e))
                {
                    throw;
                }

                completeSelf = true;
                completionException = e;
            }

            if (completeSelf)
            {
                thisPtr.Complete(false, completionException);
            }
        }

        private bool TryContinueHelper(IAsyncResult result, out AsyncCompletion callback)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            callback = null;
            if (this.checkSyncValidationFunc != null)
            {
                if (!this.checkSyncValidationFunc(result))
                {
                    return false;
                }
            }
            else if (!result.CompletedSynchronously)
            {
                return false;
            }

            callback = this.GetNextCompletion();
            if (callback == null)
            {
                ThrowInvalidAsyncResult("Only call Check/SyncContinue once per async operation (once per PrepareAsyncCompletion).");
            }

            return true;
        }

        private AsyncCompletion GetNextCompletion()
        {
            AsyncCompletion result = this.nextAsyncCompletion;
            this.nextAsyncCompletion = null;
            return result;
        }

#if DEBUG
        private class UncompletedAsyncResultMarker
        {
            public UncompletedAsyncResultMarker(AsyncResult result)
            {
                AsyncResult = result;
            }
            
            public AsyncResult AsyncResult { get; set; }
        }
#endif
    }
}
