$(document).ready(function () {



});

function loadMoreText(id) {
    /*https://codepen.io/maxds/pen/jgeoA/*/
    // Configure/customize these variables.

    var showChar = 300;  // How many characters are shown by default
    var ellipsestext = "...";
    var moretext = "tất cả";
    var lesstext = "thu lại ";


    var content = $("#postBody" + id).html();

    if (content) {
        if (content.length > showChar) {

            var c = content.substr(0, showChar);
            var h = content.substr(showChar, content.length - showChar);

            var html = c + '<span class="moreellipses">' + ellipsestext + '&nbsp;</span><span class="morecontent"><span>' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink">' + moretext + '</a></span>';

            $("#postBody" + id).html(html);
        }

        $("#postBody" + id + " .morelink").click(function () {
            if ($(this).hasClass("less")) {
                $(this).removeClass("less");
                $(this).html(moretext);
            } else {
                $(this).addClass("less");
                $(this).html(lesstext);
            }
            $(this).parent().prev().toggle();
            $(this).prev().toggle();
            return false;
        });
    }
}

function deletePost(postId) { //this function just display Confirm message
    //alert("deletePost" + postId)
    $("#deletePostModal").modal("show");
    $("#confirmDeletePostId").val(postId);
}

function sendDeleteRequest() {
    var id = $("#confirmDeletePostId").val();
    $.ajax({
        url: "/Post/deletePost",
        type: "POST",
        data: {
            postId: id,
        }, success: function (data) {
            if (data.StatusCode === 0) {
                $("#userPostItem" + data.Data).hide("300", function () {
                    $(this).remove();
                });
                $("#confirmDeletePostId").val("");
                $("#deletePostModal").modal("hide");

            } else {
                location.reload();
            }
        },
        error: function (er) {
            alert(er);
        }
    });
}


