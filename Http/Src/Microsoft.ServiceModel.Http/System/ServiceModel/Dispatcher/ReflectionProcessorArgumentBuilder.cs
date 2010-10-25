// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal class ReflectionProcessorArgumentBuilder
    {
        private static readonly Type ProcessorType = typeof(Processor);

        private MethodInfo executeMethod;
        private Type outputArgumentType;
        private Type[] inputArgumentTypes;
        private Type processorType;

        public ReflectionProcessorArgumentBuilder(Type processorType)
        {
            this.processorType = processorType;
        }

        public MethodInfo ExecuteMethod
        {
            get
            {
                if (this.executeMethod == null)
                {
                    this.executeMethod = this.FindExecuteMethod();
                }

                return this.executeMethod;
            }
        }

        private Type OutputArgumentType
        {
            get
            {
                if (this.outputArgumentType == null)
                {
                    Type baseType = GetProcessorBaseType(this.processorType);
                    if (baseType.IsGenericType)
                    {
                        Type[] genericArguments = baseType.GetGenericArguments();
                        this.outputArgumentType = genericArguments[genericArguments.Length - 1];
                    }
                    else
                    {
                        this.outputArgumentType = typeof(ProcessorResult);
                    }
                }

                return this.outputArgumentType;
            }
        }

        private Type[] InputArgumentTypes
        {
            get
            {
                if (this.inputArgumentTypes == null)
                {
                    Type baseType = GetProcessorBaseType(this.processorType);
                    if (baseType.IsGenericType)
                    {
                        Type[] genericArguments = baseType.GetGenericArguments();
                        this.inputArgumentTypes = new Type[genericArguments.Length - 1];
                        Array.Copy(genericArguments, this.inputArgumentTypes, genericArguments.Length - 1);
                    }
                    else
                    {
                        this.outputArgumentType = typeof(object[]);
                    }
                }

                return this.inputArgumentTypes;
            }
        }

        internal IEnumerable<ProcessorArgument> BuildInputArgumentCollection()
        {
            MethodInfo localExecuteMethod = this.ExecuteMethod;
            if (localExecuteMethod == null)
            {
                return Enumerable.Empty<ProcessorArgument>();
            }

            ParameterInfo[] parameterInfos = localExecuteMethod.GetParameters();
            ProcessorArgument[] arguments = new ProcessorArgument[parameterInfos.Length];
            for (int i = 0; i < parameterInfos.Length; ++i)
            {
                ParameterInfo parameter = parameterInfos[i];
                ProcessorArgument pad = new ProcessorArgument(parameter.Name, parameter.ParameterType);
                arguments[i] = pad;
            }

            return arguments;
        }

        internal IEnumerable<ProcessorArgument> BuildOutputArgumentCollection()
        {
            ProcessorArgument arg = new ProcessorArgument(this.processorType.Name + "Result", this.OutputArgumentType);
            return new ProcessorArgument[] { arg };
        }

        private static Type GetProcessorBaseType(Type processorType)
        {
            Debug.Assert(processorType != null, "The 'processorType' parameter should not be null.");

            for (Type t = processorType; t != typeof(object); t = t.BaseType)
            {
                if (t == ProcessorType)
                {
                    return t;
                }

                if (t.IsGenericType && t.BaseType == ProcessorType)
                {
                    return t;
                }
            }

            Debug.Fail("Should have found a Processor base class");
            return processorType;
        }

        private static MethodInfo FindExecuteMethod(Type processorType, Type outputArgumentType, params Type[] inputArgumentTypes)
        {
           return processorType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(m => 
                    String.Equals("OnExecute", m.Name, StringComparison.OrdinalIgnoreCase) &&
                    DoesMethodMatch(m, outputArgumentType, inputArgumentTypes)).FirstOrDefault();
        }

        private static bool DoesMethodMatch(MethodInfo method, Type outputArgumentType, Type[] inputArgumentTypes)
        {
            if (!typeof(ProcessorResult).IsAssignableFrom(method.ReturnType))
            {
                return false;
            }

            if (method.ReturnType.IsGenericType)
            {
                Type genericType = method.ReturnType.GetGenericArguments()[0];
                if (genericType != outputArgumentType)
                {
                    return false;
                }
            }

            ParameterInfo[] methodParameters = method.GetParameters();
            if (methodParameters.Length != inputArgumentTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < methodParameters.Length; ++i)
            {
                if (!methodParameters[i].ParameterType.IsAssignableFrom(inputArgumentTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private MethodInfo FindExecuteMethod()
        {
            return FindExecuteMethod(this.processorType, this.OutputArgumentType, this.InputArgumentTypes);
        }
    }
}