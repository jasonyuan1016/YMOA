$(function () {
    gridList();
})
function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "GetGridJson",
        height: $(window).height() - 128,
        colModel: [
            { label: '主键', name: 'id', hidden: true },
            { label: '角色名称', name: 'name', width: 80, align: 'left' },
            { label: 'Code', name: 'code', width: 80, align: 'left' },
            { label: '状态', name: 'state', width: 80, align: 'left' }
        ]
    });
}
function btn_add() {
    $.modalOpen({
        id: "GroupEdit",
        title: "新增",
        url: "/Group/Edit",
        width: "700px",
        height: "510px",
        btn: null
    });
}
function btn_edit() {
    var keyValue = $("#gridList").jqGridRowValue().id;
    $.modalOpen({
        id: "GroupEdit",
        title: "修改",
        url: "/Group/Edit?ID=" + keyValue,
        width: "700px",
        height: "510px",
        btn: null
    });
}
function btn_delete() {
    $.deleteForm({
        url: "/Group/Delete",
        param: { keyValue: $("#gridList").jqGridRowValue().id },
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}