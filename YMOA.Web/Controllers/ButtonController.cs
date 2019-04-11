using YMOA.DALFactory;
using YMOA.Comm;
using YMOA.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YMOA.Web.Controllers
{
    [App_Start.JudgmentLogin]
    public class ButtonController : BaseController
    {
        //
        // GET: /Button/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取页面操作按钮权限
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserAuthorizeButton()
        {
            UserEntity uInfo = ViewData["Account"] as UserEntity;
            string KeyName = Request["KeyName"];//页面名称关键字
            string KeyCode = Request["KeyCode"];//菜单标识码
            DataTable dt = DALUtility.Button.GetButtonByMenuCodeAndUserId(KeyCode, uInfo.ID);
            return Content(CommFunc.GetToolBar(dt, KeyName));
        }


        public ActionResult GetAllButtonInfo()
        {
            string strWhere = "1=1";
            string sort = Request["sort"] == null ? "id" : Request["sort"];
            string order = Request["order"] == null ? "asc" : Request["order"];
            if (!string.IsNullOrEmpty(Request["FButtonName"]) && ! SqlInjection.GetString(Request["FButtonName"]))
            {
                strWhere += " and Name like '%" + Request["FButtonName"] + "%'";
            }
            //首先获取前台传递过来的参数
            int pageindex = Request["page"] == null ? 1 : Convert.ToInt32(Request["page"]);
            int pagesize = Request["rows"] == null ? 10 : Convert.ToInt32(Request["rows"]);
            int totalCount = 0;   //输出参数
            string strJson = JsonHelper.ToJson(SqlPagerHelper.GetPager("tbButton", "Id,Name,Code,Icon,Sort,Description,CreateTime,CreateBy,UpdateTime,UpdateBy", sort + " " + order, pagesize, pageindex, strWhere, out totalCount));

            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + strJson + "}");
        }

        /// <summary>
        /// 新增页面展示
        /// </summary>
        /// <returns></returns>
        public ActionResult ButtonAdd()
        {
            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult AddButton()
        {
            try
            {
                UserEntity uInfo = ViewData["Account"] as UserEntity;       
                ButtonEntity buttonAdd = new ButtonEntity();
                buttonAdd.Name = Request["Name"];
                buttonAdd.Code = Request["Code"];
                buttonAdd.Icon = Request["Icon"];
                buttonAdd.Sort = int.Parse(Request["Sort"]);
                buttonAdd.Description = Request["Description"];
                buttonAdd.CreateBy = uInfo.AccountName;
                buttonAdd.CreateTime = DateTime.Now;
                buttonAdd.UpdateBy = uInfo.AccountName;
                buttonAdd.UpdateTime = DateTime.Now;
                int buttonId = DALUtility.Button.AddButton(buttonAdd);
                if (buttonId > 0)
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
        public ActionResult ButtonEdit()
        {
            return View();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult EditButton()
        {
            try
            {
                int id = Convert.ToInt32(Request["id"]);
                string originalName = Request["originalName"];
                UserEntity uInfo = ViewData["Account"] as UserEntity; 
                ButtonEntity buttonEdit = new ButtonEntity();
                buttonEdit.Id = id;
                buttonEdit.Name = Request["Name"];
                buttonEdit.Code = Request["Code"];
                buttonEdit.Icon = Request["Icon"];
                buttonEdit.Sort = int.Parse(Request["Sort"]);
                buttonEdit.Description = Request["Description"];
                buttonEdit.UpdateBy = uInfo.AccountName;
                buttonEdit.UpdateTime = DateTime.Now;
                if (buttonEdit.Name != originalName && DALUtility.Button.GetButtonByButtonName(buttonEdit.Name) != null)
                {
                    throw new Exception("已经存在此按钮！");
                }
                bool result = DALUtility.Button.EditButton(buttonEdit);
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

        public ActionResult DelButtonByIDs()
        {
            try
            {
                string Ids = Request["IDs"] == null ? "" : Request["IDs"];
                if (!string.IsNullOrEmpty(Ids))
                {
                    if (DALUtility.Button.DeleteButton(Ids))
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

        /// <summary>
        /// 获取按钮树
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllButtonTree()
        {
            DataTable dt = DALUtility.Button.GetAllButton("1=1");

            string sb = "[{\"id\":\"0\",\"text\":\"全选\",\"children\": [";
            foreach (DataRow dr in dt.Rows)
            {
                sb += "{\"id\":\"" + dr["id"] + "\",\"text\":\"" + dr["name"] + "\"},";
            }
            sb = sb.Trim(",".ToCharArray());
            sb += "]}]";

            return Content(sb.ToString());
        }

    }
}
