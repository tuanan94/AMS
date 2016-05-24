window.POPUPMODE = 0;
window.UPDATEMODE = 2;
window.INSERTMODE = 1;
window.UPDATEMODALTITLE = "Cập nhật dịch vụ hổ trợ";
window.UpdateServiceFailedMsg = "Cập nhật dịch vụ hổ trợ thất bại";
window.UpdateServiceSuccessMsg = "Cập nhật dịch vụ hổ trợ thành công";
window.INSERTMODALTITLE = "Thêm dịch vụ hổ trợ";
window.AddServiceFailedMsg = "Thêm dịch vụ hổ trợ thất bại";
window.AddServiceSuccessMsg = "Thêm dịch vụ hổ trợ thành công";

window.UpdateHdSrvCategoryModalTitle = "Cập nhật nhóm dịch vụ hổ trợ";
window.UpdateHdSrvCategoryFailedMsg = "Cập nhật nhóm dịch vụ hổ trợ thất bại";
window.UpdateHdSrvCategorySuccessMsg = "Cập nhật nhóm dịch vụ hổ trợ thành công";
window.InsertHdSrvCategoryModalTitle = "Thêm nhóm dịch vụ hổ trợ";
window.AddHdSrvCategoryFailedMsg = "Thêm nhóm dịch vụ hổ trợ thất bại";
window.AddHdSrvCategorySuccessMsg = "Thêm nhóm dịch vụ hổ trợ thành công";

window.deleteHdSrvList = new Array();
window.deleteHdSrvCatList = new Array();

