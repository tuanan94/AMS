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
                                '" alt="The Last of us"></div>';
                        } else {
                            listElement = listElement +
                                '<div class="link-cursor" data-src="' + image.url + '" style="border:1px solid white" ><img style="height:100% !important; width:auto !important "src="' +
                                image.thumbnailurl +
                                '" alt="The Last of us"></div>';
                        }
                        return;
                    } else {
                        $("#imagesPost" + image.postId).removeClass("one-col").addClass("testetetet");
                    }
                }
                //                if (parseInt(image.width) >= parseInt(image.height)) {
                //                    listElement = listElement + '<div style="position:relative;display: inline-block;" data-src="' + image.url + '" style="border:1px solid white"><img style="position:absolute; width:100% !important; height:auto !important" src="' + image.thumbnailurl + '" ></div>';
                //                } else {
                ////                    listElement = listElement + '<div style="position:relative" data-src="' + image.url + '" style="border:1px solid white" ><img style="height:100% !important; width:auto !important" src="' + image.thumbnailurl + '"></div>';
                //                    listElement = listElement + '<div style="position:relative;display: inline-block;" data-src="' + image.url + '" style="border:1px solid white" ><img style="position:absolute; width:100% !important; height:auto !important" src="' + image.thumbnailurl + '"></div>';
                //                }

                if (obj.length % 2 == 0) {
                    listElement = listElement + '<div style="position:relative;" data-src="' + image.url + '" style="border:1px solid white"><img style="position:absolute; width:100% !important; height:auto !important" src="' + image.thumbnailurl + '" ></div>';
                } else {
                    listElement = listElement + '<div style="position:relative;display: inline-block;" data-src="' + image.url + '" style="border:1px solid white" ><img style="position:absolute; width:100% !important; height:auto !important" src="' + image.thumbnailurl + '"></div>';
                }

            });
            listElement = listElement + "</div>";
            currentElement.append(listElement);
            currentElement.lightGallery({
                thumbnail: true,
                animateThumb: false,
                showThumbByDefault: false
            });
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
        success: function (successData) {
            if (successData.data) {
                var obj = successData;
                var commentTag = $("#countComment" + postid);
                commentTag.children("span:nth(0)").text(obj.data.length);
                commentTag.children("span:nth(0)").data("currentIndex", obj.data.length);
                commentTag.children("span:nth(1)").text(obj.totalComment);
                commentTag.children("span:nth(1)").data("totalItem", obj.totalComment);

                if (obj.data.length < obj.totalComment) {
                    $("#loadMoreComment" + postid).removeClass("hide");
                } 
                $.each(obj.data, function (index, comment) {
                    addCommentToCommentArea(postid, comment);
                    if (index === 0) {
                        $("#commentsArea" + postid).data("lastCommentId", comment.id);
                    }
                    if (obj.data.length === (index + 1)) {
                        $("#commentsArea" + postid).data("newestCommentId", comment.id);
                    }
                });
                $("#countComment" + postid).children("span, strong").removeClass("hide");
            }
            else {
                $("#commentsArea" + postid).data("lastGetComment", successData.lastGetComment);
                $("#countComment" + postid).children("span, strong").addClass("hide");
                $("#countComment" + postid).children("i").removeClass("hide");
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
                if (successData === 'success') {
                    getNewCommentsForPost(postId);
                    $("#contentDetail" + postId).val("");
                } else {
                    alert(successData);
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
                thisElement.text(timeSince(thisElement.data("commentDate")) +" trước");
            });
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
            if (data.data) {
                var listData = data.data;
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
                $("#commentsArea" + postid).data("lastGetComment", data.lastGetComment);
                //                    $("#countComment" + postid).html("Chưa có bình luận nào");
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