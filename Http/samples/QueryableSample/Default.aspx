<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="QueryableSample.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body style="height: 608px">
    <form id="form1" runat="server">
    <div>
    
    </div>
    <asp:Label ID="Label1" runat="server" Text="Welcome to Queryable Sample" 
        style="font-weight: 700"></asp:Label>
    <p>
        <asp:Button ID="GetAllContacts" runat="server" onclick="GetAllContacts_Click" 
            Text="Get All Contacts" Width="136px" />
        <asp:TextBox ID="TextBox1" runat="server" Width="291px"></asp:TextBox>
    </p>
    <p>
        <asp:Button ID="GetTop3" runat="server" onclick="GetTop3_Click" 
            Text="Get Top 3 Contacts" Width="135px" />
        <asp:TextBox ID="TextBox2" runat="server" Width="291px"></asp:TextBox>
    </p>
    <p>
        <asp:Button ID="PostNewContact" runat="server" onclick="PostNewContact_Click" 
            Text="Post new Contact" Width="135px" />
        <asp:TextBox ID="TextBox3" runat="server" Width="291px">Please enter the Name here</asp:TextBox>
    </p>
    <p>
        <asp:Button ID="GetID5" runat="server" onclick="GetId5_Click" 
            Text="Get 5th contact" Width="135px" />
        <asp:TextBox ID="TextBox4" runat="server" Width="291px"></asp:TextBox>
    </p>
    <p>
        <asp:Button ID="GetID6" runat="server" onclick="GetId6_Click" 
            Text="Get contact with ID = " Width="135px" />
        <asp:TextBox ID="TextBox5" runat="server" Width="291px">Please enter an integer here</asp:TextBox>
    </p>
    <asp:TextBox ID="Result" runat="server" Height="117px" TextMode="MultiLine" 
        Width="530px"></asp:TextBox>
    </form>
</body>
</html>
