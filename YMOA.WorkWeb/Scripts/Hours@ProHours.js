var ProjectId = $.request("ID");

function btn_back() {
    location.href = "/Hours/Index";
}

$(function () {
    gridList();
})
function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "/Hours/GetProjectByPerson?ProName=" + ProjectId ,
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

}