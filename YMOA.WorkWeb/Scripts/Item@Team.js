var strProject = ""; // 项目字符串
var strTeams = ""; // 成员字符串
var sltSort = ""; // 优先级字符串
var ID = $.request("ID");
$(function () {
    initControl();
});

// 批量任务模板
function batchTaskTemplate() {
    var name = top.clients.users;
    index = 0;
    var template = '<tr>'
        + '<td class="formValue" style="width: 75%">';
    for (index in name) {
        template += "<input id='" + name[index] + "' name='F_AllowTeam' type='checkbox' value='" + name[index] + "'/>" + name[index];
    };
    template += '</td>'
        + '</tr >';
    return template;
}

// 初始化
function initControl() {
    $.ajax({
        url: "/Item/GetTeam",
        type: "POST",
        data: { Id: ID },
        dataType: "json",
        success: function (data) {
            $("#batchTbody").append(batchTaskTemplate());
            $.each($("input[name='F_AllowTeam']"), function (i, val) {
                for (var j = 0; j < data.length; j++) {
                    if ($.trim($(val).val()) == $.trim(data[j].Person)) {
                        $(val).attr("checked", true);
                    }
                }
            })
        }
    });
}
function getFormVal() {
    var TeamEntity = function (ProjectId, TaskId, Person) {
        this.ProjectId = ProjectId;
        this.TaskId = TaskId;
        this.Person = Person;
    }
    var trs = $("#batchTbody").children("tr");
    var inputs = $(trs).find("input[name='F_AllowTeam']:checked");
    var listTeam = [];
    $.each(inputs, function (i) {
        var projectId = ID;
        var taskId = "0";
        var that = inputs[i];
        var person = $(that).val();
        console.log(person);
        listTeam.push(new TeamEntity(projectId, taskId, person));
    });
    return listTeam;
}
function submitForm() {
    if (!$("#batchAdd").formValid()) {
        return false;
    }
    var teams = getFormVal();
    console.log(teams);
    $.submitForm({
        url: "/Item/AddTeam",
        param: { teams },
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}