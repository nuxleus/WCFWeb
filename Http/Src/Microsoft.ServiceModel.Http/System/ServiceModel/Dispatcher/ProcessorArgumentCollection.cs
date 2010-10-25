// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// A read-only collection of <see cref="ProcessorArgument">ProcessorArguments</see> that
    /// represents either the input or output arguments of a <see cref="Processor"/>.
    /// </summary>
    public class ProcessorArgumentCollection : ReadOnlyCollection<ProcessorArgument>
    {
        internal ProcessorArgumentCollection(Processor processor, ProcessorArgumentDirection direction, params ProcessorArgument[] arguments) 
            : base(arguments)
        {
            Debug.Assert(processor != null, "The 'processor' parameter should not be null.");
            Debug.Assert(arguments != null, "The 'arguments' parameter should not be null.");

            HashSet<ProcessorArgument> argumentsAlreadyAdded = new HashSet<ProcessorArgument>();

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == null)
                {
                    throw new ArgumentNullException(
                        string.Empty,
                        string.Format(CultureInfo.InvariantCulture, SR.NullValueInArrayParameter, "arguments", i)); 
                }

                if (argumentsAlreadyAdded.Contains(this[i]))
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, SR.ProcessorArgumentCannotBeAddedTwice, this[i].Name));
                }

                this[i].Index = i;
                this[i].ContainingCollection = this;

                // Will trigger name validation in the context of the collection
                this[i].Name = this[i].Name;
                argumentsAlreadyAdded.Add(this[i]);
            }

            this.Direction = direction;
            this.Processor = processor;
        }

        /// <summary>
        /// Gets the <see cref="Processor"/> for which the <see cref="ProcessorArgumentCollection"/>
        /// is either the input or output collection of <see cref="ProcessorArgument">ProcessorArguments</see>.
        /// </summary>
        public Processor Processor { get; private set; }

        /// <summary>
        /// Gets an <see cref="ProcessorArgumentDirection"/> that specifies if the <see cref="ProcessorArgumentCollection"/>
        /// represents a collection of input or output <see cref="ProcessorArgument">ProcessorArguments</see>.
        /// </summary>
        public ProcessorArgumentDirection Direction { get; private set; }

        /// <summary>
        /// Gets the <see cref="ProcessorArgument"/> with the given <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the <see cref="ProcessorArgument"/> to get.</param>
        /// <returns>
        /// The <see cref="ProcessorArgument"/> with the given name if the <see cref="ProcessorArgumentCollection"/> contains
        /// a <see cref="ProcessorArgument"/> with that name and null otherwise.
        /// </returns>
        public ProcessorArgument this[string name]
        {
            get
            {
                return this.FirstOrDefault(p => String.Equals(name, p.Name, StringComparison.Ordinal));
            }
        }
    }
}