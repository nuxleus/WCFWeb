// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    /// <summary>
    /// An interface that <see cref="Processor">Processors</see> can implement
    /// to indicate to a <see cref="PipelineBuilder"/> that the <see cref="Processor"/>
    /// has certain requirements regarding order relative to other <see cref="Processor">Processors</see>
    /// in the <see cref="Pipeline"/> being built.
    /// </summary>
    public interface IOrderableProcessor
    {
        /// <summary>
        /// Returns the an <see cref="ProcessorExecutionOrder"/> value that indicates
        /// whether the <see cref="Processor"/> should be ordered before or after
        /// the given <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">
        /// The <see cref="Processor"/> for which the relative ordering constraints should determined.
        /// </param>
        /// <returns>
        /// An <see cref="ProcessorExecutionOrder"/> value that indicates
        /// whether the <see cref="Processor"/> should be ordered before or after
        /// the given <paramref name="processor"/>.
        /// </returns>
        ProcessorExecutionOrder GetRelativeExecutionOrder(Processor processor);
    }
}