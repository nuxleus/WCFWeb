$(function () {
    var serviceUrl = 'BigShelf-BigShelfService.svc';

    var profile;
    var profileDataSource = $.dataSource({
        serviceUrl: serviceUrl,
        queryName: "GetProfileForProfileUpdate",
        bufferChanges: true,
        refresh: function (profiles) {
            profile = profiles[0];
            render();
        }
    }).refresh();

    function render() {

        //
        // Profile <form>
        //

        // Data-link "profile" in the <form> and page title, so our UI updates live.
        $("#profile-properties").link(profile);
        $("#profileTitle").link(profile, {
            Name: {
                convertBack: function (value) {
                    var firstName = value.split(" ")[0];
                    $("#profileName").text(firstName);
                    return firstName;
                }
            }
        });

        //validation
        $("#profile-properties").validate({
            rules: profileDataSource.getEntityValidationRules().rules,
            errorPlacement: function (error, element) {
                error.appendTo(element.closest("tr"));
            }
        });

        // Push values from "profile" into the <form> and into page title.
        // TODO -- Replace with "pushValues" in new data-linking.
        $.each(profile, function (fieldName, value) {
            $("#profileForm input[name='" + fieldName + "']").val(value);
        });
        $(profile).data("Name", profile.Name);

        // Property changes on profile trigger an update of our "property modified" styling.
        $(profile).bind("changeData", function () { updatePropertyAdornments(); });

        // When profile leaves its "Unmodified" state, we'll enable the Save/Cancel buttons.
        // When we complete a save, we'll want to remove our updated scalar property adornments.
        profileDataSource.option("entityStateChange", function () {
            updatePropertyAdornments();
            updateSaveCancelButtonState();
        });

        // Bind the revert button on each scalar property <input>.
        $("#profile-properties tr.editableDataRow.updated span.revertDelete").live("click", function () {
            var propertyName = getPropertyName($(this).closest("tr.editableDataRow"));
            profileDataSource.revertChanges(profile, propertyName);
        });

        // Bind our Save/Cancel buttons.
        $("#submit").click(function () { profileDataSource.commitChanges(); });
        $("#cancel").click(function () { profileDataSource.revertChanges(true); });


        //
        // Friends <ul>
        //

        var friends = profile.get_Friends();

        // Update the Submit / Cancel button state when a friend is reverted
        $([friends]).bind("arrayChange", function () {
            updateSaveCancelButtonState();
        });

        var lazyLoadedFriendProfiles = {};
        var friendsList = new ListControl(friends, {
            template: "#profile-friend-template",
            container: "#friend-list",
            templateOptions: {
                getFriendName: function (friend) {
                    var profile = friend.get_FriendProfile() || lazyLoadedFriendProfiles[friend.FriendId];
                    if (profile) {
                        return profile.Name;
                    } else {
                        // A constraint of data context is that we can't refresh while we have outstanding
                        // edits.  For a newly added Friend, if we don't have a corresponding 
                        // Profile from which to determine a human-readable name, we have to lazy-load
                        // the Profile.  Importantly, this is done in a data context separate
                        // from our other data sources.

                        $.dataSource({
                            serviceUrl: serviceUrl,
                            queryName: "GetProfiles",
                            filter: { property: "Id", value: friend.FriendId },
                            refresh: function (entities) {
                                lazyLoadedFriendProfiles[friend.FriendId] = entities[0];
                                friendsList.updateElement(friend);
                                updateFriendAddDeleteAdornment(friend);
                            }
                        }).refresh();

                        return friend.FriendId.toString();
                    }
                }
            }
        });

        // Bind the "Add Friend" button in the newly rendered template.
        $("#add-friend-button").click(function () {
            $.push(friends, {
                Id: jQuery.guid++,  // Funny
                FriendId: $("#add-friend").data("friendId")
            });

            // Clear/disable "Add Friend", since we no longer have an FK value to add.
            $("#add-friend-text").val("");
            $(this).attr("disabled", "disabled");
        });

        // Bind auto-complete to the text input.
        bindAddFriendAutoComplete();


        // For each Friend child entity, transitioning from the "Unmodified" entity state
        // indicates an add/remove.  Such a change should update our per-entity added/removed styling.  
        // It should also enable/disable our Save/Cancel buttons.
        $([friends]).dataSource().option("entityStateChange", function (entity, state) {
            updateFriendAddDeleteAdornment(entity);
            updateSaveCancelButtonState();
        });
        // Establish initial values for our added/removed styling.
        $.each(friends, function (unused, friend) {
            updateFriendAddDeleteAdornment(friend);
        });

        // Establish "live" binding our per-entity "revert/delete" buttons.
        bindFriendRevertDeleteButtons();


        //
        // Helper functions
        //

        function updatePropertyAdornments() {
            $("#profile-properties tr.editableDataRow")
                .removeClass("updated")
                .filter(function () {
                    return isModifiedProfileProperty(getPropertyName($(this)));
                })
                .addClass("updated");

            function isModifiedProfileProperty(propertyName) {
                var profileEntityState = profileDataSource.getEntityState(profile)
                switch (profileEntityState) {
                    case "ClientUpdated":  // Profile entity is only updated on the client.
                    case "ServerUpdating":  // Profile entity is updated on the client and sync'ing with server (but unconfirmed).
                        return profileDataSource.isPropertyChanged(profile, propertyName);

                    default:
                        return false;
                }
            };
        };

        function getPropertyName(propertyElement) {
            return $("input", propertyElement).attr("name");
        };

        function updateSaveCancelButtonState() {
            var haveChanges = hasChanges(profileDataSource) || hasChanges($([friends]).dataSource());
            var changesValid = $("#profile-properties").valid();
            if (haveChanges && changesValid) {
                $("#submit").removeAttr("disabled");
            } else {
                $("#submit").attr("disabled", "disabled");
            }

            // Can cancel changes regardless if they are valid or not
            if (haveChanges) {
                $("#cancel").removeAttr("disabled");
            } else {
                $("#cancel").attr("disabled", "disabled");
            }

            function hasChanges(dataSource) {
                return $.grep(dataSource.getEntities(), function (entity) {
                    switch (dataSource.getEntityState(entity)) {
                        case "ClientUpdated":
                        case "ClientAdded":
                        case "ClientDeleted":
                            return true;

                        case "Unmodified":  // No changes to commit.
                        case "ServerUpdating":  // Commit is in progress, so disable Save/Cancel button.
                        case "ServerAdding":
                        case "ServerDeleting":
                            return false;
                    }
                }).length > 0;
            };
        };

        function updateFriendAddDeleteAdornment(friend) {
            var friendsListItemElement = friendsList.getElement(friend);
            var dataSource = $([friends]).dataSource();
            var entityState = dataSource.getEntityState(friend) || "";

            var isDeleted = entityState === "ClientDeleted" || entityState === "ServerDeleting";
            var isAdded = entityState === "ClientAdded" || entityState === "ServerAdding";

            $(friendsListItemElement)
                .toggleClass("deleted", isDeleted)
                .toggleClass("added", isAdded);
        };

        function bindFriendRevertDeleteButtons() {
            // It's convenient to use "live" here to bind/unbind these handlers as child entities are
            // added/removed.
            $("#friend-list span.revertDelete").live("click", function () {
                var friendElement = $(this).closest("li");
                var friend = friendElement.tmplItem().data;

                if (friendElement.hasClass("deleted")) {
                    $([friends]).dataSource().revertChanges(friend);
                } else {
                    friends.deleteEntity(friend);  // revertChange would work here too.
                }
            });
        };

        function bindAddFriendAutoComplete() {
            // Auto-complete to add to our Friends list.
            $("#add-friend-text").autocomplete({
                source: function (term, callback) {
                    $.dataSource({
                        serviceUrl: serviceUrl,
                        queryName: "GetProfiles",
                        filter: { property: "Name", operator: "Contains", value: term.term },
                        refresh: function (potentialFriends) {
                            // Filter out Friend selections that are already friends of the current Profile.
                            potentialFriends = $.grep(potentialFriends, function (potentialFriend) {
                                if (potentialFriend.Id === profile.Id) {
                                    return false;  // You can't be your own friend.
                                } else {
                                    // Filter out current friends.
                                    return $.grep(friends, function (friend) {
                                        return friend.FriendId === potentialFriend.Id;
                                    }).length === 0;
                                }
                            });
                            callback($.map(potentialFriends, function (potentialFriend) {
                                return { label: potentialFriend.Name, foreignKey: potentialFriend.Id };
                            }));
                        }
                    }).refresh();
                },
                select: function (event, data) {
                    // Stow the FK value where our "Add Friend" button's click handler can find it.
                    $("#add-friend").data("friendId", data.item.foreignKey);

                    // Enable "Add Friend", since we now have an FK value.
                    $("#add-friend-button").removeAttr("disabled").focus();
                }
            })
            .keyup(function (event) {
                // Any keystroke that doesn't select a Friend should disable "Add Friend".
                // Selecting a Friend from the auto-complete menu will enable.
                if (event.keyCode !== 13) {
                    $("#add-friend-button").attr("disabled", "disabled");
                }
            });
        };
    };

    $("#add-friend-text").watermark();
});
