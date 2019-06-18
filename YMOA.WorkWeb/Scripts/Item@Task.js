var urlinfo = window.location.href;//获取url 
var ProjectName = urlinfo.split("&")[1].split("=")[1];//拆分url得到”=”后面的参数 
var ProjectId = $.request("ID");

$(function () {
    $("#projectName").val(decodeURI(ProjectName));
    gridList();
    ClickUpdate();
    ClickDelete();
    ClickState();
    ClickChildToAdd();
})

function TaskState(id, state) {
    var str = [];
    var update = '<a href="javascript:;" class="update" data-id="' + id + '" >修改</a>';
    var del = '<a href="javascript:;" class="delete" data-id="' + id + '" >删除</a>';
    str.push(update);
    str.push(del);
    var reset = '<a href="javascript:;" class="state" data-id="' + id + '" data-state="1" >重置</a>';
    var start = '<a href="javascript:;" class="state" data-id="' + id + '" data-state="2" >启动</a>';
    var complete = '<a href="javascript:;" class="state" data-id="' + id + '" data-state="3" >完成</a>';
    var cancel = '<a href="javascript:;" class="state" data-id="' + id + '" data-state="4" >取消</a>';
    var close = '<a href="javascript:;" class="state" data-id="' + id + '" data-state="5" >关闭</a>';
    if (state == 1) {
        str.push(start);
        str.push(complete);
        str.push(close);
    }
    else if (state == 2) {
        str.push(reset);
        str.push(complete);
        str.push(cancel);
        str.push(close);
    }
    else if (state == 3) {
        str.push(reset);
        str.push(close);
    }
    else if (state == 4) {
        str.push(reset);
        str.push(start);
        str.push(close);
    }
    else if (state == 5) {
        str.push(reset);
        str.push(start);
    }
    return str.join("&nbsp;&nbsp;");

}

function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "/Task/GetGridJson?ProjectId=" + ProjectId,
        height: $(window).height() - 128,
        colModel: [
            { label: 'ID', name: 'ID', hidden: true },
            {
                label: PageResx.col_Name, name: 'Name', width: 100, align: 'left',
                formatter: function (cellvalue, options, rowObject) {
                    if (rowObject.subclass) {
                        return "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + cellvalue;
                    }
                    return cellvalue;
                }
            },
            {
                label: PageResx.col_project, name: 'ProjectId', width: 100, align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    return rowObject.ProjectIdName;
                }
            },
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
            {
                label: PageResx.col_EndTime, name: 'EndTime', width: 140, align: 'center',
                formatter: "date", formatoptions: { srcformat: 'Y-m-d', newformat: 'Y-m-d' }
            },
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
            {
                label: PageResx.col_listTeam, name: 'listTeam', width: 140, align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    var str = "";
                    var separator = "";
                    for (var i = 0; i < cellvalue.length; i++) {
                        str += separator + $.trim(cellvalue[i].Person);
                        separator = ",";
                    }
                    return str;
                }
            },
            { label: PageResx.col_CreateTime, name: 'CreateTime', width: 140, align: 'center' },
            {
                label: PageResx.col_oper, name: 'ID', width: 300, align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    var str = "";
                    if (rowObject.update == 1) {
                        str = TaskState(cellvalue, rowObject.State);
                        if (rowObject.ParentId == "0") {
                            str += "&nbsp;&nbsp;" + '<a href="javascript:;" class="childToAdd" data-pid="' + rowObject.ProjectId + '" data-id="' + cellvalue + '" >子任务</a>';
                        }
                    }
                    return str;
                }
            },
            { label: "", name: '', width: 10, align: 'left' }
        ],
        pager: "#gridPager",
        sortname: 'ProjectId',
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
function btn_back() {
    location.href = "/Item/Index";
}
function btn_batchAdd() {
    $.modalOpen({
        id: "Edit",
        title: GlobalResx.add,
        url: "/Task/BatchAdd",
        width: "1200px",
        height: "400px",
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
        height: "460px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}

function ClickUpdate() {
    $("#gridList").on("click", ".update", function () {
        var ID = $(this).data("id");
        $.modalOpen({
            id: "Edit",
            title: GlobalResx.edit,
            url: "/Task/Edit?ID=" + ID,
            width: "700px",
            height: "560px",
            callBack: function (iframeId) {
                top.frames[iframeId].submitForm();
            }
        });
    })
}

function ClickDelete() {
    $("#gridList").on("click", ".delete", function () {
        var ID = $(this).data("id");
        $.deleteForm({
            url: "Delete",
            param: { ID },
            success: function () {
                $.currentWindow().$("#gridList").trigger("reloadGrid");
            }
        })
    })
}

function ClickState() {
    $("#gridList").on("click", ".state", function () {
        var ID = $(this).data("id");
        var state = $(this).data("state");
        var str = $(this).text();
        $.modalConfirm(PageResx.confirm + str, function (r) {
            if (r) {
                $.submitForm({
                    url: "/Task/UpdateTaskState",
                    param: { ID, state },
                    success: function () {
                        $.currentWindow().$("#gridList").trigger("reloadGrid");
                    }
                })
            }
        });
    })

}

function ClickChildToAdd() {
    $("#gridList").on("click", ".childToAdd", function () {
        var pid = $(this).data("pid");
        var id = $(this).data("id");
        $.modalOpen({
            id: "Edit",
            title: GlobalResx.add,
            url: "/Task/BatchChildToAdd?pid=" + pid + "&id=" + id,
            width: "1100px",
            height: "450px",
            callBack: function (iframeId) {
                top.frames[iframeId].submitForm();
            }
        });
    })
}

