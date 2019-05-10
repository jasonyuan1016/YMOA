var clients = [];
$(function () {
    clients = $.clientsInit();
})
$.clientsInit = function () {
    var dataJson = {
        groups: [],
        departments: [],
        menuPermissions: []
    };
    var init = function () {
        $.ajax({
            url: "/ClientsData/GetClientsDataJson",
            type: "get",
            dataType: "json",
            async: false,
            success: function (data) {
                dataJson.groups = data.groups;
                dataJson.departments = data.departments;
                dataJson.menuPermissions = data.menuPermissions;
            }
        });
    }
    init();
    return dataJson;
}