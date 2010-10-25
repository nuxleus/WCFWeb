// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides data for the <see cref="Pipeline.BindingArguments"/> and 
    /// <see cref="Pipeline.BoundArguments"/> events.
    /// </summary>
    public class BindArgumentsEventArgs : EventArgs
    {
        internal BindArgumentsEventArgs(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            Debug.Assert(outArgument != null, "The 'outArgument' parameter should not be null.");
            Debug.Assert(inArgument != null, "The 'inArgument' parameter should not be null.");

            this.OutArgument = outArgument;
            this.InArgument = inArgument;
        }

        /// <summary>
        /// Gets the input <see cref="ProcessorArgument"/> that is being or has been bound.
        /// </summary>
        public ProcessorArgument InArgument { get; private set; }

        /// <summary>
        /// Gets the output <see cref="ProcessorArgument"/> that is being or has been bound.
        /// </summary>
        public ProcessorArgument OutArgument { get; private set; }
    }
}