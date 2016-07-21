//var colors = ["#bf5148", "#4cacc5", "#8164a2", "#fc953e", "#9bbb57"];
var colors = ["#00796b", "#9e9e9e", "#00796b", "#9e9e9e", "#00796b"];
var rangeNames = ["Ông, bà (Lớn hơn 2 bậc)", "Bố, mẹ, cô, dì, chú, bác, cậu, mợ...(Lớn hơn 1 bậc)", "Vợ, Chồng, Anh, Chị, Em, Bạn...", "Con, cháu (gọi bằng cô, dì, chú, bác)", "Cháu (gọi bằng ông, bà)"];
var rangeNamesManager = ["Quản lý", "Nhân viên hỗ trợ", "Quản trị hệ thống"];
var USERS = [];
var doneUser = [];
var colorLevel;
function loadAllMember(rootTreee, houseid, selectorStr, notShowModal) {
    //alert('loadAll Member' + houseid)
    $.ajax({
        url: "/Home/getUserByHouseId/",
        type: "GET",
        data: {
            HouseId: houseid,
        },
        success: function (successData) {
            if (successData == "NOT_PERMISSION") {
                if (selectorStr) {

                } else {
                    $("#memberPanelBody")
                    .append('<span style="width:100%;text-align:center"><h3>Thông tin thành viên được ẩn</h3><span>');
                    $("#memberPanelBody div.row:first").addClass("hide");
                    showModal();
                }
                return;
            } else {
                $("#memberPanelBody div.row:first").removeClass("hide");
            }

            USERS = JSON.parse(successData);
            doneUser = [];
            colorLevel = 0;
            //alert(USERS.length);
            USERS.sort(function (a, b) {
                return a['FamilyLevel'] - b['FamilyLevel'];
            });
            var familyLevel = [];
            for (var i = 0; i < USERS.length; i++) {
                if (familyLevel.indexOf(USERS[i]['FamilyLevel']) === -1) {
                    familyLevel[familyLevel.length] = USERS[i]['FamilyLevel'];
                }
            }
            console.log(familyLevel.length);
            for (var i = 0; i < familyLevel.length; i++) {
                //                console.log('addusertofamilyLevel' + familyLevel.length);
                addUserToFamilyTree(familyLevel[i], USERS, selectorStr);
            }
            if (!notShowModal) {
                showModal();
            }
        },
        error: function (er) {
            alert(er);
        }
    });
}

function addUserToFamilyTree(level, USERS, selectorStr) {
    //alert('add user to family tree' + level)
    var divLevel = {}

    if (selectorStr) {
        divLevel = $("#" + selectorStr + " #level" + level);
    } else {
        divLevel = $("#memberPanelBody " + "#level" + level);
    }

    var element = {};
    var elementLevel = {};
    if (divLevel.length) {
        //alert('yes')
    } else {

        //alert('no');
        if (selectorStr) {
            element = $("#" + selectorStr);
        } else {
            element = $("#memberPanelBody");
        }

        element.append('<div id="level_' + level + '"></div>');

        if (selectorStr) {
            elementLevel = $("#" + selectorStr + " #level_" + level);
        } else {
            elementLevel = $("#memberPanelBody " + " #level_" + level);
        }
        // Set range header
        for (var i = 0; i < USERS.length; i++) {
            var u = USERS[i];
            var rangeName = "";
            if (u['FamilyLevel'] == level && u['Status'] == '1') {
                if (u.RoleId == 3 || u.RoleId == 4) {
                    rangeName = rangeNames[colorLevel];
                } else {
                    rangeName = rangeNamesManager[colorLevel];
                }
                elementLevel.append('<div class="range-frame-header">' +
                            '<span>' + rangeName + '</span>' +
                            '</div>');
                break;
            }
        }
        elementLevel.append('<div id="level' + level + '" class="row range-frame" style="background-color: ' + colors[colorLevel] + '"></div>');
        colorLevel++;
    }
    if (selectorStr) {
        elementLevel = $("#" + selectorStr + " #level" + level);
    } else {
        elementLevel = $("#memberPanelBody " + "#level" + level);
    }
    for (var i = 0; i < USERS.length; i++) {
        var u = USERS[i];
        if (u['FamilyLevel'] == level && u['Status'] == '1') {
            var userProfile = "";
            if (u['ProfileImage'] == null || u['ProfileImage'] == '') {
                userProfile = '/Content/Images/defaultProfile.png';
            } else {
                userProfile = u['ProfileImage'];
            };

            elementLevel.append('<a class="familymember link-cursor" onclick="LoadUserProfile(' + u["Id"] + ')" style="border:none;padding-right: 0;">'
                                + '<div class="img-border in-house-avar" style="height: 120px;width: 120px">'
                                    + '<img class="loading-img" onError="this.src=\'/Content/Images/defaultProfile.png\';" src="' + userProfile + '"style="width:120px; height:120px"/>'
                                + '</div>'
                                + '<div class="member-name-title">'
                                        + u['Fullname']
                                    + '</div>'
                            + '</a>');
        }
    }
}


