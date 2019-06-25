var ID = $.request("ID");
$(function () {
    initControl();
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