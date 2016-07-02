function sendReport(postid,content) {
    console.log("Report post" + postid + content);
    $.ajax({
        url: "/Report/addReport",
        type: "POST",
        data: {
            postId: postid,
            reportContent: content
        },
        success: function (successData) {
            alert(successData)
        },
        error: function (er) {
            alert(er)
        }
    });

}