var ID = $.request("ID");
$(function () {
    initControl();
    refreshSelect($("#sltVictors"));
    if (!!ID) {
        var projectStatus = top.clients.projectStatus;
        $("#sltDutyPerson").val($("#sltDutyPerson").attr("value"));
        $("#txtStartTime").val(new Date($("#txtStartTime").val()).Format("yyyy-MM-dd"));
        $("#txtEndTime").val(new Date($("#txtEndTime").val()).Format("yyyy-MM-dd"));
        var data = $("#victors").data("val");
        var arr = new Array();
        arr = data.split(',');
        $("#sltVictors").selectpicker('val', arr);
        $("#sltState").bindSltSpe({
            id: "id", name: "name", data: projectStatus
        });
    }
});

function initControl() {
    var topData = top.clients;
    var roles = topData.groups;
    var name = topData.users;
    var index;
    var $tvictors = $("#sltVictors");
    var $tTeam = $("#sltTeam");
    for (index in roles) {
        $tvictors.append($("<option></option>").val(index).html(roles[index]));
    };
    var $element = $("#sltDutyPerson");
    $.each(name, function (i) {
        $element.append($("<option></option>").val(name[i].AccountName).html(name[i].RealName));
    });
    index = 0;
    for (index in name) {
        $tTeam.append($("<option></option>").val(name[index].AccountName).html(name[index].RealName));
    };
    refreshSelect($tTeam);
    refreshSelect($tvictors);
}
// 重置多选
function refreshSelect($element) {
    $element.selectpicker({
        'selectedText': PageResx.selectedText,
        'noneSelectedText': PageResx.noneSelectedText,
        'deselectAllText': PageResx.deselectAllText,
        'selectAllText': PageResx.selectAllText,
    });
}
function submitForm() {
    if (!$('#ProjectEdit').formValid()) {
        return false;
    }
    var teams = $("#sltTeam").selectpicker('val');
    var team = [];
    $.each(teams, function (index, ele) {
        var _p = { "Person": ele };
        console.log(_p);
        team.push(_p);
    });
    var victors = $("#sltVictors").selectpicker('val');
    var victor = "";
    $.each(victors,function (i,val) {
        victor += val+",";
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