// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager
{
    using System.Configuration;
    using System.Globalization;
    using System.Json;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    public class ContactsResource
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["ContactManagerConnectionString"].ConnectionString;

        [WebGet(UriTemplate = "{id}")]
        public JsonValue Get(string id)
        {
            JsonObject result = JsonValueExtensions.CreateFrom(GetType(id)) as JsonObject;
            result["Self"] = "contacts/" + result["ContactID"];
            result.Remove("ContactID"); // Don't need to return this to the user
            return result;
        }

        [WebGet(UriTemplate = "")]
        public JsonValue GetAll()
        {
            using (var context = new ContactsDataContext(connectionString))
            {
                // For some reason a lambda doesn't work here but a delegate does
                var result = context.Contacts.OrderBy<Contact, string>(x => x.Name).Select<Contact, JsonObject>(
                    delegate(Contact x) 
                    { 
                        return new JsonObject 
                        { 
                            { "Name", x.Name }, { "Self", "contacts/" + x.ContactID }
                        };
                    });
                return new JsonArray(result);
            }
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public JsonValue Post(JsonValue contact)
        {
            Contact added = contact.ReadAsType<Contact>();
            
            using (var context = new ContactsDataContext(connectionString))
            {
                context.Contacts.InsertOnSubmit(added);
                context.SubmitChanges();
                context.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, added);
            }

            return this.Get(added.ContactID.ToString(CultureInfo.InvariantCulture));
        }

        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public JsonValue Update(string id, JsonValue contact)
        {
            Contact original = GetType(id);
            Contact updated = contact.ReadAsType<Contact>();

            using (var context = new ContactsDataContext(connectionString))
            {
                context.Contacts.Attach(updated, original);
                context.SubmitChanges();
            }

            return this.Get(id);
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public JsonValue Delete(string id)
        {
            Contact deleted = GetType(id);

            using (var context = new ContactsDataContext(connectionString))
            {
                context.Contacts.Attach(deleted);
                context.Contacts.DeleteOnSubmit(deleted);
                context.SubmitChanges();
            }

            return JsonValueExtensions.CreateFrom(deleted);
        }

        private static Contact GetType(string id)
        {
            Contact result;
            using (var context = new ContactsDataContext(connectionString))
            {
                result = context.Contacts.Where<Contact>(x => x.ContactID == int.Parse(id)).FirstOrDefault();
            }

            if (result == null)
            {
                throw new WebFaultException<string>("Contact not found", HttpStatusCode.NotFound);
            }

            return result;
        }
    }
}
