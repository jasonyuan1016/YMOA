$(function () {
    gridList();
})
function gridList() {
    var $gridList = $("#gridList");
    $gridList.dataGrid({
        url: "GetGridJson",
        height: $(window).height() - 128,
        colModel: [
            { label: '主键', name: 'ID', hidden: true },
            { label: '账户', name: 'AccountName', width: 80, align: 'left' },
            { label: '姓名', name: 'RealName', width: 80, align: 'left' },
            { label: '手机号', name: 'MobilePhone', width: 80, align: 'left' },
            { label: 'Email', name: 'Email', width: 80, align: 'left' },
            {
                label: '角色', name: 'RoleId', width: 80, align: 'left',
                formatter: function (cellvalue, options, rowObject) {
                    return top.clients.groups[cellvalue] == null ? "" : top.clients.groups[cellvalue];
                }
            },
            {
                label: '部门', name: 'DepartmentId', width: 80, align: 'left',
                formatter: function (cellvalue, options, rowObject) {
                    return top.clients.departments[cellvalue] == null ? "" : top.clients.departments[cellvalue];
                }
            },
            {
                label: '入职时间', name: 'Entrydate', width: 80, align: 'left',
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
        title: "新增用户",
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
        title: "修改用户",
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
function btn_details() {
    var keyValue = $("#gridList").jqGridRowValue().F_Id;
    $.modalOpen({
        id: "Details",
        title: "查看用户",
        url: "/User/Details?keyValue=" + keyValue,
        width: "700px",
        height: "550px",
        btn: null,
    });
}
function btn_revisepassword() {
    var keyValue = $("#gridList").jqGridRowValue().F_Id;
    var Account = $("#gridList").jqGridRowValue().F_Account;
    var RealName = $("#gridList").jqGridRowValue().F_RealName;
    $.modalOpen({
        id: "RevisePassword",
        title: '重置密码',
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
    $.modalConfirm("注：您确定要【禁用】该项账户吗？", function (r) {
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
    $.modalConfirm("注：您确定要【启用】该项账户吗？", function (r) {
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