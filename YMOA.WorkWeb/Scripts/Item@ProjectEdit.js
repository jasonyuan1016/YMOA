var ID = $.request("ID");
$(function () {
    initControl();
    if (!!ID) {
        var projectStatus = top.clients.projectStatus;
        var roles = top.clients.groups;
        var index;
        $("#sltDutyPerson").val($("#sltDutyPerson").attr("value"));
        $("#txtStartTime").val(new Date($("#txtStartTime").val()).Format("yyyy-MM-dd"));
        $("#txtEndTime").val(new Date($("#txtEndTime").val()).Format("yyyy-MM-dd"));
        var data = $("#victors").data("val");
        var arr = new Array();
        arr = data.split(',');
        $.each($("input[name='F_AllowVictor']"), function (i, val) {
            for (var j = 0; j < arr.length; j++) {
                if ($(val).val() == arr[j]) {
                    $(val).attr("checked", true);
                }
            }
        });
        console.log($("input[id='" + roles[1] + "']").val());
        $("#formTitle").html("状态");
        
        var html = "<select id='sltState'>";
        $.each(projectStatus, function (i,n) {
            html += "<option value='" + n.id + "'>" + n.name + "</option>";
        });
        html += "</select>";
        $("#Team").html(html);
    }
});

function initControl() {
    var topData = top.clients;
    var roles = topData.groups;
    var name = topData.users;
    var index;
    var $tvictors = $("#victors");
    var $tTeam = $("#Team");
    for (index in roles) {
        $tvictors.append("<input id='" + roles[index] + "' name='F_AllowVictor' type='checkbox' value='" + index + "'/>" + roles[index]);
    };
    var $element = $("#sltDutyPerson");
    $.each(name, function (i) {
        $element.append($("<option></option>").val(name[i]).html(name[i]));
    });
    index = 0;
    for (index in name) {
        $tTeam.append("<input id='" + name[index] + "' name='F_AllowAdd' type='checkbox' value='" + name[index] + "'/>" + name[index]);
    };
}
function submitForm() {
    if (!$('#ProjectEdit').formValid()) {
        return false;
    }
    var team = [];
    $("input[name='F_AllowAdd']:checked").each(function (index,ele) {
        var _p = { "Person": $(ele).val() };
        team.push(_p);
    });
    var victor = "";
    $("input[name='F_AllowVictor']:checked").each(function () {
        victor += $(this).val()+",";
    });
    var projectEntity = $("#ProjectEdit").dataSerialize();
    projectEntity.Teams = team;
    projectEntity.Victors = victor;
    $.submitForm({
        url: "/Item/" + (!!ID == true?"Edit":"Add"),
        param: projectEntity,
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}