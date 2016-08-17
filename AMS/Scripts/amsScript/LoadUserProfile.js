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
            if (obj.HouseId == 0) {
                $("#amsMsgText").text("");
                $("#amsMsgText").append('<span style="width:100%;text-align:center">Rất tiếc! Cư dân <strong>' + obj.FullName + '</strong> không còn tồn tại trong hệ thống!<span>');
                $("#amsMessageModal").modal("show");
                return;
            }
            addModal(obj, loadMember);
            if (loadMember == true) {
                //  alert('load')
                $("#userInfoTabHeader li:last").removeClass("hide");
                loadAllMember("treeModal", obj['HouseId']);
            } else {
                $("#userInfoTabHeader li:last").addClass("hide");
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
                                    + '<a class="recent-activ-title"  href="/Post/Detail?postId=' + moreinfo['Id'] + '">'
                                         + '<strong>' + data['FullName']+ '</strong>' + ' đã đăng bài viết mới'
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
    $("#_usrInfoHouseName").unbind();
    $("#_usrInfoHouseName").get(0).lastChild.nodeValue = "";
    $("#_usrInfoHouseName").append(data["HouseName"]);
    $("#_usrInfoHouseName").data("houseId", data["HouseId"]).on("click", function () {
        location.href = "/House/" + $(this).data("houseId");
    });
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
    appendSinglePostModal();
    getSinglePostObject(postId)
}
function appendSinglePostModal() {
    var element = $("#displaySinglePost");
    if (element.length > 0) {
        return
    }
    $("body").append('<div class="modal fade" id="displaySinglePost" role="dialog" style="z-index:9999">'
    +'<div class="modal-dialog">'
        +'<div class="modal-content">'
            +'<div class="modal-body" id="singlePostModal">'
                +'</div>'
                    +'<div class="modal-footer">'
                        +'<button type="button" class="btn btn-default" data-dismiss="modal">Đóng</button>'
                    +'</div>'
                +'</div>'

            +'</div>'
       + '</div>')

}

