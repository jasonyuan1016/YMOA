$(function () {
    initControl();
});
function initControl() {
    var departments = top.clients.departments;
    var users = top.clients.users;
    var departmentid = $("#txtDepartment").data("id");
    var userid = $("#txtApplicant").data("id");

    $.each(departments, function (i) {
        if (departments[i].id == departmentid) {
            $("#txtDepartment").val(departments[i].name);
        }
    });
    $.each(users, function (i) {
        if (users[i].AccountName == userid) {
            $("#txtApplicant").val(users[i].RealName);
        }
    })
    var now = new Date();
    var endday = ("0" + now.getDate()).slice(-2);
    var month = ("0" + (now.getMonth() + 1)).slice(-2);
    var end = now.getFullYear() + "-" + (month) + "-" + (endday);
    $("#txtApplicantTime").val(end);
}
function submitForm() {
    if (!$('#Reimbursement').formValid()) {
        return false;
    }
    var reimbursement = $("#Reimbursement").dataSerialize();
    reimbursement.Department = $("#txtDepartment").data("id");
    reimbursement.State = reimbursement.Level == 1 ? 1 : 2;
    reimbursement.Applicant = $("#txtApplicant").data("id");
    $.ajax({
        url: "/Reimbursement/Add",
        data: reimbursement,
        type: "POST",
        dataType: "json",
        success: function (data) {
            $.modalMsg(data.msg, data.success);
            $.modalClose();
        }
    })
}
function btnClose() {
    $.modalClose();
}