// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Represents an input or output argument of a <see cref="Processor"/>.
    /// </summary>
    public sealed class ProcessorArgument
    {
        internal static readonly Type NullableType = typeof(Nullable<>);

        private string name;
        private Type type;
        private ProcessorArgumentCollection containingCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorArgument"/> class
        /// with the specified <paramref name="name"/>, <paramref name="type"/> and 
        /// optional <paramref name="properties"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ProcessorArgument"/>.</param>
        /// <param name="type">The type of the <see cref="ProcessorArgument"/>.</param>
        /// <param name="properties">Optional properties associated with the <see cref="ProcessorArgument"/>.</param>
        public ProcessorArgument(string name, Type type, params object[] properties)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            this.EnsureValidName(name);
            this.Name = name;
            this.ArgumentType = type;
            this.Properties = new KeyedByTypeCollection<object>(properties);
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="ProcessorArgument"/>.
        /// </summary>
        /// <remarks>
        /// The name can not be null, an empty string or only whitespace characters.
        /// If the <see cref="ProcessorArgument"/> has been added to a <see cref="ProcessorArgumentCollection"/>
        /// the name of the <see cref="ProcessorArgument"/> must be unique (case-sensitive)
        /// amongst all of the <see cref="ProcessorArgument">ProcessorArguments</see> in the
        /// <see cref="ProcessorArgumentCollection"/>.
        /// </remarks>
        public string Name 
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.EnsureValidName(value);
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the <see cref="ProcessorArgument"/>.
        /// </summary>
        public Type ArgumentType
        {
            get
            {
                return this.type;
            }

            set
            {
                EnsureValidType(value);
                this.type = value;
            }
        }

        /// <summary>
        /// Gets the properties associated with the <see cref="ProcessorArgument"/>.
        /// </summary>
        public KeyedByTypeCollection<object> Properties { get; private set; }

        /// <summary>
        /// Gets the index of the <see cref="ProcessorArgument"/> within the <see cref="ProcessorArgumentCollection"/>
        /// if the <see cref="ProcessorArgument"/> has been added to a <see cref="ProcessorArgumentCollection"/>;
        /// otherwise returns null.
        /// </summary>
        public int? Index { get; internal set; }

        /// <summary>
        /// Gets the <see cref="ProcessorArgumentCollection"/> to which the <see cref="ProcessorArgument"/> has been
        /// added.  If the <see cref="ProcessorArgument"/> has not been added to a <see cref="ProcessorArgumentCollection"/>
        /// returns null.
        /// </summary>
        public ProcessorArgumentCollection ContainingCollection
        {
            get
            {
                return this.containingCollection;
            }

            internal set
            {
                if (value != null && this.containingCollection != null && this.containingCollection != value)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            SR.ProcessorArgumentAlreadyBelongsToProcessorArgumentCollection,
                            this.Name));
                }

                this.containingCollection = value;
            }
        }

        /// <summary>
        /// Creates a copy of the current instance.
        /// </summary>
        /// <remarks>
        /// A <see cref="ProcessorArgument"/> may belong to only one 
        /// <see cref="ProcessorArgumentCollection"/>.  This copy method
        /// creates a new <see cref="ProcessorArgument"/> instance not
        /// associated with any existing collection.
        /// </remarks>
        /// <returns>A new <see cref="ProcessorArgument"/> instance with the same
        /// <see cref="Name"/>, <see cref="ProcessorType"/>, and <see cref="Properties"/> 
        /// as the current instance</returns>
        internal ProcessorArgument Copy()
        {
            int propertiesCount = this.Properties.Count;
            if (propertiesCount > 0)
            {
                object[] copiedProperties = new object[propertiesCount];
                for (int i = 0; i < propertiesCount; i++)
                {
                    ICloneable cloneable = this.Properties[i] as ICloneable;
                    copiedProperties[i] = cloneable != null ?
                        cloneable.Clone() : this.Properties[i];
                }

                return new ProcessorArgument(this.Name, this.ArgumentType, copiedProperties);
            }

            return new ProcessorArgument(this.Name, this.ArgumentType);
        }

        private static void EnsureValidType(Type argumentType)
        {
            Debug.Assert(argumentType != null, "The 'type' parameter should not be null.");

            if (argumentType.IsGenericType)
            {
                Type genericType = argumentType.GetGenericTypeDefinition();
                if (genericType == NullableType)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            SR.ProcessorArgumentTypeCannotBeNullable,
                            argumentType.GetGenericArguments()[0].Name));
                }
            }
        } 

        private void EnsureValidName(string argumentName)
        {
            Debug.Assert(argumentName != null, "The 'name' parameter should not be null.");

            if (string.IsNullOrWhiteSpace(argumentName))
            {
                throw new InvalidOperationException(
                    SR.ProcessorArgumentNameCannotBeEmptyStringOrWhitespace);
            }

            if (this.ContainingCollection != null)
            {
                ProcessorArgument argument = this.ContainingCollection[argumentName];
                if (argument != null && argument != this)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            SR.ProcessorArgumentWithSameName,
                            argumentName,
                            argument.Index));
                }
            }
        }
    }
}