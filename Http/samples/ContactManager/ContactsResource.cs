// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using System.Net.Http;

    [ServiceContract]
    [Export]
    public class ContactsResource
    {
        private readonly IContactRepository repository;

        [ImportingConstructor]
        public ContactsResource(IContactRepository repository)
        {
            this.repository = repository;
        }

        [WebGet(UriTemplate = "")]
        public List<Contact> Get()
        {
            return this.repository.GetAll();
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public Contact Post(Contact contact, HttpResponseMessage response)
        {
            this.repository.Post(contact);
            response.StatusCode = HttpStatusCode.Created;
            return contact;
        }
    }
}