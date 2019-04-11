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
    public class TableController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllTableInfo()
        {
            string strWhere = "1=1";
            string sort = Request["sort"] == null ? "id" : Request["sort"];
            string order = Request["order"] == null ? "asc" : Request["order"];
            if (!string.IsNullOrEmpty(Request["TabName"]) && !SqlInjection.GetString(Request["TabName"]))
            {
                strWhere += " and TabName like '%" + Request["TabName"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["TabViewName"]) && !SqlInjection.GetString(Request["TabViewName"]))
            {
                strWhere += " and TabViewName like '%" + Request["TabViewName"] + "%'";
            }
            //首先获取前台传递过来的参数
            int pageindex = Request["page"] == null ? 1 : Convert.ToInt32(Request["page"]);
            int pagesize = Request["rows"] == null ? 10 : Convert.ToInt32(Request["rows"]);
            int totalCount = 0;   //输出参数
            string strJson = JsonHelper.ToJson(SqlPagerHelper.GetPager("tbTable", "Id, TabName, TabViewName, IsActive, CreateTime, CreateBy, UpdateTime, UpdateBy", sort + " " + order, pagesize, pageindex, strWhere, out totalCount));

            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + strJson + "}");
        }

        /// <summary>
        /// 新增页面展示
        /// </summary>
        /// <returns></returns>
        public ActionResult TableAdd()
        {
            return View();
        }

        /// <summary>
        /// 新增数据表
        /// </summary>
        /// <returns></returns>
        public ActionResult AddTable()
        {
            try
            {
                UserEntity uInfo = ViewData["Account"] as UserEntity;
                TableEntity typeAdd = new TableEntity();
                typeAdd.TabName = Request["TabName"].Trim();
                typeAdd.TabViewName = Request["TabViewName"].Trim();
                typeAdd.IsActive = bool.Parse(Request["IsActive"]);
                typeAdd.CreateBy = uInfo.AccountName;
                typeAdd.CreateTime = DateTime.Now;
                typeAdd.UpdateBy = uInfo.AccountName;
                typeAdd.UpdateTime = DateTime.Now;

                bool ExistsTabName = DALUtility.Table.ExistsTabName(typeAdd.TabName);
                bool ExistsTabViewName = DALUtility.Table.ExistsTabViewName(typeAdd.TabViewName);
                if (ExistsTabName)
                {
                    return Content("{\"msg\":\"添加失败,物理表名已存在！\",\"success\":false}");
                }
                else if (ExistsTabViewName)
                {
                    return Content("{\"msg\":\"添加失败,表显示名已存在！\",\"success\":false}");
                }
                else
                {
                    int typeId = DALUtility.Table.Add(typeAdd);
                    if (typeId > 0)
                    {
                        //数据库-新建物理表
                        string dbTabName = "tb_" + typeAdd.TabName;
                   
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
        public ActionResult TableEdit()
        {
            return View();
        }

        /// <summary>
        /// 编辑数据表
        /// </summary>
        /// <returns></returns>
        public ActionResult EditTable()
        {
            try
            {
                UserEntity uInfo = ViewData["Account"] as UserEntity;

                int id = Convert.ToInt32(Request["id"]);
                string originalName = Request["originalName"];
                string originalViewName = Request["originalViewName"];
                TableEntity typeEdit = DALUtility.Table.GetModel(id);
                typeEdit.TabName = Request["TabName"].Trim();
                typeEdit.TabViewName = Request["TabViewName"].Trim();
                typeEdit.IsActive = bool.Parse(Request["IsActive"]);
                typeEdit.UpdateBy = uInfo.AccountName;
                typeEdit.UpdateTime = DateTime.Now;
                bool ExistsTabViewName = DALUtility.Table.ExistsTabViewName(typeEdit.TabViewName);
                if (typeEdit.TabViewName != originalViewName && ExistsTabViewName)
                {
                    return Content("{\"msg\":\"修改失败,表显示名已存在！\",\"success\":false}");
                }
                else
                {
                    int result = DALUtility.Table.Update(typeEdit);
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

        public ActionResult DelTableByIDs()
        {
            try
            {
                string Ids = Request["IDs"] == null ? "" : Request["IDs"];
                if (!string.IsNullOrEmpty(Ids))
                {
                    string[] idArr = Ids.TrimEnd(',').Split(',');
                    int num = 0;
                    foreach (string id in idArr)
                    {
                        TableEntity model = DALUtility.Table.GetModel(int.Parse(id));
                        string dbTabName = "tb_" + model.TabName;
       
                            num = num + 1;

                    }
                    if (idArr.Length == num)
                    {
                        if (DALUtility.Table.DeleteList(Ids))
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
                        return Content("{\"msg\":\"删除物理数据表失败！\",\"success\":false}");
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

        public ActionResult GetAllTableDrop()
        {
            string roleJson = "";
            return Content(roleJson);

        }

        /// <summary>
        /// 获取角色所属用户
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFilesByTabId()
        {
            int TabId = int.Parse(Request["TabId"]);
            string sort = Request["sort"] == null ? "Id" : Request["sort"];
            string order = Request["order"] == null ? "asc" : Request["order"];
            int pageindex = Request["page"] == null ? 1 : Convert.ToInt32(Request["page"]);
            int pagesize = Request["rows"] == null ? 10 : Convert.ToInt32(Request["rows"]);
            int totalCount = 0;
            string strWhere = " 1=1 and TabId = '" + Request["TabId"] + "'";
            string strJson = JsonHelper.ToJson(SqlPagerHelper.GetPager("vw_Fields", "Id,TabId,FieldName,FieldViewName,FieldDataTypeId,IsActive,Sort,CreateTime,CreateBy,UpdateTime,UpdateBy,DataType,DataTypeName,TabName,TabViewName", sort + " " + order, pagesize, pageindex, strWhere, out totalCount));
            return Content(strJson);
        }

        /// <summary>
        /// 数据表查询
        /// </summary>
        /// <returns></returns>
        public ActionResult TabDataView()
        {
            int TabId = int.Parse(Request["TabId"] == null ? "0" : Request["TabId"]);
            ViewBag.TabId = TabId;
            return View();
        }

        /// <summary>
        /// 数据表查询 动态获取列
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTabColsJsonStr()
        {
            return Content("");
        }


        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTabDataInfoByTabId()
        {
            int TabId = int.Parse(Request["TabId"] == null ? "0" : Request["TabId"]);
            string sort = Request["sort"] == null ? "Id" : Request["sort"];
            string order = Request["order"] == null ? "asc" : Request["order"];
            int pageindex = Request["page"] == null ? 1 : Convert.ToInt32(Request["page"]);
            int pagesize = Request["rows"] == null ? 10 : Convert.ToInt32(Request["rows"]);
            int totalCount = 0;

            TableEntity entity = DALUtility.Table.GetModel(TabId);
            string dbTabName = "tb_" + entity.TabName;
            string strJson = ""; //DALUtility.Fields.GetPager(dbTabName, CommFunc.GetColumnsStr(TabId), sort + " " + order, pagesize, pageindex, " 1=1 ", out totalCount);
            return Content(strJson);
        }
    }
}
