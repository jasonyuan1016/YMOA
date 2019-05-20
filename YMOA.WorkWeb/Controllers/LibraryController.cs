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
        [PermissionFilter("library")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetGridJson(int tag=1)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            if (tag > 0)
            {
                paras["tag"] = tag;
            }
            var roles = DALUtility.SystemCore.LibraryGetList<LibraryEntity>(paras);
            var data = new { rows = roles };
            return Content(data.ToJson());
        }

        public ActionResult Edit(int tag,int ID = 0)
        {
            LibraryEntity entity = new LibraryEntity();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["tag"] = tag;
            List<LibraryEntity> list = (List<LibraryEntity>)DALUtility.SystemCore.LibraryGetList<LibraryEntity>(paras);
            if (ID > 0)
            {
                foreach(LibraryEntity lib in list)
                {
                    if (lib.id == ID)
                    {
                        entity = lib;
                        break;
                    }
                }
            }
            entity.tag = tag;
            ViewData["listLibrary"] = list;
            return View(entity);
        }

        [PermissionFilter("library", "Add", Operationype.Add)]
        public ActionResult Add(LibraryEntity entity)
        {
            return Save(entity);
        }

        [PermissionFilter("library", "Update", Operationype.Update)]
        public ActionResult Update(LibraryEntity entity)
        {
            return Save(entity);
        }

        private ActionResult Save(LibraryEntity entity)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            int result = DALUtility.SystemCore.LibrarySave(entity);
            if (result != 0)
            {
                return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_codeexist"));
            }
            return OperationReturn(true);
        }

        /// <summary>
        ///  删除公共数据类型
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Delete(int ID)
        {
            return OperationReturn(DALUtility.SystemCore.DeleteLibrary(ID.ToString()));
        }

    }
}