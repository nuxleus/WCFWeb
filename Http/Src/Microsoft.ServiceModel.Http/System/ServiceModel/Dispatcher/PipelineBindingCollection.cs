// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

    internal class PipelineBindingCollection
    {
        private ProcessorCollection processors;
        private Dictionary<ProcessorArgument, List<ProcessorArgument>> bindings;
        
        public PipelineBindingCollection(ProcessorCollection processors)
        {
            Debug.Assert(processors != null, "The 'processors' parameter should not be null.");

            this.processors = processors;
            this.bindings = new Dictionary<ProcessorArgument, List<ProcessorArgument>>();
        }

        public void BindArguments(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            Debug.Assert(outArgument != null, "The 'outArgument' parameter should not be null.");
            Debug.Assert(inArgument != null, "The 'inArgument' parameter should not be null.");

            this.ValidateBinding(outArgument, inArgument);

            List<ProcessorArgument> outArgumentBindings = this.GetBindingsForArgument(outArgument);
            AddArgumentToBinding(inArgument, outArgumentBindings);

            List<ProcessorArgument> inArgumentBindings = this.GetBindingsForArgument(inArgument);
            AddArgumentToBinding(outArgument, inArgumentBindings);
        }

        public void UnbindArguments(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            Debug.Assert(outArgument != null, "The 'outArgument' parameter should not be null.");
            Debug.Assert(inArgument != null, "The 'inArgument' parameter should not be null.");

            this.ValidateBinding(outArgument, inArgument);

            List<ProcessorArgument> outArgumentBindings = this.GetBindingsForArgumentOrNull(outArgument);
            RemoveArgumentFromBinding(inArgument, outArgumentBindings);

            List<ProcessorArgument> inArgumentBindings = this.GetBindingsForArgumentOrNull(inArgument);
            RemoveArgumentFromBinding(outArgument, inArgumentBindings);
        }

        public IEnumerable<ProcessorArgument> GetBoundToArguments(ProcessorArgument argument)
        {
            Debug.Assert(argument != null, "The 'argument' parameter should not be null.");

            IEnumerable<ProcessorArgument> arguments = this.GetBindingsForArgumentOrNull(argument);
            return arguments ?? Processor.EmptyProcessorArgumentArray;
        }

        private static void AddArgumentToBinding(ProcessorArgument argument, List<ProcessorArgument> argumentBindings)
        {
            Debug.Assert(argument != null, "The 'argument' parameter should not be null.");
            Debug.Assert(argumentBindings != null, "The 'argumentBindings' parameter should not be null.");

            if (!argumentBindings.Contains(argument))
            {
                argumentBindings.Add(argument);
            }
        }

        private static void RemoveArgumentFromBinding(ProcessorArgument argument, List<ProcessorArgument> argumentBindings)
        {
            Debug.Assert(argument != null, "The 'argument' parameter should not be null.");

            if (argumentBindings != null && argumentBindings.Contains(argument))
            {
                argumentBindings.Remove(argument);
            }
        }

        private void ValidateBinding(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            Debug.Assert(outArgument != null, "The 'outArgument' parameter should not be null.");
            Debug.Assert(inArgument != null, "The 'inArgument' parameter should not be null.");

            if (inArgument.ContainingCollection == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SR.ArgumentBindingInvalidSinceNotInProcessorArgumentCollection,
                        inArgument.Name,
                        inArgument.ArgumentType.FullName));
            }

            if (outArgument.ContainingCollection == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SR.ArgumentBindingInvalidSinceNotInProcessorArgumentCollection,
                        outArgument.Name,
                        outArgument.ArgumentType.FullName));
            }

            if (outArgument.ContainingCollection.Direction != ProcessorArgumentDirection.Out)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SR.ArgumentBindingInvalidSinceNotOutputArgument,
                        outArgument.Name,
                        outArgument.ArgumentType.FullName));
            }

            if (inArgument.ContainingCollection.Direction != ProcessorArgumentDirection.In)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SR.ArgumentBindingInvalidSinceNotInputArgument,
                        inArgument.Name,
                        inArgument.ArgumentType.FullName));
            }

            if (!inArgument.ArgumentType.IsAssignableFrom(outArgument.ArgumentType))
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SR.ArgumentBindingInvalidSinceNotAssignable,
                        outArgument.Name,
                        outArgument.ArgumentType.FullName,
                        inArgument.Name,
                        inArgument.ArgumentType.FullName));
            }

            Processor inProcessor = inArgument.ContainingCollection.Processor;
            Processor outProcessor = outArgument.ContainingCollection.Processor;

            if (inProcessor == outProcessor)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SR.ArgumentBindingInvalidSinceBelongToSameProcessor,
                        outArgument.Name,
                        outArgument.ArgumentType.FullName,
                        inArgument.Name,
                        inArgument.ArgumentType.FullName));
            }

            int outProcessorIndex = -1;
            int inProcessorIndex = -1;
            
            for (int i = 0; i < this.processors.Count; i++)
            {
                if (this.processors[i] == outProcessor)
                {
                    outProcessorIndex = i;
                }
                else if (this.processors[i] == inProcessor)
                {
                    inProcessorIndex = i;
                }

                if (inProcessorIndex > -1 && outProcessorIndex > -1)
                {
                    break;
                }
            }

            if (outProcessorIndex == -1)
            {
                throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            SR.ArgumentBindingInvalidSinceProcessorNotInPipeline,
                            outArgument.Name,
                            outArgument.ArgumentType.FullName));
            }

            if (inProcessorIndex == -1)
            {
                throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            SR.ArgumentBindingInvalidSinceProcessorNotInPipeline,
                            inArgument.Name,
                            inArgument.ArgumentType.FullName));
            }

            if (outProcessorIndex >= inProcessorIndex)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        SR.ArgumentBindingInvalidSinceProcessorOrderIsWrong,
                        outArgument.Name,
                        outArgument.ArgumentType.FullName,
                        inArgument.Name,
                        inArgument.ArgumentType.FullName));
            }
        }

        private List<ProcessorArgument> GetBindingsForArgument(ProcessorArgument argument)
        {
            Debug.Assert(argument != null, "The 'argument' parameter should not be null.");

            List<ProcessorArgument> argumentBindings = this.GetBindingsForArgumentOrNull(argument);
            if (argumentBindings == null)
            {
                argumentBindings = new List<ProcessorArgument>();
                this.bindings.Add(argument, argumentBindings);
            }

            return argumentBindings;
        }

        private List<ProcessorArgument> GetBindingsForArgumentOrNull(ProcessorArgument argument)
        {
            Debug.Assert(argument != null, "The 'argument' parameter should not be null.");

            List<ProcessorArgument> argumentBindings;
            if (!this.bindings.TryGetValue(argument, out argumentBindings))
            {
                return null;
            }

            return argumentBindings;
        }
    }
}