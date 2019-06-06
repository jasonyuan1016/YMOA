﻿
var strProject = ""; // 项目字符串
var strTeams = ""; // 成员字符串
var sltSort = ""; // 优先级字符串
$(function () {
    initControl();
});

// 批量任务模板
function batchTaskTemplate(i) {
    var template = '<tr>'
        + '<td>'+i+'</td>'
        + '<td class="formValue">'
        + '<input type="text" class="form-control required txtName" placeholder="请输入名称" autocomplete="off" id="txtName_' + i + '" name="txtName_' + i +'"/>'
        + '</td>'
        + '<td class="formValue">'
        + '<select class="form-control input-select sltProject" id="sltProject">(#)</select>'
        + '</td>'
        + '<td class="formValue bor-radius">'
        + '<select class="form-control teams" multiple data-actions-box="true" id="sltTeams" name="sltTeams">($)</select>'
        + '</td>'
        + '<td class="formValue">'
        + '<textarea class="form-control texDescribe" rows="1" id="texDescribe"></textarea>'
        + '</td>'
        + '<td class="formValue">'
        + '<input type="text" class="form-control required input-wdatepicker greaterDate txtEndTime" autocomplete="off" onfocus="WdatePicker()" id="txtEndTime_' + i + '" name="txtEndTime_' + i +'"/>'
        + '</td>'
        + '<td class="formValue">'
        + '<select class="form-control input-select sltSort" id="sltSort">(%)</select>'
        + '</td>'
        + '<td class="formValue">'
        + '<input type="text" class="form-control txtEstimate isFloatGteZero" id="txtEstimate" />'
        + '</td>'
        + '<td class="formValue">'
        + '<div class="btn-group" role="group">'
        + '<button type="button" class="btn btn-default plus"><span class="glyphicon glyphicon-plus" aria-hidden="true"></span></button>'
        + '<button type="button" class="btn btn-default minus"><span class="glyphicon glyphicon-minus" aria-hidden="true"></span></button>'
        + '</div>'
        + '</td>'
        + '</tr >';
    return template;
}

// 刷新序号
function settleNumber() {
    var chils = $("#batchTbody").children("tr");
    $.each(chils, function (i) {
        $($(chils[i]).children().get(0)).text(i + 1);
    })
}

// 重置多选
function refreshSelect() {
    $('.teams').selectpicker({
        'selectedText': 'cat',
        'noneSelectedText': '没有选中任何项',
        'deselectAllText': '全不选',
        'selectAllText': '全选',
    });
}

// 更新团员
function batchGetTeams(projectId, taskId, that) {
    $(that).empty();
    $.get("/Task/GetTeams", { projectId, taskId }, function (data) {
        if (data.total > 0) {
            $(that).bindSltSpe({
                id: "Person", name: "Person", data: data.rows
            });
        }
        // 重新渲染
        $(that).selectpicker('refresh');
    }, "json")
}

// 添加任务行
function addTask() {
    var tem = batchTaskTemplate($("#batchTbody").children().length + 1);
    tem = tem.replace('(#)', strProject).replace('($)', strTeams).replace('(%)', sltSort);
    $("#batchTbody").append(tem);
    refreshSelect();
}

// 获取项目
function qryProducts() {
    $.each(top.clients.prioritys, function (i, val) {
        sltSort += '<option value="' + val.id + '" >' + val.name + '</option>';
    });
    // 查询项目
    $.get("/Task/QryProducts", function (data) {
        if (data.total > 0) {
            $.each(data.products, function (i, val) {
                strProject += '<option value="' + val.ID + '" >' + val.Name + '</option>';
            });
            $.each(data.teams, function (i, val) {
                strTeams += '<option value="' + val.Person + '" >' + val.Person + '</option>';
            });
            for (var i = 0; i < 5; i++) {
                addTask()
            }
            refreshSelect();
        }
    }, "json")
}

// 初始化
function initControl() {
    qryProducts();
    $("#batchTbody").on("click", ".plus", function () {
        settleNumber();
        addTask();
    });
    $("#batchTbody").on("click", ".minus", function () {
        if ($("#batchTbody").children().length != 1) {
            $(this).closest("tr").remove();
            settleNumber();
        }
    });

    $("#batchTbody").on("change", "select.sltProject", function () {
        var id = $(this).val();
        batchGetTeams(id, "", $(this).closest("tr").find("select.teams")[0]);
    });
}

// 获取表单值
function getFormVal() {
    
    var TaskEntity = function (name, projectId, endTime, describe, sort, estimate, listTeam) {
        this.Name = name;
        this.ProjectId = projectId;
        this.EndTime = endTime;
        this.Describe = describe;
        this.Sort = sort;
        this.Estimate = estimate;
        this.listTeam = listTeam;
    }
    var TeamEntity = function (person) {
        this.Person = person;
    }
    var chils = $("#batchTbody").children("tr");
    var listTask = [];
    $.each(chils, function (i) {
        var that = chils[i];
        var name = $($(that).find(".txtName")).val();
        var projectId = $($(that).find(".sltProject")).val();
        var endTime = $($(that).find(".txtEndTime")).val();
        var describe = $($(that).find(".texDescribe")).val();
        var sort = $($(that).find(".sltSort")).val();
        var estimate = $($(that).find(".txtEstimate")).val();
        var teams = $($(that).find("select.teams")[0]).selectpicker('val');
        var listTeam = [];
        if (teams != null) {
            $.each(teams, function (i, val) {
                listTeam.push(new TeamEntity(val));
            });
        }
        listTask.push(new TaskEntity(name, projectId, endTime, describe, sort, estimate, listTeam));
    })
    return listTask;
}

function submitForm() {
    if (!$("#batchAdd").formValid()) {
        return false;
    }
    var tasks = getFormVal();
    $.submitForm({
        url: "/Task/BatchAddTask",
        param: { tasks },
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}