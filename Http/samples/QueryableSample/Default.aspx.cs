// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace QueryableSample
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel.Http.Client;
    using Microsoft.Http;

    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // Get all the contacts
        protected void GetAllContacts_Click(object sender, EventArgs e)
        {
            string address = "http://localhost:8081/contacts/";
            HttpClient client = new HttpClient(address);
            var contacts = client.CreateQuery<Contact>();

            string result = "Received contacts:";
            foreach (Contact contact in contacts)
            {
                result += string.Format(CultureInfo.InvariantCulture, "\r\n Contact Name: {0}, Contact Id: {1}", contact.Name, contact.Id);
            }

            this.TextBox1.Text = contacts.RequestUri.ToString();

            this.Result.Text = result;
        }

        protected void GetTop3_Click(object sender, EventArgs e)
        {
            string address = "http://localhost:8081/contacts/";
            HttpClient client = new HttpClient(address);
            WebQuery<Contact> contacts = client.CreateQuery<Contact>();

            var top3 = contacts.Take<Contact>(3);

            string result = "Received top 3 contacts:";
            foreach (Contact contact in contacts.Take<Contact>(3))
            {
                result += string.Format(CultureInfo.InvariantCulture, "\r\n Contact Name: {0}, Contact Id: {1}", contact.Name, contact.Id);
            }

            this.TextBox2.Text = ((WebQuery<Contact>)top3).RequestUri.AbsoluteUri;

            this.Result.Text = result;
        }

        protected void PostNewContact_Click(object sender, EventArgs e)
        {
            string address = "http://localhost:8081/contacts/";
            HttpClient client = new HttpClient(address);
            var contact = new Contact { Name = this.TextBox3.Text, Id = 5 };
            var serializer = new DataContractSerializer(typeof(Contact));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, contact);
            stream.Position = 0;

            var request = new HttpRequestMessage("Post", new Uri("http://localhost:8081/contacts/"));
            request.Content = HttpContent.Create(stream);
            request.Headers.ContentType = "application/xml";
            var response = client.Send(request);
            var receivedContact = serializer.ReadObject(response.Content.ReadAsStream()) as Contact;

            this.Result.Text = string.Format(CultureInfo.InvariantCulture, "\r\n Contact Name: {0}, Contact Id: {1}", receivedContact.Name, receivedContact.Id);
        }

        protected void GetId5_Click(object sender, EventArgs e)
        {
            string address = "http://localhost:8081/contacts/";
            HttpClient client = new HttpClient(address);
            WebQuery<Contact> contacts = client.CreateQuery<Contact>();

            var contact = contacts.Skip<Contact>(4).Take<Contact>(1);

            string result = string.Empty;
            List<Contact> finalList = contact.ToList<Contact>();

            if (finalList.Count == 0)
            {
                result = "There are less than 5 contacts";
            }
            else
            {
                result = "Get the 5th contact: ";
                result += string.Format(CultureInfo.InvariantCulture, "\r\n Contact Name: {0}, Contact Id: {1}", finalList[0].Name, finalList[0].Id);
            }

            this.TextBox4.Text = ((WebQuery<Contact>)contact).RequestUri.AbsoluteUri;

            this.Result.Text = result;
        }

        protected void GetId6_Click(object sender, EventArgs e)
        {
            string address = "http://localhost:8081/contacts/";
            HttpClient client = new HttpClient(address);
            WebQuery<Contact> contacts = client.CreateQuery<Contact>();

            int input = 0;
            try
            {
                input = Convert.ToInt32(this.TextBox5.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                this.TextBox5.Text = "Error: please enter an integer";
                return;
            }

            var resultList = contacts.Where<Contact>(contact => contact.Id == input);

            string result = string.Empty;
            List<Contact> finalList = resultList.ToList<Contact>();

            if (finalList.Count == 0)
            {
                result = "There is no contact with ID = " + input;
            }
            else
            {
                result = "Found a matched contact: ";
                result += string.Format(CultureInfo.InvariantCulture, "\r\n Contact Name: {0}, Contact Id: {1}", finalList[0].Name, finalList[0].Id);
            }

            this.TextBox4.Text = ((WebQuery<Contact>)resultList).RequestUri.AbsoluteUri;

            this.Result.Text = result;
        }
    }
}
