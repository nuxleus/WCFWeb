/// <reference path="json2.js" />
/// <reference path="jquery/jquery-1.4.4.js" />
/// <reference path="jquery/ui/jquery-ui.js" />
/// <reference path="jquery.array.js" />
/// <reference path="DomainServiceProxy.js" />
/// <reference path="DataContext.js" />
/// <reference path="EntitySet.js" />
/// <reference path="DataSource.js" />
/// <reference path="LocalDataSource.js" />
/// <reference path="RemoteDataSource.js" />
/// <reference path="jquery.datasource.js" />
/// <reference path="ListControl.js" />

$(document).ready(function () {

    var serviceUrl = "BigShelf-BigShelfService.svc";
    var pageSize = 6;

    var profiles = [];
    var profile;
    $([profiles]).dataSource({
        serviceUrl: serviceUrl,
        queryName: "GetProfileForSearch",
        refresh: function () {
            profile = profiles[0];
            render();
        }
    }).refresh();

    function render() {
        var remoteBooks, localBooks, books, remoteBooksQueryParameters = {}, booksList;
        var View = {
            All: 0,
            MyBooks: 1,
            JustFriends: 2
        }, currentView;
        var currentSort;

        //
        // "Show me..." nav bar
        //
        $(".filterButton").click(function () {
            var title = $(this).text();

            var $this = $(this);
            $(".filterButton").not($this).removeClass("selected");
            $this.addClass("selected");

            // Friends filter only valid for "Just friends" view.
            $("#friendsList")[title === "Just friends" ? "show" : "hide"]();

            // Transition between views.
            if (title === "My books") {
                if (currentView !== View.MyBooks) {
                    currentView = View.MyBooks;

                    // "My books" loads your entire book list and uses local querying.
                    establishNewDataSource(/* useRemoteQuery: */false);
                    refreshBooksList(true);
                }
            } else {
                var needRefresh;
                if (currentView !== View.All && currentView !== View.JustFriends) {
                    // "All" and "Just friends" book lists could be of arbitrary size.  Use remote querying.
                    establishNewDataSource(/* useRemoteQuery: */true);
                    needRefresh = true;
                } else if (currentView === View.JustFriends ^ title === "Just friends") {
                    // If we're already using a remote data source, we only have to refresh
                    // if we've toggled between "All" and "Just friends".
                    needRefresh = true;
                }

                currentView = title === "Just friends" ? View.JustFriends : View.All;

                if (needRefresh) {
                    refreshBooksList();
                }
            }
        });


        //
        // Friends filter
        //
        $.each(profile.get_Friends(), function () {
            $("#friendsListTemplate").tmpl(this).appendTo($("#friendsList")).eq(0)
            .filter(".friendButton")
            .data("friendId", this.FriendId)
            .click(function () {
                var wasChecked = $(this).hasClass("selected");
                $(this).toggleClass("selected", !wasChecked);
                refreshBooksList();
            });
        });


        $(".sortButton").click(function () {
            var newSort = $(this).text();
            var $currentSortElement = $(".sortButton.selected");
            var currentSort = $currentSortElement.text();
            if (newSort !== currentSort) {
                $currentSortElement.removeClass("selected");
                $(this).addClass("selected");

                $("#pager").pager("setPage", { page: 0, refresh: false });
                refreshBooksList();
            }
        })
        .eq(0).addClass("selected");


        // Trigger a click on "All" to initialize our UI and fetch our initial page of Book data.
        currentView = View.MyBooks;
        $(".filterButton").eq(0).click();


        //
        // Helper functions
        //

        function establishNewDataSource(useRemoteQuery) {
            disposeOldData();
            if (useRemoteQuery) {
                establishDataSourceForRemoteQuery();
            } else {
                establishDataSourceForLocalQuery();
            }
            configureCurrentDataSource();
            renderCore();
        };

        function disposeOldData() {
            if (remoteBooks) {
                $([remoteBooks]).dataSource().destroy();
            }
            if (localBooks) {
                $([localBooks]).dataSource().destroy();
            }
        };

        function establishDataSourceForRemoteQuery() {
            remoteBooks = [];
            $([remoteBooks]).dataSource({
                serviceUrl: serviceUrl,
                queryName: "GetBooksForSearch",
                queryParameters: remoteBooksQueryParameters,
                dataContext: $([profiles]).dataSource().dataContext()
            });
            localBooks = null;

            books = remoteBooks;
        };

        function establishDataSourceForLocalQuery() {
            establishDataSourceForRemoteQuery();

            localBooks = [];
            $([localBooks]).dataSource({
                inputData: remoteBooks
            });

            books = localBooks;
        };

        function configureCurrentDataSource() {
            $([books]).dataSource()
                .option("refreshing", function () {
                    $("#pager, #books, #friendsList, #booksFilter, #sortBy").attr("disabled", "disabled");
                })
                .option("refresh", function () {
                    $("#pager, #books, #friendsList, #booksFilter, #sortBy").removeAttr("disabled");
                });
        };

        function refreshBooksList(refreshAll) {
            var dataSource = $([books]).dataSource();

            // Grab our search string.
            var titleSubstring = $("#searchBox").val() || "";
            dataSource.option("filter", {
                property: "Title", operator: "Contains", value: titleSubstring
            });

            // Grab the Profile id's for which we're querying Books.
            switch (currentView) {
                case View.All:
                    remoteBooksQueryParameters.profileIds = null;
                    break;

                case View.MyBooks:
                    remoteBooksQueryParameters.profileIds = [profile.Id].toString();
                    break;

                case View.JustFriends:
                    // Pluck our query parameters from the friends filter.
                    remoteBooksQueryParameters.profileIds = $(".friendButton.selected").map(function () {
                        return $(this).data("friendId");
                    }).toArray().toString();
                    break;
            }

            // Determine sort.  Unfortunately, the sort we use for "Rating" and "Might Read" is more complex than
            // a simple {property, direction}-pair, so we have to treat the local and remote querying cases separately.
            var currentSort = $(".sortButton.selected").text();
            if (localBooks) {
                var sort;
                switch (currentSort) {
                    case "Title":
                    case "Author":
                        sort = { property: currentSort, direction: "ascending" };  // TODO -- Make direction selectable throughout.
                        break;

                    case "Rating":
                        sort = getSortFunction(function getRating(book) {
                            var flaggedBook = getFlaggedBook(book);
                            return !flaggedBook ? -1 : (flaggedBook.IsFlaggedToRead ? 0 : flaggedBook.Rating);
                            // Put not-flagged books at the end of our sorted list by giving them a weighting of -1.
                            // Put flagged-to-read books (not yet rated) after all rated books by giving them a weighting of 0.
                        });
                        break;

                    case "Might Read":
                        sort = getSortFunction(function (book) {
                            var flaggedBook = getFlaggedBook(book);
                            return !flaggedBook ? -1 : (flaggedBook.IsFlaggedToRead ? 6 : flaggedBook.Rating);
                            // Put not-flagged books at the end of our sorted list by giving them a weighting of -1.
                            // Put flagged-to-read books at the top of our sorted list by giving them a weighting of 6.
                        });
                        break;
                }
                $([localBooks]).dataSource().option("sort", sort);

                function getSortFunction(getWeighting) {
                    return function (book1, book2) {
                        var weighting1 = getWeighting(book1), weighting2 = getWeighting(book2),
                            sortAscending = false,  // TODO -- Make direction selectable throughout.
                            result = weighting1 === weighting2 ? 0 : (weighting1 > weighting2 ? 1 : -1);
                        return sortAscending ? result : -result;
                    };
                };
            } else {
                remoteBooksQueryParameters.sort = currentSort;
                remoteBooksQueryParameters.sortAscending = currentSort === "Title" || currentSort === "Author";  // TODO -- Make direction selectable throughout.
            }

            // Refresh our Books data source.
            dataSource.refresh({ all: refreshAll });
        };

        function getFlaggedBook(book) {
            return $.grep(profile.get_FlaggedBooks(), function (myFlaggedBook) {
                return myFlaggedBook.get_Book() === book;
            })[0];
        };

        function renderCore() {

            // Destroy any old UI from previous "Show me..." nav bar selection.
            if (booksList) {
                booksList.dispose();
                $("#books").empty();
            }
            $("#searchBox").autocomplete("destroy");
            $("#pager").pager("destroy");
            // TODO -- Consider adding functionality to ListControl to rebind its input array, rather than destroy/recreate.

            // Create a new list control over "books".
            booksList = new ListControl(books, {
                template: "#bookTemplate",
                container: "#books",
                rowAdded: function (book, bookElement) {
                    linkBookAndBindEditControls(book, bookElement);
                }
            });

            // A "search" text box above the Books list itself.
            $("#searchBox").autocomplete({
                source: function () { refreshBooksList(); },
                minLength: 0
            });

            // A pager over Books.
            $("#pager").pager({
                dataSource: $([books]).dataSource(),
                template: "#pageNumberTemplate",
                currentClass: "selected",
                notCurrentClass: "",
                hoverClass: "hover",
                pageSize: pageSize
            });
            // TODO -- We don't block the UI while we're paging.

            // Flagged books should be disabled in the UI while they're being sync'd with the server.
            $([profile.get_FlaggedBooks()]).dataSource().option("entityStateChange", function (flaggedBook, entityState) {
                var bookElement = booksList.getElement(flaggedBook.get_Book());
                var isSynchronizingBook = entityState === "ServerAdding" || entityState === "ServerUpdating";
                // There's no UI to delete a FlaggedBook (and get to "ServerDeleting" entity state).
                if (isSynchronizingBook) {
                    $(bookElement).attr("disabled", "disabled");
                } else {
                    $(bookElement).removeAttr("disabled");
                }
            });

            function linkBookAndBindEditControls(book, bookElement) {
                var flaggedBook = getFlaggedBook(book),
                    profileHasBook = !!flaggedBook;

                // Create an not-added, new FlaggedBook.  Add it on first click.
                flaggedBook = flaggedBook || { BookId: book.Id };

                // Data-link the <li> to our flagged book, so these stay in sync without having to
                // manually push/pull data changes.
                $(bookElement).link(flaggedBook, {
                    Title: null,
                    Rating: {
                        convertBack: function (value) {
                            // Rating this Book will add an FlaggedBook to the Profile.
                            handleFlaggedBookChange();
                            return value;
                        }
                    }
                });

                $(".star-rating", bookElement).starRatings(flaggedBook.Rating).bind('ratingChanged', function (event, value) {
                    $("[name=Rating]", bookElement).val(value.rating).change();
                    return value.rating;
                });

                // Initial click on "Save" will add an FlaggedBook for this Book when clicked.
                var $button = $("input:button[name='status']", bookElement);
                $button.click(function () { handleFlaggedBookChange(); });

                $(flaggedBook).data("Rating", flaggedBook.Rating);  // TODO -- Replace with "pushValues" in new data-linking.

                function handleFlaggedBookChange() {
                    if (!profileHasBook) {
                        $.push(profile.get_FlaggedBooks(), flaggedBook);
                        profileHasBook = true;
                    }
                    $button
                        .val(flaggedBook.Rating > 0 ? "Done reading" : "Might read it")
                        .addClass(flaggedBook.Rating > 0 ? 'book-read' : 'book-saved')
                        .attr("disabled", "disabled");  // First click on "Save" disables button.
                };
            };
        };
    };

    $("#searchBox").watermark();

    // Workaround IE not firing "changed" until checkboxes/radios lose focus.
    if ($.browser.msie) {
        $("input:checkbox, input:radio").live("click", function () {
            this.blur();
            this.focus();
        });
    }
});
