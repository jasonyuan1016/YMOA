var ID = $.request("ID");
$(function () {
    initControl();
    if (!!ID) {
        var pid = $("#sltProjectId").data("val");
        getTeams(pid,"");
        $("#sltSort").val($("#sltSort").data("val"));
        $("#sltProjectId").val(pid);
        $("#txtEndTime").val(new Date($("#txtEndTime").data("val")).Format("yyyy-MM-dd"));
        getTeams("", ID, true);
    }
    else {
        var id = $("#sltProjectId").val();
        getTeams(id, "");
    }
});

function accTemplate(i) {
    var template = '<div class="formValue"><div class="input-group">'
        + '<input type = "file" class="form-control accessoryFile isFile isFileSize" id="accessoryFile_'+i+'" name="accessoryFile_'+i+'" placeholder = "附件" />'
        + '<div class="input-group-addon">标题</div>'
        + '<input type="text" class="form-control accessoryName" id="accessoryName" autocomplete="off" placeholder="标题名">'
        + '<span class="input-group-btn">'
        + '<button class="btn btn-default plus" type="button">'
        + '<span class="glyphicon glyphicon-plus" aria-hidden="true"></span>'
        + '</button>'
        + '</span>'
        + '<span class="input-group-btn">'
        + '<button class="btn btn-default minus" type="button">'
        + '<span class="glyphicon glyphicon-minus" aria-hidden="true"></span>'
        + '</button>'
        + '</span>'
        + '</div></div>';
    return template;
}

function initControl() {
    var topData = top.clients;
    $("#sltSort").bindSltSpe({
        id: "id", name: "name", data: topData.prioritys
    });
    $("select#sltProjectId").change(function () {
        var id = $("#sltProjectId").val();
        getTeams(id, "");
    });
    for (var i = 0; i < 2; i++) {
        $("#accessory").append(accTemplate($("#accessory").children().length + 1));
    }
    $("#accessory").on("click", ".plus", function () {
        $("#accessory").append(accTemplate($("#accessory").children().length+1));
    });
    $("#accessory").on("click", ".minus", function () {
        if ($("#accessory").children().length != 1) {
            $(this).closest("div.formValue").remove();
        }
    });
    $("#TaskEidt").on("click", ".remove", function () {
        var that = this;
        $.modalConfirm("你确定要删除文件吗?", function (r) {
            if (r) {
                var id = $(that).data("id");
                var url = $(that).data("name");
                $.post("/Task/DeleteFile", { id, url }, function (data) {
                    if (data.success) {
                        $(that).closest("div").remove();
                    }
                    $.modalMsg(data.msg, data.success);
                }, "json")
            }
        })
    });

    $("#TaskEidt").on("click", ".pencil", function () {
        var that = this;
        if ($(that).data("boo") == 0) {
            $(that).closest("div").find(".accessoryName").prop("readonly", false);
            $(that).closest("div").find(".glyphicon-pencil").addClass("glyphicon-folder-close");
            $(that).data("boo", 1);
        }
        else {
            var name = $(that).closest("div").find(".accessoryName").val();
            var id = $(that).data("id");
            $.post("/Task/UpdateFile", { id, name }, function (data) {
                if (data.success) {
                    $(that).data("boo", 0);
                    $(that).closest("div").find(".accessoryName").prop("readonly", true);
                    $(that).closest("div").find(".glyphicon-pencil").removeClass("glyphicon-folder-close");
                }
                $.modalMsg(data.msg, data.success);
            }, "json")
        }
    });
}
 
function getTeams(projectId, taskId, boo = false) {
    $.get("/Task/GetTeams", { projectId, taskId }, function (data) {
        if (boo) {
            var arr = [];
            $.each(data.rows, function (i, val) {
                arr.push(val.Person);
            })
            $("#sltTeam").selectpicker('val', arr);
            return false;
        }
        $("#sltTeam option").remove();
        if (data.total > 0) {
            $("#sltTeam").bindSltSpe({
                id: "Person", name: "Person", data: data.rows
            });
        }
        // 刷新
        $("#sltTeam").selectpicker('refresh');
    }, "json")
}

function getFromVal() {
    var TeamEntity = function (name) {
        this.Person = name;
    }
    var AccessoryEntity = function (name) {
        this.Name = name;
    }
    var task = $("#TaskEidt").dataSerialize();
    if (!!ID) {
        task.ID = ID;
    }
    if ($("#sltTeam").selectpicker('val') != null) {
        task.listTeam = [];
        var teams = $("#sltTeam").selectpicker('val');
        $.each(teams, function (i, val) {
            task.listTeam.push(new TeamEntity(val));
        })
    }
    task.listAccessory = [];
    var chils = $("#accessory").children();
    var name = "";
    for (var i = 0; i < chils.length; i++) {
        var boo = $($(chils[i]).find(".accessoryFile")).prop('files').length > 0 ? true : false;
        if (boo) {
            name = $($(chils[i]).find(".accessoryName")).val();
            task.listAccessory.push(new AccessoryEntity(name));
        }
    }
    console.log(task);
    return task;
}

function submitForm() {
    if (!$('#TaskEidt').formValid()) {
        return false;
    }
    var formData = new FormData();
    // 获取附件
    var file = $("#accessory .accessoryFile");
    for (var i = 0; i < file.length; i++) {
        var f = $(file[i]).prop('files');
        if (f.length > 0) {
            formData.append("file", f[0]);
        }
    }
    var task = getFromVal();
    formData.append("task", JSON.stringify(task));//追加参数
    $.submitFormFile({
        url: "/Task/" + (!!ID == true ? "Update" : "Add"),
        success: function () {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    }, formData);
}