// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// A read-only collection of <see cref="Processor">Processors</see>.
    /// </summary>
    public class ProcessorCollection : ReadOnlyCollection<Processor>
    {
        /// <summary>
        /// Initializes a new <see cref="ProcessorCollection"/> instance.
        /// </summary>
        /// <param name="containerProcessor">The <see cref="Processor"/> that will own the <see cref="ProcessorCollection"/>.</param>
        /// <param name="processor">The <see cref="Processor">Processors</see> to be added to the <see cref="ProcessorCollection"/>.</param>
        public ProcessorCollection(Processor containerProcessor, params Processor[] processor)
            : base(processor)
        {
            if (containerProcessor == null)
            {
                throw new ArgumentNullException("containerProcessor");
            }

            // Null check is done in base ctor
            Debug.Assert(processor != null, "processor array cannot be null");

            HashSet<Processor> alreadyAddedProcessors = new HashSet<Processor>();

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == null)
                {
                    throw new ArgumentNullException(
                        string.Empty,
                        string.Format(CultureInfo.InvariantCulture, SR.NullValueInArrayParameter, "processor", i)); 
                }

                if (this[i] == containerProcessor)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            SR.ContainerProcessorCannotBeInProcessorCollection,
                            containerProcessor.GetType().Name));
                }

                if (alreadyAddedProcessors.Contains(this[i]))
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, SR.ProcessorCannotBeAddedTwice, this[i].GetType().Name));
                }

                this[i].ContainingCollection = this;
                alreadyAddedProcessors.Add(this[i]);
            }

            this.ContainerProcessor = containerProcessor;
        }

        /// <summary>
        /// Gets the <see cref="Processor"/> that owns the <see cref="ProcessorCollection"/>.
        /// </summary>
        public Processor ContainerProcessor { get; private set; }
    }
}