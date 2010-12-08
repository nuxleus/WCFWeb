// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "Strong name signing not required for codeplex.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "ContactManager.ContactsResource.#Post(System.Json.JsonValue)", Justification = "Used in a using statement.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "ContactManager.ContactsResource.#Update(System.String,System.Json.JsonValue)", Justification = "Used in a using statement.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "ContactManager.ContactsResource.#GetAll()", Justification = "Service contract implementation.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "ContactManager.ContactsResource.#Get(System.String)", Justification = "Service contract implementation.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "ContactManager.ContactsResource.#GetAll()", Justification = "Service contract implementation.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "ContactManager.ContactsResource.#Delete(System.String)", Justification = "Service contract implementation.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID", Scope = "member", Target = "ContactManager.Contact.#ContactID", Justification="This is LINQ to SQL generated code.")]
