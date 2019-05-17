﻿using System;
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
        [PermissionFilter("memu")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///  获取所有选单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGridJson()
        {
            var data = new
            {
                rows = DALUtility.SystemCore.MenuGetList<MenuEntity>(null)
            };
            return Content(data.ToJson());
        }
        
        /// <summary>
        ///  添加/修改弹框
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
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

        [PermissionFilter("memu", "Update", Operationype.Add)]
        /// <summary>
        ///  修改菜单
        /// </summary>
        /// <param name="menuEntity"></param>
        /// <returns></returns>
        public ActionResult Update(MenuEntity menuEntity)
        {
            return SubmitForm(menuEntity);
        }

        [PermissionFilter("memu", "Add", Operationype.Add)]
        /// <summary>
        ///  添加菜单
        /// </summary>
        /// <param name="menuEntity"></param>
        /// <returns></returns>
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

        [PermissionFilter("memu", "Update", Operationype.Add)]
        /// <summary>
        ///  修改状态
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public ActionResult UpdateState(int ID, int state)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = ID;
            paras["state"] = state;
            return OperationReturn(DALUtility.SystemCore.MemuSave(paras) > 0);
        }

        [PermissionFilter("memu", "Delete", Operationype.Add)]
        /// <summary>
        ///  删除菜单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Delete(int ID)
        {
            return OperationReturn(DALUtility.SystemCore.DeleteMemu(ID.ToString()));
        }
    }
}