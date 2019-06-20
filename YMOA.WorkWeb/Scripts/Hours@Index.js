﻿$(function () {
    gridList();
    btn_task();
})
function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "GetAllProject",
        height: $(window).height() - 128,
        colModel: [
            { label: PageResx.col_Name, name: 'ProjectId', width: 80, align: 'left' },
            { label: PageResx.col_Describe, name: 'Hour', width: 80, align: 'left' },
            {
                label: PageResx.col_oper, name: 'ProjectId', width: 80, align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    var update = '<a id="task" authorize="yes" data-id="' + cellvalue + '" data-pid="' + rowObject.TaskId + '">进入</a>'
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
}
function btn_task() {
    $("#gridList").on("click", "#task", function () {
        var ID = $(this).data("pid");
        console.log(ID )
        location.href = "/Hours/ProHours?ID=" + ID;
    })
}