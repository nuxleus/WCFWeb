// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    /// <summary>
    /// Specifies if a <see cref="ProcessorArgumentCollection"/> is a collection of 
    /// input or output <see cref="ProcessorArgument">ProcessorArguments</see> for
    /// a <see cref="ProcessorArgument"/>.
    /// </summary>
    public enum ProcessorArgumentDirection
    {
        /// <summary>
        /// The <see cref="ProcessorArgumentCollection"/> is a collection of
        /// input <see cref="ProcessorArgument">ProcessorArguments</see>.
        /// </summary>
        In,

        /// <summary>
        /// The <see cref="ProcessorArgumentCollection"/> is a collection of
        /// output <see cref="ProcessorArgument">ProcessorArguments</see>.
        /// </summary>
        Out
    }
}