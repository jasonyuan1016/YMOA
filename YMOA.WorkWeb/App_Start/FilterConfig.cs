﻿using System.Web;
using System.Web.Mvc;
using YMOA.WorkWeb.App_Start;

namespace YMOA.WorkWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandlerErrorAttribute());
        }
    }
}
