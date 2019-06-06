var strProject = ""; // 项目字符串
var strTeams = ""; // 成员字符串
var sltSort = ""; // 优先级字符串
var ID = $.request("ID");
$(function () {
    initControl();
});

// 批量任务模板
function batchTaskTemplate(i, n) {
    var name = top.clients.users;
    var template = '<tr>'
        + '<td class="formValue" style="width: 75%">'
        + '<select class="form-control input-select sltProject" id="sltProject">';
    if (i > 0) {
        $.each(name, function (j) {
            template += '<option value="' + name[j] + '">' + name[j] + '</option>';
        });
    } else {
        template += '<option value="' + n.Person + '">' + n.Person + '</option>';
        $.each(name, function (j) {
            if ($.trim(n.Person) != $.trim(name[j])) {
                template += '<option value="' + name[j] + '">' + name[j] + '</option>';
            }
        });
    }
    template += '</select > '
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

// 初始化
function initControl() {
    $.ajax({
        url: "/Item/GetTeam",
        type: "POST",
        data: { Id: ID },
        dataType: "json",
        success: function (data) {
            $.each(data, function (i, n) {
                var team = batchTaskTemplate(0, n);
                $("#batchTbody").append(team);
            })
        }
    });
    $("#batchTbody").on("click", ".plus", function () {
        var tem = batchTaskTemplate($("#batchTbody").children().length + 1);
        $("#batchTbody").append(tem);
    });
    $("#batchTbody").on("click", ".minus", function () {
        if ($("#batchTbody").children().length != 1) {
            $(this).closest("tr").remove();
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
    var listPerson = [];
    $.each(trs, function (i) {
        var projectId = ID;
        var taskId = "0";
        var that = trs[i];
        var person = $(that).find("#sltProject option:selected").val();
        console.log(person);
        listPerson.push(new TeamEntity(projectId, taskId, person));
    });
    return listPerson;
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