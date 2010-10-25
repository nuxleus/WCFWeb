// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Class to manage the data values used by the <see cref="Processor"/>s 
    /// during the execution of a <see cref="Pipeline"/>.
    /// </summary>
    /// <remarks>
    /// Any single <see cref="Pipeline"/> instance may be used by multiple
    /// concurrent operations.  The <see cref="PipelineContext"/> manages the
    /// data values for each operation.
    /// </remarks>
    public class PipelineContext
    {
        private object[] inputValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineContext"/> class.
        /// </summary>
        /// <param name="pipeline">The <see cref="Pipeline"/> that will use this <see cref="PipelineContext"/>.</param>
        public PipelineContext(Pipeline pipeline)
        {
            this.Initialize(pipeline, null);
        }

        internal PipelineContext(Pipeline pipeline, PipelineContextInfo contextInfo)
        {
            this.Initialize(pipeline, contextInfo);
        }

        /// <summary>
        /// Gets the currently active <see cref="Processor"/>
        /// </summary>
        /// <value>
        /// This value will be <c>null</c> if no processor is active.
        /// </value>
        public Processor CurrentProcessor
        {
            get
            {
                return (this.CurrentProcessorIndex < 0 || this.CurrentProcessorIndex >= this.Pipeline.Processors.Count) 
                    ? null 
                    : this.Pipeline.Processors[this.CurrentProcessorIndex];
            }
        }

        internal PipelineContextInfo ContextInfo { get; set; }

        private Pipeline Pipeline { get; set; }

        private int CurrentProcessorIndex { get; set; }

        private object[] InputValues
        {
            get
            {
                if (this.inputValues == null)
                {
                    this.inputValues = new object[this.ContextInfo.TotalInputValueCount];
                }

                return this.inputValues;
            }
        }

        /// <summary>
        /// Reads the data value that will be used as input by the specified <paramref name="inArgument"/>.
        /// </summary>
        /// <param name="inArgument">The <see cref="ProcessorArgument"/> whose data is required.</param>
        /// <returns>The data value that will be used by the given <see cref="ProcessorArgument"/> when
        /// its <see cref="Processor"/> executes.  The data value may be <c>null</c>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "We validate the inArgument in the EnsureValidInArgument method.")]
        public object ReadInput(ProcessorArgument inArgument)
        {
            this.EnsureValidInArgument(inArgument);
            object[] value = this.OnReadAllInputs(inArgument.ContainingCollection.Processor);
            return value[inArgument.Index.Value];
        }

        /// <summary>
        /// Reads all the data values for all the input <see cref="ProcessorArgument"/>s for
        /// the given <see cref="Processor"/>.
        /// </summary>
        /// <param name="processor">The <see cref="Processor"/> whose values should be read.</param>
        /// <returns>An array containing those values.  The array will contain one element per
        /// <see cref="ProcessorArgument"/>, in the order they appear in their respective
        /// <see cref="ProcessorArgumentCollection"/>.  The array will not be <c>null</c> but
        /// it can contain <c>null values.</c></returns>
        public object[] ReadAllInputs(Processor processor)
        {
            this.EnsureValidProcessor(processor);
            return this.OnReadAllInputs(processor);
        }

        /// <summary>
        /// Writes the given data value into memory for the specified input <see cref="ProcessorArgument"/>.
        /// </summary>
        /// <remarks>
        /// This value can be retrieved by using <see cref="ReadInput"/>.
        /// </remarks>
        /// <param name="inArgument">The input <see cref="ProcessorArgument"/> whose value is being written.</param>
        /// <param name="value">The value to write.  It may be <c>null</c>.</param>
        public void WriteInput(ProcessorArgument inArgument, object value)
        {
            this.EnsureValidInArgument(inArgument);
            this.OnWriteInput(/*outArgument*/ null, inArgument, value);
        }

        internal void SetProcessorOutputs(Processor processor, object[] output)
        {
            // A null or empty output array is taken to mean "ignore my outputs"
            // for processors that normally produce output.
            if (output == null || output.Length == 0)
            {
                return;
            }

            int outArgCount = processor.OutArguments.Count;
            if (output.Length != outArgCount)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SR.ProcessorReceivedWrongNumberOfValues,
                        processor.GetType().Name,
                        outArgCount,
                        output.Length));
            }

            Debug.Assert(outArgCount == output.Length, "SetProcessorOutput -- array length does not match output value count");

            int processorIndex = this.IndexOfProcessor(processor);
            for (int outArgIndex = 0; outArgIndex < outArgCount; ++outArgIndex)
            {
                ProcessorArgument outArg;
                ProcessorArgument[] inArgs;
                int[] inputValueOffsets = this.ContextInfo.GetOutputValueInfo(processorIndex, outArgIndex, out outArg, out inArgs);

                for (int inArgIndex = 0; inArgIndex < inputValueOffsets.Length; ++inArgIndex)
                {
                    object value = output[outArgIndex];
                    this.OnWriteInput(outArg, inArgs[inArgIndex], value);
                }
            }
        }

        internal bool AdvanceToNextProcessor()
        {
            if (++this.CurrentProcessorIndex >= this.Pipeline.Processors.Count)
            {
                this.CurrentProcessorIndex = this.Pipeline.Processors.Count;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Virtual method called when a value is written for the given input <see cref="ProcessorArgument"/>.
        /// </summary>
        /// <remarks>
        /// The base class implementation writes this value into an internal memory structure.  Subclasses
        /// overriding this method may use alternate storage, alter the value, etc.   Subclasses that wish
        /// to share the base implementation for storage must call the base method to write the value.
        /// </remarks>
        /// <param name="outArgument">The output <see cref="ProcessorArgument"/> whose value is being written.
        /// This argument will be <c>null</c> in situations where that output <see cref="ProcessorArgument"/>
        /// is not available.</param>
        /// <param name="inArgument">The input <see cref="ProcessorArgument"/> to which the value belongs.</param>
        /// <param name="value">The value to write.  It may be <c>null</c>.</param>
        protected virtual void OnWriteInput(ProcessorArgument outArgument, ProcessorArgument inArgument, object value)
        {
            int inputValueOffset = this.InputValueOffset(inArgument);
            this.InputValues[inputValueOffset] = value;
        }

        /// <summary>
        /// Called to obtain the input values for the given <see cref="Processor"/>.
        /// </summary>
        /// <remarks>
        /// This base method obtains the values from an internal pool of values.
        /// Subclasses should call the base method if that behavior is desired,
        /// otherwise they can use custom logic to provide the values.
        /// </remarks>
        /// <param name="processor">The processor whose input values are needed.</param>
        /// <returns>The input values to provide to that processor's <see cref="Processor.Execute"/>
        /// method.
        /// </returns>
        protected virtual object[] OnReadAllInputs(Processor processor)
        {
            int processorIndex = this.IndexOfProcessor(processor);
            int inputValueOffset;
            int inputValueCount = this.ContextInfo.GetInputValueInfo(processorIndex, out inputValueOffset);
            object[] resultArray = new object[inputValueCount];
            Array.Copy(this.InputValues, inputValueOffset, resultArray, 0, resultArray.Length);
            return resultArray; 
        }

        private void Initialize(Pipeline pipeline, PipelineContextInfo contextInfo)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException("pipeline");
            }

            this.Pipeline = pipeline;

            // A null contextInfo indicates we should reuse the one created
            // and retained by the pipeline.  This is the nominal execution
            // path.
            if (contextInfo == null)
            {
                contextInfo = pipeline.ContextInfo;
            }
            else
            {
                Debug.Assert(contextInfo.Pipeline == pipeline, "Cannot use contextInfo from a different pipeline");
            }

            this.ContextInfo = contextInfo;
            this.CurrentProcessorIndex = -1;
        }

        private int InputValueOffset(ProcessorArgument inArgument)
        {
            Debug.Assert(inArgument != null, "inArgument cannot be null");
            Debug.Assert(inArgument.Index.HasValue, "inArgument must have an index value");

            int processorIndex = this.IndexOfProcessor(inArgument.ContainingCollection.Processor);
            int inputValueOffset;
            this.ContextInfo.GetInputValueInfo(processorIndex, out inputValueOffset);
            return inputValueOffset + inArgument.Index.Value;
        }

        private int IndexOfProcessor(Processor processor)
        {
            int index = this.Pipeline.Processors.IndexOf(processor);
            Debug.Assert(index >= 0, "Processor was not in pipeline");
            return index;
        }

        private void EnsureValidProcessor(Processor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }

            if (this.Pipeline.Processors.IndexOf(processor) < 0)
            {
                throw new ArgumentException(SR.ProcessorDoesNotBelongToCurrentPipeline, "processor");
            }
        }

        private void EnsureValidInArgument(ProcessorArgument inArgument)
        {
            if (inArgument == null)
            {
                throw new ArgumentNullException("inArgument");
            }

            if (!inArgument.Index.HasValue ||
                this.Pipeline.Processors.IndexOf(inArgument.ContainingCollection.Processor) < 0)
            {
                throw new ArgumentException(SR.ArgumentDoesNotBelongToProcessorInCurrentPipeline, "inArgument");
            }
        }
    }
}

