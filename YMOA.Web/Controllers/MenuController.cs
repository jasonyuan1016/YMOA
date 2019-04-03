using YMOA.DALFactory;
using YMOA.Comm;
using YMOA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;

namespace YMOA.Web.Controllers
{
    [App_Start.JudgmentLogin]
    public class MenuController : Controller
    {
        //
        // GET: /Menu/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllMenuInfo()
        {
            string strWhere = "1=1";
            string sort = Request["sort"] == null ? "Id" : Request["sort"];
            string order = Request["order"] == null ? "asc" : Request["order"];
            if (!string.IsNullOrEmpty(Request["MenuName"]) && !SqlInjection.GetString(Request["MenuName"]))
            {
                strWhere += " and Name like '%" + Request["MenuName"] + "%'";
            }
            //首先获取前台传递过来的参数
            int pageindex = Request["page"] == null ? 1 : Convert.ToInt32(Request["page"]);
            int pagesize = Request["rows"] == null ? 10 : Convert.ToInt32(Request["rows"]);
            int totalCount;   //输出参数
            string strJson = "";    //输出结果
            if (order.IndexOf(',') != -1)   //如果有","就是多列排序（不能拿列判断，列名中间可能有","符号）
            {
                //多列排序：
                //sort：ParentId,Sort,AddDate
                //order：asc,desc,asc
                string sortMulti = "";  //拼接排序条件，例：ParentId desc,Sort asc
                string[] sortArray = sort.Split(',');   //列名中间有","符号，这里也要出错。正常不会有
                string[] orderArray = order.Split(',');
                for (int i = 0; i < sortArray.Length; i++)
                {
                    sortMulti += sortArray[i] + " " + orderArray[i] + ",";
                }
                strJson = JsonHelper.ToJson(SqlPagerHelper.GetPager("tbMenu", "Id,Name,ParentId,Code,LinkAddress,Icon,Sort,CreateTime,CreateBy,UpdateTime,UpdateBy", sortMulti.Trim(','), pagesize, pageindex, strWhere, out totalCount));
            }
            else
            {
                strJson = JsonHelper.ToJson(SqlPagerHelper.GetPager("tbMenu", "Id,Name,ParentId,Code,LinkAddress,Icon,Sort,CreateTime,CreateBy,UpdateTime,UpdateBy", sort + " " + order, pagesize, pageindex, strWhere, out totalCount));
            }
            var jsonResult = new { total = totalCount.ToString(), rows = strJson };
            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + strJson + "}");
        }