function addPostTo(dir, index, postObject) {
    if (dir == "singlePostModal") {
        var reportDiv = "";
        if (postObject['UserId'] != '@curUser.Id') {
            reportDiv = '<div href="" class="btn btn-white btn-xs pull-right" onclick="report(' + postObject['Id'] + ')"><i class="fa fa-fw fa-warning"></i></div>';
        }
        var header = '<div class="panel-heading clearfix" style="background-color: transparent;">' +
                '<div class="form-group" style="margin-bottom:0">' +
                    '<div style="display: flex">' +
                        '<div class="pull-left" id="userLoadProfilePost' + postObject['Id'] + '" onclick="LoadUserProfile(' + postObject['UserId'] + ',true)" style="margin-right: 10px">' +
                            '<img id="userImagePost' + postObject['Id'] + '" src="' + postObject['userProfile'] + '" onerror="this.src=\'/Content/Images/defaultProfile.png\';" class="avartar-img">' +
                        '</div>' +
                        '<div style="flex: 1; width: 0">' +
                            '<h3 class="" style="margin-top: 0;margin-bottom: 0;">' +
                                '<strong>' +
                                '   <a class="text-capitalize user-name" id="userNamePost' + postObject['Id'] + '">' + postObject['userFullName'] + '</a>' +
                                '</strong>' +
                                '<strong class="house-name"> ' +
                                    '<span style="font-size: 17px;">' +
                                        '<i class="fa fa-home icon-house"></i>' +
                                            postObject['houseName'] +
                                    '</span>' +
                                '</strong>' +
                            '</h3>' +
                            '<h3 class="" style="margin-top: 0;margin-bottom: 0;">' +
            '                   <strong>' +
                                    '<span class="post-time">' +
                                        '<i class="fa fa-globe icon-house"></i> ' +
                                        timeSince(postObject['CreateDate']) +
                                    '</span>' +
                                '</strong>' +
                            '</h3>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>';
        $("#singlePostModal").html("");
        $("#singlePostModal").append('<li class="media">'
            + '<div class="media-body">'
                            + '<div class="panel panel-default panel-feed" style="width:100%">' + header
                                + '<div class="panel-body" >'
                                    + reportDiv
                                    + '<p>' + postObject['Body'] + '</p>'
                                    + '<div class="form-group">'
                                     + '<div class="grid" id="imagesPostForSinglePost' + postObject['Id'] + '">'
                                    + '</div>'
                                    + '</div>'


                                    + '<div class="row" style="height:auto;">'
                                    + postObject['EmbedCode']
                                    + '</div>'
                                + '</div>'
                                + '<div class="view-all-comments" id="countCommentForSinglePost' + postObject['Id'] + '"><a href="#"><i class="fa fa-comments-o"></i> View all</a> 10 comments</div>'
                                + '<ul class="comments" id="commentsAreaForSinglePost' + postObject['Id'] + '">'

                                + '</ul>'
                                + '<ul class="comments">'
                                 + '</div>'




            + '</div>'
            + '</li>'
            + '</ul>'
            + '</div>'
            + '</div>'
            + '</li>'
        )

    }
}
function getImagesForSiglePost(index, id) {
    if (id == null) {
        alert("postid = nul");
        return;
    }
    $.ajax({
        url: "/Post/getImagesForPost",
        type: "GET",
        data: {
            postId: id,
        },
        success: function (successData) {
            var obj = JSON.parse(successData);
            var currentElement = {};
            var listElement = "";
            var isFirst = true;
            $.each(obj, function (index, image) {
                if (isFirst) {
                    $("#imagesPostForSinglePost" + image.postId).html("");
                    currentElement = $("#imagesPostForSinglePost" + image.postId);
                    isFirst = false;
                    if (obj.length == 1) {
                        $("#imagesPostForSinglePost" + image.postId).removeClass("testetetet").addClass("one-col");
                        if (parseInt(image.width) >= parseInt(image.height)) {
                            listElement = listElement +
                                '<div data-src="' + image.url + '" style="border:1px solid white"><img style="width:100%; height:auto" src="' +
                                image.thumbnailurl +
                                '" alt=""></div>';
                        } else {
                            listElement = listElement +
                                '<div data-src="' + image.url + '" style="border:1px solid white" ><img style="height:100% !important; width:auto !important "src="' +
                                image.thumbnailurl +
                                '" alt=""></div>';
                        }
                        return;
                    } else {
                        $("#imagesPostForSinglePost" + image.postId).removeClass("one-col").addClass("testetetet");
                    }
                }
                if (parseInt(image.width) >= parseInt(image.height)) {
                    listElement = listElement + '<div data-src="' + image.url + '" style="border:1px solid white"><img style="width:100% !important; height:auto !important" src="' + image.thumbnailurl + '" ></div>';
                } else {
                    listElement = listElement + '<div data-src="' + image.url + '" style="border:1px solid white" ><img style="width:100% !important; height:auto !important" src="' + image.thumbnailurl + '"></div>';
                }

            });
            listElement = listElement + "</div>";
            currentElement.append(listElement);
            currentElement.lightGallery();
        },
        error: function (er) {
            alert(er);
        }

    });
}
function GetCommentForSinglePost(postid) {
    if (postid == null) {
        alert("postid == null");
        return;
    }
    $.ajax({
        url: "/Post/getCommentsForPost",
        type: "GET",
        data: {
            postId: postid,
        },
        success: function (successData) {
            var obj = JSON.parse(successData);
            if (obj.length === 0) {
                $("#countCommentForSinglePost" + postid).html("Chưa có bình luận nào");
            } else {
                $("#countCommentForSinglePost" + postid).html(obj.length + " bình luận");
            }
            $.each(obj, function (index, comment) {
                addCommentToCommentAreaForSinglePost(postid, comment);
            });
        },
        error: function (er) {
            alert(er)
        }

    });
}
function addCommentToCommentAreaForSinglePost(postId, comment) {
    $("#commentsAreaForSinglePost" + postId).append('<li>'
        + '<div class="media">'
        + '<a href="javascript:void(0)" onclick ="LoadUserProfile(' + comment["userId"] + ',true)"  class="pull-left">'
        + '<img  src="' + comment['userProfile'] + '" onerror="this.src=\'/Content/Images/defaultProfile.png\';" class="media-object avartar-img">'
        + '</a>'
        + '<div class="media-body">'
        + '<a href="javascript:void(0)" onclick ="LoadUserProfile(' + comment["userId"] + ',true)" class="comment-author">' + comment['username'] + ': </a>'
        + '<span>' + comment['detail'] + '</span>'
        + '<div class="comment-date">' + timeSince(comment['createdDate']) + ' trước </div>'
        + '</div>'
        + '</div>'

        + '</li>');

}



function getSinglePostObject(postId) {
    $.ajax({
        url: "/Post/getSinglePost",
        type: "GET",
        datatype: 'json',
        data: {
            postId: postId,
        },
        success: function (successData) {
            console.log("GetPostData: " + successData);
            var obj = JSON.parse(successData);
            addPostTo("singlePostModal", 0, obj);
            getImagesForSiglePost(0, obj["Id"]);
            GetCommentForSinglePost(obj["Id"]);
            //  curTokenId = obj["Id"];
            $("#displaySinglePost").modal('show');
        },
        error: function (er) {
            alert(er);
        }

    });
}
