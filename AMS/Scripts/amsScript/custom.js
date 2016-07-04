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

window.StatusOpen = 1;
window.StatusProcessing = 2;
window.StatusDone = 3;
window.StatusClose = 4;
window.StatusCancel = 5;

window.StatusUnpublished = 1;
window.StatusUnpaid = 2;
window.StatusPaid = 3;

window.StatusCompleteRecordConsumption = 1;
window.StatusUncompleteRecordConsumption = 2;

window.StatusManualReceipt = 1;
window.StatusAutomationReceipt = 2;

window.Transaction_Type_Income = 1;
window.Transaction_Type_Expense = 2;

window.Mode_Delete = 1;
window.Mode_Publish = 2;

window.Enable = 1;
window.Disable = 2;

window.IsNotExisted = 1;
window.IsExisted = 2;


window.Utility_Service_Water = 2;
window.Utility_Service_Fixed_Cost = 5;

window.UserStatusWaiting = 0;
window.UserStatusEnable = 1;
window.UserStatusReject = 2;
window.UserStatusDisable = 4;

window.UserRoleResident = 3;
window.UserRoleHouseHolder = 4;



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
        url: "/Management/ManageRequest?action=" + action,
        type: "get",
        success: function (data) {
            console.log(data.HdSrvCategories);
            var objList = data.HdSrvCategories;
            var selectTagList = [];
            var selectTag = "<option value='' selected='selected'> Hãy chọn dịch vụ hổ trợ</option>";
            selectTagList.push(selectTag);
            for (var i = 0; i < objList.length; i++) {
                var obj = objList[i];
                selectTag = "<option value='" + obj.Id + "'>" + obj.Name + "</option>";
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
        url: "/Management/ManageRequest?action=" + action,
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
            var selectTag = "<option value='' selected='selected'>  Hãy chọn nhóm dịch vụ hổ trợ</option>";
            selectTagList.push(selectTag);
            for (var i = 0; i < objList.length; i++) {
                var obj = objList[i];

                if (obj.Id === data.HelpdeskServiceCategoryId) {
                    selectTag = "<option value='" + obj.Id + "' selected='selected'>" + obj.Name + "</option>";
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
        url: "/Management/ManageRequest?action=" + action,
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
    $("#hdSrvTblBody").load("/Management/ManageRequest?action=" + action);
}
function searchHdSrv() {
    var mode = parseInt($("#hdReqFilterType").find("option:selected").val(), 10);
    var searchStr = $("#searchStr").val();
    //    if (searchStr !== "" || (searchStr === "" && mode !== 0)) {
    var action = "searchHdSrv";
    $.ajax({
        url: "/Management/ManageRequest?action=" + action,
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
        url: "/Management/ManageRequest?action=" + action,
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
    $("#hdSrvCatTblBody").load("/Management/ManageRequest?action=" + action);
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
        url: "/Management/ManageRequest?action=" + action,
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
        url: "/Management/ManageRequest?action=" + action,
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
    getHelpdeskDetail(id);
}
$(document).ready(function () {
    $("#hdReqDueDateTime").timepicker({ "scrollDefault": "5:30 pm" });
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
                url: "/Management/ManageRequest?action=" + action,
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
                url: "/Management/ManageRequest?action=" + action,
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
    $("#createHdService").validate({
        ignore: [],
        rules: {
            hdSrvCatName: {
                required: true
            }, hdService: {
                required: true
            }, hdReqTitle: {
                required: true,
                maxlength: 255
            }, hdReqUserDesc: {
                maxlength: 500
            }
        },
        messages: {
            hdSrvCatName: {
                required: "Vui lòng chọn nhóm dịch vụ hổ trợ."
            }, hdService: {
                required: "Vui lòng chọn dịch vụ hổ trợ."
            }, hdReqTitle: {
                required: "Vui lòng nhập vắn tắt mô tả yêu cầu.",
                maxlength: "Tiêu đề không dài quá 255 ký tự."
            }, hdReqUserDesc: {
                maxlength: "Mô tả không dài quá 500 ký tự."
            }
        },
        success: function (label, element) {
            label.parent().removeClass("error");
            label.remove();
        },
        errorPlacement: function (error, element) {
            if (element.attr("name") === "hdServiceId") {
                error.insertAfter($("#hdServiceId").next());
            } else if (element.attr("name") === "hdSrvCatName") {
                error.insertAfter($("#hdSrvCatName").next());
            } else {
                error.insertAfter(element);
            }
        },
        submitHandler: function () {
            addNewHdRequest();
        }
    });

    $("#addHdSrvCat").on("click", function () {
        console.log($("#hdSrvCategoryForm").serialize());
        $("#hdSrvCategoryForm").valid();
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

    $(document).on("change", "#hdSrvCatName", function () {

        $("#hdSrvPrice").val("");
        $("#hdSrvDesc").val("");
        if ($(this).find("option:selected").val()) {
            var selected = parseInt($(this).find("option:selected").val(), 10);
            $("#hdSrvCatName").prop("disable", true);
            $("#hdServiceId").prop("disabled", true);
            getHdSrvByCatId(selected, "hdServiceId", function () {
                $("#hdSrvCatName").prop("disable", false);
                $("#hdServiceId").prop("disabled", false);
                $("select[name=HdServiceId]").val(-1);
                $("#hdServiceId").selectpicker("refresh");
            });
        } else {
            var selectTag = "<option value='' selected='selected'> " + "Hãy chọn dịch vụ hổ trợ" + " </option>";
            $("#hdServiceId").html(selectTag);
            $("#hdServiceId").selectpicker("refresh");
        }
    });

    $(document).on("change", "#hdServiceId", function () {
        $("#hdSrvPrice").val("");
        $("#hdSrvDesc").val("");
        if ($(this).find("option:selected").val()) {
            var selected = parseInt($(this).find("option:selected").val(), 10);
            if (window.hdSrvList !== undefined || window.hdSrvList.length !== 0) {
                for (var i = 0; i < window.hdSrvList.length; i++) {
                    if (window.hdSrvList[i].Id === selected) {
                        $("#hdSrvPrice").val(window.hdSrvList[i].Price);
                        $("#hdSrvDesc").val(window.hdSrvList[i].Description);
                    }
                }
            }
        }
    });
    $("#addNewHdRequest").on("click", function () {
        console.log($("#createHdService").serialize());
        $("#createHdService").valid();
    });
    $("#updateHdRequest").on("click", function () {
        console.log($("#updateHelpdeskRequestForm").serialize());
        //        $("#createHdService").valid();
        $("#createHdService").submit();
    });

    var dataTable3 = $("#hdSrvCatTable").DataTable({
        "bLengthChange": false,
        "bInfo": false,
        "drawCallback": function (settings) {
            var html =
            "<div class='hide' id='delHdSrvBtnGroup'>" +
                "<span class='btn btn-inverse' onclick='cancelDeleteHdSrvCategory()'" +
                    "style='color: #ab7a4b; border-color: transparent;'>" +
                    "<i class='fa fa-plus'></i> Cancel" +
                "</span>" +
                "<span class='btn btn-primary' style='margin-left: 5px' onclick='commitDeleteHdSrvCategory()'>" +
                    "<i class='fa fa-plus'></i> Delete" +
                "</span>" +
            "</div>";
            $("#hdSrvCatTable_wrapper > div.row:nth-child(3) > div:nth-child(1) ").html(html);

            var addBtn = "<div class='col-md-1'>" +
                            "<span class='btn btn-info' data-toggle='modal' data-target='#hdSrvCategoryModal' data-placement='top' onclick='openHdSrvCategoryModal()'>" +
                        "<i class='fa fa-plus'></i>" +
                            "</span>" +
                        "</div>";
            $("#hdSrvCatTable_wrapper > div:nth-child(1) > div:nth-child(1)").html(addBtn);
        }
    });
    var dataTable4 = $("#hdSrvTable").DataTable({
        "bLengthChange": false,
        "bInfo": false,
        "drawCallback": function (settings) {
            var html =
            "<div class='hide' id='delBtnGroup'>" +
                "<span class='btn btn-inverse' onclick='cancelDeleteHelpdeskService()'" +
                    "style='color: #ab7a4b; border-color: transparent;'>" +
                    "<i class='fa fa-plus'></i> Cancel" +
                "</span>" +
                "<span class='btn btn-primary' style='margin-left: 5px' onclick='commitDeleteHelpdeskService()'>" +
                    "<i class='fa fa-plus'></i> Delete" +
                "</span>" +
            "</div>";
            $("#hdSrvTable_wrapper > div.row:nth-child(3) > div:nth-child(1) ").html(html);

            var addBtn = "<div class='col-md-1'>" +
                            "<span class='btn btn-info' data-toggle='modal' data-target='#addHelpdeskRequestModal' data-placement='top' title='Edit' onclick='loadHelpdeskServiceType()'>" +
                                "<i class='fa fa-plus'></i>" +
                            "</span>" +
                        "</div>";
            $("#hdSrvTable_wrapper > div:nth-child(1) > div:nth-child(1)").html(addBtn);
        }
    });

    activeNavigationBar();
});

function activeNavigationBar() {
    $("#main-nav > ul > li").removeClass("active");
    var pathName = window.location.pathname;
    if (pathName.indexOf("/HelpdeskRequest/") > -1) {
        $("#hdRequestNav").addClass("active");
    } else if (pathName.indexOf("/ManageReceipt/") > -1) {
        $("#receiptNav").addClass("active");
    } else if (pathName.indexOf("/BalanceSheet/") > -1) {
        $("#balanceSheetNav").addClass("active");
    } else if (pathName.indexOf("/Config/") > -1) {
        $("#configNav").addClass("active");
    } else if (pathName.indexOf("/ManageUser/") > -1) {
        $("#userNav").addClass("active");
    } else {
    }
}

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

/* Start Js for controller*/
function getHdSrvByCatId(id, tagId, callback) {
    var action = "getHdServiceByCatId";
    $.ajax({
        url: "/Management/ManageRequest?action=" + action,
        data: { hdSrvCatId: id },
        type: "get",
        dataType: "json",
        traditional: true,
        success: function (data) {
            console.log(data);
            var selectTagList = [];
            var selectTag = "<option value='' selected='selected'> Hãy chọn dịch vụ hổ trợ</option>";
            selectTagList.push(selectTag);
            if (data.StatusCode === 0) {
                window.hdSrvList = data.Data;
                var objList = data.Data;
                for (var i = 0; i < objList.length; i++) {
                    var obj = objList[i];
                    selectTag = "<option value='" + obj.Id + "'>" + obj.Name + "</option>";
                    selectTagList.push(selectTag);
                }
            } else {
                window.hdSrvList = [];
            }
            $("#" + tagId).html(selectTagList);
            callback();
        },
        error: function () {

        }
    });
}
function addNewHdRequest() {
    var action = "AddHdRequest";
    $.ajax({
        url: "/Home/HelpdeskRequest/" + action,
        type: "POST",
        data: $("#createHdService").serialize(),
        success: function (data) {
            console.log(data);
            if (data.StatusCode === 0) {
                document.getElementById("createHdService").reset();
                $("#createHdSrvSuccessNoti").removeClass("hide");
                $("#createHdSrvSuccessNoti").addClass("show");
                setTimeout(function () {
                    $("#createHdSrvSuccessNoti").removeClass("show");
                    $("#createHdSrvSuccessNoti").addClass("hide");
                }, 3000);
            } else if (data.StatusCode === -1) {
                $("#createHdSrvFailedNoti").removeClass("hide");
                $("#createHdSrvFailedNoti").addClass("show");
            }
        },
        error: function () {

        }
    });
}

function getHelpdeskDetail(id) {

    //    var action = "ViewDetail";
    //    var data = {
    //        hdReqId: id, /*Just test for demo*/
    //        userId: 5
    //    };
    //    $.ajax({
    //        url: "/Home/HelpdeskRequest/" + action,
    //        type: "get",
    //        dataType: "json",
    //        data: data,
    //        traditional: true,
    //        success: function (data) {
    //            console.log(data);
    //        },
    //        error: function () {
    //
    //        }
    //
    //    });
    var userId = document.getElementById("hdRequestTbl").dataset.userid;
    location.href = "/Home/HelpdeskRequest/ViewDetail?hdReqId=" + id + "&userId=" + userId;
}

function changeHdReqStatus(id, fromStatus, toStatus, fromUseId) {
    var action = "/Home/HelpdeskRequest/UpdateStatus?";
    var data = {
        HdReqId: id,
        FromUserId: fromUseId,
        FromStatus: fromStatus,
        ToStatus: toStatus
    }
    location.href = action + $.param(data);
}

function openModalAssignHdReq(hdReqId) {
    var action = "GetSupporters";
    $.ajax({
        url: "/Home/HelpdeskRequest/" + action,
        data: {
            hdReqId: hdReqId
        },
        type: "get",
        success: function (data) {
            console.log(data.supporterList);
            var objList = data.Data.supporterList;
            var selectTagList = [];
            for (var i = 0; i < objList.length; i++) {
                var obj = objList[i];
                var selectTag = {};
                if (data.Data.curUserId !== undefined && obj.UserId === data.Data.curUserId) {
                    selectTag = "<option value='" + obj.UserId + "' selected='selected'>" + obj.Fullname + "</option>";
                } else {
                    selectTag = "<option value='" + obj.UserId + "'>" + obj.Fullname + "</option>";
                }
                selectTagList.push(selectTag);
            }
            $("#lstSupporter").html(selectTagList);
            $("#assignTaskModal").modal("show");

            var submitData = document.getElementById("assignHdReqBtn");

            $("#HdReqId").val(submitData.dataset.id);
            $("#FromUserId").val(submitData.dataset.fromuser);
        },
        error: {

        }
    });
}
function submitAssignTask() {
    $("#assignTaskForm").submit();
}
function setDueDate() {
    var dataTag = document.getElementById("btnDuedate");
    var hdReqId = dataTag.dataset.id;
    var fromUserId = dataTag.dataset.userid;
    $("#HdReqId_2").val(hdReqId);
    $("#FromUserId_2").val(fromUserId);
    var dueDate = $("#hdReqDueDateDate").val();
    var dueTime = $("#hdReqDueDateTime").val();
    $("#DueDate").val(dueDate + " " + dueTime);
    $("#setDuedateForm").submit();
}

function showEditHdReqModal(id) {
    //    getHelpdeskServiceType("hdSrvCatName", function () {
    //        $("#hdSrvCatName").selectpicker("refresh");
    //        $("#updateHdRequestModal").modal("show");
    //    });
    getHdReqDetail(id);

}

function getHdReqDetail(id) {
    $.ajax({
        type: "get",
        url: "/Home/HelpdeskRequest/GetHdReqInfoDetail/" + id,
        success: function (data) {

            //            console.log(data.Data.HdReqInfoDetail);
            //            console.log(data.Data.HdSrvCategories);
            //            console.log(data.Data.ListHdSrvBySelectedCat);
            //            console.log(data.Data.SelectedHdSrvCatId);
            //            console.log(data.Data.SelectedHdSrvId);

            var objList = data.Data.HdSrvCategories;
            var returnHtml = parseJsonToSelectTags(objList, data.Data.SelectedHdSrvCatId, "Hãy chọn loại dịch vụ hổ trợ");
            $("#hdSrvCatName").html(returnHtml);
            $("#hdSrvCatName").selectpicker("refresh");


            objList = data.Data.ListHdSrvBySelectedCat;
            var returnHtml2 = parseJsonToSelectTags(objList, data.Data.SelectedHdSrvId, "Hãy chọn dịch vụ hổ trợ");
            $("#hdServiceId").html(returnHtml2);
            $("#hdServiceId").selectpicker("refresh");

            window.hdSrvList = objList;

            $("#hdReqTitle").val(data.Data.HdReqInfoDetail.HdReqTitle);
            $("#hdReqDesc").val(data.Data.HdReqInfoDetail.HdReqUserDesc);

            $("#updateHdRequestModal").modal("show");
        }
    });
}

function generateTableIndex(datatable) {
    datatable.on('order.dt search.dt', function () {
        datatable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            if ($(cell.parentElement.previousSibling).hasClass("hide")) {
                cell.innerHTML = i - 1;
            } else {
                cell.innerHTML = i + 1;
            }
        });
    }).draw();
}
function parseJsonToSelectTags(listJson, selectedId, msg) {
    var objList = listJson;
    var selectTagList = [];
    var selectTag = {};
    //    selectTagList.push(selectTag);
    for (var i = 0; i < objList.length; i++) {
        var obj = objList[i];
        if (obj.Id === selectedId) {
            selectTag = "<option selected='selected' value='" + obj.Id + "'>" + obj.Name + "</option>";
        } else {
            selectTag = "<option value='" + obj.Id + "'>" + obj.Name + "</option>";
        }
        selectTagList.push(selectTag);
    }
    return selectTagList;
}

function resInfoDetail(id) {
    var action = "GetResidentInfo";
    $.ajax({
        type: "get",
        url: "/Management/ResidentApprovement/" + action,
        data: { userId: id },
        success: function (data) {
            var obj = data.Data;
            var fullName = obj.FullName;
            var houseName = obj.HouseName;
            var houseHolderName = obj.HouseHolderName;
            var createDate = obj.CreateDate;
            var id = obj.Id;
            var gender = obj.Gender;

            $("#resName").val(fullName);
            $("#resHouseName").val(houseName);
            $("#resId").val(id);
            $("#resHouseHolder").val(houseHolderName);
            $("#resCreateDate").val(createDate);

            var genderText = "";
            if (gender === 0) {
                genderText = "Nữ";
            } else if (gender === 1) {
                genderText = "Nam";
            } else {
                genderText = "Chưa xác định";
            }
            $("#resGender").val(genderText);
            $("#residentInfoDetail").modal("show");
        },
        error: function (data) {

        }
    });
}
/* Start Js for controller*/
function showConfirmMsg(mode, id, userName, houseName) {
    $("#residentId").val(id);
    var modeNum = 0;
    var msg = "";
    if (mode === "accept") {
        msg = "<strong>Chấp nhận</strong> yêu cầu cư dân <strong>" + userName + "</strong> là thành viên của căn hộ <strong>" + houseName + "</strong>";
        modeNum = 1;
    } else {
        msg = "<strong>Từ chối</strong> yêu cầu cư dân <strong>" + userName + "</strong> là thành viên của căn hộ <strong>" + houseName + "</strong>";
        modeNum = 2;
    }
    $("#mode").val(modeNum);
    $("#msgConfirm").html(msg);
    $("#confirmModal").modal("show");
}
function acceptApproveUser() {
    var resId = $("#residentId").val();
    var mode = $("#mode").val();
    var action = "AcceptResidentApprovement";
    var fromUserId = document.getElementById("residentApproveTbl").dataset.fromuserid;
    $.ajax({
        type: "post",
        url: "/Management/ResidentApprovement/" + action,
        data: {
            resId: resId,
            fromUserId: fromUserId,
            mode: mode
        },
        success: function (data) {
            console.log(data);
            if (window.currentRow) {
                window.dataTable2.row(window.currentRow).remove().draw();
            }
            $("#confirmModal").modal("hide");
        },
        error: function () {

        }
    });
}
function showOrderDetail(receiptId, userId) {
    location.href = "/Home/ManageReceipt/View/Detail?userId=" + userId + "&orderId=" + receiptId;
}

function approveResident(id) {
    $("#delHdSrvBtnGroup").removeClass("show").addClass("show");
    $("#rowHdSrvCat_" + id).css("display", "none");
    deleteHdSrvCatList.push(id);

}


// Receipt manager
function calculateTotal() {
    var contentObj = $("#receiptWrapper > .form-group");
    var total = 0;
    for (var i = 0; i < contentObj.length; i++) {
        var rowIdStr = $(contentObj[i]).prop("id").split("row_");
        var id = rowIdStr[1];
        var currentPrice = $("#item_qty_price_" + id).val();
        currentPrice = replaceCommaNumber(currentPrice);
        if (currentPrice && isNaN(currentPrice) === false) {
            try {
                currentPrice = parseFloat(currentPrice);
                total += currentPrice;
            } catch (e) {
                $("#total").val("");
                return;
            }
        }
    }
    total = numberWithCommas(total);
    $("#total").val(total);
}


window.getHouseMode = "";
function parseJsonToSelectTag(floor, room, roomId) {
    var selectTagList = [];
    var selectTag = "";
    if (getHouseMode !== "floor") {
        for (var i = 0; i < floor.length; i++) {
            var obj = floor[i];
            selectTag = "<option value=\"" + obj + "\">" + obj + "</option>";
            selectTagList.push(selectTag);
        }
        $("#houseFloor").html(selectTagList);
    }

    selectTagList = [];
    for (var j = 0; j < room.length; j++) {
        var obj2 = room[j];
        selectTag = "<option value=\"" + obj2 + "\">" + obj2 + "</option>";
        selectTagList.push(selectTag);
    }
    $("#houseName").html(selectTagList);
}

function getRoomAndFloor(blockId, floorName, mode, callback) {
    window.getHouseMode = mode;
    $.ajax({
        type: "GET",
        url: "/Management/ManageReceipt/GetRoomAndFloor",
        data: {
            blockId: blockId,
            floorName: floorName
        },
        success: function (data) {
            var floor = data.Data.Floor;
            var room = data.Data.Room;
            var roomId = data.Data.RoomId;
            var msg = "<option value=\"\">" + "Không có dữ liệu " + "</option>";
            var msg2 = "<option value=\"\">" + "Không có dữ liệu" + "</option>";
            if (floor === undefined || floor === null || floor.length === 0) {
                $("#houseFloor").html(msg);
                $("#houseName").html(msg2);
            } else if (room === undefined || room === null || room.length === 0) {
                $("#houseName").html(msg2);
            } else {
                parseJsonToSelectTag(floor, room, roomId, mode);
            }
            if (callback && isFunction(callback)) {
                callback();
            }
        },
        error: function () {

        }
    });
}


$(document).ready(function () {
    //        $("#createNewOrder").validate({
    //            submitHandler: function () {
    //                console.log("AAAAAAAA");
    //            }
    //        });



    //        jQuery.validator.addClassRules("order-item-qty", {
    //            required: true,
    //            number: true
    //        });
    //        jQuery.validator.addClassRules("order-item-price", {
    //            required: true,
    //            number: true
    //        });

    $("#houseBlock").on("change", function () {
        var selected = $(this).find("option:selected").val();
        getRoomAndFloor(selected, "", "block");
    });

    $("#houseFloor").on("change", function () {
        var selectedFloor = $(this).find("option:selected").val();
        var selectedBlock = $("#houseBlock").find("option:selected").val();
        getRoomAndFloor(selectedBlock, selectedFloor, "floor");
    });

    $(".ams-modal").on("hidden.bs.modal", function () {
        $(this.querySelector("form")).find("input[type=text], textarea").val("");
        $(this.querySelector("form")).find("select option").prop("selected", function () {
            return this.defaultSelected;
        });
        removeHiddenBackgroundPopup();
    });

});

//Receipt manager
//http://stackoverflow.com/questions/2901102/how-to-print-a-number-with-commas-as-thousands-separators-in-javascript
function numberWithCommas(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".");
}
function numberWithSpace(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".");
}
function replaceCommaNumber(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/,/g, "");
    return parts.join(".");
}
function replaceSpaceNumber(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/ /g, "");
    return parts.join(".");
}
function resetFormData(id) {
    $("#" + id).closest("form").find("input[type=text], textarea").val("");
    $("#" + id).closest("form").find("input, select").prop("disabled", "");
}

function removeHiddenBackgroundPopup() {
    $("body").removeClass("modal-open");
    $(".modal-backdrop").remove();
}

function getRandomColor() {
    var letters = '0123456789ABCDEF'.split('');
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

function createChart(id, data) {
    var obj = {};
    var listLbl = [];
    var listVal = [];
    var bgColorList = [];

    for (var i = 0; i < data.length; i++) {
        obj = data[i];
        listLbl.push(obj.UtilSrvCatName);
        listVal.push(obj.TransTotalAmount);
        bgColorList.push(randomColor2());
    }
    var config = {
        type: 'pie',
        data: {
            labels: listLbl,
            datasets: [
                {
                    data: listVal,
                    backgroundColor: bgColorList
                }
            ]
        },
        options: {
            tooltips: {
                enabled: false
            },
            legend: {
                display: true,
                labels: {
                    fontSize: 15
                },
                position: "bottom"
            }
        }
    };
    var ctx2 = document.getElementById(id).getContext("2d");
    return new Chart(ctx2, config);
}
function randomColor2() {
    var r = Math.floor(Math.random() * 200);
    var g = Math.floor(Math.random() * 250);
    var b = Math.floor(Math.random() * 400);
    var v = Math.floor(Math.random() * 500);
    var c = 'rgb(' + r + ', ' + g + ', ' + b + ')';
    return c;
}

function bindingNumberWithComma(id) {
    //    $("#" + id).on("keyup", function () {
    $("#" + id).unbind();
    $("#" + id).on("keyup", function (event) {
        if (event.which >= 37 && event.which <= 40) {
            event.preventDefault();
        }
        console.log("keyup " + $(this).val());
        if ($(this).val() && (isNaN(replaceCommaNumber($(this).val())) === false)) {
            var value = replaceCommaNumber($(this).val());
            $(this).val(numberWithCommas(value));
        } else {
            $(this).val("");
        }
    });
}
function bindingClassNumberWithComma(_class) {
    //    $("#" + id).on("keyup", function () {
    $("." + _class).unbind();
    $("." + _class).on("keyup", function (event) {
        if (event.which >= 37 && event.which <= 40) {
            event.preventDefault();
        }
        console.log($(this).val());
        if ($(this).val() && (isNaN(replaceCommaNumber($(this).val())) === false)) {
            var value = replaceCommaNumber($(this).val());
            $(this).val(numberWithCommas(value));
        } else {
            $(this).val("");
        }
    });
}


function electricAggressiveCalculating(consumption, rangePrices) {
    var rangePrice = {};
    var previous = 0;
    var total = 0;
    for (var i = 0; i < rangePrices.length; i++) {
        rangePrice = rangePrices[i];
        var toAmount = parseFloat(rangePrice.ToAmount);
        var fromAmount = parseFloat(rangePrice.FromAmount);

        var calculatingPart = 0;
        if (consumption >= toAmount) {
            calculatingPart = toAmount - previous;
            previous = toAmount;
        }
        else if (consumption >= fromAmount && consumption < toAmount) {
            calculatingPart = consumption - previous;
        }
        total += calculatingPart * rangePrice.Price;
    }
    return total;
}

function waterAggressiveCalculating(consumption, rangePrices) {
    var rangePrice = {};
    var previous = 0;
    var total = 0;
    var toAmount = 0;
    var fromAmount = 0;
    for (var i = 0; i < rangePrices.length; i++) {

        rangePrice = rangePrices[i];
        toAmount = parseFloat(rangePrice.ToAmount);
        fromAmount = parseFloat(rangePrice.FromAmount);

        var calculatingPart = 0;
        if (consumption >= toAmount) {
            calculatingPart = toAmount - previous;
            previous = toAmount;
        }
        else if (consumption >= fromAmount && consumption < toAmount) {
            calculatingPart = consumption - previous;
        }
        total += calculatingPart * rangePrice.Price;
    }
    return total;
}

function calculateDateFromTodayToEndOfMonth() {
    var oneDay = 24 * 60 * 60 * 1000;
    var today = new Date();
    var lastDate = new Date(today.getFullYear(), today.getMonth(0) + 1, 0);
    var diffDays = Math.round(Math.abs(lastDate.getTime() - today.getTime()) / (oneDay));
    return diffDays;
}

function onChangeFromNumber() {
    $(".order-item-price").unbind();
    $(".order-item-price").on("change", function (event) {
        if (event.which >= 37 && event.which <= 40) {
            event.preventDefault();
            return;
        }
        var nextElement = event.target.parentNode.parentNode.nextElementSibling;
        var nextFromValue = {};
        var fromValue = event.target.parentNode.previousElementSibling.querySelector(".order-item-qty");

        if (nextElement == null) {
            event.target.value = "*";
            return;
            return;
        } else {
            nextFromValue = nextElement.querySelector(".order-item-qty");
            var nextToValue = nextElement.querySelector(".order-item-price");
            if (parseInt(event.target.value, 10) >= parseInt(nextToValue.value, 10)) {
                var nextNode = event.target.parentNode.parentNode.nextElementSibling;
                while (nextNode && nextNode.nodeType === 1 && nextNode !== this) {
                    nextNode.querySelector(".order-item-price").value = "";
                    nextNode.querySelector(".order-item-qty").value = "";
                    nextNode = nextNode.nextElementSibling;
                }
            }// if next node is greater than this node clean all next node
        }
        if (event.target.parentNode.parentNode.previousElementSibling) {
            var previousValue = event.target.parentNode.parentNode.previousElementSibling
                .querySelector(".order-item-price");

            if (isNaN(event.target.value) && event.target.value !== "*") {
                event.target.value = "";
            } // clean next node when input other character except *
            else if (isNaN(event.target.value) && event.target.value === "*") {
                if (nextElement != null) {
                    event.target.value = "";
                    var nextNode = event.target.parentNode.parentNode.nextElementSibling;
                    while (nextNode && nextNode.nodeType === 1 && nextNode !== this) {
                        nextNode.querySelector(".order-item-price").value = "";
                        nextNode.querySelector(".order-item-qty").value = "";
                        nextNode = nextNode.nextElementSibling;
                    }
                }// clean next node when input * in the middle
            } else if (parseInt(event.target.value, 10) > parseInt(previousValue.value, 10)) {
                nextFromValue.value = event.target.value;
            } else {
                event.target.value = "";
                fromValue.value = "";
                var previousNode = event.target.parentNode.parentNode.previousElementSibling;
                while (previousNode && previousNode.nodeType === 1 && previousNode !== this) {
                    previousNode.querySelector(".order-item-price").value = "";
                    if (previousNode.querySelector(".order-item-qty").value != 0) {
                        previousNode.querySelector(".order-item-qty").value = "";
                    }
                    previousNode = previousNode.previousElementSibling;
                }
                var nextNode = event.target.parentNode.parentNode.nextElementSibling;
                while (nextNode && nextNode.nodeType === 1 && nextNode !== this) {
                    nextNode.querySelector(".order-item-price").value = "";
                    nextNode.querySelector(".order-item-qty").value = "";
                    nextNode = nextNode.nextElementSibling;
                }
            }// when in put in the middle wrong clean all element
        } else {
            if (isNaN(event.target.value) && event.target.value !== "*") {
                event.target.value = "";
            } else {
                nextFromValue.value = event.target.value;
            }
        }
    });
}

function bindingCalculateSubTotal() {
    $("#receiptWrapper").on("keyup", ".order-item-qty", function () {
        if (event.which >= 37 && event.which <= 40) {
            event.preventDefault();
            return;
        }
        console.log($(this).val());
        var idStr = $(this).prop("id").split("item_qty_");
        if ($(this).val() && (isNaN(replaceCommaNumber($(this).val())) === false)) {
            $("#item_qty_" + idStr[1]).val(numberWithCommas($(this).val()));
            var unitPriceValue = $("#item_unit_price_" + idStr[1]).val();
            unitPriceValue = replaceCommaNumber(unitPriceValue);

            if (unitPriceValue && isNaN(unitPriceValue) === false) {
                var unitPrice = parseFloat(unitPriceValue);
                var qty = parseFloat(replaceCommaNumber($(this).val()));

                var formatedMoney = numberWithCommas(unitPrice * qty);
                $("#item_qty_price_" + idStr[1]).val(formatedMoney);
                calculateTotal();

            } else {
                $("#item_unit_price_" + idStr[1]).val("");
            }
        } else {
            $("#item_qty_" + idStr[1]).val("");
            $("#item_qty_price_" + idStr[1]).val("");

        }
    });
    $("#receiptWrapper").on("keyup", ".order-item-price", function () {
        if (event.which >= 37 && event.which <= 40) {
            event.preventDefault();
            return;
        }
        console.log("change " + $(this).val());
        var idStr = $(this).prop("id").split("item_unit_price_");
        if ($(this).val() && (isNaN(replaceCommaNumber($(this).val())) === false)) {
            $("#item_unit_price_" + idStr[1]).val(numberWithCommas($(this).val()));
            var qtyValue = $("#item_qty_" + idStr[1]).val();
            qtyValue = replaceCommaNumber(qtyValue);
            if (qtyValue && isNaN(qtyValue) === false) {

                var qty = parseFloat(qtyValue);
                var unitPrice = parseFloat(replaceCommaNumber($(this).val()));

                var formatedMoney = numberWithCommas(unitPrice * qty);
                $("#item_qty_price_" + idStr[1]).val(formatedMoney);
                $("#item_unit_price_" + idStr[1]).val(numberWithCommas(unitPrice));
                calculateTotal();
            } else {
                $("#item_qty_price_" + idStr[1]).val("");
            }
        } else {
            $("#item_unit_price_" + idStr[1]).val("");
            $("#item_qty_price_" + idStr[1]).val("");
        }
    });
}
function smoothScrollToTop() {
    $("html, body").animate({
        scrollTop: 0
    }, 600);
}
/*
http://jsfiddle.net/yWTLk/164/

$('input.number').keyup(function (event) {
    // skip for arrow keys
    if (event.which >= 37 && event.which <= 40) {
        event.preventDefault();
    }
    var $this = $(this);
    var num = $this.val().replace(/,/gi, "").split("").reverse().join("");

    var num2 = RemoveRougeChar(num.replace(/(.{3})/g, "$1,").split("").reverse().join(""));

    console.log(num2);


    // the following line has been simplified. Revision history contains original.
    $this.val(num2);
});

function RemoveRougeChar(convertString){
    if(convertString.substring(0,1) == ","){
        return convertString.substring(1, convertString.length)            
    }
    return convertString;
}


*/
/* Thanks to CSS Tricks for pointing out this bit of jQuery
http://css-tricks.com/equal-height-blocks-in-rows/
It's been modified into a function called at page load and then each time the page is resized. One large modification was to remove the set height before each new calculation. */

equalheight = function (container) {

    var currentTallest = 0,
         currentRowStart = 0,
         rowDivs = new Array(),
         $el,
         topPosition = 0;
    $(container).each(function () {

        $el = $(this);
        $($el).height('auto');
        topPostion = $el.position().top;

        if (currentRowStart != topPostion) {
            for (currentDiv = 0 ; currentDiv < rowDivs.length ; currentDiv++) {
                rowDivs[currentDiv].height(currentTallest);
            }
            rowDivs.length = 0; // empty the array
            currentRowStart = topPostion;
            currentTallest = $el.height();
            rowDivs.push($el);
        } else {
            rowDivs.push($el);
            currentTallest = (currentTallest < $el.height()) ? ($el.height()) : (currentTallest);
        }
        for (currentDiv = 0 ; currentDiv < rowDivs.length ; currentDiv++) {
            rowDivs[currentDiv].height(currentTallest);
        }
    });
}


