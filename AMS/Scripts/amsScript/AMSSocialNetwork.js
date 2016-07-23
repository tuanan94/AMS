var currentUserId = -1;
var currentUserProfileImage;
var lastestTokenID = -1;//ID của bài viết mới nhất
function loadSocialNetwork(isNew,curUserId,curUserProfileImage) { // isNew; curUserId; curUserProfileImage
    console.log("load social network - called; CUR_USER_ID = "+curUserId);
    currentUserId = curUserId;
    currentUserProfileImage = curUserProfileImage;
    //Step 1 load Post from tokenId
    if (isNew) {
        loadPost(-1)
    } else {
        loadPost(lastestTokenID)
    }
}
function loadPost(tokenId) {
    console.log("LoadSocialNetwork: start load post from token id = " + tokenId);
    $.ajax({
        url: "/Post/getPost",
        type: "GET",
        datatype: 'json',
        data: {
            idToken: tokenId,
        },
        success: function (successData) {
            console.log("LoadSocialNetwork: end load post. Reponse length = " + successData.length);
            var PostData = JSON.parse(successData);
            $.each(PostData, function (index, postObject) {
                addPostToTimeline(index, postObject);
                appenImageToPost(index,postObject["Id"])
                getCommentsForPost(postObject["Id"]);
                lastestTokenID = postObject["Id"];
            });
            $("#loadMoreBtn").button('reset');
        },
        error: function (er) {
            alert(er);
        }

    });
}
function addPostToTimeline(index, postObject) {

    //REPORT DIV - START
    var reportDiv = "";
    if (postObject['UserId'] != currentUserId) {
        reportDiv = '<div style="position: absolute;right: 0;width: 30px;">' +
          '<div style="position: relative;width: 100%;text-align: center;margin-top: 5px;">' +
          '<div style="color:#f9a825" onclick="report(' + postObject['Id'] + ')"><i class="fa fa-fw fa-warning"></i></div>' +
                   '</div>' +
                   '</div>';
    }
    else if (postObject['UserId'] == currentUserId) {
        reportDiv = '<div style="position: absolute;right: 0;width: 30px;">' +
          '<div style="position: relative;width: 100%;text-align: center;margin-top: 5px;">' +
          '<div class="dropdown">' +
                        '<a class="dropdown-toggle" data-toggle="dropdown" style="color: #ffffff;"> <i class="fa fa-angle-down"></i>' +
                       '</a>' +
                       '<ul class="dropdown-menu" role="menu" style="min-width: 0px;min-height: 0px;padding: 0;">' +
                           '<li><a style="color:##" class="btn btn-white btn-xs pull-right" onclick="deletePost(' + postObject['Id'] + ')"><i class="fa fa-fw fa-times"></i></a></li>' +
                           '<li><a href="#postContent" class="btn btn-white btn-xs pull-right" onclick="editPost(' + postObject['Id'] + ')"><i class="fa fa-fw fa-edit"></i></a></li>' +
                       '</ul>' +
                   '</div>' +
                   '</div>' +
                   '</div>';
    }
    //REPORT DIV - END

    //POST HEADER - START
    var header = '<div class="panel-heading clearfix user-post" style="background-color: transparent;">' +
            '<div class="form-group" style="margin-bottom:0">' +
                '<div style="display: flex">' +
                    '<div class="pull-left" id="userLoadProfilePost' + postObject['Id'] + '" onclick="LoadUserProfile(' + postObject['UserId'] + ',true)" style="margin-right: 10px">' +
                        '<img id="userImagePost' + postObject['Id'] + '" src="' + postObject['userProfile'] + '" onerror="this.src=\'/Content/Images/defaultProfile.png\';" class="avartar-img  link-cursor">' +
                    '</div>' +
                    '<div style="flex: 1; width: 0;padding-right: 40px;">' +
                        '<div class="" style="margin-top: 0;margin-bottom: 0;">' +
                            '<strong>' +
                            '   <a class="text-capitalize" onclick="LoadUserProfile(' + postObject['UserId'] + ',true)" user-name link-cursor" id="userNamePost' + postObject['Id'] + '">' + postObject['userFullName'] + '</a>' +
                            '</strong>' +
                            '<strong class="house-name"> ' +
                                '<a class="link-cursor" href="/House/' + postObject['houseId'] + '">' +
                                    '<span style="font-size: 17px;">' +
                                        '<i class="fa fa-home icon-house"></i>' +
                                            postObject['houseName'] +
                                    '</span>' +
                                '</a>' +
                            '</strong>' +
                        '</div>' +
                        '<div class="" style="margin-top: 0px;margin-bottom: 0;">' +
        '                   <strong>' +
                                '<span class="post-time">' +
                                    '<i class="fa fa-globe icon-house"></i> ' +
                                    timeSince(postObject['CreateDate']) +
                                '</span>' +
                            '</strong>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>' +
        '</div>';
    //POST HEADER - END

    ////IMAGES DIV - START
    //var imagesDiv = "";
    //var listElement = "";
    //var isFirst = true;

    //imageList = postObject['Images'];
    

    //$.each(imageList, function (index,image) {
    //    if (isFirst) {
    //        isFirst = false;
    //        if (imageList.length == 1) {
    //            imagesDiv = '<div class="grid one-col" id="imagesPost' + postObject['Id'] + '">';
    //            if (parseInt(image.width) >= parseInt(image.height)) {
    //                listElement = listElement +
    //                    '<div  data-src="' + image.url + '" style="border:1px solid white"><img style="width:100%; height:auto" src="' +
    //                    image.thumbnailurl +
    //                    '" alt="The Last of us"></div>';
    //            } else {
    //                listElement = listElement +
    //                    '<div class="link-cursor" data-src="' + image.url + '" style="border:1px solid white" ><img style="height:100% !important; width:auto !important "src="' +
    //                    image.thumbnailurl +
    //                    '" alt="The Last of us"></div>';
    //            }
    //            return;
    //        } else {
    //            imagesDiv = '<div class="grid testetetet" id="imagesPost' + postObject['Id'] + '">';
    //        }
    //    }
    //    if (imageList.length % 2 == 0) {
    //        listElement = listElement + '<div style="position:relative;" data-src="' + image.url + '" style="border:1px solid white"><img style="position:absolute; width:100% !important; height:auto !important" src="' + image.thumbnailurl + '" ></div>';
    //    } else {
    //        listElement = listElement + '<div style="position:relative;display: inline-block;" data-src="' + image.url + '" style="border:1px solid white" ><img style="position:absolute; width:100% !important; height:auto !important" src="' + image.thumbnailurl + '"></div>';
    //    }
    //    listElement = listElement;
    //    imagesDiv += (listElement + "</div>");
    //    console.log(imagesDiv)

    //});
    ////IMAGES DIV - END


    //CONNECT ELEMENTS AND ADD TO TIMELINE - START
    $("#timeline").append('<li class="media">'
                    + '<div class="media-body">'
                        + '<div class="panel panel-default panel-feed">' + header
                            + '<div class="panel-body" >'
                                + '<p>' + postObject['Body'] + '</p>'
                                + '<div class="form-group">'
                                + '<div class="grid" id="imagesPost' + postObject['Id'] + '">'
                                + '</div>'

                               
                                + '</div>'
                                + '<div class="row" style="height:auto;">'
                                + postObject['EmbedCode']
                                + '</div>'
                            + '</div>'
                            + '<div class="view-all-comments" id="countComment' + postObject['Id'] + '"><a href="#"><i class="fa fa-comments-o"></i> View all</a> 10 comments</div>'
                            + '<ul class="comments" id="commentsArea' + postObject['Id'] + '">'
                            + '</ul>'
                            + '<ul class="comments">'
                             + '<li class="comment-form">'
                                    + '<div class="input-group" style="width: 100%;">'
                                    + '<a onclick="LoadUserProfile("' + currentUserId + '")" class="pull-left"><img src="'+currentUserProfileImage+'" onerror="this.src=\'/Content/Images/defaultProfile.png\';" class="media-object avatar-comment"></a>'
                                    + '<div class="media-body" style="padding-left: 10px;">' +
                                        '<input id="contentDetail' + postObject['Id'] + '" type="text" class="form-control input-cmnt" placeholder="Viết bình luận của bạn..."/>' +
                                         '<i class="fa fa-fw fa-chevron-circle-right " onclick="addComment(' + postObject['Id'] + ')" style="font-size: xx-large;color: #25AD9F;position: absolute;top: 2px;right: 0px;transition: right 0.2s; z-index:2"></i>' +
                                    '</div>'
        + '</div>'
        + '</li>'
        + '</ul>'
        + '</div>'
         + reportDiv
        + '</div>'
        + '</li>'

    );
    //CONNECT ELEMENTS AND ADD TO TIMELINE - END
    $("#imagesPost" + postObject['Id']).lightGallery({
        thumbnail: true,
        animateThumb: false,
        showThumbByDefault: false
    });
    $(".dropdown-toggle").dropdown();
}
function getCommentsForPost(postid) {
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
                $("#countComment" + postid).html("Chưa có bình luận nào");
            } else {
                $("#countComment" + postid).html(obj.length + " bình luận");
            }
            $.each(obj, function (index, comment) {
                addCommentToCommentArea(postid, comment);
            });
        },
        error: function (er) {
            alert(er);
        }

    });
}
function addCommentToCommentArea(postId, comment) { // Chức năng này support cho Function get comment. Add post vào commentsArea
    $("#commentsArea" + postId).append('<li>'
        + '<div class="media ">'
        + '<a onclick ="LoadUserProfile(' + comment["userId"] + ',true)"  class="pull-left link-cursor">'
        + '<img  src="' + comment['userProfile'] + '" onerror="this.src=\'/Content/Images/defaultProfile.png\';" class="media-object avartar-img link-cursor">'
        + '</a>'
        + '<div class="media-body">'
        + '<a onclick ="LoadUserProfile(' + comment["userId"] + ',true)" class="comment-author link-cursor">' + comment['fullName'] + ': </a>'
        + '<span>' + comment['detail'] + '</span>'
        + '<div class="comment-date">' + timeSince(comment['createdDate']) + ' trước </div>'
        + '</div>'
        + '</div>'

        + '</li>');

}
function getLastestTokenID(){
    return lastestTokenID;
}
//Report
function report(id) {
    $("#reportPostId").val(id);
    $("#reportContent").val("");
    $("#reportModal").modal();
}
function processSendReport() {
    var postId = $("#reportPostId").val();
    var reportContent = $("#reportContent").val();
    console.log("processSendReport: postid=" + postId + "reportContent=" + reportContent);
    sendReport(postId, reportContent);
}