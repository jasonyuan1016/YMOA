﻿$(function () {
    gridList();
})
function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "GetGridJson",
        height: $(window).height() - 128,
        sortname: 'sort',
        colModel: [
            { label: 'ID', name: 'id', hidden: true },
            { label: 'Tag', name: 'tag', hidden: true },
            { label: PageResx.col_name, name: 'name', width: 80, align: 'left' },
            { label: PageResx.col_code, name: 'code', width: 80, align: 'left' },
            { label: PageResx.col_sort, name: 'sort', width: 80, align: 'left' },
        ]
    });
    $("input[name='tag']").click(function () {
        $gridList.jqGrid('setGridParam', {
            postData: { tag: $("input[name='tag']:checked").val() },
        }).trigger('reloadGrid');
    });
}
function btn_add() {
    var tag = $("input[name='tag']:checked").val();
    var height = "280px";
    if (tag == "1") {
        height = "330px";
    }
    $.modalOpen({
        id: "Edit",
        title: GlobalResx.add,
        url: "/Library/Edit?tag=" + tag,
        width: "420px",
        height: height,
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
function btn_edit() {
    var keyValue = $("#gridList").jqGridRowValue().id;
    var height = "280px";
    var tag = $("input[name='tag']:checked").val();
    if (tag == "1") {
        height = "330px";
    }
    $.modalOpen({
        id: "Edit",
        title: GlobalResx.edit,
        url: "/Library/Edit?ID=" + keyValue + "&tag=" + tag,
        width: "420px",
        height: height,
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
function btn_delete() {
    var obj = $("#gridList").jqGridRowValue();
    $.deleteForm({
        url: "Delete",
        param: { ID: obj.id, tag: obj.tag },
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}
