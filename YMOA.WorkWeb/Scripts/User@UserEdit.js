var ID = $.request("ID");
$(function () {
    initControl();
    if (!!ID) {
        $("#txtPassword").val("******").attr('disabled', 'disabled');
        $("#txtAccountName").attr('disabled', 'disabled');
        $("#sltDepartmentId").val($("#sltDepartmentId").attr("value"));
        $("#sltRoleId").val($("#sltRoleId").attr("value"));
        $("#sltIsAble").val($("#sltIsAble").attr("value") == undefined ? "false" : "true");
        $("#txtEntrydate").val(new Date($("#txtEntrydate").val()).Format("yyyy-MM-dd"));
        $("#txtBirthday").val(new Date($("#txtBirthday").val()).Format("yyyy-MM-dd"));
        $("#txtRealName").val($.trim($("#txtRealName").val()));
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
        url: "/User/" + (!!ID == true ? "Update":"Add"),
        param: userEntity,
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}