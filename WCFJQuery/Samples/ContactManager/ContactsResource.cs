// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Json;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    public class ContactsResource
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["ContactManagerConnectionString"].ConnectionString;

        public static dynamic SqlDataReaderToJsonValue(SqlDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            List<JsonObject> rows = new List<JsonObject>();
            while (reader.Read())
            {
                JsonObject row = new JsonObject();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = Convert.ToString(reader.GetValue(i), CultureInfo.InvariantCulture);
                }

                rows.Add(row);
            }   

            if (rows.Count == 0)
            {
                return null;
            }
            else if (rows.Count == 1)
            {
                return rows[0];
            }
            else
            {
                return new JsonArray(rows);
            }
        }

        [WebGet(UriTemplate = "{id}")]
        public JsonValue Get(string id)
        {
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                using (SqlCommand getSingle = new SqlCommand("SELECT * FROM Contact WHERE ContactID = @id", sc))
                {
                    getSingle.Parameters.AddWithValue("@id", id);
                    dynamic result = SqlDataReaderToJsonValue(getSingle.ExecuteReader());

                    if (result == null)
                    {
                        throw new WebFaultException<string>("Contact not found", HttpStatusCode.NotFound);
                    }
                    else
                    {
                        result.Self = "contacts/" + (string)result.ContactID;
                        
                        result.Remove("ContactID"); // Don't need to return this to the user
                    }

                    return result;
                }
            }
        }

        [WebGet(UriTemplate = "")]
        public JsonValue GetAll()
        {
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                using (SqlCommand getAll = new SqlCommand("SELECT Name, 'contacts/' + Convert(nvarchar,ContactID) as Self FROM Contact ORDER BY Name", sc))
                {
                    return SqlDataReaderToJsonValue(getAll.ExecuteReader());
                }
            }
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public JsonValue Post(JsonValue contact)
        {
            dynamic input = contact;

            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                using (SqlCommand post = new SqlCommand("INSERT INTO Contact (Name, Address, City, State, Zip, Email, Twitter) VALUES (@name, @address, @city, @state, @zip, @email, @twitter)", sc))
                {
                    post.Parameters.AddWithValue("@name", (string)input.Name);
                    post.Parameters.AddWithValue("@address", (string)input.Address);
                    post.Parameters.AddWithValue("@city", (string)input.City);
                    post.Parameters.AddWithValue("@state", (string)input.State);
                    post.Parameters.AddWithValue("@zip", (string)input.Zip);
                    post.Parameters.AddWithValue("@email", (string)input.Email);
                    post.Parameters.AddWithValue("@twitter", (string)input.Twitter);
                    post.ExecuteNonQuery();
                }

                using (SqlCommand count = new SqlCommand("SELECT @@IDENTITY", sc))
                {
                    return this.Get(count.ExecuteScalar().ToString());
                }
            }
        }

        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public JsonValue Update(string id, JsonValue contact)
        {
            this.Get(id);
            dynamic input = contact;

            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                using (SqlCommand update = new SqlCommand("UPDATE Contact SET Name=@name, Address=@address, City=@city, State=@state, Zip=@zip, Email=@email, Twitter=@twitter WHERE ContactID=@id", sc))
                {
                    update.Parameters.AddWithValue("@name", (string)input.Name);
                    update.Parameters.AddWithValue("@address", (string)input.Address);
                    update.Parameters.AddWithValue("@city", (string)input.City);
                    update.Parameters.AddWithValue("@state", (string)input.State);
                    update.Parameters.AddWithValue("@zip", (string)input.Zip);
                    update.Parameters.AddWithValue("@email", (string)input.Email);
                    update.Parameters.AddWithValue("@twitter", (string)input.Twitter);
                    update.Parameters.AddWithValue("@id", id);
                    update.ExecuteNonQuery();
                }
            }

            return this.Get(id);
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public JsonValue Delete(string id)
        {
            dynamic deleted = this.Get(id);

            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (SqlCommand delete = new SqlCommand("DELETE FROM Contact WHERE ContactID=@id", sc))
                {
                    delete.Parameters.AddWithValue("@id", id);
                    delete.ExecuteNonQuery();
                }
            }

            return deleted;
        }
    }
}
