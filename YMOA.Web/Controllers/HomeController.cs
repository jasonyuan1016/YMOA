using YMOA.DALFactory;
using YMOA.Comm;
using YMOA.Model;
using YMOA.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Text;

namespace YMOA.Web.Controllers
{
    [App_Start.JudgmentLogin]
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            UserEntity uInfo = ViewData["Account"] as UserEntity;
            if (uInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.RealName = uInfo.RealName;
            ViewBag.TimeView = DateTime.Now.ToLongDateString();
            ViewBag.DayDate = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
            return View();
        }

        public ActionResult IndexNew()
        {
            UserEntity uInfo = ViewData["Account"] as UserEntity;
            if (uInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.RealName = uInfo.RealName;
            ViewBag.TimeView = DateTime.Now.ToLongDateString();
            ViewBag.DayDate = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
            return View();
        }

        /// <summary>
        /// 查询出数据显示在菜单栏目中
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadMenuData()
        {
            UserEntity uInfo = ViewData["Account"] as UserEntity;
            DataTable dt = DALCore.GetMenuDAL().GetUserMenu(uInfo.ID);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            DataRow[] rows = dt.Select("menuparentid = 0");   //赋权限每个角色都必须有父节点的权限，否则一个都不输出了
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    string stateStr = "open";// "closed";
                    if (i == 0)
                    {
                        stateStr = "open";
                    }
                    sb.Append("{\"id\":\"" + rows[i]["menuid"].ToString() + "\",\"text\":\"" + rows[i]["menuname"].ToString() + "\",\"iconCls\":\"" + rows[i]["icon"].ToString() + "\",\"state\":\"" + stateStr + "\",\"children\":[");
                    sb.Append(GetChildMenuStr(dt, rows[i]["menuid"].ToString(), stateStr));
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
            }
            else
            {
                sb.Append("]");
            }
            string menuJson = sb.ToString();
            return Content(menuJson);
        }

        string GetChildMenuStr(DataTable dt, string menuid, string stateStr)
        {
            StringBuilder sb = new StringBuilder();
            DataRow[] r_list = dt.Select(string.Format("menuparentid={0}", menuid));
            if (r_list.Length > 0)
            {
                for (int j = 0; j < r_list.Length; j++)
                {
                    DataRow[] child_list = dt.Select(string.Format("menuparentid={0}", r_list[j]["menuid"].ToString()));
                    if (child_list.Length > 0)
                    {
                        sb.Append("{\"id\":\"" + r_list[j]["menuid"].ToString() + "\",\"text\":\"" + r_list[j]["menuname"].ToString() + "\",\"iconCls\":\"" + r_list[j]["icon"].ToString() + "\",\"state\":\"" + stateStr + "\",\"children\":[");
                        sb.Append(GetChildMenuStr(dt, r_list[j]["menuid"].ToString(), stateStr));
                    }
                    else
                    {
                        sb.Append("{\"id\":\"" + r_list[j]["menuid"].ToString() + "\",\"text\":\"" + r_list[j]["menuname"].ToString() + "\",\"iconCls\":\"" + r_list[j]["icon"].ToString() + "\",\"attributes\":{\"url\":\"" + r_list[j]["linkaddress"].ToString() + "\"}},");
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]},");
            }
            else  //根节点下没有子节点
            {
                sb.Append("]},");  //跟上面if条件之外的字符串拼上
            }
            return sb.ToString();
        }



        /// <summary>
        /// 判断是否修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckIsChangePwd()
        {
            UserEntity uInfo = ViewData["Account"] as UserEntity;
            return Content("{\"msg\":" + new JavaScriptSerializer().Serialize(uInfo) + ",\"success\":true}");
        }

        /// <summary>
        /// 获取导航菜单
        /// </summary>
        /// <param name="id">所属</param>
        /// <returns>树</returns>
        public JsonResult GetTreeByEasyui(string id)
        {
            try
            {
                UserEntity uInfo = ViewData["Account"] as UserEntity;
                if (uInfo != null)
                {
                    DataTable dt = DALCore.GetMenuDAL().GetUserMenuData(uInfo.ID, int.Parse(id));
                    List<SysModuleNavModel> list = new List<SysModuleNavModel>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        SysModuleNavModel model = new SysModuleNavModel();
                        model.id = dt.Rows[i]["menuid"].ToString();
                        model.text = dt.Rows[i]["menuname"].ToString();
                        model.attributes = dt.Rows[i]["linkaddress"].ToString();
                        model.iconCls = dt.Rows[i]["icon"].ToString();
                        if (DALCore.GetMenuDAL().GetAllMenu(" AND ParentId= " + model.id).Rows.Count > 0)
                        {
                            model.state = "closed";
                        }
                        else
                        {
                            model.state = "open";
                        }
                        list.Add(model);
                    }
                    return Json(list);
                }
                else
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
