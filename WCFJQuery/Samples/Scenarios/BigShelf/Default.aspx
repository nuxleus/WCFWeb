﻿<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="BigShelf._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <title>Big Shelf - Read books, share books.</title>
    <!-- Frameworks -->
    <script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.4.min.js" type="text/javascript"></script>
    <!-- jquery plugins -->
    <script src="Scripts/jquery.tmpl.js" type="text/javascript"></script>
    <script src="Scripts/jquery.datalink.js" type="text/javascript"></script>
    <script src="Scripts/jquery.bb.watermark.min.js" type="text/javascript"></script>
    <!-- jquery ui -->
    <script src="http://ajax.microsoft.com/ajax/jquery.ui/1.8.6/jquery-ui.js" type="text/javascript"></script>
    <!-- Client library code -->
    <script src="Scripts/jquery.array.js" type="text/javascript"></script>
    <script src="Scripts/jquery.datasource.js" type="text/javascript"></script>
    <script src="Scripts/jquery.pager.js" type="text/javascript"></script>
    <script src="Scripts/jquery.starrating.js" type="text/javascript"></script>
    <script src="Scripts/DomainServiceProxy.js" type="text/javascript"></script>
    <script src="Scripts/EntitySet.js" type="text/javascript"></script>
    <script src="Scripts/DataContext.js" type="text/javascript"></script>
    <script src="Scripts/DataSource.js" type="text/javascript"></script>
    <script src="Scripts/LocalDataSource.js" type="text/javascript"></script>
    <script src="Scripts/RemoteDataSource.js" type="text/javascript"></script>
    <script src="Scripts/AssociatedEntitiesDataSource.js" type="text/javascript"></script>
    <script src="Scripts/ListControl.js" type="text/javascript"></script>
    <!-- Page Scripts -->
    <script src="Scripts/Default.js" type="text/javascript"></script>
    <!-- Templates -->
    <script id="friendsListTemplate" type="text/x-jquery-tmpl">
        <td class="friendButton selected">${get_FriendProfile().Name}</td>
        <td width='10px'></td>
    </script>
    <script id="bookTemplate" type="text/x-jquery-tmpl">
        <tr class='book-item'>
            <td valign="top"><img src="${'http://images.amazon.com/images/P/' + ASIN + '.01._AA100_PU_PU-5_.jpg'}" /></td>
            <td width="4px"></td>
            <td valign="top">
                <table>
                    <tr>
                        <td class="book-title">${Title}</td>
                    </tr>
                    <tr>
                        <td class="book-author">by ${Author}</td>
                    </tr>
                    <tr>
                        <td class="star-rating">
                            <select name="Rating">
                                <option value="0"></option>
                                <option value="1">1 star</option>
                                <option value="2">2 star</option>
                                <option value="3">3 star</option>
                                <option value="4">4 star</option>
                                <option value="5">5 star</option>
                            </select>
                        </td>
                    </tr>
                    <tr class="save-button">
                        <td><input type="button" name="status" value="Save" class="book-notadded"/></td>
                    </tr>
                </table>
            </td>
        </tr>
    </script>
    <script id="pageNumberTemplate" type="text/x-jquery-tmpl">
        <td class="pagerButton">${$item.pageNumber * $item.pageSize + 1}-${($item.pageNumber + 1) * $item.pageSize}</td>
        <td width='10px'></td>
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:LoginView ID="LoginView1" runat="server" EnableViewState="false">
        <AnonymousTemplate>
            <div class="anonymousContent">
                Welcome to Big Shelf! Login first to access your personalized shelf.
            </div>
        </AnonymousTemplate>
        <LoggedInTemplate>
    <div class="catalogFilter" id="booksFilter">
        <table border="0">
            <tr>
                <td style="color:#999999; padding: 4px 4px 4px 4px">Show Me</td>
                <td width="4px"></td>
                <td class="filterButton">All</td>
                <td width="4px"></td>
                <td class="filterButton">My books</td>
                <td width="4px"></td>
                <td class="filterButton">Just friends</td>
            </tr>
        </table>
    </div>

    <div class="friendsList">
        <table border="0">
            <tr id="friendsList" style="display: none">
                <td style="color:#999999; padding: 4px 4px 4px 4px">Show Friends</td>
                <td width="4px"></td>
            </tr>
        </table>
    </div>

    <div class="search"><input id="searchBox" type="text" size="25" title="search books..." /></div>

    <div class="sortBy" id="sortBy">
        <table border="0">
            <tr>
                <td style="color:#999999; padding: 4px 4px 4px 4px">Sort By</td>
                <td width="4px"></td>
                <td class="sortButton">Title</td>
                <td width="4px"></td>
                <td class="sortButton">Author</td>
                <td width="4px"></td>
                <td class="sortButton">Rating</td>
                <td width="4px"></td>
                <td class="sortButton">Might Read</td>
            </tr>
        </table>
    </div>

    <div class="catalog">
        <table border="0">
            <tbody id="books"></tbody>
        </table>
    </div>

    <div class="catalogNavigation">
        <table border="0">
            <tr id="pager"></tr>
        </table>
    </div>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
