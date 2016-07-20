//var colors = ["#bf5148", "#4cacc5", "#8164a2", "#fc953e", "#9bbb57"];
var colors = ["#00796b", "#9e9e9e", "#00796b", "#9e9e9e", "#00796b"];
var rangeNames = ["Ông, bà (Lớn hơn 2 bậc)", "Bố, mẹ, cô, dì, chú, bác, cậu, mợ...(Lớn hơn 1 bậc)", "Vợ, Chồng, Anh, Chị, Em, Bạn...", "Con, cháu (gọi bằng cô, dì, chú, bác)", "Cháu (gọi bằng ông, bà)"];
var rangeNamesManager = ["Quản lý", "Nhân viên hỗ trợ", "Quản trị hệ thống"];
var USERS = [];
var doneUser = [];
var colorLevel;
function loadAllMember(rootTreee, houseid) {
    //alert('loadAll Member' + houseid)
    $.ajax({
        url: "/Home/getUserByHouseId/",
        type: "GET",
        data: {
            HouseId: houseid,
        },
        success: function (successData) {
            if (successData == "NOT_PERMISSION") {
                $("#memberPanelBody")
                    .append('<span style="width:100%;text-align:center"><h3>Thông tin thành viên được ẩn</h3><span>');
                $("#memberPanelBody div.row:first").addClass("hide");
                showModal();
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
                console.log('addusertofamilyLevel' + familyLevel.length);
                addUserToFamilyTree(familyLevel[i], USERS);
            }
            showModal();
        },
        error: function (er) {
            alert(er);
        }
    });
}
function addUserToFamilyTree(level, USERS) {
    //alert('add user to family tree' + level)
    var divLevel = $("#level" + level);
    if (divLevel.length) {
        //alert('yes')
    } else {
        //alert('no');
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
                $("#level_" + level).append('<div class="range-frame-header">' +
                            '<span>' + rangeName + '</span>' +
                            '</div>');
                break;
            }
        }
        
        $("#level_" + level).append('<div id="level' + level + '" class="row range-frame" style="background-color: ' + colors[colorLevel] + '"></div>');
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
            $('#level' + level).append('<a href="#" class="familymember" onclick="LoadUserProfile(' + u["Id"] + ')" style="border:none">'
                                + '<div style="height:120px;float:left">'
                                    + '<img class="loading-img img-border" onError="this.src=\'/Content/Images/defaultProfile.png\';" src="' + userProfile + '"style="height:85%">'
                                    + '<div class="member-name-title">'
                                        + u['Fullname']
                                    + '</div>'
                                + '</div>'
                            + '</a>');
        }
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
