function LoadUserProfile(UserId) {
    LoadUserProfile(UserId, false);
}
function LoadUserProfile(UserId, loadMember) {
    // alert(loadMember)
    getUser(UserId, loadMember);
}
function getUser(userid, loadMember) {
    $.ajax({
        url: "/Home/getUser",
        type: "GET",
        data: {
            UserId: userid,
        },
        success: function (successData) {
            console.log("Get user: " + successData);
            var obj = JSON.parse(successData);
            addModal(obj, loadMember);
            if (loadMember == true) {
                //  alert('load')
                loadAllMember("treeModal", obj['HouseId']);
            } else {
                showModal(obj);
            }
        },
        error: function (er) {
            alert(er);
        }

    });
}
function showModal() {
    $("#userInforModal").modal('show');
}
function clearModal(id) {
    $("#userprofile" + id).remove();
}
function addModal(data, loadMember) {

    var imgSrc = "/Content/images/defaultProfile.png";
    // alert(data['DateOfBirth']);
    var DateOfBirth = new Date(data['DateOfBirth']);
    var StringDateOfBirth = DateOfBirth.toDateString();
    if (StringDateOfBirth == 'Invalid Date') {
        StringDateOfBirth = "Không xác định";
    } else {
        StringDateOfBirth = DateOfBirth.getDate() + "/" + (DateOfBirth.getMonth() + 1) + "/" + DateOfBirth.getFullYear();

    }
    var StringSex = "Male";


    if (data['Gender'] === 0) {
        StringSex = "Nam";
    } else {
        StringSex = "Nữ";
    }
    if (data['ProfileImage'] != null && data['ProfileImage'] != '') {
        imgSrc = data['ProfileImage'];
    }
    var moreinfos = data['moreInfos'];
    console.log("Moreinfo: " + moreinfos.length);
    var moreInfoHtmlArea = "";
    for (var i = 0; i < moreinfos.length; i++) {
        var moreinfo = moreinfos[i];
        moreInfoHtmlArea += '<li style="border-bottom: 1px solid #9E9E9E;">'
                                + '<div class="row" style="font-size: 16px;">'
                                    + '<div class="col-md-8">'
                                    + '<a class="recent-activ-title" href="javascript:void(0)" onclick="displaySinglePost(' + moreinfo['Id'] + ')">'
                                         + '<strong style="color: #009688;font-size: 18px;">' + data['FullName']+ '</strong>' + ' đã đăng bài viết mới'
                                    + '</a>'
                                    + '</div>'
                                    + '<div class="col-md-4" style="text-align: right;">'
                                        + '<span style="pull-right">'
                                        + '<i>'
                                            + timeSince(moreinfo['createdDate'])
                                        + '</i>'
                                        + '</span>'
                                    + '</div>'
                                + '</div>'
                                + '<div class="row">' +
                                    '<div class="col-md-11">' +
                                        '<p class="truncate" style="font-size: 14px; color: #212121; margin-bottom: 0px; max-height: 61px; overflow: hidden; text-overflow: ellipsis; max-width: 100%;">' +
                                        moreinfo['PostText'] +
                                        '</p>' +
                                    '</div>' +
                                '</div>'
                                + '</li>';
    }
    $("#recentActivity").append(moreInfoHtmlArea);
    $("#usrInfoAva").prop("src", imgSrc);
    $("#usrInfoFullname").text(data.FullName);
    $("#usrInfoDob").html(StringDateOfBirth + '(<strong>' + data['Age'] + '</strong> tuổi)');
    $("#usrInfoSex").text(StringSex);
    $("#usrInfoCreateDate").text(data['CreatedDate']);
    if (loadMember == true) {
        $("#usrInfoUrlToFamily").prop("href", "/House/" + data["HouseId"]);
        $("#usrInfoHouseImg").prop("src", data['HouseProfile']);
        $("#usrInfoHouseName").text(data["HouseName"]);
    }
//    $("#userInforModal").modal("show");

    //    $("body").append('<!-- Modal -->'
    //+ '<div id="userprofile' + data['Id'] + '" class="modal fade" role="dialog" style="height:auto">'
    //    +'<div class="modal-dialog" style="width:60%">'
    //        +'<!-- Modal content-->'
    //        +'<div class="modal-content">'
    //            
    //            +'<div class="modal-body">'
    //                +'<div class="row">'
    //                    +'<div class="col-md-4">'
    //                        +'<div class="avatar" style="text-align:center">'
    //                            + '<img src="' + imgSrc + '" alt="" class="img-circle" style="height: 130px;">'
    //                            + '<h3 style="background-color: #21988C;color: white;border-top-left-radius: 30px;border-bottom-left-radius: 30px;">' + data['FullName'] + '</h3>'
    //                        +'</div>'
    //                    +'</div>'
    //                    +'<div class="col-md-8">'
    //                        +'<div class="panel panel-default">'
    //                            +'<div class="panel-heading panel-heading-gray">'
    //                                + '<i class="fa fa-info-circle"></i> Thông tin'
    //                            +'</div>'
    //                            +'<div class="panel-body">'
    //                                +'<ul class="list-unstyled profile-about">'
    //                                    +'<li>'
    //                                        +'<div class="row">'
    //                                            +'<div class="col-sm-4">'
    //                                                +'<span class="text-muted">Ngày sinh</span>'
    //                                            +'</div>'
    //                                            +'<div class="col-sm-8">'+StringDateOfBirth +'</div>'
    //                                        +'</div>'
    //                                    +'</li>'
    //                                   
    //                                    +'<li>'
    //                                        +'<div class="row">'
    //                                            +'<div class="col-sm-4">'
    //                                                +'<span class="text-muted">Giới tính</span>'
    //                                            +'</div>'
    //                                            +'<div class="col-sm-8">'+StringSex+ '</div>'
    //                                        +'</div>'
    //                                    +'</li>'
    //                                 
    //                                +'</ul>'
    //                            +'</div>'
    //                        + '</div>'
    //                         + '<div class="panel panel-default">'
    //                            + '<div class="panel-heading panel-heading-gray">'
    //                                + '<i class="fa fa-info-circle"></i> Hoạt động gần đây'
    //                            + '</div>'
    //                            + '<div class="panel-body">'
    //                                + '<ul class="list-unstyled profile-about" style="margin-left: 10%;">'
    //                                    +moreInfoHtmlArea
    //
    //                                + '</ul>'
    //                            + '</div>'
    //                        + '</div>'
    //                    +'</div>'
    //                + '</div>'
    //                  + '<div class="row" style="text-align:center" id="houseinfoRow">'
    //                  
    //                            + '</div>'
    //                              
    //            +'</div>'
    //            +'<div class="modal-footer">'
    //                + '<button type="button" class="btn btn-default" data-dismiss="modal" onclick="clearModal("' + data['Id'] + '")">Close</button>'
    //            +'</div>'
    //        +'</div>'
    //
    //    +'</div>'
    //+ '</div>');

    //    if (loadMember == true) {
    //        $("#houseinfoRow").append('<div class="panel panel-default">'
    //                            + '<div class="panel-heading panel-heading-gray">'
    //                                + '<i class="fa fa-info-circle"></i> Thông tin gia đình'
    //                            + '</div>'
    //                            + '<div class="panel-body" id="memberPanelBody">'
    //                            +'<div class="row" style="text-align:center">'
    //                                            +'<a href="/House/'+data["HouseId"]+'" class="familymember">'
    //                                               +' <div style="width:100px;height:120px;float:left">'
    //                                                    +'<img src="'+data['HouseProfile']+'" style="width:100%;">'
    //                                                   +' <div style="margin-top: 5px; font-size: small;font-weight: 700;background-color: aliceblue;">'
    //                                                        +data["HouseName"]
    //                                                    +'</div>'
    //                                               +' </div>'
    //                                            +'</a>'
    //                            +'</div>'
    //                           + ' </div>'
    //                            + '</div>');
}


function displaySinglePost(postId) {
    alert("Display single post " + postId + " Not implemented");
}