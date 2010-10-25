// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    /// <summary>
    /// An interface that <see cref="Processor">Processors</see> can implement
    /// to indicate to a <see cref="PipelineBuilder"/> whether or not the
    /// given processor should be included in the <see cref="Pipeline"/> being
    /// built.
    /// </summary>
    public interface IConditionalExecutionProcessor
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="Processor"/> will
        /// or will not execute if it is included in the <see cref="Pipeline"/>
        /// currently being built by a <see cref="PipelineBuilder"/>.  If the
        /// <see cref="Processor"/> will not execute, the <see cref="PipelineBuilder"/>
        /// will remove the <see cref="Processor"/> from the <see cref="Pipeline"/>
        /// being built.
        /// </summary>
        bool WillExecute { get; }
    }
}