        /// <summary>
        /// 新增页面展示
        /// </summary>
        /// <returns></returns>
        public ActionResult MenuAdd()
        {
            return View();
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult AddMenu()
        {
            try
            {
                UserEntity uInfo = ViewData["Account"] as UserEntity;
                MenuEntity menuAdd = new MenuEntity();
                menuAdd.Name = Request["MenuName"];
                menuAdd.Code = Request["MenuCode"];
                menuAdd.LinkAddress = Request["MenuLinkAddress"];
                menuAdd.Icon = Request["MenuIcon"];
                menuAdd.Sort = int.Parse(Request["MenuSort"]);
                if (Request["MenuParentId"] != null && Request["MenuParentId"] != "")
                {
                    menuAdd.ParentId = Convert.ToInt32(Request["MenuParentId"]);
                }
                else
                {
                    menuAdd.ParentId = 0;
                }
                menuAdd.CreateBy = uInfo.AccountName;
                menuAdd.CreateTime = DateTime.Now;
                menuAdd.UpdateBy = uInfo.AccountName;
                menuAdd.UpdateTime = DateTime.Now;

                int menuId = DALCore.GetMenuDAL().AddMenu(menuAdd);
                if (menuId > 0)
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
        public ActionResult MenuEdit()
        {
            return View();
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult EditMenu()
        {
            try
            {
                UserEntity uInfo = ViewData["Account"] as UserEntity;     
                int id = Convert.ToInt32(Request["id"]);
                string originalName = Request["originalName"];
                MenuEntity menuEdit = new MenuEntity();
                menuEdit.Id = id;
                menuEdit.Name = Request["MenuName"];
                menuEdit.Code = Request["MenuCode"];
                menuEdit.Icon = Request["MenuIcon"];
                menuEdit.Sort = int.Parse(Request["MenuSort"]);
                menuEdit.LinkAddress = Request["MenuLinkAddress"];
                if (Request["MenuParentId"] != null && Request["MenuParentId"] != "")
                {
                    menuEdit.ParentId = Convert.ToInt32(Request["MenuParentId"]);
                }
                else
                {
                    menuEdit.ParentId = 0;   //根节点
                }
                //menuEdit.CreateBy = uInfo.AccountName;
                //menuEdit.CreateTime = DateTime.Now;
                menuEdit.UpdateBy = uInfo.AccountName;
                menuEdit.UpdateTime = DateTime.Now;

                if (menuEdit.Name != originalName && DALCore.GetMenuDAL().GetMenuByName(menuEdit.Name) != null)
                {
                    throw new Exception("已经存在此菜单！");
                }
                bool result = DALCore.GetMenuDAL().EditMenu(menuEdit);
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

        public ActionResult DelMenuByIDs()
        {
            try
            {
                string Ids = Request["IDs"] == null ? "" : Request["IDs"];
                if (!string.IsNullOrEmpty(Ids))
                {
                    if (DALCore.GetMenuDAL().DeleteMenu(Ids))
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
        /// 菜单树
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllMenuTree()
        {
            DataTable dt = DALCore.GetMenuDAL().GetAllMenu("");
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            DataRow[] rows = dt.Select("parentid = 0");   //赋权限每个角色都必须有父节点的权限，否则一个都不输出了
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    sb.Append("{\"id\":\"" + rows[i]["id"].ToString() + "\",\"text\":\"" + rows[i]["name"].ToString() + "\",\"iconCls\":\"" + rows[i]["icon"].ToString() + "\",\"children\":[");
                    sb.Append(GetChildMenu(dt, rows[i]["id"].ToString()));
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
            }
            else
            {
                sb.Append("]");
            }
            return Content(sb.ToString());
        }

        string GetChildMenu(DataTable dt, string id)
        {
            StringBuilder sb = new StringBuilder();
            DataRow[] r_list = dt.Select(string.Format("parentid={0}", id));
            if (r_list.Length > 0)
            {
                for (int j = 0; j < r_list.Length; j++)
                {
                    DataRow[] child_list = dt.Select(string.Format("parentid={0}", r_list[j]["id"].ToString()));
                    if (child_list.Length > 0)
                    {
                        sb.Append("{\"id\":\"" + r_list[j]["id"].ToString() + "\",\"text\":\"" + r_list[j]["name"].ToString() + "\",\"iconCls\":\"" + r_list[j]["icon"].ToString() + "\",\"children\":[");
                        sb.Append(GetChildMenu(dt, r_list[j]["id"].ToString()));
                    }
                    else
                    {
                        sb.Append("{\"id\":\"" + r_list[j]["id"].ToString() + "\",\"text\":\"" + r_list[j]["name"].ToString() + "\",\"iconCls\":\"" + r_list[j]["icon"].ToString() + "\",\"attributes\":{\"url\":\"" + r_list[j]["linkaddress"].ToString() + "\"}},");
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
        /// 角色菜单树
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllRoleMenuTree()
        {
            int roleid = Convert.ToInt32(Request["roleid"]);
            string roleMenuJson = JsonHelper.ToJson(DALCore.GetMenuDAL().GetAllMenu(roleid));
            return Content(roleMenuJson);
        }

        /// <summary>
        /// 分配按钮页面
        /// </summary>
        /// <returns></returns>
        public ActionResult MenuButtonSet()
        {
            return View();
        }

        /// <summary>
        /// 分配按钮权限
        /// </summary>
        /// <returns></returns>
        public ActionResult SetMenuButton()
        {
            string menuid = Request["menuid"];
            string buttonids = Request["buttonids"];

            bool result = DALCore.GetMenuButtonDAL().SaveMenuButton(menuid, buttonids);
            if (result)
            {
                return Content("{\"msg\":\"分配按钮成功！\",\"success\":true}");
            }
            else
            {
                return Content("{\"msg\":\"分配按钮失败！\",\"success\":false}");
            }
        }

        /// <summary>
        /// 获取已配置的菜单按钮权限
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMenuButtonByMenuID()
        {
            int mid = Convert.ToInt32(Request["menuid"]);  //菜单id
            string jsonStr = JsonHelper.ToJson(DALCore.GetMenuButtonDAL().GetButtonByMenuId(mid));
            return Content(jsonStr);
        }

        /// <summary>
        /// 角色菜单树
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllRoleMenuButtonTree()
        {
            int roleid = Convert.ToInt32(Request["roleid"]);
            DataTable dt = DALCore.GetMenuDAL().GetAllMenuButton(roleid);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            DataRow[] rows = dt.Select("parentid = 0");
            if (rows.Length > 0)
            {
                DataView dataView = new DataView(dt);
                DataTable dtDistinct = dataView.ToTable(true, new string[] { "menuname", "menuid", "parentid" });   //distinct取不重复的子节点
                for (int i = 0; i < rows.Length; i++)
                {
                    string stateStr = "closed";
                    sb.Append("{\"id\":\"" + rows[i]["menuid"].ToString() + "\",\"text\":\"" + rows[i]["menuname"].ToString() + "\",\"state\":\"" + stateStr + "\",\"attributes\":{\"menuid\":\"" + rows[i]["menuid"].ToString() + "\",\"buttonid\":\"0\"},\"children\":[");
                    sb.Append(GetChildMenuButton(dt, dtDistinct, rows[i]["menuid"].ToString(), roleid, stateStr));
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
            }
            else
            {
                sb.Append("]");
            }
            return Content(sb.ToString());
        }

        /// <summary>
        /// 递归
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="menuid"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        string GetChildMenuButton(DataTable dt, DataTable dtDistinct, string menuid, int roleId, string stateStr)
        {
            StringBuilder sb = new StringBuilder();
            DataRow[] r_list = dtDistinct.Select(string.Format("parentid={0}", menuid));
            if (r_list.Length > 0)
            {
                for (int j = 0; j < r_list.Length; j++)
                {
                    DataRow[] child_list = dt.Select(string.Format("parentid={0}", r_list[j]["menuid"].ToString()));
                    if (child_list.Length > 0)
                    {
                        sb.Append("{\"id\":\"" + r_list[j]["menuid"].ToString() + "\",\"text\":\"" + r_list[j]["menuname"].ToString() + "\",\"state\":\"" + stateStr + "\",\"attributes\":{\"menuid\":\"" + r_list[j]["menuid"].ToString() + "\",\"buttonid\":\"0\"},\"children\":[");
                        sb.Append(GetChildMenuButton(dt, dtDistinct, r_list[j]["menuid"].ToString(), roleId, stateStr));
                    }
                    else
                    {
                        sb.Append("{\"id\":\"" + r_list[j]["menuid"].ToString() + "\",\"text\":\"" + r_list[j]["menuname"].ToString() + "\",\"state\":\"" + stateStr + "\",\"attributes\":{\"menuid\":\"" + r_list[j]["menuid"].ToString() + "\",\"buttonid\":\"0\"},\"children\":[");
                        DataRow[] r_listButton = dt.Select(string.Format("menuid = {0} ", r_list[j]["menuid"]));  //子子节点.
                        if (r_listButton.Length > 0)    //有子子节点就遍历进去
                        {
                            for (int k = 0; k < r_listButton.Length; k++)
                            {
                                sb.Append("{\"id\":\"" + roleId + "\",\"text\":\"" + r_listButton[k]["buttonname"].ToString() + "\",\"checked\":" + r_listButton[k]["checked"].ToString() + ",\"attributes\":{\"menuid\":\"" + r_listButton[k]["menuid"].ToString() + "\",\"buttonid\":\"" + r_listButton[k]["buttonid"].ToString() + "\"}},");
                            }
                            sb.Remove(sb.Length - 1, 1);
                        }
                        sb.Append("]},");
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

    }
}
