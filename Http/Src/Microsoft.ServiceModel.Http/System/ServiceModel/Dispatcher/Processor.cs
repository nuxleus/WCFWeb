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

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on declared input and output <see cref="ProcessorArgument">arguments</see>. 
    /// </summary>
    public abstract class Processor
    {
        internal static readonly ProcessorArgument[] EmptyProcessorArgumentArray = new ProcessorArgument[0];

        private ProcessorArgumentCollection inArguments;
        private ProcessorArgumentCollection outArguments;
        private ProcessorCollection containingCollection;

        /// <summary>
        /// Gets or sets the event handler for the event that occurs when a <see cref="Proccessor"/> is initializing.
        /// </summary>
        public EventHandler Initializing { get; set; }

        /// <summary>
        /// Gets or sets the event handler for the event that occurs when a <see cref="Proccessor"/> is initialized.
        /// </summary>
        public EventHandler Initialized { get; set; }

        /// <summary>
        /// Gets the collection of input <see cref="ProcessorArgument">ProcessorArguments</see> 
        /// that the <see cref="Processor"/> expects for execution.
        /// </summary>
        public ProcessorArgumentCollection InArguments
        {
            get
            {
                if (this.inArguments == null)
                {
                    IEnumerable<ProcessorArgument> arguments = 
                        this.OnGetInArguments() ?? 
                        EmptyProcessorArgumentArray;
                    this.inArguments = new ProcessorArgumentCollection(
                        this,
                        ProcessorArgumentDirection.In,
                        arguments.ToArray());
                }

                return this.inArguments;
            }
        }

        /// <summary>
        /// Gets the collection of output <see cref="ProcessorArgument">ProcessorArguments</see> 
        /// that the <see cref="Processor"/> returns from execution.
        /// </summary>
        public ProcessorArgumentCollection OutArguments
        {
            get
            {
                if (this.outArguments == null)
                {
                    IEnumerable<ProcessorArgument> arguments = 
                        this.OnGetOutArguments() ?? 
                        EmptyProcessorArgumentArray;
                    this.outArguments = new ProcessorArgumentCollection(
                        this,
                        ProcessorArgumentDirection.Out,
                        arguments.ToArray());
                }

                return this.outArguments;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Processor"/> has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="Processor">Processors</see> to which the
        /// given processor belongs.  Returns null if the <see cref="Processor"/> has
        /// not been added to a <see cref="ProcessorCollection"/>.
        /// </summary>
        public ProcessorCollection ContainingCollection
        {
            get
            {
                return this.containingCollection;
            }

            internal set
            {
                if (value != null && this.containingCollection != null && value != this.containingCollection)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            SR.ProcessorAlreadyBelongsToProcessorCollection,
                            this.GetType().Name));
                }

                this.containingCollection = value;
            }
        }

        /// <summary>
        /// Initializes the <see cref="Processor"/>.
        /// </summary>
        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                this.OnInitializing();
                this.OnInitialize();
                this.IsInitialized = true;
                this.OnInitialized();
            }
        }

        /// <summary>
        /// Executes the processor with the given <paramref name="input"/> values and returns 
        /// a <see cref="ProcessorResult"/>.
        /// </summary>
        /// <param name="input">
        /// The input values that the <see cref="Processor"/> should use for execution. 
        /// The values should agree in order and type with the input <see cref="ProcessorArgument">
        /// ProcessorArguments</see> given by the <see cref="Processor.InArguments"/> property.
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output values from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "This is by design. One processor throwing exception sometimes does not want to stop the rest of processors")]
        public ProcessorResult Execute(object[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (!this.IsInitialized)
            {
                throw new InvalidOperationException(SR.ProcessorMustBeInitializedBeforeExecution);
            }

            ProcessorResult result = null;
            try
            {
                result = this.OnExecute(input);
            }
            catch (Exception e)
            {
                result = new ProcessorResult();
                result.Error = e;
                result.Status = ProcessorStatus.Error;
            }

            if (result == null)
            {
                result = new ProcessorResult();
                result.Status = ProcessorStatus.Error;
                result.Error = new InvalidOperationException(SR.ProcessorReturnedNullProcessorResult);
            }

            if (result.Status == ProcessorStatus.Error)
            {
                this.OnError(result);
            }

            return result;
        }

        /// <summary>
        /// Implemented in a derived class to return the input <see cref="ProcessorArgument">ProcessorArguments</see>
        /// that the <see cref="Processor"/> expects to be provided whenever the <see cref="Processor.Execute"/> method 
        /// is called.  The <see cref="ProcessorArgument">ProcessorArguments</see> must be returned in the same order
        /// the the <see cref="Processor.Execute"/> method will expect them in the input object array.
        /// </summary>
        /// <remarks>
        /// <see cref="OnGetInArguments"/> is only called once and the <see cref="ProcessorArgument">ProcessorArguments</see>
        /// are cached in a readonly <see cref="ProcessorArgumentCollection"/>.
        /// </remarks>
        /// <returns>
        /// The input <see cref="ProcessorArgument">ProcessorArguments</see> that the <see cref="Processor"/>
        /// expects.
        /// </returns>
        protected abstract IEnumerable<ProcessorArgument> OnGetInArguments();

        /// <summary>
        /// Implemented in a derived class to return the ouput <see cref="ProcessorArgument">ProcessorArguments</see>
        /// that the <see cref="Processor"/> will provided whenever the <see cref="Processor.Execute"/> method 
        /// is called.  The <see cref="ProcessorArgument">ProcessorArguments</see> must be returned in the same order
        /// the the <see cref="Processor.Execute"/> method will provide then in the output object array of 
        /// the <see cref="ProcessorResult"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="OnGetInArguments"/> is only called once and the <see cref="ProcessorArgument">ProcessorArguments</see>
        /// are cached in a readonly <see cref="ProcessorArgumentCollection"/>.
        /// </remarks>
        /// <returns>
        /// The output <see cref="ProcessorArgument">ProcessorArguments</see> that the <see cref="Processor"/>
        /// will provide.
        /// </returns>
        protected abstract IEnumerable<ProcessorArgument> OnGetOutArguments();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input">
        /// The input values that the <see cref="Processor"/> should use for execution. 
        /// The values should agree in order and type with the input <see cref="ProcessorArgument">
        /// ProcessorArguments</see> given by the <see cref="Processor.InArguments"/> property.
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output values from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        protected abstract ProcessorResult OnExecute(object[] input);

        /// <summary>
        /// Invoked whenever the <see cref="ProcessorResult"/> returned from calling 
        /// <see cref="Processor.Execute"/> has a <see cref="ProcessorStatus"/>
        /// of <see cref="ProcessorStatus.Error"/>.
        /// </summary>
        /// <param name="result">
        /// The <see cref="ProcessorResult"/> returned from calling <see cref="Processor.Execute"/>
        /// </param>
        protected virtual void OnError(ProcessorResult result)
        {
        }

        /// <summary>
        /// Invoked when the <see cref="Processor"/> is transitioning from an un-initialized
        /// state to an initialized state.
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        private void OnInitialized()
        {
            EventHandler handler = this.Initialized;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnInitializing()
        {
            EventHandler handler = this.Initializing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
    
    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on a single declared input and a single declared output. 
    /// </summary>
    /// <typeparam name="T">The type of the input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
            Justification = "We allow generic classes and non-generic classes live in the same file")]
    public abstract class Processor<T, TOutput> : Processor
    {
        private ArgumentValueConverter<T> converter = ArgumentValueConverter.CreateValueConverter<T>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input">
        /// The input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(T input);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            Debug.Assert(input != null, "input cannot be null since OnExecute method is protected sealed");
            return this.OnExecute(this.converter.ConvertFrom(input[0]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on two declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(T1 input1, T2 input2);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on three declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1, 
                                                    T2 input2, 
                                                    T3 input3);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on four declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1, 
                                                    T2 input2, 
                                                    T3 input3, 
                                                    T4 input4);
        
        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on five declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on six declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on seven declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="T7">The type of the seventh input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, T7, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();
        private ArgumentValueConverter<T7> converter7 = ArgumentValueConverter.CreateValueConverter<T7>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input7">
        /// The seventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6,
                                                    T7 input7);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]),
                this.converter7.ConvertFrom(input[6]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on eight declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="T7">The type of the seventh input</typeparam>
    /// <typeparam name="T8">The type of the eighth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, T7, T8, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();
        private ArgumentValueConverter<T7> converter7 = ArgumentValueConverter.CreateValueConverter<T7>();
        private ArgumentValueConverter<T8> converter8 = ArgumentValueConverter.CreateValueConverter<T8>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input7">
        /// The seventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input8">
        /// The eighth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6,
                                                    T7 input7,
                                                    T8 input8);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]),
                this.converter7.ConvertFrom(input[6]),
                this.converter8.ConvertFrom(input[7]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on nine declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="T7">The type of the seventh input</typeparam>
    /// <typeparam name="T8">The type of the eighth input</typeparam>
    /// <typeparam name="T9">The type of the ninth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();
        private ArgumentValueConverter<T7> converter7 = ArgumentValueConverter.CreateValueConverter<T7>();
        private ArgumentValueConverter<T8> converter8 = ArgumentValueConverter.CreateValueConverter<T8>();
        private ArgumentValueConverter<T9> converter9 = ArgumentValueConverter.CreateValueConverter<T9>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input7">
        /// The seventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input8">
        /// The eighth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input9">
        /// The ninth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6,
                                                    T7 input7,
                                                    T8 input8,
                                                    T9 input9);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]),
                this.converter7.ConvertFrom(input[6]),
                this.converter8.ConvertFrom(input[7]),
                this.converter9.ConvertFrom(input[8]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on ten declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="T7">The type of the seventh input</typeparam>
    /// <typeparam name="T8">The type of the eighth input</typeparam>
    /// <typeparam name="T9">The type of the ninth input</typeparam>
    /// <typeparam name="T10">The type of the tenth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();
        private ArgumentValueConverter<T7> converter7 = ArgumentValueConverter.CreateValueConverter<T7>();
        private ArgumentValueConverter<T8> converter8 = ArgumentValueConverter.CreateValueConverter<T8>();
        private ArgumentValueConverter<T9> converter9 = ArgumentValueConverter.CreateValueConverter<T9>();
        private ArgumentValueConverter<T10> converter10 = ArgumentValueConverter.CreateValueConverter<T10>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input7">
        /// The seventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input8">
        /// The eighth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input9">
        /// The ninth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input10">
        /// The tenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6,
                                                    T7 input7,
                                                    T8 input8,
                                                    T9 input9,
                                                    T10 input10);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]),
                this.converter7.ConvertFrom(input[6]),
                this.converter8.ConvertFrom(input[7]),
                this.converter9.ConvertFrom(input[8]),
                this.converter10.ConvertFrom(input[9]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on eleven declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="T7">The type of the seventh input</typeparam>
    /// <typeparam name="T8">The type of the eighth input</typeparam>
    /// <typeparam name="T9">The type of the ninth input</typeparam>
    /// <typeparam name="T10">The type of the tenth input</typeparam>
    /// <typeparam name="T11">The type of the eleventh input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();
        private ArgumentValueConverter<T7> converter7 = ArgumentValueConverter.CreateValueConverter<T7>();
        private ArgumentValueConverter<T8> converter8 = ArgumentValueConverter.CreateValueConverter<T8>();
        private ArgumentValueConverter<T9> converter9 = ArgumentValueConverter.CreateValueConverter<T9>();
        private ArgumentValueConverter<T10> converter10 = ArgumentValueConverter.CreateValueConverter<T10>();
        private ArgumentValueConverter<T11> converter11 = ArgumentValueConverter.CreateValueConverter<T11>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input7">
        /// The seventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input8">
        /// The eighth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input9">
        /// The ninth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input10">
        /// The tenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input11">
        /// The eleventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6,
                                                    T7 input7,
                                                    T8 input8,
                                                    T9 input9,
                                                    T10 input10,
                                                    T11 input11);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
                   Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]),
                this.converter7.ConvertFrom(input[6]),
                this.converter8.ConvertFrom(input[7]),
                this.converter9.ConvertFrom(input[8]),
                this.converter10.ConvertFrom(input[9]),
                this.converter11.ConvertFrom(input[10]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on twelve declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="T7">The type of the seventh input</typeparam>
    /// <typeparam name="T8">The type of the eighth input</typeparam>
    /// <typeparam name="T9">The type of the ninth input</typeparam>
    /// <typeparam name="T10">The type of the tenth input</typeparam>
    /// <typeparam name="T11">The type of the eleventh input</typeparam>
    /// <typeparam name="T12">The type of the twelfth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();
        private ArgumentValueConverter<T7> converter7 = ArgumentValueConverter.CreateValueConverter<T7>();
        private ArgumentValueConverter<T8> converter8 = ArgumentValueConverter.CreateValueConverter<T8>();
        private ArgumentValueConverter<T9> converter9 = ArgumentValueConverter.CreateValueConverter<T9>();
        private ArgumentValueConverter<T10> converter10 = ArgumentValueConverter.CreateValueConverter<T10>();
        private ArgumentValueConverter<T11> converter11 = ArgumentValueConverter.CreateValueConverter<T11>();
        private ArgumentValueConverter<T12> converter12 = ArgumentValueConverter.CreateValueConverter<T12>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input7">
        /// The seventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input8">
        /// The eighth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input9">
        /// The ninth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input10">
        /// The tenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input11">
        /// The eleventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input12">
        /// The twelfth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6,
                                                    T7 input7,
                                                    T8 input8,
                                                    T9 input9,
                                                    T10 input10,
                                                    T11 input11,
                                                    T12 input12);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]),
                this.converter7.ConvertFrom(input[6]),
                this.converter8.ConvertFrom(input[7]),
                this.converter9.ConvertFrom(input[8]),
                this.converter10.ConvertFrom(input[9]),
                this.converter11.ConvertFrom(input[10]),
                this.converter12.ConvertFrom(input[11]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on thirteen declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="T7">The type of the seventh input</typeparam>
    /// <typeparam name="T8">The type of the eighth input</typeparam>
    /// <typeparam name="T9">The type of the ninth input</typeparam>
    /// <typeparam name="T10">The type of the tenth input</typeparam>
    /// <typeparam name="T11">The type of the eleventh input</typeparam>
    /// <typeparam name="T12">The type of the twelfth input</typeparam>
    /// <typeparam name="T13">The type of the thirteenth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();
        private ArgumentValueConverter<T7> converter7 = ArgumentValueConverter.CreateValueConverter<T7>();
        private ArgumentValueConverter<T8> converter8 = ArgumentValueConverter.CreateValueConverter<T8>();
        private ArgumentValueConverter<T9> converter9 = ArgumentValueConverter.CreateValueConverter<T9>();
        private ArgumentValueConverter<T10> converter10 = ArgumentValueConverter.CreateValueConverter<T10>();
        private ArgumentValueConverter<T11> converter11 = ArgumentValueConverter.CreateValueConverter<T11>();
        private ArgumentValueConverter<T12> converter12 = ArgumentValueConverter.CreateValueConverter<T12>();
        private ArgumentValueConverter<T13> converter13 = ArgumentValueConverter.CreateValueConverter<T13>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input7">
        /// The seventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input8">
        /// The eighth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input9">
        /// The ninth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input10">
        /// The tenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input11">
        /// The eleventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input12">
        /// The twelfth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input13">
        /// The thirteenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6,
                                                    T7 input7,
                                                    T8 input8,
                                                    T9 input9,
                                                    T10 input10,
                                                    T11 input11,
                                                    T12 input12,
                                                    T13 input13);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]),
                this.converter7.ConvertFrom(input[6]),
                this.converter8.ConvertFrom(input[7]),
                this.converter9.ConvertFrom(input[8]),
                this.converter10.ConvertFrom(input[9]),
                this.converter11.ConvertFrom(input[10]),
                this.converter12.ConvertFrom(input[11]),
                this.converter13.ConvertFrom(input[12]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on fourteen declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="T7">The type of the seventh input</typeparam>
    /// <typeparam name="T8">The type of the eighth input</typeparam>
    /// <typeparam name="T9">The type of the ninth input</typeparam>
    /// <typeparam name="T10">The type of the tenth input</typeparam>
    /// <typeparam name="T11">The type of the eleventh input</typeparam>
    /// <typeparam name="T12">The type of the twelfth input</typeparam>
    /// <typeparam name="T13">The type of the thirteenth input</typeparam>
    /// <typeparam name="T14">The type of the fourteenth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();
        private ArgumentValueConverter<T7> converter7 = ArgumentValueConverter.CreateValueConverter<T7>();
        private ArgumentValueConverter<T8> converter8 = ArgumentValueConverter.CreateValueConverter<T8>();
        private ArgumentValueConverter<T9> converter9 = ArgumentValueConverter.CreateValueConverter<T9>();
        private ArgumentValueConverter<T10> converter10 = ArgumentValueConverter.CreateValueConverter<T10>();
        private ArgumentValueConverter<T11> converter11 = ArgumentValueConverter.CreateValueConverter<T11>();
        private ArgumentValueConverter<T12> converter12 = ArgumentValueConverter.CreateValueConverter<T12>();
        private ArgumentValueConverter<T13> converter13 = ArgumentValueConverter.CreateValueConverter<T13>();
        private ArgumentValueConverter<T14> converter14 = ArgumentValueConverter.CreateValueConverter<T14>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input7">
        /// The seventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input8">
        /// The eighth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input9">
        /// The ninth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input10">
        /// The tenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input11">
        /// The eleventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input12">
        /// The twelfth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input13">
        /// The thirteenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input14">
        /// The fourteenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6,
                                                    T7 input7,
                                                    T8 input8,
                                                    T9 input9,
                                                    T10 input10,
                                                    T11 input11,
                                                    T12 input12,
                                                    T13 input13,
                                                    T14 input14);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]),
                this.converter7.ConvertFrom(input[6]),
                this.converter8.ConvertFrom(input[7]),
                this.converter9.ConvertFrom(input[8]),
                this.converter10.ConvertFrom(input[9]),
                this.converter11.ConvertFrom(input[10]),
                this.converter12.ConvertFrom(input[11]),
                this.converter13.ConvertFrom(input[12]),
                this.converter14.ConvertFrom(input[13]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }
    
    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on fifteen declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="T7">The type of the seventh input</typeparam>
    /// <typeparam name="T8">The type of the eighth input</typeparam>
    /// <typeparam name="T9">The type of the ninth input</typeparam>
    /// <typeparam name="T10">The type of the tenth input</typeparam>
    /// <typeparam name="T11">The type of the eleventh input</typeparam>
    /// <typeparam name="T12">The type of the twelfth input</typeparam>
    /// <typeparam name="T13">The type of the thirteenth input</typeparam>
    /// <typeparam name="T14">The type of the fourteenth input</typeparam>
    /// <typeparam name="T15">The type of the fifteenth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", 
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TOutput> : Processor
    {   
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();
        private ArgumentValueConverter<T7> converter7 = ArgumentValueConverter.CreateValueConverter<T7>();
        private ArgumentValueConverter<T8> converter8 = ArgumentValueConverter.CreateValueConverter<T8>();
        private ArgumentValueConverter<T9> converter9 = ArgumentValueConverter.CreateValueConverter<T9>();
        private ArgumentValueConverter<T10> converter10 = ArgumentValueConverter.CreateValueConverter<T10>();
        private ArgumentValueConverter<T11> converter11 = ArgumentValueConverter.CreateValueConverter<T11>();
        private ArgumentValueConverter<T12> converter12 = ArgumentValueConverter.CreateValueConverter<T12>();
        private ArgumentValueConverter<T13> converter13 = ArgumentValueConverter.CreateValueConverter<T13>();
        private ArgumentValueConverter<T14> converter14 = ArgumentValueConverter.CreateValueConverter<T14>();
        private ArgumentValueConverter<T15> converter15 = ArgumentValueConverter.CreateValueConverter<T15>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input7">
        /// The seventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input8">
        /// The eighth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input9">
        /// The ninth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input10">
        /// The tenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input11">
        /// The eleventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input12">
        /// The twelfth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input13">
        /// The thirteenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input14">
        /// The fourteenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input15">
        /// The fifteenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6,
                                                    T7 input7,
                                                    T8 input8,
                                                    T9 input9,
                                                    T10 input10,
                                                    T11 input11,
                                                    T12 input12,
                                                    T13 input13,
                                                    T14 input14,
                                                    T15 input15);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]),
                this.converter7.ConvertFrom(input[6]),
                this.converter8.ConvertFrom(input[7]),
                this.converter9.ConvertFrom(input[8]),
                this.converter10.ConvertFrom(input[9]),
                this.converter11.ConvertFrom(input[10]),
                this.converter12.ConvertFrom(input[11]),
                this.converter13.ConvertFrom(input[12]),
                this.converter14.ConvertFrom(input[13]),
                this.converter15.ConvertFrom(input[14]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }

    /// <summary>
    /// An abstract base class used to create custom processors that execute based 
    /// on sixteen declared inputs and a single declared output. 
    /// </summary>
    /// <typeparam name="T1">The type of the first input</typeparam>
    /// <typeparam name="T2">The type of the second input</typeparam>
    /// <typeparam name="T3">The type of the third input</typeparam>
    /// <typeparam name="T4">The type of the fourth input</typeparam>
    /// <typeparam name="T5">The type of the fifth input</typeparam>
    /// <typeparam name="T6">The type of the sixth input</typeparam>
    /// <typeparam name="T7">The type of the seventh input</typeparam>
    /// <typeparam name="T8">The type of the eighth input</typeparam>
    /// <typeparam name="T9">The type of the ninth input</typeparam>
    /// <typeparam name="T10">The type of the tenth input</typeparam>
    /// <typeparam name="T11">The type of the eleventh input</typeparam>
    /// <typeparam name="T12">The type of the twelfth input</typeparam>
    /// <typeparam name="T13">The type of the thirteenth input</typeparam>
    /// <typeparam name="T14">The type of the fourteenth input</typeparam>
    /// <typeparam name="T15">The type of the fifteenth input</typeparam>
    /// <typeparam name="T16">The type of the sixteenth input</typeparam>
    /// <typeparam name="TOutput">The type of the output</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "This is consistent with number of generic types defined for Func<>")]
    public abstract class Processor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TOutput> : Processor
    {
        private ArgumentValueConverter<T1> converter1 = ArgumentValueConverter.CreateValueConverter<T1>();
        private ArgumentValueConverter<T2> converter2 = ArgumentValueConverter.CreateValueConverter<T2>();
        private ArgumentValueConverter<T3> converter3 = ArgumentValueConverter.CreateValueConverter<T3>();
        private ArgumentValueConverter<T4> converter4 = ArgumentValueConverter.CreateValueConverter<T4>();
        private ArgumentValueConverter<T5> converter5 = ArgumentValueConverter.CreateValueConverter<T5>();
        private ArgumentValueConverter<T6> converter6 = ArgumentValueConverter.CreateValueConverter<T6>();
        private ArgumentValueConverter<T7> converter7 = ArgumentValueConverter.CreateValueConverter<T7>();
        private ArgumentValueConverter<T8> converter8 = ArgumentValueConverter.CreateValueConverter<T8>();
        private ArgumentValueConverter<T9> converter9 = ArgumentValueConverter.CreateValueConverter<T9>();
        private ArgumentValueConverter<T10> converter10 = ArgumentValueConverter.CreateValueConverter<T10>();
        private ArgumentValueConverter<T11> converter11 = ArgumentValueConverter.CreateValueConverter<T11>();
        private ArgumentValueConverter<T12> converter12 = ArgumentValueConverter.CreateValueConverter<T12>();
        private ArgumentValueConverter<T13> converter13 = ArgumentValueConverter.CreateValueConverter<T13>();
        private ArgumentValueConverter<T14> converter14 = ArgumentValueConverter.CreateValueConverter<T14>();
        private ArgumentValueConverter<T15> converter15 = ArgumentValueConverter.CreateValueConverter<T15>();
        private ArgumentValueConverter<T16> converter16 = ArgumentValueConverter.CreateValueConverter<T16>();

        /// <summary>
        /// Implemented in a derived class to provide the execution logic of the custom <see cref="Processor"/>.
        /// </summary>
        /// <param name="input1">
        /// The first input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input2">
        /// The second input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input3">
        /// The third input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input4">
        /// The fourth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input5">
        /// The fifth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input6">
        /// The sixth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input7">
        /// The seventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input8">
        /// The eighth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input9">
        /// The ninth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input10">
        /// The tenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input11">
        /// The eleventh input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input12">
        /// The twelfth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input13">
        /// The thirteenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input14">
        /// The fourteenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input15">
        /// The fifteenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <param name="input16">
        /// The sixteenth input value that the <see cref="Processor"/> should use for execution. 
        /// </param>
        /// <returns>
        /// A <see cref="ProcessorResult"/> that provides the output value from execution and
        /// indicates the execution <see cref="ProcessorStatus">status</see>.
        /// </returns>
        public abstract ProcessorResult<TOutput> OnExecute(
                                                    T1 input1,
                                                    T2 input2,
                                                    T3 input3,
                                                    T4 input4,
                                                    T5 input5,
                                                    T6 input6,
                                                    T7 input7,
                                                    T8 input8,
                                                    T9 input9,
                                                    T10 input10,
                                                    T11 input11,
                                                    T12 input12,
                                                    T13 input13,
                                                    T14 input14,
                                                    T15 input15,
                                                    T16 input16);

        /// <summary>
        /// This method calls the generic version of OnExecute method to change 
        /// the main processor's execution logic
        /// </summary>
        /// <param name="input">The input to the processor</param>
        /// <returns>The result of the exection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
           Justification = "input cannot be null since OnExecute method is protected sealed")]
        protected override sealed ProcessorResult OnExecute(object[] input)
        {
            return this.OnExecute(
                this.converter1.ConvertFrom(input[0]),
                this.converter2.ConvertFrom(input[1]),
                this.converter3.ConvertFrom(input[2]),
                this.converter4.ConvertFrom(input[3]),
                this.converter5.ConvertFrom(input[4]),
                this.converter6.ConvertFrom(input[5]),
                this.converter7.ConvertFrom(input[6]),
                this.converter8.ConvertFrom(input[7]),
                this.converter9.ConvertFrom(input[8]),
                this.converter10.ConvertFrom(input[9]),
                this.converter11.ConvertFrom(input[10]),
                this.converter12.ConvertFrom(input[11]),
                this.converter13.ConvertFrom(input[12]),
                this.converter14.ConvertFrom(input[13]),
                this.converter15.ConvertFrom(input[14]),
                this.converter16.ConvertFrom(input[15]));
        }

        /// <summary>
        /// This method calculates the input arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the input arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
        }

        /// <summary>
        /// This method calculates the output arguments based on the generic type information
        /// </summary>
        /// <returns>A list of the output arguments</returns>
        protected override sealed IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
        }
    }
}