function appenImageToPost(index, id) {
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
            var twoVerticalImg = false;
            if(obj && obj.length === 2) {
                var image1 = obj[0];
                var image2 = obj[1];
                if(image1.height > image1.width && image2.height > image2.width) {
                    $.each(obj, function (index, image) {
                        if (isFirst) {
                            $("#imagesPost" + image.postId).html("");
                            $("#imagesPost" + image.postId).removeClass("one-col");
                            isFirst = false;
                            currentElement = $("#imagesPost" + image.postId);
                        }
                        listElement = listElement + '<div style="float:left;width:50%;height:50%" data-src="' + image.url + '" style="border:1px solid white" >' +
                                                        '<img style="max-width:100%; height:100%" src="' + image.thumbnailurl + '" />' +
                                                    '</div>';
                    });
                    twoVerticalImg = true;
                } else if (image1.width > image1.height && image2.width > image2.height) {
                    $.each(obj, function (index, image) {
                        if (isFirst) {
                            $("#imagesPost" + image.postId).html("");
                            $("#imagesPost" + image.postId).removeClass("one-col");
                            isFirst = false;
                            currentElement = $("#imagesPost" + image.postId);
                        }
                        listElement = listElement + '<div style="float:left;width:100%;height:50%" data-src="' + image.url + '" style="border:1px solid white" >' +
                                                        '<img style="max-width:100%; width:100%" src="' + image.thumbnailurl + '" />' +
                                                    '</div>';
                    });
                    twoVerticalImg = true;
                }
            }

            if (!twoVerticalImg) {
                $.each(obj, function (index, image) {
                    if (isFirst) {
                        $("#imagesPost" + image.postId).html("");
                        currentElement = $("#imagesPost" + image.postId);
                        isFirst = false;
                        if (obj.length == 1) {
                            $("#imagesPost" + image.postId).removeClass("testetetet").addClass("one-col");
                            if (parseInt(image.width) >= parseInt(image.height)) {
                                listElement = listElement +
                                    '<div  data-src="' + image.url + '" style="border:1px solid white"><img style="width:100%; height:auto" src="' +
                                    image.thumbnailurl +
                                    '" alt=""></div>';
                            } else {
                                listElement = listElement +
                                    '<div class="link-cursor" data-src="' + image.url + '" style="border:1px solid white" ><img style="height:100% !important; width:auto !important "src="' +
                                    image.thumbnailurl +
                                    '" alt=""/></div>';
                            }
                            return;
                        } else {
                            //                        $("#imagesPost" + image.postId).removeClass("one-col").addClass("testetetet");
                            $("#imagesPost" + image.postId).removeClass("one-col");
                        }
                        if (obj.length === 3) {
                            if (image.width > image.height) {
                                listElement = listElement + '<div style="float:left;width:100%;height:50%" data-src="' + image.url + '" style="border:1px solid white" >' +
                                '<img style="max-width:100%; width:100%" src="' + image.thumbnailurl + '" />' +
                                '</div>';
                                return;
                            }
                        }
                    }
                    listElement = listElement + '<div style="float:left;width:50%;height:50%" data-src="' + image.url + '" style="border:1px solid white" >' +
                        '<img style="max-width:100%; width:100%" src="' + image.thumbnailurl + '" />' +
                        '</div>';

                });
            }
            
            listElement = listElement + "</div>";
            if (currentElement) {
                currentElement.append(listElement);
                if (currentElement.data("lightGallery")) {
                    currentElement.data("lightGallery").destroy(true);
                }
                currentElement.lightGallery({
                    thumbnail: true,
                    animateThumb: false,
                    showThumbByDefault: false
                });
            }
        },
        error: function (er) {
            alert(er);
        }

    });
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
            lastId: 0,
        },
        success: function (data) {
            if (data.StatusCode === 0) {
                var obj = data.Data;
                if (obj.listComment) {
                    var listData = obj.listComment;
                    var commentTag = $("#countComment" + postid);
                    commentTag.children("span:nth(0)").text(listData.length);
                    commentTag.children("span:nth(0)").data("currentIndex", listData.length);
                    commentTag.children("span:nth(1)").text(obj.totalComment);
                    commentTag.children("span:nth(1)").data("totalItem", obj.totalComment);

                    if (listData.length < obj.totalComment) {
                        $("#loadMoreComment" + postid).removeClass("hide");
                    }
                    $.each(listData, function (index, comment) {
                        addCommentToCommentArea(postid, comment);
                        if (index === 0) {
                            $("#commentsArea" + postid).data("lastCommentId", comment.id);
                        }
                        if (listData.length === (index + 1)) {
                            $("#commentsArea" + postid).data("newestCommentId", comment.id);
                        }
                    });
                    $("#countComment" + postid).children("span, strong").removeClass("hide");
                }
                else {
                    $("#commentsArea" + postid).data("lastGetComment", obj.lastGetComment);
                    $("#countComment" + postid).children("span, strong").addClass("hide");
                    $("#countComment" + postid).children("i").removeClass("hide");
                }
            } else {
                $("#amsMsgText").html("Rất tiếc! Bài viết này đã không còn tồn tại trong hệ thống.");
                $("#amsMessageModal").unbind();
                $("#amsMessageModal").modal("show");
                $("#amsMessageModal").data("postId", data.Data);
                $("#amsMessageModal").on("hidden.bs.modal", function () {
                    var postId = $("#amsMessageModal").data("postId");
                    $("#userPostItem" + postId).hide("300", function () {
                        $(this).remove();
                    });
                });
            }
        },
        error: function (er) {
            alert(er);
        }

    });
}


function addComment(postId) {
    var content = $("#contentDetail" + postId).val();
    var re = /<[a-z][\s\S]*>/i;

    if (content && (!re.exec(content))) {
        $.ajax({
            url: "/Post/CreateComment",
            type: "POST",
            data: {
                detail: content,
                postId: postId,
            },
            success: function (successData) {
                if (successData.StatusCode === 0) {
                    getNewCommentsForPost(postId);
                    $("#contentDetail" + postId).val("");
                } else if (successData.StatusCode === 2) {
                    $("#amsMsgText").html("Rất tiếc! Bài viết này đã không còn tồn tại trong hệ thống.");
                    $("#amsMessageModal").unbind();
                    $("#amsMessageModal").modal("show");
                    $("#amsMessageModal").data("postId", successData.Data);
                    $("#amsMessageModal").on("hidden.bs.modal", function () {
                        if (location.pathname.indexOf("Post/Detail") > -1) {
                            location.href = "/";
                        } else {
                            var postId = $("#amsMessageModal").data("postId");
                            $("#userPostItem" + postId).hide("500", function () {
                                $(this).remove();
                            });
                        }
                    });
                } else {
                    $("#amsMsgText").html("Việc bình luận thất bại xin vui lòng thử lại !");
                    $("#amsMessageModal").unbind();
                    $("#amsMessageModal").modal("show");
                }
            },
            error: function (er) {
                alert(er);
            }
        });
    }
}

