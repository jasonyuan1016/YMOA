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
            { label: PageResx.col_groupName, name: 'name', width: 80, align: 'left' },
            { label: PageResx.col_code, name: 'code', width: 80, align: 'left' },
            {
                label: PageResx.col_state, name: "state", width: 80, align: "left",
                formatter: function (cellvalue, options, rowObject) {
                    if (cellvalue == true) {
                        return '<span class=\"label label-success\">启用</span>';
                    } else if (cellvalue == false) {
                        return '<span class=\"label label-default\">禁用</span>';
                    }
                }
            }
        ]
    });
}
function btn_add() {
    $.modalOpen({
        id: "GroupEdit",
        title: GlobalResx.add,
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
        title: GlobalResx.edit,
        url: "/Group/Edit?ID=" + keyValue,
        width: "700px",
        height: "510px",
        btn: null
    });
}
function btn_delete() {
    $.deleteForm({
        url: "Delete",
        param: { keyValue: $("#gridList").jqGridRowValue().id },
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}