/*ANTLNM not change logic code for render range of member from AnLTT*/
function loadAllMemberForSettingPage(houseid, selectorStr) {
    //alert('loadAll Member' + houseid)
    $.ajax({
        url: "/Home/getUserByHouseId/",
        type: "GET",
        data: {
            HouseId: houseid,
        },
        success: function (successData) {

            USERS = JSON.parse(successData);
            doneUser = [];
            colorLevel = 0;
            USERS.sort(function (a, b) {
                return a['FamilyLevel'] - b['FamilyLevel'];
            });
            var familyLevel = [];
            for (var i = 0; i < USERS.length; i++) {
                if (familyLevel.indexOf(USERS[i]['FamilyLevel']) === -1) {
                    familyLevel[familyLevel.length] = USERS[i]['FamilyLevel'];
                }
            }
            console.log(familyLevel.length);
            for (var i = 0; i < familyLevel.length; i++) {
                addUserToFamilyTreeNew(familyLevel[i], USERS);
            }
            $("#memberPanelBody").removeClass("hide");
        },
        error: function (er) {
            alert(er);
        }
    });
}

function addUserToFamilyTreeNew(level, USERS) {
    //alert('add user to family tree' + level)
    var divLevel = $("#level" + level);
    if (divLevel.length) {
    } else {
        $("#memberPanelBody").append('<div id="level_' + level + '"></div>');
        // Set range header
        for (var i = 0; i < USERS.length; i++) {
            var u = USERS[i];
            var rangeName = "";
            if (u['FamilyLevel'] == level && u['Status'] == '1') {
                if (u.RoleId == 3 || u.RoleId == 4) {
                    rangeName = rangeNames[colorLevel];
                } else {
                    rangeName = rangeNamesManager[colorLevel];
                }
                $("#level_" + level).append($("<div/>").addClass("range-frame-header").
                                                    append($("<span/>").text(rangeName)));
                break;
            }
        }
        $("#level_" + level).append(
            $('<div/>')
            .attr("id", "level" + level)
            .addClass("row range-frame")
            .css({ "background-color": colors[colorLevel] }));
        colorLevel++;
    }
    for (var i = 0; i < USERS.length; i++) {
        var u = USERS[i];
        if (u['FamilyLevel'] == level && u['Status'] == '1') {
            var userProfile = "";
            if (u['ProfileImage'] == null || u['ProfileImage'] == '') {
                userProfile = '/Content/Images/defaultProfile.png';
            } else {
                userProfile = u['ProfileImage'];
            };
            var curUserId = $("#memberPanelBody").data("curUserId");
            var curUserRoleId = $("#memberPanelBody").data("curUserRoleId");

            if (curUserRoleId == window.UserRoleHouseHolder.toString()) {
                if (curUserId == u["Id"].toString() && u["RoleId"].toString() == curUserRoleId) {
                    $('#level' + level).append(imageFrame(u["Id"], userProfile, u["Fullname"], false));
                }// if current user has role houseHolder of this house
                else {
                    $('#level' + level).append(imageFrame(u["Id"], userProfile, u["Fullname"], true));
                }
            }// if current user has role houseHolder 
            else {
                $('#level' + level).append(imageFrame(u["Id"], userProfile, u["Fullname"], false));
            }
        }
    }
}

