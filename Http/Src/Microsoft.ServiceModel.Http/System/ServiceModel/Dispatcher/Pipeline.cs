// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Class that manages the binding of <see cref="ProcessorArgument"/>s and the
    /// runtime execution of a collection of <see cref="Processor"/> instances.
    /// </summary>
    public class Pipeline : Processor
    {
        private IEnumerable<ProcessorArgument> inArguments;
        private IEnumerable<ProcessorArgument> outArguments;
        private PipelineBindingCollection bindings;
        private PipelineContextInfo contextInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pipeline"/> class.
        /// </summary>
        /// <param name="processors">The set of <see cref="Processor"/> instances the <see cref="Pipeline"/> instance
        /// will use.</param>
        /// <param name="inArguments">The set of <see cref="ProcessorArgument"/> instances describing the input values
        /// that will be provided to <see cref="Execute"/>.</param>
        /// <param name="outArguments">The set of <see cref="ProcessorArgument"/> instances describing the output
        /// values that will be returned by <see cref="Execute"/>.</param>
        public Pipeline(IEnumerable<Processor> processors, IEnumerable<ProcessorArgument> inArguments, IEnumerable<ProcessorArgument> outArguments)
        {
            if (processors == null)
            {
                throw new ArgumentNullException("processors");
            }

            if (inArguments == null)
            {
                throw new ArgumentNullException("inArguments");
            }

            if (outArguments == null)
            {
                throw new ArgumentNullException("outArguments");
            }

            this.inArguments = inArguments;
            this.outArguments = outArguments;

            List<Processor> workingProcessors = new List<Processor>(processors);
            workingProcessors.Insert(0, new PipelineEntryProcessor(this));
            workingProcessors.Add(new PipelineExitProcessor(this));
            this.Processors = new ProcessorCollection(this, workingProcessors.ToArray());

            this.bindings = new PipelineBindingCollection(this.Processors);
        }

        /// <summary>
        /// Gets or sets the event handler for the event that occurs when two <see cref="ProcessorArgument">ProcessorArguments</see> are being bound.
        /// </summary>
        public EventHandler BindingArguments { get; set; }

        /// <summary>
        /// Gets or sets the event handler for the event that occurs after two <see cref="ProcessorArgument">ProcessorArguments</see> have been bound.
        /// </summary>
        public EventHandler BoundArguments { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="Processor"/> instances.
        /// </summary>
        public ProcessorCollection Processors { get; private set; }

        internal PipelineContextInfo ContextInfo
        {
            get
            {
                if (this.contextInfo == null)
                {
                    this.contextInfo = new PipelineContextInfo(this);
                }

                return this.contextInfo;
            }
        }

        private PipelineEntryProcessor EntryProcessor
        {
            get
            {
                return this.Processors[0] as PipelineEntryProcessor;
            }
        }

        private PipelineExitProcessor ExitProcessor
        {
            get
            {
                return this.Processors[this.Processors.Count - 1] as PipelineExitProcessor;
            }
        }

        /// <summary>
        /// Binds <paramref name="inArgument"/> to <paramref name="outArgument"/>
        /// </summary>
        /// <remarks>
        /// In this context, "bind" means that during execution, data values provided to
        /// <paramref name="outArgument"/> will be available to <paramref name="inArgument"/>
        /// at the time its respective <see cref="Processor.OnExecute"/> method is called.
        /// </remarks>
        /// <param name="outArgument">The output <see cref="ProcessorArgument"/> that will provide the data at runtime.</param>
        /// <param name="inArgument">The input <see cref="ProcessorArgument"/> that will receive the data at runtime.</param>
        public void BindArguments(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            if (outArgument == null)
            {
                throw new ArgumentNullException("outArgument");
            }

            if (inArgument == null)
            {
                throw new ArgumentNullException("inArgument");
            }

            if (this.IsInitialized)
            {
                throw new InvalidOperationException(
                    SR.ArgumentsCannotBeBoundAfterInitialization);
            }

            this.OnBindingArguments(outArgument, inArgument);
            this.OnBindArguments(outArgument, inArgument);
            this.bindings.BindArguments(outArgument, inArgument);
            this.OnBoundArguments(outArgument, inArgument);
        }

        /// <summary>
        /// Binds the given input <paramref name="argument"/> to the given <see cref="Pipeline"/> input argument.
        /// </summary>
        /// <remarks>
        /// This binding means the <see cref="Processor"/> containing <paramref name="argument"/> will receive
        /// the corresponding input value directly from the <see cref="Pipeline"/> instance when execution begins.
        /// </remarks>
        /// <param name="pipelineInputArgumentIndex">The relative zero-based index of the <see cref="Pipeline"/>'s
        /// input <see cref="ProcessorArgument"/>.   That index must match the order of the <see cref="Pipeline"/>'s
        /// <see cref="InArguments"/> collection.</param>
        /// <param name="argument">The input <see cref="ProcessorArgument"/> to bind to that <see cref="Pipeline"/> argument.</param>
        public void BindArgumentToPipelineInput(int pipelineInputArgumentIndex, ProcessorArgument argument)
        {
            ProcessorArgumentCollection pipelineInputs = this.Processors[0].OutArguments;

            if (pipelineInputArgumentIndex < 0 || pipelineInputArgumentIndex >= pipelineInputs.Count)
            {
                throw new ArgumentOutOfRangeException("pipelineInputArgumentIndex");
            }

            this.BindArguments(pipelineInputs[pipelineInputArgumentIndex], argument);
        }

        /// <summary>
        /// Binds the given input <paramref name="argument"/> to the given <see cref="Pipeline"/> input argument.
        /// </summary>
        /// <remarks>
        /// This binding means the <see cref="Processor"/> containing <paramref name="argument"/> will receive
        /// the corresponding input value directly from the <see cref="Pipeline"/> instance when execution begins.
        /// </remarks>
        /// <param name="pipelineInputArgumentName">The <see cref="ProcessorArgument.Name"/> of the <see cref="Pipeline"/>'s
        /// argument.</param>
        /// <param name="argument">The input <see cref="ProcessorArgument"/> to bind to that <see cref="Pipeline"/> argument.</param>
        public void BindArgumentToPipelineInput(string pipelineInputArgumentName, ProcessorArgument argument)
        {
            if (string.IsNullOrWhiteSpace(pipelineInputArgumentName))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, SR.ParameterCannotBeNullEmptyStringOrWhitespace, "pipelineInputArgumentName"),
                    "pipelineInputArgumentName");
            }

            ProcessorArgument pipelineInput = this.Processors[0].OutArguments[pipelineInputArgumentName];

            if (pipelineInput == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SR.PipelineDoesNotHaveGivenInputArgument,
                        pipelineInputArgumentName));
            }

            this.BindArguments(pipelineInput, argument);
        }

        /// <summary>
        /// Binds the given output <paramref name="argument"/> to the given <see cref="Pipeline"/> output argument.
        /// </summary>
        /// <remarks>
        /// This binding means the <see cref="Processor"/> containing <paramref name="argument"/> will provide
        /// the corresponding output value directly to the <see cref="Pipeline"/> instance's output values
        /// when execution begins.
        /// </remarks>
        /// <param name="argument">The <see cref="ProcessorArgument"/> to bind to the <see cref="Pipeline"/> argument.</param>
        /// <param name="pipelineOutputArgumentIndex">The relative zero-based index of the respective output argument
        /// in the <see cref="Pipeline"/>'s <see cref="OutArgument"/> collection.</param>
        public void BindArgumentToPipelineOutput(ProcessorArgument argument, int pipelineOutputArgumentIndex)
        {
            ProcessorArgumentCollection pipelineInputs = this.Processors[this.Processors.Count - 1].InArguments;

            if (pipelineOutputArgumentIndex < 0 || pipelineOutputArgumentIndex >= pipelineInputs.Count)
            {
                throw new ArgumentOutOfRangeException("pipelineOutputArgumentIndex");
            }

            this.BindArguments(argument, pipelineInputs[pipelineOutputArgumentIndex]);
        }

        /// <summary>
        /// Binds the given output <paramref name="argument"/> to the given <see cref="Pipeline"/> output argument.
        /// </summary>
        /// <remarks>
        /// This binding means the <see cref="Processor"/> containing <paramref name="argument"/> will provide
        /// the corresponding output value directly to the <see cref="Pipeline"/> instance's output values
        /// when execution begins.
        /// </remarks>
        /// <param name="argument">The <see cref="ProcessorArgument"/> to bind to the <see cref="Pipeline"/> argument.</param>
        /// <param name="pipelineOutputArgumentName">The <see cref="ProcessorArgument.Name"/> of the <see cref="Pipeline"/> argument.</param>
        public void BindArgumentToPipelineOutput(ProcessorArgument argument, string pipelineOutputArgumentName)
        {
            if (string.IsNullOrWhiteSpace(pipelineOutputArgumentName))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, SR.ParameterCannotBeNullEmptyStringOrWhitespace, "pipelineOutputArgumentName"),
                    "pipelineOutputArgumentName");
            }

            ProcessorArgument pipelineOutput = this.Processors[this.Processors.Count - 1].InArguments[pipelineOutputArgumentName];

            if (pipelineOutput == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SR.PipelineDoesNotHaveGivenOutputArgument,
                        pipelineOutputArgumentName));
            }

            this.BindArguments(argument, pipelineOutput);
        }

        /// <summary>
        /// Removes the binding between <paramref name="outArgument"/> and <paramref name="inArgument"/>.
        /// </summary>
        /// <param name="outArgument">The output <see cref="ProcessorArgument"/>.</param>
        /// <param name="inArgument">The input <see cref="ProcessorArgument"/></param>
        public void UnbindArguments(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            if (outArgument == null)
            {
                throw new ArgumentNullException("outArgument");
            }

            if (inArgument == null)
            {
                throw new ArgumentNullException("inArgument");
            }

            if (this.IsInitialized)
            {
                throw new InvalidOperationException(SR.ArgumentsCannotBeUnboundAfterInitialization);
            }

            this.bindings.UnbindArguments(outArgument, inArgument);
        }

        /// <summary>
        /// Returns the collection of <see cref="ProcessorArgument"/> instances bound to <paramref name="argument"/>.
        /// </summary>
        /// <remarks>
        /// This method can be used for both input and output arguments.
        /// </remarks>
        /// <param name="argument">The <see cref="ProcessorArgument"/> whose bindings are required.</param>
        /// <returns>The collection of <see cref="ProcessorArgument"/>s bound to <paramref name="argument"/></returns>
        public IEnumerable<ProcessorArgument> GetBoundToArguments(ProcessorArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            return this.bindings.GetBoundToArguments(argument);
        }

        /// <summary>
        /// Override this method to return a different list of InArguments
        /// </summary>
        /// <returns>A list of ProcessorArgument</returns>
        protected sealed override IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return this.inArguments;
        }

        /// <summary>
        /// Override this method to returna a different list of OutArguments
        /// </summary>
        /// <returns>A list of ProcessorArgument</returns>
        protected sealed override IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return this.outArguments;
        }

        /// <summary>
        /// Virtual method called from <see cref="OnExecute"/> to set the pipeline 
        /// input values on the <see cref="PipelineContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PipelineContext"/> for the current execution of the pipeline.</param>
        /// <param name="inputs">The inputs provided to the pipeline for the current execution.</param>
        protected virtual void OnSetPipelineInputs(PipelineContext context, object[] inputs)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.SetProcessorOutputs(this.EntryProcessor, inputs);
        }

        /// <summary>
        /// Virtual method called from <see cref="OnExecute"/> to get the pipeline 
        /// output values from the <see cref="PipelineContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PipelineContext"/> for the current execution of the pipeline.</param>
        /// <returns>The outputs that the pipeline will return for the current execution.</returns>
        protected virtual object[] OnGetPipelineOutputs(PipelineContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.ReadAllInputs(this.ExitProcessor);
        }

        /// <summary>
        /// Override this method to override the main execution logic of the pipeline
        /// </summary>
        /// <param name="input">The list of input to the piepline</param>
        /// <returns>The result of pipeline processing</returns>
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            PipelineContext context = this.OnCreateContext();

            this.OnSetPipelineInputs(context, input);
            ProcessorResult result = new ProcessorResult();
            while (result.Status == ProcessorStatus.Ok && context.AdvanceToNextProcessor())
            {
                result = this.OnExecuteProcessor(context);
            }

            if (result.Status == ProcessorStatus.Ok)
            {
                result = new ProcessorResult() { Output = this.OnGetPipelineOutputs(context) };
            }

            return result;
        }

        /// <summary>
        /// Virtual method called when the current <see cref="Pipeline"/>'s <see cref="Initialize"/> method has been called.
        /// </summary>
        /// <remarks>
        /// The default implementation invokes <see cref="Processor.Initialize"/> on each of its <see cref="Processor"/> instances.
        /// </remarks>
        protected override void OnInitialize()
        {
            foreach (Processor processor in this.Processors)
            {
                processor.Initialize();
            }

            base.OnInitialize();
        }

        /// <summary>
        /// Virtual method called when <see cref="BindArguments"/> has been called to bind two <see cref="ProcessorArguments"/>.
        /// </summary>
        /// <param name="outArgument">The output <see cref="ProcessorArgument"/>.</param>
        /// <param name="inArgument">The input <see cref="ProcessorArgument"/>.</param>
        protected virtual void OnBindArguments(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
        }

        /// <summary>
        /// Virtual method called from <see cref="OnExecute"/> to create a <see cref="PipelineContext"/>
        /// to contain the data values for <see cref="ProcessorArgument"/>s.
        /// </summary>
        /// <returns>A <see cref="PipelineContext"/> instance.</returns>
        protected virtual PipelineContext OnCreateContext()
        {
            return new PipelineContext(this, this.ContextInfo);
        }

        /// <summary>
        /// Virtual method called to execute the <paramref name="context"/>'s currently active <see cref="Processor"/>.
        /// </summary>
        /// <param name="context">The <see cref="PipelineContext"/> managing execution state.</param>
        /// <returns>The <see cref="ProcessorResult"/> returned by that <see cref="Processor"/>.</returns>
        protected virtual ProcessorResult OnExecuteProcessor(PipelineContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            Processor currentProcessor = context.CurrentProcessor;

             // The entry processor has no inputs and its "outputs" were already
             // set via OnSetPipelineInputs, so we can bypass its execution.
             // The exit processor's inputs have already been set by all the other
             // processors' execution, and it has no outputs of its own, so it can
             // be bypassed as well.
             if (currentProcessor == this.EntryProcessor || currentProcessor == this.ExitProcessor)
             {
                 return new ProcessorResult();
             }

            object[] input = context.ReadAllInputs(currentProcessor);
            ProcessorResult result = currentProcessor.Execute(input);
            switch (result.Status)
            {
                case ProcessorStatus.Error:
                    break;

                case ProcessorStatus.Ok:
                    context.SetProcessorOutputs(currentProcessor, result.Output);
                    break;
            }

            return result;
        }

        private void OnBoundArguments(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            EventHandler handler = this.BoundArguments;
            if (handler != null)
            {
                handler(this, new BindArgumentsEventArgs(outArgument, inArgument));
            }
        }

        private void OnBindingArguments(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            EventHandler handler = this.BindingArguments;
            if (handler != null)
            {
                handler(this, new BindArgumentsEventArgs(outArgument, inArgument));
            }
        }

        /// <summary>
        /// This inner class is responsible for moving the pipeline input arguments
        /// into the context.
        /// </summary>
        private class PipelineEntryProcessor : Processor
        {
            public PipelineEntryProcessor(Pipeline pipeline)
            {
                this.Pipeline = pipeline;
            }

            public Pipeline Pipeline { get; set; }

            protected override IEnumerable<ProcessorArgument> OnGetInArguments()
            {
                return Enumerable.Empty<ProcessorArgument>();
            }

            protected override IEnumerable<ProcessorArgument> OnGetOutArguments()
            {
                return this.Pipeline.InArguments.Select(a => a.Copy());
            }

            protected override ProcessorResult OnExecute(object[] input)
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// This inner class is responsible for moving the the final pipeline output
        /// values into the final <see cref="ProcessorResult"/>.
        /// </summary>
        private class PipelineExitProcessor : Processor
        {
            public PipelineExitProcessor(Pipeline pipeline)
            {
                this.Pipeline = pipeline;
            }

            private Pipeline Pipeline { get; set; }

            protected override IEnumerable<ProcessorArgument> OnGetInArguments()
            {
                return this.Pipeline.OutArguments.Select(a => a.Copy());
            }

            protected override IEnumerable<ProcessorArgument> OnGetOutArguments()
            {
                return Enumerable.Empty<ProcessorArgument>();
            }

            protected override ProcessorResult OnExecute(object[] input)
            {
                throw new NotSupportedException();
            }
        }
    }
}