function getNewCommentsForPost(postid) {
    if (postid == null) {
        alert("postid == null");
        return;
    }
    var newestCommentId = $("#commentsArea" + postid).data("newestCommentId");
    if (!newestCommentId) {
        newestCommentId = 0;
    }

    $.ajax({
        url: "/Post/GetNewCommentsForPost",
        type: "GET",
        data: {
            postId: postid,
            newestCommentId: newestCommentId
        },
        success: function (data) {

            if (data.StatusCode === 0) {
                var objectData = data.Data;
                $.each(objectData.listComment, function (index, comment) {
                    addCommentToCommentArea(postid, comment, false);
                    //                if (data.Data.listComment.length == (index + 1)) {
                    //                    $("#commentsArea" + postid).data("lastGetComment", data.Data.lastGetComment);
                    //                }
                });

                if (objectData.listComment.length !== 0) {
                    var commentTag = $("#countComment" + postid);
                    var lastViewItem = commentTag.children("span:nth(0)").data("currentIndex");
                    var totalItem = commentTag.children("span:nth(1)").data("totalItem");

                    if ((!lastViewItem) || (!totalItem)) {
                        lastViewItem = 0;
                        totalItem = 0;
                    }

                    var lastCommentListLength = objectData.listComment.length;
                    totalItem = parseInt(totalItem, 10) + lastCommentListLength;
                    lastViewItem = parseInt(lastViewItem, 10) + lastCommentListLength;

                    commentTag.children("span:nth(0)").data("currentIndex", lastViewItem);
                    commentTag.children("span:nth(1)").data("totalItem", totalItem);
                    commentTag.children("span:nth(0)").text(lastViewItem);
                    commentTag.children("span:nth(1)").text(totalItem);

                    $("#countComment" + postid).children("span, strong").removeClass("hide");
                    $("#countComment" + postid).children("i:nth(1)").addClass("hide");
                }
                $("#commentsArea" + postid).data("newestCommentId", objectData.newestCommentId);

                $("#commentsArea" + postid + " .comment-date").each(function () {
                    var thisElement = $(this);
                    thisElement.text(timeSince(thisElement.data("commentDate")) + " trước");
                });
            } else {
                $("#amsMsgText").html("Rất tiếc! Bài viết này đã không còn tồn tại trong hệ thống.");
                $("#amsMessageModal").unbind();
                $("#amsMessageModal").modal("show");
                $("#amsMessageModal").data("postId", data.Data);
                $("#amsMessageModal").on("hidden.bs.modal", function () {
                    if (location.pathname.indexOf("Post/Detail") > -1) {
                        location.href = "/";
                    } else {
                        var postId = $("#amsMessageModal").data("postId");
                        $("#userPostItem" + postId).hide("500", function () {
                            $(this).remove();
                        });
                    }
                });
            }
        },
        error: function (er) {
            alert(er);
        }
    });
}

function loadMorePost(postid) {
    if (postid == null) {
        alert("postid == null");
        return;
    }
    var lastId = $("#commentsArea" + postid).data("lastCommentId");
    if (!lastId) {
        lastId = 0;
    }
    $.ajax({
        url: "/Post/getCommentsForPost",
        type: "GET",
        data: {
            postId: postid,
            lastId: lastId
        },
        success: function (data) {
            if (data.StatusCode === 0) {
                var obj = data.Data;
                if (obj.listComment) {
                    var listData = obj.listComment;

                    var commentTag = $("#countComment" + postid);
                    var lastViewItem = commentTag.children("span:nth(0)").data("currentIndex");
                    var totalItem = commentTag.children("span:nth(1)").data("totalItem");

                    commentTag.children("span:nth(0)").data("currentIndex", parseInt(lastViewItem, 10) + listData.length);
                    if ((parseInt(lastViewItem, 10) + listData.length) === parseInt(totalItem, 10)) {
                        $("#loadMoreComment" + postid).addClass("hide");
                    }
                    commentTag.children("span:nth(0)").text(parseInt(lastViewItem, 10) + listData.length);

                    $.each(listData, function (index, comment) {
                        addCommentToCommentArea(postid, comment, true);
                        if (listData.length === (index + 1)) {
                            $("#commentsArea" + postid).data("lastCommentId", comment.id);
                        }
                    });
                } else {
                    $("#commentsArea" + postid).data("lastGetComment", obj.lastGetComment);
                    //                    $("#countComment" + postid).html("Chưa có bình luận nào");
                }
            } else {
                $("#amsMsgText").html("Rất tiếc! Bài viết này đã không còn tồn tại trong hệ thống.");
                $("#amsMessageModal").unbind();
                $("#amsMessageModal").modal("show");
                $("#amsMessageModal").data("postId", data.Data);
                $("#amsMessageModal").on("hidden.bs.modal", function () {
                    if (location.pathname.indexOf("Post/Detail") > -1) {
                        location.href = "/";
                    } else {
                        var postId = $("#amsMessageModal").data("postId");
                        $("#userPostItem" + postId).hide("500", function () {
                            $(this).remove();
                        });
                    }
                });
            }
        },
        error: function (er) {
            alert(er);
        }
    });
}

