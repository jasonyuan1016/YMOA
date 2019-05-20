$(function () {
    gridList();
})
function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "GetGridJson",
        height: $(window).height() - 128,
        colModel: [
            { label: 'ID', name: 'id', hidden: true },
            { label: PageResx.col_name, name: 'name', width: 80, align: 'left' },
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
    $.modalOpen({
        id: "Edit",
        title: GlobalResx.add,
        url: "/Library/Edit?tag=" + $("input[name='tag']:checked").val(),
        width: "420px",
        height: "330px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
function btn_edit() {
    var keyValue = $("#gridList").jqGridRowValue().id;
    $.modalOpen({
        id: "Edit",
        title: GlobalResx.edit,
        url: "/Library/Edit?ID=" + keyValue + "&tag=" + $("input[name='tag']:checked").val(),
        width: "420px",
        height: "330px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
function btn_delete() {
    $.deleteForm({
        url: "Delete",
        param: { ID: $("#gridList").jqGridRowValue().ID },
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}