function loadHelpdeskServiceType() {
    var action = "loadHdSrvCat";
    //    $("#hdSrvType").load("ManageRequest?action=" + action);
    window.POPUPMODE = window.INSERTMODE;
    $("#hdSrvModalTitle").text(window.INSERTMODALTITLE);
    //    $.ajax({
    //        url: "ManageRequest?action=" + action,
    //        type: "get",
    //        success: function (data) {
    //            console.log(data.HdSrvCategories);
    //            var objList = data.HdSrvCategories;
    //            var selectTagList = [];
    //            for (var i = 0; i < objList.length; i++) {
    //                var obj = objList[i];
    //                var selectTag = "<option value='" + obj.Id + "'>" + obj.Name + "</option>";
    //                selectTagList.push(selectTag);
    //            }
    //            $("#hdSrvCat").html(selectTagList);
    //        },
    //        error: function () {
    //        }
    //    });
    getHelpdeskServiceType("hdSrvCat");
};
function getHelpdeskServiceType(tagId, callback) {
    var action = "loadHdSrvCat";
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
            $("#" + tagId).html(selectTagList);
            callback();
        },
        error: function () {
        }
    });
}
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
            $("#addHelpdeskRequestModal").modal("show");
        },
        error: function () {
        }
    });
};
function deleteHelpdeskService(id) {
    $("#delBtnGroup").removeClass("show").addClass("show");
    $("#row_" + id).css("display", "none");
    deleteHdSrvList.push(id);
}
function cancelDeleteHelpdeskService() {
    for (var i = 0; i < deleteHdSrvList.length; i++) {
        $("#row_" + deleteHdSrvList[i]).css("display", "table-row");
    }
    window.deleteHdSrvList = new Array();
    $("#delBtnGroup").removeClass("show").addClass("hide");;

}
function commitDeleteHelpdeskService() {
    var postData = { hdSrvDeletedList: window.deleteHdSrvList }
    var action = "delHelpdeskSrv";
    $.ajax({
        url: "ManageRequest?action=" + action,
        type: "post",
        data: postData,
        dataType: "json",
        traditional: true,
        success: function (data) {
            reloadHdSrvTable();
            if (data.StatusCode === 0) {
                window.deleteHdSrvList = new Array();
            } else {
                $("#delBtnGroup").removeClass("show");
                $("#delBtnGroup").addClass("hide");
                $("#messageModal .msgContent").text("Xóa dịch vụ hổ trợ thất bại!");
            }
        },
        error: function () {

        }
    });
    setTimeout(function () {
        $("#delBtnGroup").removeClass("show");
        $("#delBtnGroup").addClass("hide");
    }, 1000);
}
function reloadHdSrvTable() {
    var action = "loadHdSrvTable";
    $("#hdSrvTblBody").load("ManageRequest?action=" + action);
}
function searchHdSrv() {
    var mode = parseInt($("#hdReqFilterType").find("option:selected").val(), 10);
    var searchStr = $("#searchStr").val();
//    if (searchStr !== "" || (searchStr === "" && mode !== 0)) {
        var action = "searchHdSrv";
        $.ajax({
            url: "ManageRequest?action=" + action,
            type: "post",
            data: $("#searchHdSrv").serialize(),
            success: function (data) {
                $("#hdSrvTblBody").html(data);
            },
            error: function () {

            }
        });
//    }
}
function openHdSrvCategoryModal() {
    window.POPUPMODE = window.INSERTMODE;
    $("#hdSrvCateModalTitle").text(window.InsertHdSrvCategoryModalTitle);
}
function hdSrvCategoryDetail(id) {
    var action = "hdSrvCatDetail&id=" + id;
    window.POPUPMODE = window.UPDATEMODE;
    $("#hdSrvCateModalTitle").text(window.UpdateHdSrvCategoryModalTitle);
    $.ajax({
        url: "ManageRequest?action=" + action,
        type: "get",
        success: function (data) {
            if (data.StatusCode === 0) {
                var returnData = data.Data;
                $("#hdSrvCatName").val(returnData.Name);
                $("#hdSrvCatId").val(returnData.Id);
                $("#hdSrvCategoryModal").modal("show");
            } else if (data.StatusCode === 4) {

            } else {

            }
        }
    });
}
function reloadHdSrvCategoryTable() {
    var action = "loadHdSrvCatTable";
    $("#hdSrvCatTblBody").load("ManageRequest?action=" + action);
}
function deleteHdSrvCategory(id) {
    $("#delHdSrvBtnGroup").removeClass("show").addClass("show");
    $("#rowHdSrvCat_" + id).css("display", "none");
    deleteHdSrvCatList.push(id);
}
function searchHdSrvCategory() {
    var searchStr = $("#searchStrHdSrvCat").val();
//    if (searchStr !== "") {
        var action = "searchHdSrvCategory";
        $.ajax({
            url: "ManageRequest?action=" + action,
            type: "post",
            data: $("#searchStrHdSrvCatForm").serialize(),
            success: function (data) {
                $("#hdSrvCatTblBody").html(data);
            },
            error: function () {

            }
        });
//    }
}
function commitDeleteHdSrvCategory() {
    var postData = { hdSrvCatDeletedList: window.deleteHdSrvCatList }
    var action = "delHdSrvCategory";
    $.ajax({
        url: "ManageRequest?action=" + action,
        type: "post",
        data: postData,
        dataType: "json",
        traditional: true,
        success: function (data) {
            reloadHdSrvTable();
            if (data.StatusCode === 0) {
                window.deleteHdSrvCatList = new Array();
            } else {
                $("#delHdSrvBtnGroup").removeClass("show");
                $("#delHdSrvBtnGroup").addClass("hide");
                $("#messageModal .msgContent").text("Xóa dịch vụ hổ trợ thất bại!");
            }
            setTimeout(function () {
                $("#delBtnGroup").removeClass("show");
                $("#delBtnGroup").addClass("hide");
            }, 1000);
        },
        error: function () {

        }
    });
    setTimeout(function () {
        $("#delBtnGroup").removeClass("show");
        $("#delBtnGroup").addClass("hide");
    }, 1000);
}
function cancelDeleteHdSrvCategory() {
    for (var i = 0; i < deleteHdSrvCatList.length; i++) {
        $("#rowHdSrvCat_" + deleteHdSrvCatList[i]).css("display", "table-row");
    }
    window.deleteHdSrvCatList = new Array();
    $("#delHdSrvBtnGroup").removeClass("show").addClass("hide");;
}
function sentHepdeskRequest() {
    $("#messageModal").modal("show");
}
function hdRequestDetail(id) {
    $("#hdSrvTblBody > tr").removeClass("active");
    $("#hdReq_" + id).addClass("active");
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
            label.parent().removeClass("error");
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
                    reloadHdSrvTable();
                    var msg = "";
                    if (data.StatusCode === 0) {
                        if (window.POPUPMODE === window.UPDATEMODE) {
                            msg = window.UpdateServiceSuccessMsg;
                        } else {
                            msg = window.AddServiceSuccessMsg;
                        }
                        $("#addHelpdeskRequestModal .alert-info  > span").text(msg);
                        $("#successNotify").show().delay(3000).fadeOut(1000);
                    } else if (data.StatusCode === -1) {
                        if (window.POPUPMODE === window.UPDATEMODE) {
                            msg = window.UpdateServiceFailedMsg;
                        } else {
                            msg = window.AddServiceFailedMsg;
                        }
                        $("#addHelpdeskRequestModal .alert-warning > span").text(msg);
                        $("#failedNotify").show().delay(3000).fadeOut(1000);
                    }
                }
            });
        }
    });

    $("#addHdSrvCat").on("click", function () {
        console.log($("#hdSrvCategoryForm").serialize());
        $("#hdSrvCategoryForm").valid();
    });
    $("#hdSrvCategoryForm").validate({
        rules: {
            hdSrvCatName: {
                required: true,
                maxlength: 255
            }
        },
        messages: {
            hdSrvCatName: {
                required: "Vui lòng nhập vào tên nhóm dịch vụ hổ trợ.",
                maxlength: "Tên nhóm dịch vụ hổ trợ không dài quá 255 ký tự."
            }
        },
        success: function (label, element) {
            label.parent().removeClass("error");
            label.remove();
        },
        submitHandler: function () {
            var action = "";
            if (window.POPUPMODE === window.UPDATEMODE) {
                action = "updateHdSrvCategory";
            } else {
                action = "addHdSrvCategory";
            }
            $.ajax({
                url: "ManageRequest?action=" + action,
                type: "post",
                data: $("#hdSrvCategoryForm").serialize(),
                success: function (data) {
                    reloadHdSrvCategoryTable();
                    var msg = "";
                    if (data.StatusCode === 0) {
                        if (window.POPUPMODE === window.UPDATEMODE) {
                            msg = window.UpdateHdSrvCategorySuccessMsg;
                        } else {
                            msg = window.AddHdSrvCategorySuccessMsg;
                        }
                        $("#hdSrvCategoryModal .alert-info  > span").text(msg);
                        $("#hdSrvCatSuccessNotify").show().delay(3000).fadeOut(1000);
                    } else if (data.StatusCode === -1 || data.StatusCode === 4) {
                        if (window.POPUPMODE === window.UPDATEMODE) {
                            msg = window.UpdateHdSrvCategoryFailedMsg;
                        } else {
                            msg = window.AddHdSrvCategoryFailedMsg;
                        }
                        $("#hdSrvCategoryModal .alert-warning > span").text(msg);
                        $("#hdSrvCatFailedNotify").show().delay(3000).fadeOut(1000);
                    }
                }
            });
        }
    });


    $("#addHelpdeskRequestModal").on("hidden.bs.modal", function () {
        document.getElementById("addHelpdeskRequestForm").reset();
    });
    $("#hdSrvCategoryModal").on("hidden.bs.modal", function () {
        document.getElementById("hdSrvCategoryForm").reset();
    });
    $(document).on("change", "#hdReqFilterType", function () {
        var selected = parseInt($(this).find("option:selected").val(), 10);
        if (selected === 0) {
            $("#hdReqFilterResult").prop("disabled", true).html("");
        } else if (selected === 1) {
            $("#hdReqFilterType").prop("disabled", true);
            $("#hdReqFilterResult").prop("disabled", false).html("");
            getHelpdeskServiceType("hdReqFilterResult", function () {
                $("#hdReqFilterType").prop("disabled", false);
            });
        } else if (selected === 2) {
            $("#hdReqFilterResult").prop("disabled", false);
            var opt = "<option value='1'>Hổ trợ</option><option value='0'>Không hổ trợ</option>";
            $("#hdReqFilterResult").html(opt);
        }
    });
    $("#testDatePicker").datepicker();
});

function setModalMaxHeight(element) {
    this.$element = $(element);
    this.$content = this.$element.find(".modal-content");
    var borderWidth = this.$content.outerHeight() - this.$content.innerHeight();
    var dialogMargin = $(window).width() < 768 ? 20 : 60;
    var contentHeight = $(window).height() - (dialogMargin + borderWidth);
    var headerHeight = this.$element.find(".modal-header").outerHeight() || 0;
    var footerHeight = this.$element.find(".modal-footer").outerHeight() || 0;
    var maxHeight = contentHeight - (headerHeight + footerHeight);

    this.$content.css({
        "overflow": "hidden"
    });

    this.$element
      .find(".modal-body").css({
          "max-height": maxHeight,
          "overflow-y": "auto"
      });
}

$(".modal").on("show.bs.modal", function () {
    $(this).show();
    setModalMaxHeight(this);
});

$(window).resize(function () {
    if ($(".modal.in").length !== 0) {
        setModalMaxHeight($(".modal.in"));
    }
});