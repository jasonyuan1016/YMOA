
$(function () {
    var ID = parseInt($("#hidID").val());
    initControl();
    if (ID > 0) {
        $("#txtPassword").val("******");
    }
});
function initControl() {
    var topData = top.clients;
    $("#sltDepartmentId").bindSlt(topData.departments);
    $("#sltRoleId").bindSlt(topData.groups);
}
function submitForm() {
    if (!$('#UserEidt').formValid()) {
        return false;
    }
    var userEntity = $("#UserEidt").dataSerialize();
    if (userEntity.Password != "******") {
        userEntity.Password = $.md5($.trim(userEntity.Password));
    }
    $.submitForm({
        url: "/User/SubmitForm",
        param: userEntity,
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}