// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The result returned from executing a <see cref="Processor"/>.
    /// </summary>
    public class ProcessorResult
    {
        /// <summary>
        /// Gets or sets a value that indicates the status of the <see cref="Processor"/> 
        /// execution that produced the <see cref="ProcessorResult"/>.
        /// </summary>
        public ProcessorStatus Status { get; set; }

        /// <summary>
        /// Gets or sets an exception that is associated with the <see cref="Processor"/>
        /// execution that produced the <see cref="ProcessorResult"/>.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets or sets the output values from the <see cref="Processor"/>
        /// execution that produced the <see cref="ProcessorResult"/>. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Using object array is to be consistent with the Operation Invoker")]
        public object[] Output { get; set; }
    }

    /// <summary>
    /// The result returned from executing a <see cref="Processor"/> that provides
    /// a single declared output.
    /// </summary>
    /// <typeparam name="TOutput">type of the output</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
            Justification = "We allow generic classes and non-generic classes live in the same file")]
    public class ProcessorResult<TOutput> : ProcessorResult
    {
        private ArgumentValueConverter<TOutput> converter = ArgumentValueConverter.CreateValueConverter<TOutput>();

        /// <summary>
        /// Gets or sets the output value from the <see cref="Processor"/>
        /// execution that produced the <see cref="ProcessorResult"/>. 
        /// </summary>
        public new TOutput Output
        {
            get
            {
                object[] baseValue = base.Output;
                return (baseValue == null || baseValue.Length < 1) 
                    ? default(TOutput) 
                    : this.converter.ConvertFrom(baseValue[0]);
            }

            set
            {
                base.Output = new object[] { (object)value };
            }
        }
    }
}