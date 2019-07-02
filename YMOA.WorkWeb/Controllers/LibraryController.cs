using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.Model;
using YMOA.WorkWeb.Domain;
using YMOA.WorkWeb.Resources;

namespace YMOA.WorkWeb.Controllers
{
    /// <summary>
    /// 数据基本类型控制器
    /// </summary>
    public class LibraryController : BaseController
    {
        /// <summary>
        /// 管理页面
        /// </summary>
        /// <returns></returns>
        [PermissionFilter]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///  获取列表
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [PermissionFilter("library", "index")]
        public ActionResult GetGridJson(int tag = 1)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["tag"] = tag;
            var roles = DALUtility.SystemCore.LibraryGetList<LibraryEntity>(paras);
            var data = new { rows = roles };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 添加修改页面
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [PermissionFilter("library", "index")]
        public ActionResult Edit(int tag,int ID = 0)
        {
            LibraryEntity entity = new LibraryEntity();
            if (ID > 0)
            {
                Dictionary<string, object> paras = new Dictionary<string, object>();
                paras["tag"] = tag;
                paras["id"] = ID;
                List<LibraryEntity> list = DALUtility.SystemCore.LibraryGetList<LibraryEntity>(paras).ToList();
                entity = list[0];
                // 获取负责人
                ViewData["DutyPerson"] = DALUtility.UserCore.GetCharge(ID);
            }
            if (tag == 1)
            {
                // 获取部门用户
                Dictionary<string, object> paras = new Dictionary<string, object>();
                if (ID != 0)
                {
                    paras["DepartmentId"] = ID;
                }
                ViewData["users"] = DALUtility.UserCore.QryRealName<UserEntity>(paras);
            }
            entity.tag = tag;
            return View(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="DutyPerson"></param>
        /// <returns></returns>
        [PermissionFilter("library", "index", Operationype.Add)]
        public ActionResult Add(LibraryEntity entity, string DutyPerson)
        {
            return Save(entity, DutyPerson);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="DutyPerson"></param>
        /// <returns></returns>
        [PermissionFilter("library", "index", Operationype.Update)]
        public ActionResult Update(LibraryEntity entity, string DutyPerson)
        {
            return Save(entity, DutyPerson);
        }

        /// <summary>
        /// 添加/修改底层
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="DutyPerson"></param>
        /// <returns></returns>
        private ActionResult Save(LibraryEntity entity,string DutyPerson)
        {
            int result = DALUtility.SystemCore.LibrarySave(entity);
            if (result != 0)
            {
                return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_codeexist"));
            }
            if (entity.tag == 1 && DutyPerson != null && DutyPerson != "")
            {
                // 保存负责人
                DALUtility.UserCore.SetCharge(entity.id,DutyPerson);
            }
            return OperationReturn(true);
        }

        /// <summary>
        ///  删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [PermissionFilter("library", "index", Operationype.Delete)]
        public ActionResult Delete(int ID, int tag)
        {
            return OperationReturn(DALUtility.SystemCore.DeleteLibrary(ID, tag));
        }

    }
}