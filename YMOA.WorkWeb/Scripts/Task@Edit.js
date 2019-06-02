var ID = $.request("ID");
$(function () {
    initControl();
    if (!!ID) {
        
    }
});
function initControl() {
    var topData = top.clients;
    $("#sltSort").bindSltSpe({
        id: "id", name: "name", data: topData.prioritys
    });
}
function submitForm() {
    if (!$('#TaskEidt').formValid()) {
        return false;
    }
    var TaskEntity = $("#TaskEidt").dataSerialize();
    $.submitForm({
        url: "/Task/" + (!!ID == true ? "Update" : "Add"),
        param: TaskEntity,
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}