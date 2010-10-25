// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    /// <summary>
    /// Indicates the ordering constraints of a given <see cref="Processor"/> relative to 
    /// a second <see cref="Processor"/>.
    /// </summary>
    public enum ProcessorExecutionOrder
    {
        /// <summary>
        /// The given <see cref="Processor"/> can be ordered either before or after a second
        /// <see cref="Processor"/>.
        /// </summary>
        Impartial,

        /// <summary>
        /// The given <see cref="Processor"/> must be ordered before a second
        /// <see cref="Processor"/>.
        /// </summary>
        Before,

        /// <summary>
        /// The given <see cref="Processor"/> must be ordered after a second
        /// <see cref="Processor"/>.
        /// </summary>
        After
    }
}