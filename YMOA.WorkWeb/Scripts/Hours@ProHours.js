var ProjectId = $.request("ID");

function btn_back() {
    location.href = "/Hours/Index";
}

$(function () {
    gridList();
})
function gridList() {
    var now = new Date();
    var startday = ("0" + (now.getDate() - 7)).slice(-2);
    var endday = ("0" + now.getDate()).slice(-2);
    var month = ("0" + (now.getMonth() + 1)).slice(-2);
    var start = now.getFullYear() + "-" + (month) + "-" + (startday);
    var end = now.getFullYear() + "-" + (month) + "-" + (endday);
    $("#StartTime").val(start);
    $("#EndTime").val(end);
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "/Hours/GetProjectByPerson?ProName=" + ProjectId,
        height: $(window).height() - 128,
        colModel: [
            { label: PageResx.col_Name, name: 'TaskId', width: 80, align: 'left' },
            { label: PageResx.col_Describe, name: 'Hour', width: 80, align: 'left' },
            { label: PageResx.col_oper, name: 'Person', width: 80, align: 'left' }

        ],
        pager: "#gridPager",
        //sortname: 'ID',
        //sortorder: "desc", // 倒叙
        rowNum: 20,
        rowList: [10, 20, 30, 40, 50],
        viewrecords: true
    });
    $("#btn_search").click(function () {
        start = new Date($("#StartTime").val()).Format("yyyy-MM-dd");
        end = new Date($("#EndTime").val());
        console.log(start);
        var time = (end - start) / (1000 * 60 * 60 * 24);
        if (time >= 7) {
            $gridList.jqGrid('setGridParam', {
                postData: {
                    startTime: start,
                    endTime: end
                }
            }).trigger('reloadGrid');
        }
        else {
            alert("七天内无法查询！");
        }
    });
}
//function ChangeDateFormat(date) {
//    return date.Format("yyyy-MM-dd hh:mm:ss");
//}