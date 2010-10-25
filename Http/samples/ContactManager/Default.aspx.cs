// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager
{
    using System;
    using System.Linq;

    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var repository = new ContactRepository();
            this.Repeater1.DataSource = repository.GetAll().OrderBy(c => c.Name);
            this.Repeater1.DataBind();
        }
    }
}