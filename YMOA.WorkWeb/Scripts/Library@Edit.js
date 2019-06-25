var ID = $.request("ID");
$(function () {
    initControl();
    var tag = $("#hidtag").val();
    //if (tag == 1) {
    //    $("#sltDutyPerson").bindSltSpe({
    //        data: top.clients.users, id: "AccountName", name: "RealName"
    //    });
    //}
    if (!!ID) {
        $("#sltpid").val($("#sltpid").data("value"));
        $("#sltDutyPerson").val($("#sltDutyPerson").data("value"));
    }

});
function initControl() {

}
function submitForm() {
    if (!$('#LibraryEidt').formValid()) {
        return false;
    }
    var userEntity = $("#LibraryEidt").dataSerialize();
    $.submitForm({
        url: "/Library/" + (!!ID == true ? "Update" : "Add"),
        param: userEntity,
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}