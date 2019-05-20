using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.Model;
using YMOA.WorkWeb.Domain;

namespace YMOA.WorkWeb.Controllers
{
    /// <summary>
    ///  功能说明: 选单控制器
    ///  创建人: zxy
    ///  创建时间: 2019年5月17日
    /// </summary>
    public class MemuController : BaseController
    {
        // GET: Menu
        [PermissionFilter]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///  获取所有选单
        /// </summary>
        /// <returns></returns>
        [PermissionFilter("memu", "index")]
        public ActionResult GetGridJson()
        {
            var data = DALUtility.SystemCore.MenuGetList<MenuEntity>(null);
            var treeList = new List<TreeGridModel>();
            foreach (MenuEntity item in data)
            {
                TreeGridModel treeModel = new TreeGridModel();
                bool hasChildren = data.Count(t => t.parentid == item.id) == 0 ? false : true;
                treeModel.id = item.id.ToString();
                treeModel.isLeaf = hasChildren;
                treeModel.parentId = item.parentid.ToString();
                treeModel.expanded = hasChildren;
                treeModel.entityJson = item.ToJson();
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeGridJson());
        }

        /// <summary>
        ///  添加/修改弹框
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [PermissionFilter("memu", "index")]
        public ActionResult Edit(int ID = 0)
        {
            MenuEntity entity = new MenuEntity();
            if (ID>0)
            {
                Dictionary<string, object> paras = new Dictionary<string, object>();
                paras["id"] = ID;
                List<MenuEntity> menuEntities = (List<MenuEntity>)DALUtility.SystemCore.MenuGetList<MenuEntity>(paras);
                entity = menuEntities[0];
            }
            return View(entity);
        }

        /// <summary>
        ///  修改菜单
        /// </summary>
        /// <param name="menuEntity"></param>
        /// <returns></returns>
        [PermissionFilter("memu", "index", Operationype.Update)]
        public ActionResult Update(MenuEntity menuEntity)
        {
            return SubmitForm(menuEntity);
        }


        /// <summary>
        ///  添加菜单
        /// </summary>
        /// <param name="menuEntity"></param>
        /// <returns></returns>
        [PermissionFilter("memu", "index", Operationype.Add)]
        public ActionResult Add(MenuEntity menuEntity)
        {
            return SubmitForm(menuEntity);
        }
        
        private ActionResult SubmitForm(MenuEntity menuEntity)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = menuEntity.id;
            paras["name"] = menuEntity.name;
            paras["code"] = menuEntity.code;
            paras["controller"] = menuEntity.controller == null ? "" : menuEntity.controller;
            paras["action"] = menuEntity.action == null ? "" : menuEntity.action;
            paras["parentid"] = menuEntity.parentid;
            paras["state"] = menuEntity.state;
            paras["sortvalue"] = menuEntity.sortvalue;
            return OperationReturn(DALUtility.SystemCore.MemuSave(paras) > 0);
        }

        /// <summary>
        ///  修改状态
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        [PermissionFilter("memu", "Update", Operationype.Update)]
        public ActionResult UpdateState(int ID, int state)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = ID;
            paras["state"] = state;
            return OperationReturn(DALUtility.SystemCore.MemuSave(paras) > 0);
        }

        /// <summary>
        ///  删除菜单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [PermissionFilter("memu", "Delete", Operationype.Add)]
        public ActionResult Delete(int ID)
        {
            return OperationReturn(DALUtility.SystemCore.DeleteMemu(ID.ToString()));
        }
    }
}