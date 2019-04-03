using YMOA.Web.App_Start;
using YMOA.Web.Models.ActionFilters;
using System.Web;
using System.Web.Mvc;

namespace YMOA.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            //错误的处理路由
            filters.Add(new BaseHandleErrorAttribute());
            //错误日志记录
            filters.Add(new StatisticsTrackerAttribute());
            //http重定向到https
            //filters.Add(new App_Start.Attribute.RequireHttpsAttribute());
        }
    }
}