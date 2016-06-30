var colors = ["#6cf4f2", "#68ccdc", "#7ae781", "#83ddcc", "#b1d14e"]
var USERS = [];
var doneUser = [];
var colorLevel;
    function loadAllMember(rootTreee, houseid) {
        alert('loadAll Member' + houseid)
            $.ajax({
                url: "/Home/getUserByHouseId/",
                type: "GET",
                data: {
                    HouseId:houseid,

                },
                success: function (successData) {
                  alert(successData)
                    USERS = JSON.parse(successData);
                    doneUser = [];
                    colorLevel = 0;
                   alert(USERS.length);
                    USERS.sort(function (a, b) {
                        return a['FamilyLevel'] - b['FamilyLevel']
                    })
                    var familyLevel = [];
                    for (var i = 0; i < USERS.length; i++) {
                        if (familyLevel.indexOf(USERS[i]['FamilyLevel']) === -1) {
                            familyLevel[familyLevel.length] = USERS[i]['FamilyLevel'];
                        }
                    }
                    console.log(familyLevel.length)
                    for (var i = 0; i < familyLevel.length; i++) {
                        console.log('addusertofamilyLevel' + familyLevel.length);
                        addUserToFamilyTree(familyLevel[i], USERS);
                    }
                },
                error: function(er){
                    alert(er);
                }
            });
    }
    function addUserToFamilyTree(level, USERS) {
        alert('add user to family tree' + level)
        var divLevel = $("#level" + level);
        if (divLevel.length) {
            //alert('yes')
        } else {
            //alert('no');
            $("#memberPanelBody").append('<div id="level' + level + '" class="row" style="width: 100%;background-color: '+colors[colorLevel]+';text-align:center;border-radius: 30px;border-top-style: groove;border-top-color: #787878;margin-bottom: 5vh;"></div>')
            colorLevel++;
        }
        for (var i = 0; i < USERS.length; i++) {
            var u = USERS[i];
            if (u['FamilyLevel'] == level&&u['Status']=='1') {
                var userProfile = "";
                if (u['ProfileImage']==null || u['ProfileImage'] == '') {
                    userProfile = '/Content/Images/defaultProfile.png';
                } else {
                    userProfile = u['ProfileImage']
                }
                $('#level' + level).append('<a href="#" class="familymember" onclick="LoadUserProfile('+u["Id"]+')">'
                                    +'<div style="height:120px;float:left">'
                                        +'<img src="'+userProfile+'" style="height:85%">'
                                        + '<div style="margin-top: 5px; font-size: small;font-weight: 700;background-color: aliceblue;padding-left: 10px;padding-right: 10px;border-top-left-radius: 10px;border-top-right-radius: 10px;">'
                                            +u['Fullname']
                                        +'</div>'
                                    + '</div>'
                                +'</a>')
            }
        }
    }

    function addUserToMap(user,root){
        
        var ulRoot = $("#ulRoot"+root);
        if(ulRoot.length){
            //alert('ulRootYes')
        }else{
            //alert('ulRootNo')
            $("#Couple"+root).append('<ul id="ulRoot'+root+'"></ul>')
        }
        var liCoupleId =  $("#Couple"+user["CoupleId"]);
        if(liCoupleId.length){
            //alert('liCoupleYes')
        }else{
            //alert('liCoupleNo');
            $("#ulRoot"+root).append('<li id="Couple'+user["CoupleId"]+'"></li>')
        }
        var profileSrc = "";
        if(user["ProfileImage"]==null||user["ProfileImage"]==""){
            profileSrc="/Content/images/defaultProfile.png"
        }else{
            profileSrc = user["ProfileImage"]
        }
        $("#Couple" + user["CoupleId"]).prepend('<a href="#" onclick="LoadUserProfile('+user["Id"]+')">'
                                                       +' <div style="height:150px;float:left">'
                                                            +'<img src="'+profileSrc+'" style="width:auto;height:80%;max-width:100%"><br/>'
                                                            + '<div style="margin-top: 5px; font-size: small;font-weight: 700;background-color: aliceblue;">'
                                                            + user["Fullname"]
                                                            + '</div>'
                                                        +'</div>'
                                                    +'</a>')
        for(var j = USERS.length-1;j>=0;j--){
            var u = USERS[j];
            if(u['RootCoupleId']==user['CoupleId']){
                if(doneUser.indexOf(u["Id"])<0){
                    doneUser[doneUser.length]=u['Id'];
                    addUserToMap(u,user["CoupleId"])
                }
               
            }
        }
    }
