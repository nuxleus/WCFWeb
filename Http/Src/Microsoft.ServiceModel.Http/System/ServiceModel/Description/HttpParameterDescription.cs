// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Description
{
    /// <summary>
    /// Represents the description of input parameters, output parameters,
    /// or return values for an <see cref="HttpOperationDescription"/>.
    /// </summary>
    public class HttpParameterDescription
    {
        private MessagePartDescription messagePartDescription;
        private string name;
        private string namespaceName;
        private Type type;
        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpParameterDescription"/> class.
        /// </summary>
        /// <remarks>
        /// The instance created using this constructor will be empty and will need to be
        /// configured via its public properties.   To create an instance based on an existing
        /// <see cref="MessagePartDescription"/> use the <see cref="HttpParmeterDescriptionsExtensionMethods.ToHttpParameterDescription"/>
        /// </remarks>
        public HttpParameterDescription()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpParmeterDescription"/> class.
        /// </summary>
        /// <remarks>
        /// This form of the constructor creates an instance based on an existing
        /// <see cref="MessagePartDescription"/> instance.  To create such an instance, use the extension method
        /// <see cref="HttpParmeterDescriptionsExtensionMethods.ToHttpParameterDescription"/>.
        /// </remarks>
        /// <param name="messagePartDescription">The existing <see cref="MessagePartDescription"/>.</param>
        internal HttpParameterDescription(MessagePartDescription messagePartDescription)
        {
            if (messagePartDescription == null)
            {
                throw new ArgumentNullException("messagePartDescription");
            }

            this.messagePartDescription = messagePartDescription;
        }

        /// <summary>
        /// Gets or sets the relative (zero-based) index of the current instance.
        /// </summary>
        public int Index
        {
            get
            {
                return (this.messagePartDescription != null) ? this.messagePartDescription.Index : this.index;
            }

            set
            {
                if (this.messagePartDescription != null)
                {
                    this.messagePartDescription.Index = value;
                }
                else
                {
                    this.index = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the current instance.
        /// </summary>
        public string Name
        {
            get
            {
                return (this.messagePartDescription != null) ? this.messagePartDescription.Name : this.name;
            }

            set
            {
                if (this.messagePartDescription != null)
                {
                    throw new NotSupportedException(SR.HttpParameterDescriptionNameImmutable);
                }
                else
                {
                    this.name = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the namespace of the current instance.
        /// </summary>
        public string Namespace
        {
            get
            {
                return (this.messagePartDescription != null) ? this.messagePartDescription.Namespace : this.namespaceName;
            }

            set
            {
                if (this.messagePartDescription != null)
                {
                    throw new NotSupportedException(SR.HttpParameterDescriptionNamespaceImmutable);
                }
                else
                {
                    this.namespaceName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the current instance.
        /// </summary>
        public Type ParameterType
        {
            get
            {
                return (this.messagePartDescription != null) ? this.messagePartDescription.Type : this.type;
            }

            set
            {
                if (this.messagePartDescription != null)
                {
                    this.messagePartDescription.Type = value;
                }
                else
                {
                    this.type = value;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="MessagePartDescription"/> used to construct this instance
        /// if it is available.
        /// </summary>
        internal MessagePartDescription MessagePartDescription
        {
            get
            {
                return this.messagePartDescription;
            }
        }
    }
}
