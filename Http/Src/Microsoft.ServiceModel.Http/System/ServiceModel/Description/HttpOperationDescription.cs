// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Description
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents the description of a contract operation using 
    /// <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see>.
    /// </summary>
    public class HttpOperationDescription
    {
        private OperationDescription operationDescription;
        private Collection<Attribute> attributes;
        private HttpParameterDescriptionCollection inputParameters;
        private HttpParameterDescriptionCollection outputParameters;
        private HttpParameterDescription returnValue;
        private MethodInfo beginMethod;
        private MethodInfo endMethod;
        private MethodInfo syncMethod;
        private KeyedByTypeCollection<IOperationBehavior> behaviors;
        private ContractDescription declaringContract;
        private Collection<Type> knownTypes;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpOperationDescription"/> class.
        /// </summary>
        /// <remarks>This constructor creates an empty instance that must be
        /// populated via its public properties before use.   To create an
        /// instance from an existing <see cref="OperationDescription"/>, use the
        /// extension method <see cref="HttpOperationDescriptionExtensionMethods.ToHttpOperationDescription"/>.
        /// </remarks>
        public HttpOperationDescription()
        {
            Debug.Assert(!this.IsSynchronized, "Default ctor is not synchronized");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpOperationDescription"/> class
        /// using an existing <see cref="OperationDescription"/>.
        /// </summary>
        /// <remarks>
        /// An instance created using this constructor will synchronize changes made to the
        /// instance properties back to the original <see cref="OperationDescription"/>.
        /// </remarks>
        /// <param name="operationDescription">An existing <see cref="OperationDescription"/>
        /// instance on which to base this new <see cref="HttpOperationDescription"/>.</param>
        internal HttpOperationDescription(OperationDescription operationDescription)
        {
            if (operationDescription == null)
            {
                throw new ArgumentNullException("operationDescription");
            }

            this.operationDescription = operationDescription;
            Debug.Assert(this.IsSynchronized, "This ctor must be synchronized");
        }

        /// <summary>
        /// Gets or sets the name of the operation.
        /// </summary>
        /// <remarks>
        /// Attempting to set the name for an instance created from an existing
        /// <see cref="OperationDescription"/> will throw <see cref="NotSupportedException"/>.
        /// </remarks>
        public string Name
        {
            get
            {
                return this.IsSynchronized ? this.operationDescription.Name : this.name;
            }

            set
            {
                if (this.IsSynchronized)
                {
                     throw new NotSupportedException(SR.HttpOperationDescriptionNameImmutable);
                }

                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets the begin method of the asynchronous operation.
        /// </summary>
        public MethodInfo BeginMethod
        {
            get
            {
                return this.IsSynchronized ? this.operationDescription.BeginMethod : this.beginMethod;
            }

            set
            {
                if (this.IsSynchronized)
                {
                    this.operationDescription.BeginMethod = value;
                }
                else
                {
                    this.beginMethod = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the end method of the asynchronous operation.
        /// </summary>
        public MethodInfo EndMethod
        {
            get
            {
                return this.IsSynchronized ? this.operationDescription.EndMethod : this.endMethod;
            }

            set
            {
                if (this.IsSynchronized)
                {
                    this.operationDescription.EndMethod = value;
                }
                else
                {
                    this.endMethod = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the method of the synchronous operation.
        /// </summary>
        public MethodInfo SyncMethod
        {
            get
            {
                return this.IsSynchronized ? this.operationDescription.SyncMethod : this.syncMethod;
            }

            set
            {
                if (this.IsSynchronized)
                {
                    this.operationDescription.SyncMethod = value;
                }
                else
                {
                    this.syncMethod = value;
                }
            }
        }

        /// <summary>
        /// Gets the operation behaviors associated with the operation.
        /// </summary>
        public KeyedByTypeCollection<IOperationBehavior> Behaviors
        {
            get
            {
                if (this.IsSynchronized)
                {
                    return this.operationDescription.Behaviors;
                }
                else
                {
                    if (this.behaviors == null)
                    {
                        this.behaviors = new KeyedByTypeCollection<IOperationBehavior>();
                    }

                    return this.behaviors;
                }
            }
        }

        /// <summary>
        /// Gets or sets the contract to which the operation belongs.
        /// </summary>
        public ContractDescription DeclaringContract
        {
            get
            {
                return this.IsSynchronized ? this.operationDescription.DeclaringContract : this.declaringContract;
            }

            set
            {
                if (this.IsSynchronized)
                {
                    this.operationDescription.DeclaringContract = value;
                }
                else
                {
                    this.declaringContract = value;
                }
            }
        }

        /// <summary>
        /// Gets the known types associated with the operation description.
        /// </summary>
        public Collection<Type> KnownTypes
        {
            get
            {
                if (this.IsSynchronized)
                {
                    return this.operationDescription.KnownTypes;
                }
                else
                {
                    if (this.knownTypes == null)
                    {
                        this.knownTypes = new Collection<Type>();
                    }

                    return this.knownTypes;
                }
            }
        }

        /// <summary>
        /// Gets the custom attributes associated with the operation.
        /// </summary>
        public Collection<Attribute> Attributes
        {
            get
            {
                if (this.IsSynchronized)
                {
                    IEnumerable<Attribute> attrs = (this.operationDescription.SyncMethod != null)
                                   ? this.operationDescription.SyncMethod.GetCustomAttributes(true).Cast<Attribute>()
                                   : (this.operationDescription.BeginMethod != null)
                                       ? this.operationDescription.BeginMethod.GetCustomAttributes(true).Cast<Attribute>()
                                       : Enumerable.Empty<Attribute>();
                    return new Collection<Attribute>(attrs.ToList());
                }

                if (this.attributes == null)
                {
                    this.attributes = new Collection<Attribute>();
                }

                return this.attributes;
            }
        }

        /// <summary>
        ///  Gets or sets the description of the value returned by the operation.
        /// </summary>
        /// <value>
        /// This value may be <c>null</c>.  If the current instance is synchronized
        /// with respect to an <see cref="OperationDescription"/>, it will be modified
        /// to reflect the new value.
        /// </value>
        public HttpParameterDescription ReturnValue
        {
            get
            {
                if (this.IsSynchronized)
                {
                    return ((this.operationDescription.Messages.Count > 1) &&
                            (this.operationDescription.Messages[1].Body.ReturnValue != null))
                                ? new HttpParameterDescription(this.operationDescription.Messages[1].Body.ReturnValue)
                                : null;
                }

                return this.returnValue;
            }

            set
            {
                if (this.IsSynchronized)
                {
                    MessagePartDescription mpd = null;
                    if (value != null)
                    {
                        mpd = value.MessagePartDescription;
                        if (mpd == null)
                        {
                            throw new InvalidOperationException(
                                    SR.HttpParameterDescriptionMustBeSynchronized);
                        }
                    }

                    // Ensure Messages[1] exists, because it holds the return value
                    CreateMessageDescriptionIfNecessary(this.operationDescription, messageIndex: 1);
                    this.operationDescription.Messages[1].Body.ReturnValue = mpd;
                }
                else
                {
                    this.returnValue = value;
                }
            }
        }

        /// <summary>
        /// Gets the collection of input parameters used by this operation.
        /// </summary>
        public HttpParameterDescriptionCollection InputParameters
        {
            get
            {
                if (this.IsSynchronized)
                {
                    return new HttpParameterDescriptionCollection(this.operationDescription, isOutputCollection: false);
                }

                if (this.inputParameters == null)
                {
                    this.inputParameters = new HttpParameterDescriptionCollection();
                }

                return this.inputParameters;
            }
        }

        /// <summary>
        ///  Gets the collection of output parameters used by this operation.
        /// </summary>
        public HttpParameterDescriptionCollection OutputParameters
        {
            get
            {
                if (this.IsSynchronized)
                {
                    return new HttpParameterDescriptionCollection(this.operationDescription, isOutputCollection: true);
                }

                if (this.outputParameters == null)
                {
                    this.outputParameters = new HttpParameterDescriptionCollection();
                }

                return this.outputParameters;
            }
        }

        /// <summary>
        /// Gets a <see cref="OperationDescription"/> instance that corresponds to the current instance.
        /// </summary>
        /// <remarks>
        /// If the current instance was created from an <see cref="OperationDescription"/>,
        /// that instance will be returned.   Otherwise, the value will be <c>null</c>.
        /// Derived classes may override this virtual method to supply an <see cref="OperationDescription"/>.
        /// </remarks>
        protected virtual OperationDescription OperationDescription
        {
            get
            {
                return this.operationDescription;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current instance is synchronized with
        /// respect to an <see cref="OperationDescription"/>.
        /// </summary>
        private bool IsSynchronized
        {
            get
            {
                return this.operationDescription != null;
            }
        }

        /// <summary>
        /// Retrieves an <see cref="OperationDescription"/> that matches the current instance.
        /// </summary>
        /// <remarks>
        /// This method will throw <see cref="NotImplementedException"/> if a matching
        /// <see cref="OperationDescription"/> is not available.
        /// </remarks>
        /// <returns>The <see cref="OperationDescription"/>.   It will not be <c>null</c>.</returns>
        public OperationDescription ToOperationDescription()
        {
            OperationDescription operationDesc = this.OperationDescription;
            if (operationDesc == null)
            {
                throw new NotImplementedException(SR.HttpOperationDescriptionNullOperationDescription);
            }

            return operationDesc;
        }

        /// <summary>
        /// Ensures that a <see cref="MessageDescription"/> exists for the given <paramref name="messageIndex"/>
        /// within the specified <see cref="OperationDescription"/>.
        /// A default one will be created if it does not yet exist.
        /// </summary>
        /// <param name="operationDescription">The <see cref="OperationDescription"/> to check and modify.</param>
        /// <param name="messageIndex">The index within the <see cref="MessageDescriptionCollection"/> that must exist.</param>
        internal static void CreateMessageDescriptionIfNecessary(OperationDescription operationDescription, int messageIndex)
        {
            Debug.Assert(operationDescription != null, "OperationDescription cannot be null");
            Debug.Assert(messageIndex >= 0 && messageIndex <= 1, "MessageIndex must be 0 or 1");

            if (operationDescription.Messages.Count <= messageIndex)
            {
                // Messages[0] is input and must be created in all cases
                if (operationDescription.Messages.Count == 0)
                {
                    operationDescription.Messages.Add(new MessageDescription(string.Empty, MessageDirection.Input));
                }

                // Messages[1] is always output
                if (messageIndex > 0 && operationDescription.Messages.Count <= 1)
                {
                    operationDescription.Messages.Add(new MessageDescription(string.Empty, MessageDirection.Output));
                }
            }

            // Post-condition guarantee
            Debug.Assert(operationDescription.Messages.Count > messageIndex, "Expected Messages[messageIndex] to exist");
        }
    }
}