function addCommentToCommentArea(postId, comment, isPrepend) {
    var elementStr = '<li>'
        + '<div class="media ">'
            + '<a onclick ="LoadUserProfile(' + comment["userId"] + ',true)"  class="pull-left link-cursor">'
                + '<img  src="' + comment['userProfile'] + '" onerror="this.src=\'/Content/Images/defaultProfile.png\';" class="media-object avartar-img link-cursor">'
            + '</a>'
            + '<div class="media-body">'
                + '<a onclick ="LoadUserProfile(' + comment["userId"] + ',true)" class="comment-author link-cursor">' + comment['fullName'] + ': </a>'
                + '<span>' + comment['detail'] + '</span>'
                + '<div class="comment-date" data-comment-date= "' + comment['createdDate'] + '">' + timeSince(comment['createdDate']) + ' trước </div>'
            + '</div>'
        + '</div>'
        + '</li>';
    if (isPrepend) {
        $("#commentsArea" + postId).prepend(elementStr);
    } else {
        $("#commentsArea" + postId).append(elementStr);
    }
}

function getPostDetail(postId) {
    $.ajax({
        url: "/Post/GetPostDetail",
        type: "get",
        data: {
            postId: postId
        },
        success: function (data) {
            if (data.StatusCode === 0) {
                var obj = data.Data;
                $("#userAvatarModal img").prop("src", obj.userProfile);
                $("#postContentModal").val(obj.Body.replace(/<br *\/?>/gi, '\n'));

                var imagePreviewRow = $("#previewListModal");
                if (obj.ListImages.length !== 0) {
                    for (var i = 0; i < obj.ListImages.length; i++) {
                        var img = obj.ListImages[i];
                        imagePreviewRow.append(parseJsonToImgReview(img));
                    }
                }
                if (obj.EmbedCode) {
                    $("#previewContentModal").append(getYoutubeFrameFromText(obj.EmbedCode));
                    $("#previewContentModal").data("embedCode", getYoutubeFrameFromText(obj.EmbedCode));
                    $("#previewEmbedModal").removeClass("hide");
                }

                $("#editPostModal").unbind();
                $("#editPostModal").modal("show");
                $("#editPostModal").on("shown.bs.modal", function () {
                    textAreaAdjust(document.getElementById("postContentModal"));
                    var textAreaElement = $("#postContentModal");
                    textAreaElement.unbind();
                    textAreaElement.on("keyup", function () {
                        textAreaAdjust(this);
                    });
                });

                $("#editPostModal").data("postId", obj.Id);
                $("#editPostModal").on("hidden.bs.modal", function () {
                    $("#previewListModal").html("");
                    $("#previewContentModal").removeData("embedCode");
                    $("#postContentModal").val("");
                    $("#previewContentModal").html("");
                    textAreaAdjust(document.getElementById("postContentModal"));
                    window.removeReviewImage = [];
                    $("#userAvatarModal img").prop("src", "");
                    $(this).removeData("postId");
                });

                $("#btnUpdatePost").unbind();
                $("#btnUpdatePost").on("click", function () {
                    $("#editPostModal").modal({
                        backdrop: 'static',
                        keyboard: false
                    });

                    var listImageElement = $("#previewListModal").children();
                    var imgList = [];
                    var imgObj = {};
                    listImageElement.each(function () {
                        var thisEle = $(this);
                        var imgId = thisEle.data("imgId");
                        if (!imgId) {
                            imgId = 0;
                        }
                        var thumbnailUrl = thisEle.data("imgThumbUrl");
                        var url = thisEle.data("imgUrl");
                        var originUrl = thisEle.data("originImgUrl");
                        if(!originUrl) {
                            originUrl = "";
                        }
                        imgObj = {
                            id: imgId,
                            thumbnailurl: thumbnailUrl,
                            url: url,
                            originUrl: originUrl
                        }
                        imgList.push(imgObj);
                    });

                    var bodyContent = $("#postContentModal").val();
                    var postId = $("#editPostModal").data("postId");
                    var embedEdCode = $("#previewContentModal").data("embedCode");

                    if (embedEdCode) {
                        var youTubeLink = getYoutubeLinkFromText(embedEdCode);
                        if (youTubeLink !== -1) {
                            embedEdCode = "https://youtu.be/" + youTubeLink;
                        } else {
                            embedEdCode = null;
                        }
                    } else if (imgList.length === 0) {
                        var youTubeLinkBodyText = getYoutubeLinkFromText(bodyContent);
                        if (youTubeLinkBodyText !== -1) {
                            embedEdCode = "https://youtu.be/" + youTubeLinkBodyText;
                        } else {
                            embedEdCode = null;
                        }
                    }

                    var newPostData = {}
                    if (!window.removeReviewImage) {
                        window.removeReviewImage = [];
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Post/Update",
                        data: {
                            Id: postId,
                            Body: bodyContent,
                            EmbedCode: embedEdCode,
                            ListImages: imgList,
                            ListImgRemoved: window.removeReviewImage
                        },
                        success: function (data) {
                            if (data.StatusCode === 0) {
                                var obj = data.Data;
                                if (obj.ImageCount !== 0) {
                                    appenImageToPost(0, obj.Id);
                                } else {
                                    $("#imagesPost" + obj.Id).html("");
                                }
                                $("#postBody" + obj.Id).html(replaceNewLineWithBrTag(obj.Body));
                                if (obj.EmbedCode) {
                                    var youtubeFrame = getYoutubeFrameFromText(obj.EmbedCode);
                                    if (youtubeFrame !== "") {
                                        $("#embedYoutubeFrame").html(youtubeFrame);
                                    }
                                } else {
                                    $("#embedYoutubeFrame").html("");
                                }
                                $("#editPostModal").modal({
                                    backdrop: 'true',
                                    keyboard: true
                                });
                                $("#editPostModal").modal("hide");
                            }
                        }
                    });

                });
            }
        }
    });
}

