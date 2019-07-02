var PerId = $.request("ID");

function btn_back() {
    location.href = "/Hours/PerHours";
}

$(function () {
    gridList();
    btn_task();
})
function gridList() {
    //var now = new Date();
    //var startday = ("0" + (now.getDate() - 7)).slice(-2);
    //var endday = ("0" + now.getDate()).slice(-2);
    //var month = ("0" + (now.getMonth() + 1)).slice(-2);
    //var start = now.getFullYear() + "-" + (month) + "-" + (startday);
    //var end = now.getFullYear() + "-" + (month) + "-" + (endday);
    //$("#StartTime").val(start);
    //$("#EndTime").val(end);
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "/Hours/GetProjectHoursByPerson?PerName=" + PerId,
        height: $(window).height() - 128,
        colModel: [
            { label: PageResx.col_Name, name: 'TaskId', width: 80, align: 'left' },
            { label: PageResx.col_oper, name: 'Person', width: 80, align: 'left' },
            { label: PageResx.col_Describe, name: 'Hour', width: 80, align: 'left' },
            {
                label: PageResx.col_operation, name: 'ProjectId', width: 80, align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    var update = '<a id="task" authorize="yes" data-id="' + cellvalue + '" data-pid="' + rowObject.ProjectId + '">进入</a>'
                    return update;
                }
            }
        ],
        pager: "#gridPager",
        //sortname: 'ID',
        //sortorder: "desc", // 倒叙
        rowNum: 20,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true
    });
    $("#btn_search").click(function () {
        start = $("#StartTime").val();
        end = $("#EndTime").val();
            $gridList.jqGrid('setGridParam', {
                postData: {
                    startTime: start,
                    endTime: end
                }
            }).trigger('reloadGrid');
    });
}
function btn_task() {
    $("#gridList").on("click", "#task", function () {
        var ID = $(this).data("pid");
        location.href = "/Hours/TaskHours?proID=" + ID + "&person=" + PerId+"&type=1";
    })
}