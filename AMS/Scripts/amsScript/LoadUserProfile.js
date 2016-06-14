function LoadUserProfile(UserId) {
    LoadUserProfile(UserId,false)
}
function LoadUserProfile(UserId, loadMember) {
   // alert(loadMember)
    getUser(UserId, loadMember);
}
function getUser(userid,loadMember) {
    $.ajax({
        url: "/Home/getUser",
        type: "GET",
        data: {
            UserId: userid,
        },
        success: function (successData) {
            var obj = JSON.parse(successData);
            addModal(obj,loadMember);
            showModal(obj);
            if (loadMember == true) {
              //  alert('load')
                loadAllMember("treeModal", obj['HouseId']);
            }
        },
        error: function (er) {
            alert(er)
        }

    });
}
function showModal(successData) {
    $("#userprofile" + successData["Id"]).modal('show')
    $("#userprofile" + successData["Id"]).on('hidden.bs.modal', function (e) {
        clearModal(successData["Id"])
    })

}
function clearModal(id) {
    $("#userprofile" + id).remove();
}
function addModal(data,loadMember) {
    var imgSrc = "/Content/images/defaultProfile.png";
    var DateOfBirth = new Date(data['dateOfBirth']);
    var StringDateOfBirth = "Không có thông tin";
    var StringSex = "Male";
    if (DateOfBirth.toDateString() != 'Invalid Date') {
        StringDateOfBirth = DateOfBirth.toDateString();
    }
    if (data['Gender'] === 0) {
        StringSex = "Nam"
    } else {
        StringSex = "Nữ"
    }
    $("body").append('<!-- Modal -->'
+ '<div id="userprofile' + data['Id'] + '" class="modal fade" role="dialog" style="height:auto">'
    +'<div class="modal-dialog" style="width:60%">'
        +'<!-- Modal content-->'
        +'<div class="modal-content">'
            
            +'<div class="modal-body">'
                +'<div class="row">'
                    +'<div class="col-md-4">'
                        +'<div class="avatar" style="text-align:center">'
                            + '<img src="' + imgSrc + '" alt="" class="img-circle" style="height: 130px;">'
                            + '<h3>' + data['FullName'] + '</h3>'
                            +'<a href="#" class="btn btn-success">Following <i class="fa fa-check-circle fa-fw"></i></a>'
                        +'</div>'
                    +'</div>'
                    +'<div class="col-md-8">'
                        +'<div class="panel panel-default">'
                            +'<div class="panel-heading panel-heading-gray">'
                                +'<a href="#" class="btn btn-white btn-xs pull-right"><i class="fa fa-pencil"></i></a>'
                                +'<i class="fa fa-info-circle"></i> Thông tin'
                            +'</div>'
                            +'<div class="panel-body">'
                                +'<ul class="list-unstyled profile-about">'
                                    +'<li>'
                                        +'<div class="row">'
                                            +'<div class="col-sm-4">'
                                                +'<span class="text-muted">Ngày sinh</span>'
                                            +'</div>'
                                            +'<div class="col-sm-8">'+StringDateOfBirth +'</div>'
                                        +'</div>'
                                    +'</li>'
                                   
                                    +'<li>'
                                        +'<div class="row">'
                                            +'<div class="col-sm-4">'
                                                +'<span class="text-muted">Giới tính</span>'
                                            +'</div>'
                                            +'<div class="col-sm-8">'+StringSex+ '</div>'
                                        +'</div>'
                                    +'</li>'
                                    +'<li>'
                                        +'<div class="row">'
                                            +'<div class="col-sm-4">'
                                                +'<span class="text-muted">Email</span>'
                                            +'</div>'
                                            +'<div class="col-sm-8">'+data['Email']+'</div>'
                                        +'</div>'
                                    +'</li>'
                                    +'<li>'
                                        +'<div class="row">'
                                            +'<div class="col-sm-4">'
                                                +'<span class="text-muted">Số điện thoại</span>'
                                            +'</div>'
                                            +'<div class="col-sm-8"></div>'
                                        +'</div>'
                                    +'</li>'
                                +'</ul>'
                            +'</div>'
                        +'</div>'
                    +'</div>'
                + '</div>'
                  + '<div class="row" style="text-align:center" id="houseinfoRow">'
                  
                            + '</div>'
                              
            +'</div>'
            +'<div class="modal-footer">'
                + '<button type="button" class="btn btn-default" data-dismiss="modal" onclick="clearModal("' + data['Id'] + '")">Close</button>'
            +'</div>'
        +'</div>'

    +'</div>'
+ '</div>')

    if (loadMember == true) {
        $("#houseinfoRow").append('<div class="panel panel-default">'
                            + '<div class="panel-heading panel-heading-gray">'
                                + '<a href="#" class="btn btn-white btn-xs pull-right"><i class="fa fa-pencil"></i></a>'
                                + '<i class="fa fa-info-circle"></i> Thông tin gia đình'
                            + '</div>'
                            + '<div class="panel-body">'
                              + '<div class="tree">'
                                    + '<ul>'

                                        + '<li id="CoupletreeModal">'
                                            + '<a href="#">'
                                                + '<div style="width:100px;height:120px;float:left">'
                                                    + '<img src="' + data['HouseProfile'] + '" style="width:100%;" />'
                                                    + '<div style="margin-top: 5px; font-size: small;font-weight: 700;background-color: aliceblue;">'
                                                        + data['HouseName']
                                                    + '</div>'
                                                + '</div>'
                                            + '</a>'
                                       + ' </li>'
                                    + '</ul>'
                                + '</div>'
                           + ' </div>'
                            + '</div>');
    }
}