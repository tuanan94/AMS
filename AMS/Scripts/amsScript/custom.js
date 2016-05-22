window.POPUPMODE = 0;
window.UPDATEMODE = 2;
window.INSERTMODE = 1;
window.UPDATEMODALTITLE = "Cập nhật dịch vụ hổ trợ";
window.INSERTMODALTITLE = "Thêm dịch vụ hổ trợ";
window.deleteHdSrvList = [];

function loadHelpdeskServiceType() {
    var action = "loadHdSrvCat";
    //    $("#hdSrvType").load("ManageRequest?action=" + action);
    window.POPUPMODE = window.INSERTMODE;
    $("#hdSrvModalTitle").text(window.INSERTMODALTITLE);

    $.ajax({
        url: "ManageRequest?action=" + action,
        type: "get",
        success: function (data) {
            console.log(data.HdSrvCategories);
            var objList = data.HdSrvCategories;
            var selectTagList = [];
            for (var i = 0; i < objList.length; i++) {
                var obj = objList[i];
                var selectTag = "<option value='" + obj.Id + "'>" + obj.Name + "</option>";
                selectTagList.push(selectTag);
            }
            $("#hdSrvCat").html(selectTagList);
        },
        error: function () {

        }
    });
};
function helpdeskServiceDetail(id) {
    var action = "hdSrvDetail&id=" + id;
    window.POPUPMODE = window.UPDATEMODE;
    $("#hdSrvModalTitle").text(window.UPDATEMODALTITLE);

    $.ajax({
        url: "ManageRequest?action=" + action,
        type: "get",
        success: function (data) {
            $("#hdSrvName").val(data.Name);
            $("#hdSrvPrice").val(data.Price);
            $("#hdSrvDesc").val(data.Description);
            $("#hdSrvId").val(data.Id);
            if (data.Status === 1) {
                $("#hdSrvStatusOn").prop("checked", true);
            } else {
                $("#hdSrvStatusOff").prop("checked", true);
            }

            var objList = data.HdSrvCategories;
            var selectTagList = [];
            for (var i = 0; i < objList.length; i++) {
                var obj = objList[i];
                var selectTag = {};
                if (obj.Id === data.HelpdeskServiceCategoryId) {
                    selectTag = "<option value='" + obj.Id + "' checked>" + obj.Name + "</option>";
                } else {
                    selectTag = "<option value='" + obj.Id + "'>" + obj.Name + "</option>";
                }
                selectTagList.push(selectTag);
            }
            $("#hdSrvCat").html(selectTagList);
            $("#addHelpdeskRequestModal").modal('show');
        },
        error: function () {

        }
    });
};
function deleteHelpdeskService(id) {
    $("#delBtnGroup").css("display", "block");
    $("#row_" + id).css("display", "none");
    deleteHdSrvList.push(id);
}
function cancelDeleteHelpdeskService() {
    for (var i = 0; i < deleteHdSrvList.length; i++) {
        $("#row_" + deleteHdSrvList[i]).css("display", "table-row");
    }
    window.deleteHdSrvList = [];
    setTimeout(function () {
        $("#delBtnGroup").css("display", "none");
    }, 1000);
}
function commitDeleteHelpdeskService() {
//    var postData = { hdSrvDeletedList:  }
    var action = "delHelpdeskSrv";
    var postData = JSON.stringify(window.deleteHdSrvList);
    $.ajax({
        url: "ManageRequest?action=" + action,
        type: "post",
        data: postData,
        success: function(data) {


        },
        error: function() {

        }
    });

    setTimeout(function () {
        $("#delBtnGroup").css("display", "none");
    }, 1000);
}
$(document).ready(function () {

    $("#addHelpdeskRequestForm").validate({
        rules: {
            hdSrvName: {
                required: true,
                maxlength: 255
            },
            hdSrvPrice: {
                required: true,
                number: true,
                maxlength: 9
            },
            hdSrvDesc: {
                maxlength: 500
            }
        },
        messages: {
            hdSrvName: {
                required: "Vui lòng nhập vào tên dịch vụ hổ trợ.",
                maxlength: "Tên dịch vụ hổ trợ không dài quá 255 ký tự."
            },
            hdSrvPrice: {
                required: "Vui lòng nhập vào giá dịch vụ hổ trợ.",
                maxlength: "Giá dịch vụ hổ trợ không quá 9 chữ số",
                number: "Giá dịch vụ hổ trợ phải là con số."
            },
            hdSrvDesc: {
                maxlength: "Mô tả dịch vụ sửa chữa không quá 500 ký tự."
            }
        },
        success: function (label, element) {
            label.parent().removeClass('error');
            label.remove();
        },
        submitHandler: function () {
            console.log($("#addHelpdeskRequestForm").serialize());
            var action = "";

            if (window.POPUPMODE === window.UPDATEMODE) {
                action = "updateHelpdeskSrv";
            } else {
                action = "addHelpdeskSrv";
            }
            $.ajax({
                url: "ManageRequest?action=" + action,
                type: "post",
                data: $("#addHelpdeskRequestForm").serialize(),
                success: function (data) {
                    if (data.StatusCode === 0) {
                        $("#successNotify").show().delay(3000).fadeOut(1000);
                    } else if (data.StatusCode === -1) {
                        $("#failedNotify").show().delay(3000).fadeOut(1000);
                    }
                }
            });
        }
        //            $("button#addHdSrv").click(function () {
        //        }
    });

    $("#addHdSrv").on('click', function () {
        console.log($("#addHelpdeskRequestForm").serialize());
        $("#addHelpdeskRequestForm").valid();
    });

    $("#addHelpdeskRequestModal").on("hidden.bs.modal", function () {
        document.getElementById("addHelpdeskRequestForm").reset();
    });
});

