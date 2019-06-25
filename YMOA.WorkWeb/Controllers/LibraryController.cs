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
    public class LibraryController : BaseController
    {
        [PermissionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionFilter("library", "index")]
        public ActionResult GetGridJson(int tag = 1)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["tag"] = tag;
            var roles = DALUtility.SystemCore.LibraryGetList<LibraryEntity>(paras);
            var data = new { rows = roles };
            return Content(data.ToJson());
        }

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

        [PermissionFilter("library", "index", Operationype.Add)]
        public ActionResult Add(LibraryEntity entity, string DutyPerson)
        {
            return Save(entity, DutyPerson);
        }

        [PermissionFilter("library", "index", Operationype.Update)]
        public ActionResult Update(LibraryEntity entity, string DutyPerson)
        {
            return Save(entity, DutyPerson);
        }

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
        ///  删除公共数据类型
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