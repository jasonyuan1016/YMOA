using YMOA.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace YMOA.Web.Models.MyTest
{
    public class AjaxPager
    {
        public PagedList<ArticleEntity> Articles { get; set; } 
    }
}