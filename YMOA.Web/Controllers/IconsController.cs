using YMOA.DALFactory;
using YMOA.Comm;
using YMOA.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YMOA.Web.Controllers
{
    [App_Start.JudgmentLogin]
    public class IconsController : BaseController
    {
        //
        // GET: /Icons/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllIconsInfo()
        {
            string strWhere = "1=1";
            string sort = Request["sort"] == null ? "id" : Request["sort"];
            string order = Request["order"] == null ? "asc" : Request["order"];
            if (!string.IsNullOrEmpty(Request["IconName"]) && !SqlInjection.GetString(Request["IconName"]))
            {
                strWhere += " and IconName like '%" + Request["IconName"] + "%'";
            }

            //首先获取前台传递过来的参数
            int pageindex = Request["page"] == null ? 1 : Convert.ToInt32(Request["page"]);
            int pagesize = Request["rows"] == null ? 10 : Convert.ToInt32(Request["rows"]);
            int totalCount = 0;   //输出参数
            string strJson = JsonHelper.ToJson(SqlPagerHelper.GetPager("tbIcons", "Id,IconName,IconCssInfo,CreateTime,CreateBy,UpdateTime,UpdateBy", sort + " " + order, pagesize, pageindex, strWhere, out totalCount));
            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + strJson + "}");
        }

        /// <summary>
        /// 新增页面展示
        /// </summary>
        /// <returns></returns>
        public ActionResult IconsAdd()
        {
            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult AddIcons()
        {
            try
            {
                UserEntity uInfo = ViewData["Account"] as UserEntity;
                IconsEntity typeAdd = new IconsEntity();
                typeAdd.IconName = Request["IconName"];
                typeAdd.IconCssInfo = Request["IconCssInfo"];
                typeAdd.CreateBy = uInfo.AccountName;
                typeAdd.CreateTime = DateTime.Now;
                typeAdd.UpdateBy = uInfo.AccountName;
                typeAdd.UpdateTime = DateTime.Now;
                bool exists = DALUtility.Icons.ExistsIconName(typeAdd.IconName);
                if (exists)
                {
                    return Content("{\"msg\":\"添加失败,图标名称已存在！\",\"success\":false}");
                }
                else
                {
                    int typeId = DALUtility.Icons.Add(typeAdd);
                    if (typeId > 0)
                    {
                        return Content("{\"msg\":\"添加成功！\",\"success\":true}");
                    }
                    else
                    {
                        return Content("{\"msg\":\"添加失败！\",\"success\":false}");
                    }
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
        public ActionResult IconsEdit()
        {
            return View();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult EditIcons()
        {
            try
            {
                UserEntity uInfo = ViewData["Account"] as UserEntity;

                int id = Convert.ToInt32(Request["id"]);
                string originalName = Request["originalName"];

                IconsEntity typeEdit = DALUtility.Icons.GetModel(id);
                typeEdit.IconName = Request["IconName"];
                typeEdit.IconCssInfo = Request["IconCssInfo"];
                typeEdit.UpdateBy = uInfo.AccountName;
                typeEdit.UpdateTime = DateTime.Now;
                bool exists = DALUtility.Icons.ExistsIconName(typeEdit.IconName);
                if (typeEdit.IconName != originalName && exists)
                {
                    return Content("{\"msg\":\"修改失败,图标名称已存在！\",\"success\":false}");
                }
                else
                {
                    int result = DALUtility.Icons.Update(typeEdit);
                    if (result > 0)
                    {
                        return Content("{\"msg\":\"修改成功！\",\"success\":true}");
                    }
                    else
                    {
                        return Content("{\"msg\":\"修改失败！\",\"success\":false}");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"修改失败," + ex.Message + "\",\"success\":false}");
            }
        }

        public ActionResult DelIconsByIDs()
        {
            try
            {
                string Ids = Request["IDs"] == null ? "" : Request["IDs"];
                if (!string.IsNullOrEmpty(Ids))
                {
                    if (DALUtility.Icons.DeleteList(Ids))
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
        public ActionResult GetAllIconsTree()
        {
            DataTable dt = DALUtility.Icons.GetList("1=1");

            string sb = "[";//[{\"id\":\"0\",\"text\":\"图标\",\"iconCls\":\"icon-application_view_icons\",\"children\": [
            foreach (DataRow dr in dt.Rows)
            {
                sb += "{\"id\":\"" + dr["IconCssInfo"] + "\",\"text\":\"" + dr["IconCssInfo"] + "\",\"iconCls\":\"" + dr["IconCssInfo"] + "\"},";//dr["IconName"]
            }
            sb = sb.Trim(",".ToCharArray());
            sb += "]"; //"]}]";
            return Content(sb.ToString());
        }

        /// <summary>
        /// 获取所有图片 并显示
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllImgInfo()
        {
            FileInfo[] fs = (new DirectoryInfo(Server.MapPath("~/Content/themes/icon"))).GetFiles();
            string sb = "[";
            foreach (FileInfo file in fs)
            {
                string iconName = "icon-" + Path.GetFileNameWithoutExtension(file.Name);
                sb += "{\"id\":\"" + iconName + "\",\"text\":\"" + iconName + "\",\"iconCls\":\"" + iconName + "\"},";
            }
            sb = sb.Trim(",".ToCharArray());
            sb += "]";
            return Content(sb.ToString());
        }
    }
}