function parseJsonToImgReview(img) {
    if (img.id) {
        return '<div id="previewImageModal' + img.id + '" data-img-id="' + img.id + '" data-img-thumb-url="' + img.thumbnailurl + '" ' + ' data-img-url="' + img.url + '" class="img-review">' +
                            '<img onclick="removeImgPreview(\'' + img.id + '\')" src="/Content/images/delete.png" class="img-review-del-btn" />' +
                            '<img src="' + img.userCropUrl + '" class="preview-img"/>' +
                            '</div>';
    } else {
        var tempId = new Date().getTime();
        return '<div id="previewImageModal' + tempId + '" data-img-thumb-url="' + img.thumbnailUrl + '" ' + ' data-img-url="' + img.imageUrl + '" data-origin-img-url="' + img.originUrl + '" class="img-review">' +
                            '<img onclick="removeImgPreview(\'' + tempId + '\')" src="/Content/images/delete.png" class="img-review-del-btn" />' +
                            '<img src="' + img.thumbnailUrl + '" class="preview-img"/>' +
                            '</div>';
    }
}

function removeImgPreview(id) {
    var removeElement = $("#previewImageModal" + id);
    if (removeElement.data("imgId")) {
        if (window.removeReviewImage) {
            window.removeReviewImage.push(id);
        } else {
            window.removeReviewImage = [];
            window.removeReviewImage.push(id);
        }
    }
    removeElement.remove();
}

function showEditEmbedCode(id) {
    var imageListTag = $("#previewListModal").children();
    if (imageListTag.length === 0) {
        $("#embedCodeEditModal").val($("#previewContentModal").data("embedCode"));
        $("#btnOkSaveEnbedCode").unbind();
        $("#btnOkSaveEnbedCode")
            .on("click",
                function () {
                    $("#previewContentModal").data("embedCode", $("#embedCodeEditModal").val());
                    $("#addEmbedModal").modal("hide");
                    $("#previewContentModal").html("");
                    $("#previewEmbedModal").removeClass("hide");
                    $("#previewContentModal")
                        .append(getYoutubeFrameFromText($("#previewContentModal").data("embedCode")));
                });
        $("#addEmbedModal").modal("show");
    }
}

