using YMOA.DALFactory;
using YMOA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;
using YMOA.Comm;

namespace YMOA.Web.Controllers
{
    [App_Start.JudgmentLogin]
    public class DepartmentController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllDepartmentInfo()
        {
            DataTable dt = DALUtility.Department.GetAllDepartment(null);
            StringBuilder str = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                str.Append(Recursion(dt, 0));
                str = str.Remove(str.Length - 2, 2);
            }
            return Content(str.ToString());
        }

        string Recursion(DataTable dt, object parentId)
        {
            StringBuilder sbJson = new StringBuilder();
            DataRow[] rows = dt.Select("ParentId = " + parentId);
            if (rows.Length > 0)
            {
                sbJson.Append("[");
                for (int i = 0; i < rows.Length; i++)
                {
                    string childString = Recursion(dt, rows[i]["id"]);
                    if (!string.IsNullOrEmpty(childString))
                    {
                        //comboTree必须设置【id】和【text】，一个是id一个是显示值
                        sbJson.Append("{\"id\":\"" + rows[i]["Id"].ToString() + "\",\"ParentId\":\"" + rows[i]["ParentId"].ToString() + "\",\"Sort\":\"" + rows[i]["Sort"].ToString() + "\",\"UpdateBy\":\"" + rows[i]["UpdateBy"].ToString() + "\",\"UpdateTime\":\"" + rows[i]["UpdateTime"].ToString() + "\",\"text\":\"" + rows[i]["DepartmentName"].ToString() + "\",\"children\":");
                        sbJson.Append(childString);
                    }
                    else
                        sbJson.Append("{\"id\":\"" + rows[i]["Id"].ToString() + "\",\"ParentId\":\"" + rows[i]["ParentId"].ToString() + "\",\"Sort\":\"" + rows[i]["Sort"].ToString() + "\",\"UpdateBy\":\"" + rows[i]["UpdateBy"].ToString() + "\",\"UpdateTime\":\"" + rows[i]["UpdateTime"].ToString() + "\",\"text\":\"" + rows[i]["DepartmentName"].ToString() + "\"},");
                }
                sbJson.Remove(sbJson.Length - 1, 1);
                sbJson.Append("]},");
            }
            return sbJson.ToString();
        }

        public ActionResult GetDepartmentUser()
        {
            string userDepartmentIds = Request["departmentId"];
            string sortDepartmentUser = Request["sort"];  //排序列
            string orderDepartmentUser = Request["order"];  //排序方式 asc或者desc
            int pageindexDepartmentUser = int.Parse(Request["page"]);
            int pagesizeDepartmentUser = int.Parse(Request["rows"]);
            var iDal = DALUtility.Department;
            DataTable dt = iDal.GetPagerDepartmentUser(userDepartmentIds, sortDepartmentUser + " " + orderDepartmentUser, pagesizeDepartmentUser, pageindexDepartmentUser);
            int totalCount = iDal.GetDepartmentUserCount(userDepartmentIds);
            string strJsonDepartmentUser = "{\"total\": " + totalCount.ToString() + ",\"rows\":" + JsonHelper.ToJson(dt) + "}";
            return Content(strJsonDepartmentUser);
        }

        /// <summary>
        /// 新增页面展示
        /// </summary>
        /// <returns></returns>
        public ActionResult DepartmentAdd()
        {
            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult AddDepartment()
        {
            try
            {
                UserEntity uInfo = ViewData["Account"] as UserEntity;       
                DepartmentEntity departmentAdd = new DepartmentEntity();
                departmentAdd.DepartmentName = Request["DepartmentName"];
                departmentAdd.Sort = Convert.ToInt32(Request["Sort"]);
                if (Request["ParentId"] != null && Request.Params["ParentId"] != "")
                {
                    departmentAdd.ParentId = Convert.ToInt32(Request["ParentId"]);
                }
                else
                {
                    departmentAdd.ParentId = 0;   //根节点
                }
                departmentAdd.CreateBy = uInfo.AccountName;
                departmentAdd.CreateTime = DateTime.Now;
                departmentAdd.UpdateBy = uInfo.AccountName;
                departmentAdd.UpdateTime = DateTime.Now;
                int departmentId = DALUtility.Department.AddDepartment(departmentAdd);
                if (departmentId > 0)
                {
                    return Content("{\"msg\":\"添加成功！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"添加失败！\",\"success\":false}");
                }
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"添加失败," + ex.Message + "\",\"success\":false}");
            }
        }

        /// <summary>
        /// 编辑页面展示
        /// </summary>
        /// <returns></returns>
        public ActionResult DepartmentEdit()
        {
            return View();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult EditDepartment()
        {
            try
            {
                int id = Convert.ToInt32(Request["id"]);
                UserEntity uInfo = ViewData["Account"] as UserEntity;      
                DepartmentEntity departmentEdit = new DepartmentEntity();
                departmentEdit.Id = id;
                departmentEdit.DepartmentName = Request["DepartmentName"];
                departmentEdit.Sort = Convert.ToInt32(Request["Sort"]);
                departmentEdit.UpdateBy = uInfo.AccountName;
                departmentEdit.UpdateTime = DateTime.Now;
                bool result = DALUtility.Department.EditDepartment(departmentEdit);
                if (result)
                {
                    return Content("{\"msg\":\"修改成功！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"修改失败！\",\"success\":false}");
                }
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"修改失败," + ex.Message + "\",\"success\":false}");
            }
        }

        public ActionResult DelDepartmentByIDs()
        {
            try
            {
                string Ids = Request["IDs"] == null ? "" : Request["IDs"];
                if (!string.IsNullOrEmpty(Ids))
                {
                    if (DALUtility.Department.DeleteDepartment(Ids))
                    {
                        return Content("{\"msg\":\"删除成功！\",\"success\":true}");
                    }
                    else
                    {
                        return Content("{\"msg\":\"删除失败！\",\"success\":false}");
                    }
                }
                else
                {
                    return Content("{\"msg\":\"删除失败！\",\"success\":false}");
                }
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"删除失败," + ex.Message + "\",\"success\":false}");
            }
        }

        public ActionResult GetAllDepartmentTree()
        {
            DataTable dt = DALUtility.Department.GetAllDepartment("1=1");
            StringBuilder str = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                str.Append(Recursion(dt, 0));
                str = str.Remove(str.Length - 2, 2);
            }
            return Content(str.ToString());
        }

    }
}
