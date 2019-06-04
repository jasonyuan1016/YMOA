var clients = [];
$(function () {
    clients = $.clientsInit();
})
$.clientsInit = function () {
    var dataJson = {
        groups: [],
        departments: [],
        menus:[],
        menuPermissions: [],
        prioritys: [], // 优先级
        projectStatus: [], // 项目状态
        taskStatus: [] // 任务状态
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
                dataJson.menus = data.menus;
                dataJson.prioritys = data.prioritys;
                dataJson.projectStatus = data.projectStatus;
                dataJson.taskStatus = data.taskStatus;
                dataJson.users = data.users;
            }
        });
    }
    init();
    return dataJson;
}