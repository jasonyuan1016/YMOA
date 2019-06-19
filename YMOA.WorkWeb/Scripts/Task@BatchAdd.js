var strProject = ""; // 项目字符串
var strTeams = ""; // 成员字符串
var sltSort = ""; // 优先级字符串
var template = ""; // 模板
$(function () {
    initControl();
});

// 批量任务模板
function batchTaskTemplate() {
    if (template == "") {
        template = $("#batchTbody").html();
        $("#batchTbody").html("");
    }
    var i = $("#batchTbody").children().length + 1;
    var temp = template.replace('tr hidden', "tr").replace(/\(i\)/g, i);
    return temp;
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
                id: "AccountName", name: "RealName", data: data.rows
            });
        }
        // 重新渲染
        $(that).selectpicker('refresh');
    }, "json")
}

// 添加任务行
function addTask() {
    var tem = batchTaskTemplate();
    tem = tem.replace('(#)', strProject).replace('($)', strTeams).replace('(%)', sltSort);
    $("#batchTbody").append(tem);
    refreshSelect();
}

// 获取项目
function qryProducts() {
    sltSort = $.generateSlt({
        data: top.clients.prioritys, val: "id", name: "name"
    })
    // 查询项目
    $.get("/Task/QryProducts", function (data) {
        if (data.total > 0) {
            strProject = $.generateSlt({
                data: data.products, val: "ID", name: "Name"
            })
            strTeams = $.generateSlt({
                data: data.users, val: "AccountName", name: "RealName"
            })
            for (var i = 0; i < 5; i++) {
                addTask()
            }
            refreshSelect();
        }
        else {
            $(".empty-prompt").prop("hidden", false);
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
        if (name != "" && name != undefined) {
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
        }
    })
    return listTask;
}

function submitForm() {
    if (!$("#batchAdd").formValid()) {
        return false;
    }
    var tasks = getFormVal();
    if (tasks.length == 0 || tasks == null) {
        $.modalClose();
        return false;
    }
    $.submitForm({
        url: "/Task/BatchAddTask",
        param: { tasks },
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}