function innitializeCroper() {
    var options = {
        preview: '.reviewImg',
        aspectRatio: 1,
        multiple: false,
        minCropBoxWidth: 243,
        minCropBoxHeight: 243,
        dragCrop: false,
        movable: true,
        dashed: false,
        zoomable: false,
        resizable: true,
        crop: function (e) {
        },
        zoom: function (e) {
            console.log(e.type, e.detail.ratio);
        },
        built: function () {
        }
    };
    var tempImage = document.getElementById("testLoadImage");
    window.cropper = new Cropper(tempImage, options);
}

function showEditImagePopup() {
    $("#editImageModal").unbind();
    $("#editImageModal").on("hidden.bs.modal", function () {
        window.cropper.destroy();
    }).on("shown.bs.modal", function () {
        innitializeCroper();
    }).modal("show");
}

function getCroptImageSizeModal() {
    var imageData = window.cropper.getData();
    // progessBar(true);
    $.ajax({
        url: "/Management/Image/UploadPostImageResize",
        type: "POST",
        data: {
            imgUrl: $("#testLoadImage").prop("src").replace(location.origin, ""),
            width: parseInt(imageData.width),
            height: parseInt(imageData.height),
            "x": parseInt(imageData.x),
            "y": parseInt(imageData.y)
        },
        success: function (successData) {
            $("#previewListModal").prepend(parseJsonToImgReview(successData.Data));
            $("#editImageModal").modal("hide");
        },
        error: function (er) {
            alert(er);
            //progessBar(false);
        }
    });
}

function addImageModal() {
    var embedData = $("#previewContentModal").data("embedCode");
    if (!embedData) {

        $("#uploadPostImageModal").click();
        $("#uploadPostImageModal").unbind();
        $("#uploadPostImageModal").change(function () {
            var data = new FormData();

            //        data.append("dir", "@SLIM_CONFIG.dirPostImage");

            var files = $("#uploadPostImageModal").get(0).files;
            if (files.length > 0) {
                var imageFile = files[0];
                if (checkURL(imageFile.name)) {
                    data.append("image", files[0]);
                    // progessBar(true);
                    $.ajax({
                        url: "/Management/Image/UploadOriginalImage",
                        type: "POST",
                        processData: false,
                        contentType: false,
                        data: data,
                        success: function (successData) {
                            var thumbnailData = successData.Data.oriImageUrl;
                            $("#testLoadImage").unbind();
                            $("#testLoadImage").prop("src", thumbnailData);
                            $("#testLoadImage").on("load", function () {
                                showEditImagePopup();
                            });
                            $("#uploadPostImageModal").val("");
                        },
                        error: function (er) {
                            alert(er);
                        }
                    });
                } else {
                    $("#amsMsgText").text("Chỉ hổ trợ hình ảnh có định đạng [png, jpeg, bmp]");
                    $("#amsMessageModal").modal("show");
                }
            }
        });
    }
}



function getId(url) {
    var regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/;
    var match = url.match(regExp);

    if (match && match[2].length == 11) {
        return match[2];
    } else {
        return 'error';
    }
}
function getYoutubeFrame(youtubeUrl) {
    var myId = getId(youtubeUrl);
    if (myId !== "error") {
        return '<iframe width="560" height="315" src="//www.youtube.com/embed/' +
            myId +
            '" frameborder="0" allowfullscreen></iframe>';
    } else {
        return -1;
    }
}

function getYoutubeFrameFromText(commentText) {
    if (commentText) {
        var text = commentText.replace(/["'\n]/g, "").split(' ');
        for (i = 0; i < text.length; i++) {
            var test = getYoutubeFrame(text[i]);
            if (test !== -1) {
                return test;
            }
        }
    }
    return "";
}

function getYoutubeLinkFromText(commentText) {
    if (commentText) {
        var text = commentText.replace(/["'\n]/g, "").split(' ');
        for (i = 0; i < text.length; i++) {
            var test = getId(text[i]);
            if (test !== "error") {
                return test;
            }
        }
    }
    return "";
}

function replaceNewLineWithBrTag(commentText) {
    if (commentText) {
        return commentText.replace(/[\n]/g, "<br/>");
    }
    return "";
}

function checkURL(url) {
    return (url.toLowerCase().match(/\.(jpeg|jpg|bmp|png)$/) != null);
}