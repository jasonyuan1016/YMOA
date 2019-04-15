$(function () {
    $.ajax({     //请求当前用户可以操作的按钮
        url: "/Button/GetUserAuthorizeButton?r=" + Math.random(),
        type: "post",
        data: { "KeyCode": "user", "KeyName": "User" },
        dataType: "json",
        timeout: 5000,
        success: function (data) {
            if (data.success) {
                var toolbar = getToolBar(data);      //common.js
                if (data.search) {     //判断是否有浏览权限
                    $("#ui_user_dg").datagrid({
                        url: "/User/GetAllUserInfo",
                        striped: true, rownumbers: true, pagination: true, pageSize: 20,
                        idField: 'ID',
                        sortName: 'UpdateTime',
                        sortOrder: 'desc',
                        pageList: [20, 40, 60, 80, 100],
                        frozenColumns: [[
                            { field: 'ck', checkbox: true },
                            { width: 100, title: '登录名', field: 'AccountName' },
                            { width: 100, title: '用户名称', field: 'RealName' },
                            {
                                width: 100, title: '所属角色', field: 'RoleID',
                                formatter: function (value, row, index) {
                                    return $("#qryRole option[value='" + value+ "']").text();
                                }
                            }
                        ]],
                        columns: [[
                            { field: 'MobilePhone', title: '联系人手机', width: 100 },
                            { field: 'Email', title: '邮箱', width: 150 },
                            {
                                field: 'IsAble', title: '启用', width: 40, align: 'center',
                                formatter: function (value, row, index) {
                                    return value ? '<img src="../../Content/themes/icon/chk_checked.gif" alt="已启用" title="用户已启用" />' : '<img src="../../Content/themes/icon/chk_unchecked.gif" alt="未启用" title="用户未启用" />';
                                }
                            },
                            {
                                field: 'IfChangePwd', title: '改密', width: 40, align: 'center',
                                formatter: function (value, row, index) {
                                    return value ? '<img src="../../Content/themes/icon/chk_checked.gif" alt="已改密" title="用户已改密" />' : '<img src="../../Content/themes/icon/chk_unchecked.gif" alt="未改密" title="用户未改密" />';
                                }
                            },
                            { field: 'UpdateTime', title: '最后更新时间', sortable: true, width: 130 },
                            { field: 'UpdateBy', title: '最后更新人', width: 130 }
                        ]],
                        toolbar: toolbar.length == 0 ? null : toolbar
                    });
                }
                else {
                    $.show_alert("提示", "无权限，请联系管理员！");
                }
            } else {
                $.show_alert("错误", data.result);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (textStatus == "timeout") {
                $.show_alert("提示", "请求超时，请刷新当前页重试！");
            }
            else {
                $.show_alert("错误", textStatus + "：" + errorThrown);
            }
        }
    })
});

//添加用户
function AddUser() {
    $("<div/>").dialog({
        id: "ui_user_add_dialog",
        href: "/User/UserEdit",
        title: "添加用户",
        height: 370,
        width: 610,
        modal: true,
        buttons: [{
            id: "ui_user_add_btn",
            text: '添 加',
            handler: function () {
                $("#UserEditForm").form("submit", {
                    url: "/User/AddUser",
                    onSubmit: function (param) {
                        GatherParam(param);
                        if ($(this).form('validate')) {
                            $('#ui_user_add_btn').linkbutton('disable');
                            return true;
                        }
                        else {
                            $('#ui_user_add_btn').linkbutton('enable');   //恢复按钮
                            return false;
                        }
                    },
                    success: function (data) {
                        var dataJson = eval('(' + data + ')');
                        if (dataJson.success) {
                            $("#ui_user_add_dialog").dialog('destroy');
                            $.show_alert("提示", dataJson.msg);
                            $("#ui_user_dg").datagrid("reload").datagrid('clearSelections').datagrid('clearChecked');
                        } else {
                            $('#ui_user_add_btn').linkbutton('enable');
                            $.show_alert("提示", dataJson.msg);
                        }
                    }
                });
            }
        }, {
            text: '取 消',
            handler: function () {
                $("#ui_user_add_dialog").dialog('destroy');
                if ($("#qryRole").val() != "0") {
                    $("#sltRole").val($("#qryRole").val());
                } else {
                    $("#sltRole option").eq(1).attr("selected", true);
                }
            }
        }],
        onLoad: function () {
            loadRoleDataFromQryRole();
        },
        onClose: function () {
            $("#ui_user_add_dialog").dialog('destroy');  //销毁dialog对象
        }
    });
}

//修改用户
function EditUser() {
    var row = $("#ui_user_dg").datagrid("getChecked");
    if (row.length < 1) {
        $.show_alert("提示", "请先勾选要修改的用户");
        return;
    }
    if (row.length > 1) {
        $.show_alert("提示", "不支持批量修改用户");
        $("#ui_user_dg").datagrid('clearSelections').datagrid('clearChecked');
        return;
    }
    $("<div/>").dialog({
        id: "ui_user_edit_dialog",
        href: "/User/UserEdit?ID=" + row[0].ID,
        title: "修改用户",
        height: 370,
        width: 610,
        modal: true,
        buttons: [{
            id: "ui_user_edit_btn",
            text: '修 改',
            handler: function () {
                $("#UserEditForm").form("submit", {
                    url: "/User/EditUser",
                    onSubmit: function (param) {
                        GatherParam(param);
                        if ($(this).form('validate')) {
                            $('#ui_user_edit_btn').linkbutton('disable');
                            return true;
                        }
                        else {
                            $('#ui_user_edit_btn').linkbutton('enable');
                            return false;
                        }
                    },
                    success: function (data) {
                        var dataJson = eval('(' + data + ')');
                        if (dataJson.success) {
                            $("#ui_user_edit_dialog").dialog('destroy');
                            $.show_alert("提示", dataJson.msg);
                            $("#ui_user_dg").datagrid("reload").datagrid('clearSelections').datagrid('clearChecked');
                        } else {
                            $('#ui_user_edit_btn').linkbutton('enable');
                            $.show_alert("提示", dataJson.msg);
                        }
                    }
                });
            }
        }, {
            text: '取 消',
            handler: function () {
                $("#ui_user_edit_dialog").dialog('destroy');
            }
        }],
        onLoad: function () {
            loadRoleDataFromQryRole();
        },
        onClose: function () {
            $("#ui_user_edit_dialog").dialog('destroy');  //销毁dialog对象
        }
    });
}

//收集参数-新增/修改 时
function GatherParam(param) {
    param.id = $("#hidid").val();
    param.UserID = $("#txtUserIDEdit").val()
    param.UserName = $("#txtUserNameEdit").val();
    param.RoleID = $("#sltRole").val();
    param.Isable = document.getElementById("selUserIsableEdit").checked;
    param.IfChangepwd = document.getElementById("selIfChangepwdEdit").checked;
    param.Description = $("#txtUserDescriptionEdit").val();
    param.MobilePhone = $("#txtMobilePhone").val();
    param.Email = $("#txtEmail").val();
}

//删除用户（可批量）
function DelUser() {
    var rows = $("#ui_user_dg").datagrid("getChecked");
    if (rows.length < 1) {
        $.show_alert("提示", "请先勾选要删除的用户");
        return;
    }
    $.messager.confirm('提示', '确定删除选中行吗？', function (r) {
        if (r) {
            var parmIds = "";
            $.each(rows, function (i, row) {
                parmIds += row.ID + ",";
            });
            parmIds = parmIds.substring(0, parmIds.length - 1);
            $.ajax({
                url: "/User/DelUserByIDs",
                data: {
                    IDs: parmIds
                },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (data.success) {
                        $.show_alert("提示", data.msg);
                        $("#ui_user_dg").datagrid("reload").datagrid('clearSelections').datagrid('clearChecked');
                    } else {
                        $.show_alert("提示", data.msg);
                    }
                }
            });
        }
    });
}

