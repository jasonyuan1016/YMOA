var ProjectId = $.request("proID");
var PerId = $.request("person");
var type = $.request("type");

function btn_back() {
    if (type == 1) {
        location.href = "/Hours/PerProHours?ID=" + PerId;
    } else if (type == 2) {
        location.href = "/Hours/ProHours?ID=" + ProjectId;
    } else {
        location.href = "/Hours/Index";
    }
    
}

$(function () {
    gridList();
    //console.log(PerId)
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
        url: "/Hours/GetTaskByPerAndPro",
        postData: { ProName: ProjectId, PerName: PerId },
        height: $(window).height() - 128,
        colModel: [
            { label: PageResx.col_Name, name: 'ProjectId', width: 80, align: 'left' },
            { label: PageResx.col_TaskName, name: 'TaskId', width: 80, align: 'left' },
            { label: PageResx.col_oper, name: 'Person', width: 80, align: 'left' },
            { label: PageResx.col_Describe, name: 'Hour', width: 80, align: 'left' }
            
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