// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Dispatcher;

    public class MockNonGenericProcessor : Processor
    {
        private ProcessorArgument[] inArguments;
        private ProcessorArgument[] outArguments;

        public Action OnInitializeAction;

        public Action<ProcessorResult> OnErrorAction;

        public Func<object[], ProcessorResult> OnExecuteAction;

        public void SetInputArguments(params ProcessorArgument[] inArguments)
        {
            this.inArguments = inArguments;
        }

        public void SetOutputArguments(params ProcessorArgument[] outArguments)
        {
            this.outArguments = outArguments;
        }

        protected override IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return this.inArguments;
        }

        protected override IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return this.outArguments;
        }

        protected override ProcessorResult OnExecute(object[] input)
        {
            if (this.OnExecuteAction != null)
            {
                return this.OnExecuteAction(input);
            }
            
            return null;
        }

        protected override void OnInitialize()
        {
            if (this.OnInitializeAction != null)
            {
                this.OnInitializeAction();
            }
            base.OnInitialize();
        }

        protected override void OnError(ProcessorResult result)
        {
            if (this.OnErrorAction != null)
            {
                this.OnErrorAction(result);
            }
            base.OnError(result);
        }
    }

    // General purpose generic processor whose protected methods can be invoked
    public abstract class MockProcessor<T, TOutput> : Processor<T, TOutput>
    {
        public string Name { get; set; }
        public Action<ProcessorResult> OnErrorCalled { get; set; }

        public ProcessorResult CallExecute(object[] input)
        {
            return this.Execute(input);
        }

        public ProcessorResult<TOutput> CallOnExecute(T input)
        {
            return this.OnExecute(input);
        }

        protected override void OnError(ProcessorResult result)
        {
            base.OnError(result);

            if (this.OnErrorCalled != null)
            {
                this.OnErrorCalled(result);
            }
        }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(this.Name)) ? this.GetType().Name : this.Name;
        }
    }

    public abstract class MockProcessor<T1, T2, TOutput> : Processor<T1, T2, TOutput>
    {
        public ProcessorResult<TOutput> CallExecute(object[] input)
        {
            return this.Execute(input) as ProcessorResult<TOutput>;
        }

        public ProcessorResult<TOutput> CallOnExecute(T1 input1, T2 input2)
        {
            return this.OnExecute(input1, input2);
        }
    }

    // General purpose mock processor that treat T as a nullable
    // and implements OnExecute to indicate whether it was null or had a value
    public class MockNullableProcessor<T> : MockProcessor<T?, string> where T : struct
    {
        public const string NOVALUE = "no value";

        public override ProcessorResult<string> OnExecute(T? input)
        {
            return new ProcessorResult<string>()
            {
                Output = input.HasValue ? input.Value.ToString() : NOVALUE
            };
        }
    }


    // General purpose mock processor that treats T as a value type
    // and implements OnExecute to simply format it and return
    public class MockValueTypeProcessor<T> : MockProcessor<T, string> where T : struct
    {
        public override ProcessorResult<string> OnExecute(T input)
        {
            return new ProcessorResult<string>()
            {
                Output = input.ToString()
            };
        }
    }

    public class MockProcessor1 : MockProcessor<int, string>
    {
        public Func<int, ProcessorResult<string>> OnExecuteCalled { get; set; }
        public Action<ProcessorResult> OnErrorCalledAction { get; set; }

        public override ProcessorResult<string> OnExecute(int intValue)
        {
            if (this.OnExecuteCalled != null)
            {
                return this.OnExecuteCalled(intValue);
            }

            ProcessorResult<string> result = new ProcessorResult<string>() { Output = intValue.ToString() };
            return result;
        }

        protected override void OnError(ProcessorResult result)
        {
            if (this.OnErrorCalledAction != null)
            {
                this.OnErrorCalledAction(result);
            }
        }
    }

    public class MockProcessor1Derived : MockProcessor1
    {
        public override ProcessorResult<string> OnExecute(int intValueDerived)
        {
            ProcessorResult<string> result = base.OnExecute(intValueDerived);
            result.Output = "Derived" + result.Output;
            return result;
        }
    }

    public class MockProcessor2 : MockProcessor<int, double, string>
    {
        public Action<ProcessorResult<string>> OnAfterExecute { get; set; }

        public override ProcessorResult<string> OnExecute(int intValue, double doubleValue)
        {
            ProcessorResult<string> result = new ProcessorResult<string>() { Output = ((double) (intValue) + doubleValue).ToString() };

            if (this.OnAfterExecute != null)
            {
                this.OnAfterExecute(result);
            }

            return result;
        }
    }

    public class MockProcessorEchoString : MockProcessor<string, string>
    {

        public override ProcessorResult<string> OnExecute(string input)
        {
            return new ProcessorResult<string>() { Output = input };
        }
    }

    public class MockProcessorNoArgs : Processor
    {

        protected override IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            return Enumerable.Empty<ProcessorArgument>();
        }

        protected override IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            return Enumerable.Empty<ProcessorArgument>();
        }

        protected override ProcessorResult OnExecute(object[] input)
        {
            return new ProcessorResult();
        }
    }
}
