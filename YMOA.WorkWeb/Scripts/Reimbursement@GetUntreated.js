var departmentId = $("#topPanel").data('departmentid');
var flag = false;
$(function () {
    falg = departmentId == 4 ? false : true;
    gridList();
    disagree();
    agree();
});
function gridList() {
    var $gridList = $("#gridList");
    var department = top.clients.departments;
    var users = top.clients.users;
    $gridList.dataGrid({
        url: "GetUntreatedRB",
        height: $(window).height() - 128,
        colModel: [
            { label: 'ID', name: 'ID', hidden: true },
            {
                label: PageResx.col_Department, name: 'Department', width: 140, align: 'left',
                formatter: function (cellvalue) {
                    var dep = "";
                    $.each(department, function (i) {
                        if (department[i].id == cellvalue) {
                            dep = department[i].name;
                        }
                    });
                    return dep;
                }
            },
            { label: PageResx.col_Purpose, name: 'Purpose', width: 140, align: 'left' },
            { label: PageResx.col_Money, name: 'Money', width: 140, align: 'left' },
            { label: PageResx.col_Applicant, name: 'Applicant', width: 140, align: 'left' },
            {
                label: PageResx.col_ApplicantTime, name: 'ApplicantTime', width: 140, align: 'left',
                formatter: "date", formatoptions: { srcformat: 'Y-m-d', newformat: 'Y-m-d' }
            },
            {
                label: PageResx.col_Manager, name: 'Manager', width: 140, align: 'left', hidden: flag,
                formatter: function (cellvalue) {
                    var user = "";
                    $.each(users, function (i) {
                        if (users[i].AccountName == cellvalue) {
                            user = users[i].RealName;
                        }
                    });
                    return user;
                }
            },
            {
                label: PageResx.col_oper, name: 'State', align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    var oper = "<a id=\"agree\" data-state=" + cellvalue + " data-id = " + rowObject.ID + " authorize=\"yes\">同意</a>";
                    oper += "&nbsp;&nbsp;&nbsp;&nbsp" + "<a id=\"disagree\" data-state=" + cellvalue + " data-id = " + rowObject.ID + " authorize=\"yes\">不同意</a>";
                    return oper;
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
}
function disagree() {
    $("#gridList").on("click", "#disagree", function () {
        var state = $("#disagree").data("state");
        var id = $("#disagree").data("id");
        if (state == 1) {
            state += 3;
            console.log(state + "," + id);
        }
        else {
            state += 2;
            console.log(state + "," + id);
        }
        $.modalConfirm("注：您确定要【同意】该项报销吗？", function (r) {
            if (r) {
                $.submitForm({
                    url: "/Reimbursement/Edit",
                    param: { ID: id,State:state},
                    success: function () {
                        $.currentWindow().$("#gridList").trigger("reloadGrid");
                    }
                })
            }
        });
    });
}
function agree() {
    $("#gridList").on("click", "#agree", function () {
        var state = $("#agree").data("state");
        var id = $("#agree").data("id");
        state += 1;
        console.log(state+","+id);
        $.modalConfirm("注：您确定要【同意】该项报销吗？", function (r) {
            if (r) {
                $.submitForm({
                    url: "/Reimbursement/Edit",
                    param: { ID: id,State:state},
                    success: function () {
                        $.currentWindow().$("#gridList").trigger("reloadGrid");
                    }
                })
            }
        });
    });
}