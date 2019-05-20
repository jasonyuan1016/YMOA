$(function () {
    gridList();
})
function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "GetGridJson",
        treeGrid: true,
        treeGridModel: "adjacency",
        ExpandColumn: "controller",
        height: $(window).height() - 128,
        colModel: [
            { label: 'ID', name: 'id', hidden: true },
            { label: PageResx.col_name, name: 'name', width: 100, align: 'left' },
            { label: PageResx.col_controller, name: 'controller', width: 80, align: 'left' },
            { label: PageResx.col_action, name: 'action', width: 80, align: 'left' },
            { label: PageResx.col_code, name: 'code', width: 100, align: 'left' },
            { label: PageResx.col_sortvalue, name: 'sortvalue', width: 80, align: 'left' },
            {
                label: PageResx.col_state, name: "state", width: 80, align: "left",
                formatter: function (cellvalue, options, rowObject) {
                    if (cellvalue == 1) {
                        return '<span class=\"label label-success\">已启用</span>';
                    } else if (cellvalue == 0) {
                        return '<span class=\"label label-default\">已禁用</span>';
                    }
                }
            }
        ]
    });
}
function btn_add() {
    $.modalOpen({
        id: "MemuEdit",
        title: GlobalResx.add,
        url: "/Memu/Edit",
        width: "420px",
        height: "430px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
function btn_edit() {
    var keyValue = $("#gridList").jqGridRowValue().id;
    $.modalOpen({
        id: "MemuEdit",
        title: GlobalResx.edit,
        url: "/Memu/Edit?ID=" + keyValue,
        width: "420px",
        height: "430px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
function btn_delete() {
    $.deleteForm({
        url: "Delete",
        param: { ID: $("#gridList").jqGridRowValue().id },
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}

function btn_disabled() {
    UpdateState(PageResx.confirm4disable, 0);
}
function btn_enabled() {
    UpdateState(PageResx.confirm4enable, 1);
}

function UpdateState(prompt, state) {
    var keyValue = $("#gridList").jqGridRowValue().id;
    $.modalConfirm(prompt, function (r) {
        if (r) {
            $.submitForm({
                url: "UpdateState",
                param: { "ID": keyValue, "state": state },
                success: function () {
                    $.currentWindow().$("#gridList").trigger("reloadGrid");
                }
            })
        }
    });
}

