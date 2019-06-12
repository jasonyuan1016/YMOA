$(function () {
    gridList();
    btn_edit();
    btn_delete();
    btn_team();
    btn_task();
})
function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "GetItem",
        height: $(window).height() - 128,
        colModel: [
            { label: 'ID', name: 'ID', hidden: true },
            { label: PageResx.col_Name, name: 'Name', width: 140, align: 'left' },
            { label: PageResx.col_Describe, name: 'Describe', width: 140, align: 'left' },
            { label: PageResx.col_DutyPerson, name: 'DutyPerson', width: 140, align: 'left' },
            { label: PageResx.col_CreateBy, name: 'CreateBy', width: 140, align: 'left' },
            {
                label: PageResx.col_StartTime, name: 'StartTime', width: 140, align: 'left',
                formatter: "date", formatoptions: { srcformat: 'Y-m-d', newformat: 'Y-m-d' }
            },
            {
                label: PageResx.col_EndTime, name: 'EndTime', width: 140, align: 'left',
                formatter: "date", formatoptions: { srcformat: 'Y-m-d', newformat: 'Y-m-d' }
            },
            {
                label: PageResx.col_oper, name: 'ID', width: 200, align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    var update = '<a id="update" authorize="yes" data-id="' + cellvalue + '"><i class="fa fa-pencil-square-o"></i>编辑</a>';
                    update += "&nbsp;&nbsp;" + '<a id="delete" authorize="yes" data-id="' + cellvalue + '"><i class="fa fa-trash-o"></i>删除</a>';
                    update += "&nbsp;&nbsp;" + '<a id="team" authorize="yes" data-id="' + cellvalue + '"><i class="fa fa-product-hunt"></i>团队</a>';
                    update += "&nbsp;&nbsp;" + '<a id="task" authorize="yes" class="btn btn-primary dropdown-text" data-id="' + cellvalue + '" data-pid="' + rowObject.Name + '">进入</a>'
                    return update;
                }
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
    $("#gridList").on("click", "#update", function () {
        var ID = $(this).data("id");
        $.modalOpen({
            id: "ProjectEdit",
            title: GlobalResx.edit,
            url: "/Item/ProjectEdit?ID=" + ID,
            width: "700px",
            height: "510px",
            callBack: function (iframeId) {
                top.frames[iframeId].submitForm();
            }
        });
    })
}
function btn_delete() {
    $("#gridList").on("click", "#delete", function () {
        var ID = $(this).data("id");
        $.deleteForm({
            url: "Delete",
            param: { ID },
            success: function () {
                $.currentWindow().$("#gridList").trigger("reloadGrid");
            }
        });
    });
}

function btn_team() {
    $("#gridList").on("click", "#team", function () {
        var ID = $(this).data("id");
        $.modalOpen({
            id: "Team",
            title: "团队",
            url: "/Item/Team?ID=" + ID,
            width: "700px",
            height: "510px",
            callBack: function (iframeId) {
                top.frames[iframeId].submitForm();
            }
        });
    });
}
function btn_task() {
    $("#gridList").on("click", "#task", function () {
        var ID = $(this).data("id");
        var Name = $(this).data("pid");
        location.href = "/Item/Task?ID=" + ID + "&Name=" + Name;
    })
}
