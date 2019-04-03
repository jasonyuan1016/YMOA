using YMOA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace YMOA.Web.Models.MyTest
{
    public class MVCPager
    {
        //信息列表
        public PagedList<ArticleEntity> Articles { get; set; }
    }
}