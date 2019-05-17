var ID = $.request("ID");
$(function () {
    initControl();
    if (!!ID) {
        $("#sltparentid").val($("#sltparentid").data("value"));
    }
});
function initControl() {
    var topData = top.clients;
    $("#sltparentid").bindSltSpe({
        id: "id", name: "name", data: top.clients.menus
    });
}
function submitForm() {
    if (!$('#MemuEidt').formValid()) {
        return false;
    }
    var userEntity = $("#MemuEidt").dataSerialize();
    $.submitForm({
        url: "/Memu/" + (!!ID == true ? "Update" : "Add"),
        param: userEntity,
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}