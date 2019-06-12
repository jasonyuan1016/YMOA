using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.Model;
using YMOA.WorkWeb.Domain;
using YMOA.WorkWeb.Resources;

namespace YMOA.WorkWeb.Controllers
{
    public class GroupController : BaseController
    {
        [PermissionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionFilter("group", "index")]
        public ActionResult GetGridJson()
        {
            var roles = DALUtility.SystemCore.RoleGetList<RoleEntity>(null);
            var data = new { rows = roles };
            return Content(data.ToJson());
        }

        [PermissionFilter("group", "index")]
        public ActionResult Edit(int ID = 0)
        {
            RoleMenuEntity roleMenuEntity = new RoleMenuEntity();
            if (ID > 0)
            {
                Dictionary<string, object> paras = new Dictionary<string, object>();
                paras["id"] = ID;
                roleMenuEntity.roleEntity = DALUtility.SystemCore.RoleGetList<RoleEntity>(paras).FirstOrDefault();
                roleMenuEntity.allowOperations = DALUtility.SystemCore.RoleMenuGetListByRoleId<AllowOperation>(paras).ToList();
            }
            else
            {
                roleMenuEntity.roleEntity = new RoleEntity();
                roleMenuEntity.allowOperations = new List<AllowOperation>();
            }
            ViewData["MenuEntity"] = DALUtility.SystemCore.MenuGetList<MenuEntity>(null).ToList();
            return View(roleMenuEntity);
        }

        [PermissionFilter("group", "index", Operationype.Add)]
        public ActionResult Add(RoleMenuEntity roleMenuEntity)
        {
            return Save(roleMenuEntity);
        }

        [PermissionFilter("group", "index", Operationype.Update)]
        public ActionResult Update(RoleMenuEntity roleMenuEntity)
        {
            return Save(roleMenuEntity);
        }
        private ActionResult Save(RoleMenuEntity roleMenuEntity)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["id"] = roleMenuEntity.roleEntity.id;
            paras["name"] = roleMenuEntity.roleEntity.name;
            paras["code"] = roleMenuEntity.roleEntity.code;
            paras["state"] = roleMenuEntity.roleEntity.state;
            var dtCheckInfo = new DataTable();
            dtCheckInfo.Columns.Add("m_id", typeof(int));
            dtCheckInfo.Columns.Add("rm_add", typeof(int));
            dtCheckInfo.Columns.Add("rm_update", typeof(int));
            dtCheckInfo.Columns.Add("rm_delete", typeof(bool));
            dtCheckInfo.Columns.Add("rm_other", typeof(bool));
            if (roleMenuEntity.allowOperations != null && roleMenuEntity.allowOperations.Count > 0)
            {
                foreach (var item in roleMenuEntity.allowOperations)
                {
                    var row = dtCheckInfo.NewRow();
                    row[0] = item.id;
                    row[1] = item.add;
                    row[2] = item.update;
                    row[3] = item.delete;
                    row[4] = item.other;
                    dtCheckInfo.Rows.Add(row);
                }
            }
            paras["rolemenu"] = dtCheckInfo;
            return OperationReturn(DALUtility.SystemCore.RoleSave(paras) == 0);
        }

        [PermissionFilter("group", "index", Operationype.Delete)]
        public ActionResult Delete(int ID)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["id"] = ID;
            int result = DALUtility.SystemCore.RoleDelete(paras);
            if (result == 0)
            {
                return OperationReturn(true);
            }
            return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_role_" + result));
        }

    }
}