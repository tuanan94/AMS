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