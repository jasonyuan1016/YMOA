$(function () {
    gridList();
})
function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "GetItem",
        height: $(window).height() - 128,
        colModel: [
            { label: 'ID', name: 'ID', hidden: true },
            { label: '项目名称', name: 'Name', width: 140, align: 'left' },
            { label: '项目描述', name: 'Describe', width: 140, align: 'left' },
            { label: '负责人', name: 'DutyPerson', width: 140, align: 'left' },
            { label: '创建人', name: 'CreateBy', width: 140, align: 'left' },
            {
                label: '开始日期', name: 'StartTime', width: 140, align: 'left',
                formatter: "date", formatoptions: { srcformat: 'Y-m-d', newformat: 'Y-m-d' }
            },
            {
                label: '结束日期', name: 'EndTime', width: 140, align: 'left',
                formatter: "date", formatoptions: { srcformat: 'Y-m-d', newformat: 'Y-m-d' }
            }
        ],
        pager: "#gridPager",
        sortname: 'ID',
        //sortorder: "desc", // 倒叙
        rowNum: 20,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true
    });
    $("input[name='tag']").click(function () {
        $gridList.jqGrid('setGridParam', {
            postData: {
                State: $("input[name='tag']:checked").val(),
                Person: null
            },
        }).trigger('reloadGrid');
    });
    $("#btn_search").click(function () {
        $gridList.jqGrid('setGridParam', {
            postData: {
                keywords: $("#txt_keyword").val(),
                Person: null
            },
        }).trigger('reloadGrid');
    });
    $("#join").click(function () {
        $("#txt_keyword").val("");
        $gridList.jqGrid('setGridParam', {
            postData: { Person: 1 },
        }).trigger('reloadGrid');
    });
    $gridList.dblclick(function () {
        var ID = $("#gridList").jqGridRowValue().ID;
        var Name = $("#gridList").jqGridRowValue().Name;
        location.href = "/Item/Task?ID=" + ID + "&Name=" + Name;
    })
}
function btn_add() {
    $.modalOpen({
        id: "ProjectEdit",
        title: GlobalResx.add,
        url: "/Item/ProjectEdit",
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
        id: "ProjectEdit",
        title: GlobalResx.edit,
        url: "/Item/ProjectEdit?ID=" + keyValue,
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

function btn_team() {
    var keyValue = $("#gridList").jqGridRowValue().ID;
    $.modalOpen({
        id: "Team",
        title: "团队",
        url: "/Item/Team?ID=" + keyValue,
        width: "700px",
        height: "510px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
