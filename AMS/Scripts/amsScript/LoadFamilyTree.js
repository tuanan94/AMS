  var USERS = [];
    var doneUser = [];
    function loadAllMember(rootTreee,houseid){
            $.ajax({
                url: "/Home/getUserByHouseId/",
                type: "GET",
                data: {
                    HouseId:houseid,

                },
                success: function(successData){
                    USERS = JSON.parse(successData);
                  //  alert(USERS.length)
                    for(var i= USERS.length -1 ;i>=0;i--){
                        user = USERS[i];
                        if (user['RootCoupleId']==null || user['RootCoupleId']==''){
                            if(doneUser.indexOf(user["Id"])<0){
                                doneUser[doneUser.length]=user['Id'];
                                addUserToMap(user,rootTreee)
                            }
                           
                        }
                    }
                },
                error: function(er){
                    alert(er);
                }
            });
    }

    function addUserToMap(user,root){
     
        var ulRoot = $("#ulRoot"+root);
        if(ulRoot.length){
           // alert('ulRootYes')
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
        $("#Couple"+user["CoupleId"]).prepend('<a href="#">'
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
