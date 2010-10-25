// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    internal class PipelineContextInfo
    {
        public PipelineContextInfo(Pipeline pipeline)
        {
            Debug.Assert(pipeline != null, "pipeline cannot be null");
            this.Pipeline = pipeline;
            this.Initialize();
        }

        public Pipeline Pipeline { get; private set; }

        /// <summary>
        /// Gets the total number of input values in the pipeline.
        /// </summary>
        public int TotalInputValueCount { get; private set; }

        private ProcessorValueInfo[] ProcessorValueInfos { get; set; }

        /// <summary>
        /// Retrieves information about the input values for the given processor.
        /// </summary>
        /// <param name="processorIndex">The 0-based index of the processor in this context.</param>
        /// <param name="inputValueOffset">The out parameter to receive the offset in the pool of input values
        /// where the input values for this processor start.  They are contiguous.</param>
        /// <returns>The number of input values for this processor.</returns>
        public int GetInputValueInfo(int processorIndex, out int inputValueOffset)
        {
            Debug.Assert(processorIndex >= 0 && processorIndex < this.ProcessorValueInfos.Length, "processorIndex out of bounds");

            ProcessorValueInfo info = this.ProcessorValueInfos[processorIndex];
            inputValueOffset = info.InputValueOffset;
            return info.InputValueCount;
        }

        /// <summary>
        /// Retrieves information about the output values for the given processor.
        /// </summary>
        /// <param name="processorIndex">The 0-based index of the processor in this context.</param>
        /// <param name="outArgumentIndex">The 0-based index of the output argument for this processor.</param>
        /// <param name="outArgument">Out parameter to receive the <see cref="ProcessorArgument"/> for this output argument.</param>
        /// <param name="inArguments">Out parameter to receive the collection of input value <see cref="ProcessorArgument"/>s.</param>
        /// <returns>An array of offsets in the input value pool for each of the input values.</returns>
        public int[] GetOutputValueInfo(int processorIndex, int outArgumentIndex, out ProcessorArgument outArgument, out ProcessorArgument[] inArguments)
        {
            Debug.Assert(processorIndex >= 0 && processorIndex < this.ProcessorValueInfos.Length, "processorIndex out of bounds");

            OutputValueInfo info = this.ProcessorValueInfos[processorIndex].OutputValueInfos[outArgumentIndex];
            outArgument = info.OutArgument;
            inArguments = info.InArguments;
            return info.InputValueIndices;
        }

        private void Initialize()
        {
            ReadOnlyCollection<Processor> processors = this.Pipeline.Processors;
            int numberOfProcessors = processors.Count;
            this.ProcessorValueInfos = new ProcessorValueInfo[numberOfProcessors];
            this.TotalInputValueCount = 0;

            // First pass -- compute all the input value information for each processor
            for (int processorIndex = 0; processorIndex < processors.Count; ++processorIndex)
            {
                Processor processor = processors[processorIndex];
                int numberOfInputs = processor.InArguments.Count;
                
                ProcessorValueInfo valueInfo = new ProcessorValueInfo()
                {
                    InputValueCount = numberOfInputs,
                    InputValueOffset = this.TotalInputValueCount
                };

                this.ProcessorValueInfos[processorIndex] = valueInfo;
                this.TotalInputValueCount += numberOfInputs;
            }

            // Second pass -- compute all the output value information (uses input value info above)
            for (int processorIndex = 0; processorIndex < processors.Count; ++processorIndex)
            {
                Processor processor = processors[processorIndex];
                this.ProcessorValueInfos[processorIndex].OutputValueInfos = this.CreateOutputValueInfos(processor);
            }
        }

        private OutputValueInfo[] CreateOutputValueInfos(Processor processor)
        {
            int numberOfOutArgs = processor.OutArguments.Count;
            OutputValueInfo[] result = new OutputValueInfo[numberOfOutArgs];
            for (int outArgIndex = 0; outArgIndex < numberOfOutArgs; ++outArgIndex)
            {
                ProcessorArgument outArg = processor.OutArguments[outArgIndex];
                ProcessorArgument[] inArgs = this.Pipeline.GetBoundToArguments(processor.OutArguments[outArgIndex]).ToArray();
                int[] inputValueIndices = new int[inArgs.Length];
                for (int inArgIndex = 0; inArgIndex < inArgs.Length; ++inArgIndex)
                {
                    int slotOffset = this.SlotOffSetOfInArgument(inArgs[inArgIndex]);
                    inputValueIndices[inArgIndex] = slotOffset;
                }

                result[outArgIndex] = new OutputValueInfo(outArg, inputValueIndices, inArgs);
            }

            return result;
        }

        private int SlotOffSetOfInArgument(ProcessorArgument inArgument)
        {
            int processorIndex = this.Pipeline.Processors.IndexOf(inArgument.ContainingCollection.Processor);

            Debug.Assert(processorIndex >= 0, "Processor does not belong to the current pipeline");
            Debug.Assert(inArgument != null, "inArgument cannot be null");
            Debug.Assert(inArgument.Index.HasValue, "inArgument must have valid index");

            return this.ProcessorValueInfos[processorIndex].InputValueOffset + inArgument.Index.Value;
        }

        internal class ProcessorValueInfo
        {
            public int InputValueOffset { get; set; }

            public int InputValueCount { get; set; }

            public OutputValueInfo[] OutputValueInfos { get; set; }
        }

        internal class OutputValueInfo
        {
            public OutputValueInfo(ProcessorArgument outArgument, int[] inputValueIndices, ProcessorArgument[] inArguments)
            {
                this.OutArgument = outArgument;
                this.InputValueIndices = inputValueIndices;
                this.InArguments = inArguments;
            }

            public ProcessorArgument OutArgument { get; private set; }

            public int[] InputValueIndices { get; private set; }

            public ProcessorArgument[] InArguments { get; private set; }
        }
    }
}