//角色设置（可批量）
function SetUserRole() {
    var row = $("#ui_user_dg").datagrid("getChecked");
    if (row.length < 1) {
        $.show_alert("提示", "请先勾选要设置角色的用户");
        return;
    }
    $("<div/>").dialog({
        id: "ui_user_setrole_dialog",
        href: "/User/UserRole",
        title: row.length == 1 ? "设置角色" : "批量设置角色：" + row.length + "个用户",
        height: 220,
        width: 380,
        modal: true,
        buttons: [{
            id: "ui_user_setrole_btn",
            text: '确 定',
            handler: function () {
                $("#SetRoleForm").form("submit", {
                    url: "/User/SetUserRole",
                    onSubmit: function (param) {
                        $('#ui_user_setrole_btn').linkbutton('disable');
                        param.UserIDs = $("#hiduserid").val();
                        param.RoleIDs = $("#comboxrole").combobox('getValues');
                    },
                    success: function (data) {
                        var dataJson = eval('(' + data + ')');
                        if (dataJson.success) {
                            $("#ui_user_setrole_dialog").dialog('destroy');
                            $.show_alert("提示", dataJson.msg);
                            $("#ui_user_dg").datagrid("reload").datagrid('clearSelections').datagrid('clearChecked');
                        } else {
                            $('#ui_user_setrole_btn').linkbutton('enable');
                            $.show_alert("提示", dataJson.msg);
                        }
                    }
                });
            }
        }],
        onLoad: function () {
            //如果是设置一个用户就自动勾选已经有的角色
            if (row.length == 1) {
                $('#comboxrole').combobox('setValues', stringToList(row[0].UserRoleId));
                $("#hiduserid").val(row[0].ID);
                $("#txtusernameR").val(row[0].AccountName);
            }
            else {
                var userids = "";
                var usernames = "";
                for (var i = 0; i < row.length; i++) {
                    userids += row[i].ID + ",";
                    usernames += row[i].AccountName + ",";
                }
                $("#hiduserid").val(userids.substring(0, userids.length - 1));
                $("#txtusernameR").val(usernames.substring(0, usernames.length - 1));
            }
        },
        onClose: function () {
            $("#ui_user_setrole_dialog").dialog('destroy');  //销毁dialog对象
        }
    });
}

