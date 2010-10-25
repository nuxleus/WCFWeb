// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    /// <summary>
    /// Specifies the status of a <see cref="Processor">Processor's</see> execution.
    /// </summary>
    public enum ProcessorStatus
    {
        /// <summary>
        /// The <see cref="Processor"/> executed successfully.
        /// </summary>
        Ok,
 
        /// <summary>
        /// The <see cref="Processor"/> encountered an error during execution.
        /// </summary>
        Error,
    }
}