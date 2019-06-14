/*-------------------------------------*
 * 創建人:         J.Y
 * 創建時間:       2019/04/28
 * 最后修改時間:    
 * 最后修改原因:
 * 修改歷史:
 * 2019/04/28       J.Y       創建
 *-------------------------------------*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Model;
using YMOA.WorkWeb.Resources;

namespace YMOA.WorkWeb.Domain
{
    /// <summary>
    /// 权限拦截过滤
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionFilterAttribute : ActionFilterAttribute
    {
        private string controller { get; set; }
        private string action { get; set; }
        private Operationype operationype { get; set; }
        private bool isViewPage = false;


        /// <summary>
        /// 权限过滤
        /// </summary>
        /// <param name="_controller">Controller</param>
        /// <param name="_action">Action</param>
        /// <param name="_operationype">执行动作类型</param>
        public PermissionFilterAttribute(string _controller = "", string _action = "", Operationype _operationype = Operationype.View)
        {
            isViewPage = _controller.Equals(string.Empty) && _action.Equals(string.Empty);
            controller = _controller;
            action = _action;
            operationype = _operationype;
        }
        /// <summary>
        /// 权限拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var allowAccess = false;
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (controller.Equals(string.Empty))
            {
                controller = filterContext.RouteData.Values["controller"].ToString();
            }
            if (action.Equals(string.Empty))
            {
                action = filterContext.RouteData.Values["action"].ToString();
            }
            if (HttpContext.Current.Session["MemuList"] != null)
            {
                var memuInfo = ((List<MenuPermission>)HttpContext.Current.Session["MemuList"]).SingleOrDefault(
                    x => x.controller.Equals(controller, StringComparison.CurrentCultureIgnoreCase)
                    && x.action.Equals(action, StringComparison.CurrentCultureIgnoreCase));

                if (memuInfo != null)
                {
                    switch (operationype)
                    {
                        case Operationype.View:
                            allowAccess = true;
                            break;
                        case Operationype.Add:
                            allowAccess = memuInfo.add;
                            break;
                        case Operationype.Update:
                            allowAccess = memuInfo.update;
                            break;
                        case Operationype.Delete:
                            allowAccess = memuInfo.delete;
                            break;
                        case Operationype.Other:
                            allowAccess = memuInfo.other;
                            break;
                    }
                    filterContext.Controller.ViewData["Add"] = memuInfo.add;
                    filterContext.Controller.ViewData["Update"] = memuInfo.update;
                    filterContext.Controller.ViewData["Delete"] = memuInfo.delete;
                    filterContext.Controller.ViewBag.Title = true;
                }
            }

            if (!allowAccess)
            {
                filterContext.HttpContext.Session.Clear();
                //filterContext.HttpContext.Request.IsAjaxRequest
                if (isViewPage)
                {
                    // 修改时间: 2019年6月14日; 修改人: 朱星宇; 修改原因:页面跳转可能存在嵌套页面
                    // 原代码: filterContext.RequestContext.HttpContext.Response.Redirect("~/Login/Index");
                    // 修改后: 页面跳转到登录页面
                    var obj = new { success = false, msg = Resource.ResourceManager.GetString("ormsg_distanceLogin"), code = "-101" };
                    filterContext.Result = new ContentResult() { Content = JsonConvert.SerializeObject(obj) };
                }
                else
                {
                    var obj = new { success = false, msg = Resource.ResourceManager.GetString("ormsg_nopermissions"), code = "-100" };
                    filterContext.Result = new ContentResult() { Content = JsonConvert.SerializeObject(obj) };
                }
            }
            else
            {
                //判断重复登入
                var CurrentOnline = System.Web.HttpContext.Current.Application["CurrentOnline"];
                Hashtable htOnline = (Hashtable)CurrentOnline;
                if (htOnline != null && htOnline[filterContext.HttpContext.Session["UserId"].ToString()].ToString() != filterContext.HttpContext.Session["LoginTime"].ToString())
                {
                    filterContext.HttpContext.Session.Clear();
                    if (isViewPage)
                    {
                        filterContext.RequestContext.HttpContext.Response.Redirect("~/Manager/Login?t=rl");
                    }
                    else
                    {
                        var obj = new { success = false, msg = Resource.ResourceManager.GetString("ormsg_distanceLogin"), code = "-101" };
                        filterContext.Result = new ContentResult() { Content = JsonConvert.SerializeObject(obj) };
                    }
                    return;
                }
            }
        }
    }
}