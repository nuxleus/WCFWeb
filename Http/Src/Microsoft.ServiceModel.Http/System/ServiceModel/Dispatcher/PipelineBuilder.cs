// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Class the manages the creation and validation of a <see cref="Pipeline"/>.
    /// </summary>
    public class PipelineBuilder : PipelineBuilder<Pipeline>
    {
    }

    /// <summary>
    /// Class that manages the creation and validation of a <typeparamref name="TPipeline"/>.
    /// </summary>
    /// <typeparam name="TPipeline">The type of the <see cref="Pipeline"/> thie builder manages.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
            Justification = "We allow generic classes and non-generic classes live in the same file")]
    public class PipelineBuilder<TPipeline> where TPipeline : Pipeline
    {
        private static readonly Type pipelineType = typeof(Pipeline);
        private static readonly Type IEnumerableProcessorType = typeof(IEnumerable<Processor>);
        private static readonly Type IEnumerableProcessorArgumentType = typeof(IEnumerable<ProcessorArgument>);

        /// <summary>
        /// Creates a <typeparamref name="TPipeline"/> instance.
        /// </summary>
        /// <remarks>
        /// This factory method invokes virtual methods to filter, order, create, bind, and validate
        /// the elements in the <typeparamref name="TPipeline"/> instance before it is returned.
        /// </remarks>
        /// <param name="processors">The collection of <see cref="Processors"/> to include in the pipeline.
        /// This collection may be in arbitrary order.  Some of the processors may ultimately be filtered out
        /// by the <see cref="OnFilter"/> method.</param>
        /// <param name="inArguments">The collection of <see cref="Pipeline"/> input arguments.  These describe the
        /// values that will be provided to the <typeparamref name="TPipeline"/> instance when its begins execution.</param>
        /// <param name="outArguments">The collection of <see cref="Pipeline"/> output arguments.  These describe
        /// the values that will be available from the <typeparamref name="TPipeline"/> when it has completed execution.</param>
        /// <returns>A <typeparamref name="TPipeline"/> instance, ready for execution.</returns>
        public TPipeline Build(IEnumerable<Processor> processors, IEnumerable<ProcessorArgument> inArguments, IEnumerable<ProcessorArgument> outArguments)
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

            processors = this.OnFilter(processors);

            if (processors == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SR.DerivedMethodCannotReturnNull,
                        "OnFilter"));
            }

            processors = this.OnOrder(processors);

            if (processors == null)
            {
                throw new InvalidOperationException(
                    string.Format( 
                        CultureInfo.InvariantCulture,
                        SR.DerivedMethodCannotReturnNull,
                        "OnOrder"));
            }

            TPipeline pipeline = this.OnCreatePipeline(processors, inArguments, outArguments);

            if (pipeline == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SR.DerivedMethodCannotReturnNull,
                        "OnCreatePipeline"));
            }

            this.OnInitialize(pipeline);
            this.BindPipeline(pipeline);
            this.OnValidate(pipeline);
            pipeline.Initialize();
            return pipeline;
        }

        /// <summary>
        /// Virtual method called by <see cref="Build"/> to allow specific <see cref="Processor"/> instances
        /// to be removed from the collection of original <see cref="Processors"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method filters out all processors
        /// that implement <see cref="IConditionalExecutionProcessor"/> and respond <c>false</c>
        /// to <see cref="IConditionalExecutionProcessor.WillExecute"/>.
        /// <para>
        /// This virtual method is called before any ordering, binding or validation is performed.
        /// </para>
        /// </remarks>
        /// <param name="processors">The original set of <see cref="Processor"/> instances given to <see cref="Build"/>.</param>
        /// <returns>The complete set of <see cref="Processor"/> instances the <see cref="PipelineBuilder"/> should use.</returns>
        protected virtual IEnumerable<Processor> OnFilter(IEnumerable<Processor> processors)
        {
            if (processors == null)
            {
                throw new ArgumentNullException("processors");
            }

            foreach (Processor processor in processors)
            {
                IConditionalExecutionProcessor conditionalProcessor = processor as IConditionalExecutionProcessor;
                if (conditionalProcessor == null || conditionalProcessor.WillExecute)
                {
                    yield return processor;
                }
            }
        }

        /// <summary>
        /// Virtual method called by <see cref="Build"/> to manage the execution order 
        /// of the <see cref="Processor"/> instances.
        /// </summary>
        /// <remarks>
        /// This method is responsible for choosing the order in which the <see cref="Processor"/>
        /// instances will execute.  The default implementation will order them such that all <see cref="Processor"/>
        /// instances with bound input <see cref="ProcessorArguments"/> will execute after the respective
        /// <see cref="Processor"/> that sets those values.
        /// <para>
        /// This method is called after <see cref="OnFilter"/> but before any binding or validation
        /// has occurred.
        /// </para>
        /// </remarks>
        /// <param name="processors">The set of processors to order.</param>
        /// <returns>The set of processors in the order they should be executed.</returns>
        protected virtual IEnumerable<Processor> OnOrder(IEnumerable<Processor> processors)
        {
            if (processors == null)
            {
                throw new ArgumentNullException("processors");
            }

            return this.OrderProcessors(processors);
        }

        /// <summary>
        /// Virtual method called during a <see cref="Build"/> to choose the relative execution order of two
        /// <see cref="Processor"/> instances.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method examines the input and output <see cref="ProcessorArgument"/>s
        /// of the two processors.   If either one depends on argument values from the other, this method will indicate
        /// that <see cref="Processor"/> should execute after the one on which it depends.  
        /// If no dependency is found, <see cref="ProcessorExecutionOrder.Impartial"/> is returned.
        /// </remarks>
        /// <param name="firstProcessor">The first <see cref="Processor"/> to consider.</param>
        /// <param name="secondProcessor">The second <see cref="Processor"/> to consider.</param>
        /// <returns>An <see cref="ProcessorExecutionOrder"/> value that indicates how <paramref name="firstProcessor"/>
        /// should execute with respect to <paramref name="secondProcessor"/>.  For example, returning
        /// <see cref="ProcessorExecutionOrder.Before"/> indicates <paramref name="firstProcessor"/> should execute
        /// before <paramref name="secondProcessor"/>.   Subclasses can override this behavior to alter
        /// the default policy for execution order.</returns>
        protected virtual ProcessorExecutionOrder OnGetRelativeExecutionOrder(Processor firstProcessor, Processor secondProcessor)
        {
            if (firstProcessor == null)
            {
                throw new ArgumentNullException("firstProcessor");
            }

            if (secondProcessor == null)
            {
                throw new ArgumentNullException("secondProcessor");
            }

            return this.GetExecutionOrderFromBindings(firstProcessor, secondProcessor);
        }

        /// <summary>
        /// Virtual method called during a <see cref="Build"/> to create the <typeparamref name="TPipeline"/> instance.
        /// </summary>
        /// <remarks>
        /// Subclasses of <see cref="PipelineBuilder"/> should override this method to provide their strongly-typed
        /// <typeparamref name="TPipeline"/> instance.
        /// </remarks>
        /// <param name="processors">The set of <see cref="Processor"/> instances the <typeparamref name="TPipeline"/>
        /// will execute.</param>
        /// <param name="inArguments">The set of <see cref="ProcessorArgument"/>s describing the values that will be passed to
        /// the <typeparamref name="TPipeline"/> instance when it executes.</param>
        /// <param name="outArguments">The set of <see cref="ProcessorArgument"/>s describing the values that will be returned from
        /// the <typeparamref name="TPipeline"/> instance after it has executed.</param>
        /// <returns>A <typeparamref name="TPipeline"/> instance.</returns>
        protected virtual TPipeline OnCreatePipeline(IEnumerable<Processor> processors, IEnumerable<ProcessorArgument> inArguments, IEnumerable<ProcessorArgument> outArguments)
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

            Type typeOfPipeline = typeof(TPipeline);
            if (typeOfPipeline == pipelineType)
            {
                return new Pipeline(processors, inArguments, outArguments) as TPipeline;
            }

            ValidatePipelineHasExpectedConstructor(pipelineType);
            return Activator.CreateInstance(pipelineType, processors, inArguments, outArguments) as TPipeline;
        }

        /// <summary>
        /// Virtual method called during <see cref="Build"/> to initialize the <typeparamref name="TPipeline"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation invokes <see cref="Processor.Initialize"/> for each <see cref="Processor"/>
        /// that belongs to the <typeparamref name="TPipeline"/>.   This method is called after <see cref="OnCreatePipeline"/>
        /// but before any <see cref="ProcessorArgument"/> bindings have been created or <see cref="OnValidate"/> is called.
        /// </remarks>
        /// <param name="pipeline">The <typeparamref name="TPipeline"/> to initialize.</param>
        protected virtual void OnInitialize(TPipeline pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException("pipeline");
            }

            foreach (Processor processor in pipeline.Processors)
            {
                processor.Initialize();
            }
        }

        /// <summary>
        /// Virtual method called during a <see cref="Build"/> to determine whether an input 
        /// <see cref="ProcessorArgument"/> should bind to a specific output <see cref="ProcessorArgument"/>.
        /// </summary>
        /// <remarks>
        /// In this context, "bind" means that a data value for the output <see cref="ProcessorArgument"/> will be
        /// provided to the input <see cref="ProcessorArgument"/> during execution of the <typeparamref name="TPipeline"/>.
        /// <para>
        /// This method is called for every pair of input and output <see cref="ProcessorArguments"/> 
        /// in the <typeparamref name="TPipeline"/>.   The default implementation will return <c>true</c> if the arguments'
        /// names match (case insensitive) and the output type is assignable to the input type.
        /// </para>
        /// </remarks>
        /// <param name="outArgument">The candidate output <see cref="ProcessorArgument"/>.</param>
        /// <param name="inArgument">The candidate input <see cref="ProcessorArgument"/>.</param>
        /// <returns>A <c>true</c> value means they should bind.  A <c>false</c> value means they should not bind.</returns>
        protected virtual bool OnShouldArgumentsBind(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            if (outArgument == null)
            {
                throw new ArgumentNullException("outArgument");
            }

            if (inArgument == null)
            {
                throw new ArgumentNullException("inArgument");
            }

            return inArgument.ArgumentType.IsAssignableFrom(outArgument.ArgumentType) &&
                string.Equals(outArgument.Name, inArgument.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Virtual method called during <see cref="Build"/> to validate the integrity of the <typeparamref name="TPipeline"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// The default implementation examines all required input <see cref="ParameterArgument"/>s to determine whether they
        /// are bound to a corresponding output <see cref="ParameterArgument"/>.
        /// <para>
        /// An <see cref="InvalidOperationException"/> will be thrown to report validation errors.
        /// </para>
        /// </remarks>
        /// <param name="pipeline">The <typeparamref name="TPipeline"/> to validate.</param>
        protected virtual void OnValidate(TPipeline pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException("pipeline");
            }

            for (int processorIndex = 0; processorIndex < pipeline.Processors.Count; processorIndex++)
            {
                Processor processor = pipeline.Processors[processorIndex];
                for (int argumentIndex = 0; argumentIndex < processor.InArguments.Count; argumentIndex++)
                {
                    ProcessorArgument argument = processor.InArguments[argumentIndex];
                    if (!pipeline.GetBoundToArguments(argument).Any())
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture, 
                                SR.PipelineInvalidSinceArgumentNotBound,
                                argument.Name,
                                processor.GetType().Name,
                                processorIndex));
                    }
                }
            }
        }

        private static void ValidatePipelineHasExpectedConstructor(Type pipelineType)
        {
            Debug.Assert(pipelineType != null, "The 'pipelineType' parameter should not be null.");

            foreach (ConstructorInfo constructor in pipelineType.GetConstructors())
            {
                if (HasExpectedConstructorArguments(constructor))
                {
                    return;
                }
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.InvariantCulture, 
                    SR.PipelineBuilderRequiresPipelineConstructorHaveCertainParameters,
                    pipelineType.Name));
        }

        private static bool HasExpectedConstructorArguments(ConstructorInfo constructor)
        {
            Debug.Assert(constructor != null, "The 'constructor' parameter should not be null.");

            ParameterInfo[] parameters = constructor.GetParameters();

            return parameters.Length == 3 &&
                   parameters[0].ParameterType == IEnumerableProcessorType &&
                   parameters[1].ParameterType == IEnumerableProcessorArgumentType &&
                   parameters[2].ParameterType == IEnumerableProcessorArgumentType;  
        }

        private void BindPipeline(TPipeline pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException("pipeline");
            }

            for (int outProcessorIndex = 0; outProcessorIndex < pipeline.Processors.Count; outProcessorIndex++)
            {
                Processor outProcessor = pipeline.Processors[outProcessorIndex];
                for (int outArgumentIndex = 0; outArgumentIndex < outProcessor.OutArguments.Count; outArgumentIndex++)
                {
                    ProcessorArgument outArgument = outProcessor.OutArguments[outArgumentIndex];
                    for (int inProcessorIndex = outProcessorIndex + 1; inProcessorIndex < pipeline.Processors.Count; inProcessorIndex++)
                    {
                        Processor inProcessor = pipeline.Processors[inProcessorIndex];
                        for (int inArgumentIndex = 0; inArgumentIndex < inProcessor.InArguments.Count; inArgumentIndex++)
                        {
                            ProcessorArgument inArgument = inProcessor.InArguments[inArgumentIndex];
                            if (this.OnShouldArgumentsBind(outArgument, inArgument))
                            {
                                pipeline.BindArguments(outArgument, inArgument);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<Processor> OrderProcessors(IEnumerable<Processor> processors)
        {
            Processor[] processorArray = processors.ToArray();

            bool swapped = true;
            while (swapped)
            {
                swapped = false;
                for (int i = 0; i < processorArray.Length - 1; ++i)
                {
                    for (int j = i + 1; j < processorArray.Length; ++j)
                    {
                        if (!this.AreProcessorsInCorrectOrder(processorArray[i], processorArray[j]))
                        {
                            swapped = true;
                            Processor temp = processorArray[i];
                            processorArray[i] = processorArray[j];
                            processorArray[j] = temp;
                            break;
                        }
                    }

                    if (swapped)
                    {
                        break;
                    }
                }
            }

            return processorArray;
        }

        private bool AreProcessorsInCorrectOrder(Processor firstProcessor, Processor secondProcessor)
        {
            ProcessorExecutionOrder firstExecutionOrder;
            ProcessorExecutionOrder secondExecutionOrder;

            this.GetProcessorExecutionOrder(firstProcessor, secondProcessor, out firstExecutionOrder, out secondExecutionOrder);

            if (firstExecutionOrder == ProcessorExecutionOrder.Impartial)
            {
                return secondExecutionOrder == ProcessorExecutionOrder.Impartial || secondExecutionOrder == ProcessorExecutionOrder.After;
            }
            else
            {
                if (secondExecutionOrder == ProcessorExecutionOrder.Impartial)
                {
                    return firstExecutionOrder != ProcessorExecutionOrder.After;
                }

                if (firstExecutionOrder == secondExecutionOrder)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            SR.ProcessorOrderingConflictCannotBeResolved,
                            firstProcessor.GetType().Name,
                            secondProcessor.GetType().Name));
                }

                if (firstExecutionOrder == ProcessorExecutionOrder.After || secondExecutionOrder == ProcessorExecutionOrder.Before)
                {
                    return false;
                }

                return true;
            }
        }

        private void GetProcessorExecutionOrder(
                        Processor firstProcessor, 
                        Processor secondProcessor, 
                        out ProcessorExecutionOrder firstExecutionOrder, 
                        out ProcessorExecutionOrder secondExecutionOrder)
        {
            IOrderableProcessor orderableFirstProcessor = firstProcessor as IOrderableProcessor;
            IOrderableProcessor orderableSecondProcessor = secondProcessor as IOrderableProcessor;

            // Precedence goes to the IOrderableProcessor overrides
            firstExecutionOrder = orderableFirstProcessor != null 
                            ? orderableFirstProcessor.GetRelativeExecutionOrder(secondProcessor) 
                            : ProcessorExecutionOrder.Impartial;

            secondExecutionOrder = orderableSecondProcessor != null
                ? orderableSecondProcessor.GetRelativeExecutionOrder(firstProcessor)
                : ProcessorExecutionOrder.Impartial;

            // If neither processor expressed a choice, let the pipeline builder dictate order.
            // We ask each processor its opinion in case they are contradictory.
            // The caller is responsible for reporting or resolving any conflicts.
            if (firstExecutionOrder == ProcessorExecutionOrder.Impartial && secondExecutionOrder == ProcessorExecutionOrder.Impartial)
            {
                firstExecutionOrder = this.OnGetRelativeExecutionOrder(firstProcessor, secondProcessor);
                secondExecutionOrder = this.OnGetRelativeExecutionOrder(secondProcessor, firstProcessor);
            }
        }

        private ProcessorExecutionOrder GetExecutionOrderFromBindings(Processor firstProcessor, Processor secondProcessor)
        {
            // Algorithm -- if the 2nd processor has any incoming dependency on the 1st,
            // we say the 1st must run before.
            foreach (ProcessorArgument inArg in secondProcessor.InArguments)
            {
                foreach (ProcessorArgument outArg in firstProcessor.OutArguments)
                {
                    if (this.OnShouldArgumentsBind(outArg, inArg))
                    {
                        return ProcessorExecutionOrder.Before;
                    }
                }
            }

            // But we also need to make the reverse dependency check in case the 1st
            // depends on the second, in which case we explicitly ask it to run after.
            foreach (ProcessorArgument inArg in firstProcessor.InArguments)
            {
                foreach (ProcessorArgument outArg in secondProcessor.OutArguments)
                {
                    if (this.OnShouldArgumentsBind(outArg, inArg))
                    {
                        return ProcessorExecutionOrder.After;
                    }
                }
            }

            return ProcessorExecutionOrder.Impartial;
        }
    }
}