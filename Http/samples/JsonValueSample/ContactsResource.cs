// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace JsonValueSample
{
    using System.Json;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public class ContactsResource
    {
        private static int nextId = 1;

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public JsonValue Post(JsonValue contact)
        {
            var postedContact = (dynamic)contact;
            var contactResponse = (dynamic)new JsonObject();
            contactResponse.Name = postedContact.Name;
            contactResponse.ContactId = nextId++;
            return contactResponse;
        }
    }
}