function imageFrame(userId, userProfileImg, fullName, hasDeleteBtn) {
    if (hasDeleteBtn) {
        return '<a class="familymember link-cursor" onclick="LoadUserProfile(\'' + userId + '\')" style="border:none;padding-right: 0;">'
//        return '<a href="#" class="familymember" style="border:none;padding-right: 0;">'
                                        + '<div class="img-border in-house-avar" style="height: 120px;width: 120px">'
                                            + '<img class="loading-img" onError="this.src=\'/Content/Images/defaultProfile.png\';" src="' + userProfileImg + '"style="width:120px; height:120px"/>'
                                                + "<div class='mem-remove' onclick='openModalDeleteBlock(\"" + userId + "\",\"" + fullName + "\")'>" +
                                                    '<i class="del-time fa fa-times" >' + '</i>' +
                                                '</div>'
                                        + '<i class="del-time fa fa-times" >' + '</i>'
                                        + '</div>'
                                        + '<div class="member-name-title">'
                                                + fullName
                                            + '</div>'
                                    + '</a>';
    } else {
        //        return '<a href="#" class="familymember" style="border:none;padding-right: 0;">'
        return '<a  class="familymember link-cursor" onclick="LoadUserProfile(\'' + userId + '\')" style="border:none;padding-right: 0;">'
                     + '<div class="img-border in-house-avar" style="height: 120px;width: 120px">'
                         + '<img class="loading-img" onError="this.src=\'/Content/Images/defaultProfile.png\';" src="' + userProfileImg + '"style="width:120px; height:120px"/>'
                     + '</div>'
                     + '<div class="member-name-title">'
                             + fullName
                         + '</div>'
                 + '</a>';
    }

}

function addUserToMap(user, root) {

    var ulRoot = $("#ulRoot" + root);
    if (ulRoot.length) {
        //alert('ulRootYes')
    } else {
        //alert('ulRootNo')
        $("#Couple" + root).append('<ul id="ulRoot' + root + '"></ul>');
    }
    var liCoupleId = $("#Couple" + user["CoupleId"]);
    if (liCoupleId.length) {
        //alert('liCoupleYes')
    } else {
        //alert('liCoupleNo');
        $("#ulRoot" + root).append('<li id="Couple' + user["CoupleId"] + '"></li>');
    }
    var profileSrc = "";
    if (user["ProfileImage"] == null || user["ProfileImage"] == "") {
        profileSrc = "/Content/images/defaultProfile.png";
    } else {
        profileSrc = user["ProfileImage"];
    }
    $("#Couple" + user["CoupleId"]).prepend('<a href="#" onclick="LoadUserProfile(' + user["Id"] + ')">'
                                                   + ' <div style="height:150px;float:left">'
                                                        + '<img src="' + profileSrc + '" style="width:auto;height:80%;max-width:100%"><br/>'
                                                        + '<div style="margin-top: 5px; font-size: small;font-weight: 700;background-color: aliceblue;">'
                                                        + user["Fullname"]
                                                        + '</div>'
                                                    + '</div>'
                                                + '</a>');
    for (var j = USERS.length - 1; j >= 0; j--) {
        var u = USERS[j];
        if (u['RootCoupleId'] == user['CoupleId']) {
            if (doneUser.indexOf(u["Id"]) < 0) {
                doneUser[doneUser.length] = u['Id'];
                addUserToMap(u, user["CoupleId"]);
            }

        }
    }
}
