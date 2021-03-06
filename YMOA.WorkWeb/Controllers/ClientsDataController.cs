﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.DALFactory;
using YMOA.Model;
using YMOA.WorkWeb.Models;
using YMOA.WorkWeb.Resources;

namespace YMOA.WorkWeb.Controllers
{
    /// <summary>
    /// 返回公共数据
    /// </summary>
    public class ClientsDataController : BaseController
    {
        /// <summary>
        ///  返回公共数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetClientsDataJson()
        {
            var data = new ClientData();
            var groups = new List<group>();
            var departments = new List<LibraryEntity>();
            var projectStatus = new List<LibraryEntity>();
            var taskStatus = new List<LibraryEntity>();
            var prioritys = new List<LibraryEntity>();
            var expenses = new List<LibraryEntity>();
            var duty = new List<LibraryEntity>();
            var menuPermissions = new List<MenuPermission>();
            var userName = new List<UserEntity>();
            DALCore.GetInstance().SystemCore.SystemDataInit<group, LibraryEntity, MenuPermission>(RoleId, ref groups, ref departments, ref projectStatus, ref taskStatus, ref prioritys, ref expenses, ref duty, ref menuPermissions);
            var users = DALUtility.UserCore.QryAllUser<UserEntity>();
            foreach(var user in users)
            {
                userName.Add(user);
            }
            foreach (var m in menuPermissions)
            {
                m.name = Resource.ResourceManager.GetString("menu_" + m.code);
            } 
            Dictionary<string, object> dictionaryItem = new Dictionary<string, object>();
            foreach (var itm in groups)
            {
                itm.name = Resource.ResourceManager.GetString("role_" + itm.code);
                dictionaryItem.Add(itm.id.ToString(), itm.name);
            }
            data.groups = dictionaryItem;
            data.departments = departments;
            data.menuPermissions = menuPermissions;
            data.projectStatus = projectStatus;
            data.taskStatus = taskStatus;
            data.prioritys = prioritys;
            data.expenses = expenses;
            data.duty = duty;
            data.users = userName;
            // 保存角色权限
            Session["MemuList"] = menuPermissions;
            Dictionary<string, object> menuItem = new Dictionary<string, object>();
            menuItem["noState"] = 0;
            data.menus = DALCore.GetInstance().SystemCore.MenuGetList<MenuEntity>(menuItem).ToList();
            menuItem = new Dictionary<string, object>();
            menuItem["userName"] = UserId;
            return Content(data.ToJson());
        }
    }
}