function ui_user_searchdata() {
    $("#ui_user_dg").datagrid('load', {
        accountid: $('#txtUserId').val(),
        username: $('#txtUserName').val(),
        isable: $('#selUserIsable').val(),
        ifchangepwd: $('#selIfChangepwd').val(),
        adddatestart: $('#txtAddBeginDate').datetimebox('getValue'),
        adddateend: $('#txtAddEndDate').datetimebox('getValue'),
        roleid: $("#qryRole").val()
    });
    $("#ui_user_dg").datagrid('clearSelections').datagrid('clearChecked');
}

function ui_user_cleardata() {
    $('#ui_user_search input').val('');
    $('#ui_user_search select').val('select');
    $("#qryRole").val("0");
    $('#txtAddBeginDate').datetimebox('setValue', '');
    $('#txtAddEndDate').datetimebox('setValue', '');
    $("#ui_user_dg").datagrid('load', {});

    $("#ui_user_dg").datagrid('clearSelections').datagrid('clearChecked');
}

function loadRoleDataFromQryRole() {
    $("#sltRole").html($("#qryRole").html()); //获得列表查询的群组下拉框内容
    $("#sltRole option[value='0']").remove(); //去除“全部”选项
    if ($("#hidRole").val() != "0") {
        $("#sltRole").val($("#hidRole").val());
        $("#txtUserDescriptionEdit").val($("#hidDescription").val())
    }
    
}

//部门设置
function SetUserDept() {
    var row = $("#ui_user_dg").datagrid("getChecked");
    if (row.length < 1) {
        $.show_alert("提示", "请先勾选要设置部门的用户");
        return;
    }
    $("<div/>").dialog({
        id: "setdepdialog",
        href: "/User/SetUserDept",
        title: row.length == 1 ? "设置部门" : "批量设置部门：" + row.length + "个用户",
        height: 220,
        width: 380,
        modal: true,
        buttons: [{
            id: "ui_user_setdep_btn",
            text: '确 定',
            handler: function () {
                $("#SetUserDeptForm").form("submit", {
                    url: "/User/UserDeptSet",
                    onSubmit: function (param) {
                        $('#ui_user_setdep_btn').linkbutton('disable');    //点击就禁用按钮，防止连击
                        var parentid = $("#treeDepartmentParentId").combotree("getValues").toString();
                        param.UserIds = $("#hidUserIdDept").val();
                        param.DeptIds = parentid;
                    },
                    success: function (data) {
                        var dataJson = eval('(' + data + ')');    //转成json格式
                        if (dataJson.success) {
                            $("#setdepdialog").dialog('destroy');  //销毁dialog对象
                            $.show_alert("提示", dataJson.msg);
                            $("#ui_user_dg").datagrid("reload").datagrid('clearSelections').datagrid('clearChecked');
                        } else {
                            $('#ui_user_setdep_btn').linkbutton('enable');   //恢复按钮
                            $.show_alert("提示", dataJson.msg);
                        }
                    }
                });
            }
        }],
        onLoad: function () {
            if (row.length == 1) {
                $('#treeDepartmentParentId').combotree('setValues', stringToList(row[0].UserDepartmentId));
                $("#hidUserIdDept").val(row[0].ID);
                $("#txtUserNameDept").val(row[0].RealName);
            }
            else {
                var userids = "";
                var usernames = "";
                for (var i = 0; i < row.length; i++) {
                    userids += row[i].ID + ",";
                    usernames += row[i].RealName + ",";
                }
                $("#hidUserIdDept").val(userids.substring(0, userids.length - 1));
                $("#txtUserNameDept").val(usernames.substring(0, usernames.length - 1));
            }
        },
        onClose: function () {
            $("#setdepdialog").dialog('destroy');  //销毁dialog对象
        }
    });
}