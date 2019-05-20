using System;
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
    public class ClientsDataController : BaseController
    {
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetClientsDataJson()
        {
            var data = new ClientData();
            var groups = new List<group>();
            var departments = new List<LibraryEntity>();
            var menuPermissions = new List<MenuPermission>();
            DALCore.GetInstance().SystemCore.SystemDataInit<group, LibraryEntity, MenuPermission>(RoleId, ref groups, ref departments, ref menuPermissions);
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
            dictionaryItem = new Dictionary<string, object>();
            foreach (var itm in departments)
            {
                itm.name = Resource.ResourceManager.GetString("dp_" + itm.code); 
                dictionaryItem.Add(itm.id.ToString(), itm.name);
            }
            data.departments = dictionaryItem;
            data.menuPermissions = menuPermissions;
            // 保存角色权限
            Dictionary<string, MenuPermission> ListToDictionary = menuPermissions.ToDictionary(key => key.code, value => value);
            Session["MemuList"] = ListToDictionary;
            data.menus = (List<MenuEntity>)DALCore.GetInstance().SystemCore.MenuGetList<MenuEntity>(null);
            return Content(data.ToJson());
        }
    }
}
