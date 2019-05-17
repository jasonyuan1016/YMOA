var keyValue = $.request("ID");
$(function () {
    initControl();
    if (!!keyValue) {
        $("#sltstate").val($("#sltstate").attr("value"));
    }
})
function initControl() {
    $('#wizard').wizard().on('change', function (e, data) {
        var $finish = $("#btn_finish");
        var $next = $("#btn_next");
        if (data.direction == "next") {
            switch (data.step) {
                case 1:
                    if (!$('#GroupEdit').formValid()) {
                        return false;
                    }
                    $finish.show();
                    $next.hide();
                    break;
                default:
                    break;
            }
        } else {
            $finish.hide();
            $next.show();
        }
    });
}
function submitForm() {
    var _roleEntity = $("#tabBasicInfo").dataSerialize();
    var _allowOperations = new Array();
    $("#tbPermission :input[id^='cbx_']:checkbox:checked").each(function () {
        var currentMemu = new Object();
        currentMemu.id = $(this).val();
        currentMemu.add = $("#cbxAdd_" + currentMemu.id).prop("checked");
        currentMemu.update = $("#cbxUpdate_" + currentMemu.id).prop("checked");
        currentMemu.delete = $("#cbxDelete_" + currentMemu.id).prop("checked");
        currentMemu.other = $("#cbxOther_" + currentMemu.id).prop("checked");
        _allowOperations.push(currentMemu);
    }); 

    var roleMenuEntity = {
        roleEntity: _roleEntity,
        allowOperations: _allowOperations
    }
    _roleEntity.id = $("#hidid").val();
    var doUrl = _roleEntity.id == 0 ? "Add" : "Update";
    $.submitForm({
        url: doUrl,
        param: roleMenuEntity,
        success: function (data) {
            $.currentWindow().$("#gridList").trigger("reloadGrid");
        }
    })
}

function checkChange(cbxPrefix, pid) {
    $("tr[name='trSubMenuFrom_" + pid + "'] :input[id^='" + cbxPrefix + "']").prop("checked", $("#" + cbxPrefix + pid).prop("checked"));
}