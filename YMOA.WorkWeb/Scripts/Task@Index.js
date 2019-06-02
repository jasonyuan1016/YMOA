$(function () {
    gridList();
})
function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "GetGridJson",
        height: $(window).height() - 128,
        colModel: [
            { label: 'ID', name: 'ID', hidden: true },
            {
                label: PageResx.col_Sort, name: 'Sort', width: 40, align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    for (var i = 0; i < top.clients.prioritys.length; i++) {
                        if (top.clients.prioritys[i].id == cellvalue) {
                            return top.clients.prioritys[i].name;
                        }
                    }
                    return "";
                }
            },
            { label: PageResx.col_Name, name: 'Name', width: 120, align: 'center' },
            { label: PageResx.col_EndTime, name: 'EndTime', width: 180, align: 'center' },
            { label: PageResx.col_Estimate, name: 'Estimate', width: 80, align: 'center' },
            { label: PageResx.col_Consume, name: 'Consume', width: 80, align: 'center' },
            {
                label: PageResx.col_State, name: 'State', width: 80, align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    for (var i = 0; i < top.clients.taskStatus.length; i++) {
                        if (top.clients.taskStatus[i].id == cellvalue) {
                            return top.clients.taskStatus[i].name;
                        }
                    }
                    return "";
                }
            },
            { label: PageResx.col_CreateTime, name: 'CreateTime', width: 180, align: 'center' },
            { label: "", name: '', width: 10, align: 'left' }
        ],
        pager: "#gridPager",
        sortname: 'ID',
        //sortorder: "desc", // 倒叙
        rowNum: 20,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true
    });
    $("#taskSelect").on("click", ".btn", function () {
        var qryTag = $(this).data("val");
        $("#taskSelect .btn-primary").removeClass("btn-primary");
        $(this).addClass("btn-primary");
        $gridList.jqGrid('setGridParam', {
            postData: { qryTag },
        }).trigger('reloadGrid');
    })
}

function btn_batchAdd() {
    $.modalOpen({
        id: "Edit",
        title: GlobalResx.add,
        url: "/Task/BatchAdd",
        width: "1200px",
        height: "510px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
function btn_add() {
    $.modalOpen({
        id: "Edit",
        title: GlobalResx.add,
        url: "/Task/Edit",
        width: "700px",
        height: "510px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
function btn_edit() {
    var keyValue = $("#gridList").jqGridRowValue().ID;
    $.modalOpen({
        id: "Edit",
        title: GlobalResx.edit,
        url: "/Task/Edit?ID=" + keyValue,
        width: "700px",
        height: "510px",
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
