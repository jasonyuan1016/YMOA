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
            { label: PageResx.col_account, name: 'AccountName', width: 80, align: 'left' },
            { label: PageResx.col_realname, name: 'RealName', width: 80, align: 'left' },
            { label: PageResx.col_cellphone, name: 'MobilePhone', width: 80, align: 'left' },
            { label: 'Email', name: 'Email', width: 80, align: 'left' },
            {
                label: PageResx.col_role, name: 'RoleId', width: 80, align: 'left',
                formatter: function (cellvalue, options, rowObject) {
                    return top.clients.groups[cellvalue] == null ? "" : top.clients.groups[cellvalue];
                }
            },
            {
                label: PageResx.col_department, name: 'DepartmentId', width: 80, align: 'left',
                formatter: function (cellvalue, options, rowObject) {
                    return top.clients.departments[cellvalue] == null ? "" : top.clients.departments[cellvalue];
                }
            },
            {
                label: PageResx.col_entrydate, name: 'Entrydate', width: 80, align: 'left',
                formatter: "date", formatoptions: { srcformat: 'Y-m-d', newformat: 'Y-m-d' }
            }
        ],
        pager: "#gridPager",
        sortname: 'ID',
        viewrecords: true
    });
    $("#btn_search").click(function () {
        $gridList.jqGrid('setGridParam', {
            postData: { keyword: $("#txt_keyword").val() },
        }).trigger('reloadGrid');
    });
}
function btn_add() {
    $.modalOpen({
        id: "UserEdit",
        title: GlobalResx.add,
        url: "/User/UserEdit",
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
        id: "UserEdit",
        title: GlobalResx.edit,
        url: "/User/UserEdit?ID=" + keyValue,
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
        param: { keyValue: $("#gridList").jqGridRowValue().ID },
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}
function btn_revisepassword() {
    var keyValue = $("#gridList").jqGridRowValue().F_Id;
    var Account = $("#gridList").jqGridRowValue().F_Account;
    var RealName = $("#gridList").jqGridRowValue().F_RealName;
    $.modalOpen({
        id: "RevisePassword",
        title: PageResx.resetpwd,
        url: '/User/RevisePassword?keyValue=' + keyValue + "&account=" + escape(Account) + '&realName=' + escape(RealName),
        width: "450px",
        height: "260px",
        callBack: function (iframeId) {
            top.frames[iframeId].submitForm();
        }
    });
}
function btn_disabled() {
    var keyValue = $("#gridList").jqGridRowValue().ID;
    $.modalConfirm(PageResx.confirm4disable, function (r) {
        if (r) {
            $.submitForm({
                url: "/User/DisabledAccount",
                param: { keyValue: keyValue },
                success: function () {
                    $.currentWindow().$("#gridList").trigger("reloadGrid");
                }
            })
        }
    });
}
function btn_enabled() {
    var keyValue = $("#gridList").jqGridRowValue().ID;
    $.modalConfirm(PageResx.confirm4enable, function (r) {
        if (r) {
            $.submitForm({
                url: "/User/EnabledAccount",
                param: { keyValue: keyValue },
                success: function () {
                    $.currentWindow().$("#gridList").trigger("reloadGrid");
                }
            })
        }
    });
}