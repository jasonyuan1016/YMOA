using System.Web;
using System.Web.Optimization;

namespace YMOA.WorkWeb
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //登入页JS
            bundles.Add(new ScriptBundle("~/bundles/js_login").Include(
                    "~/Content/js/jquery/jquery-2.1.1.min.js",
                    "~/Content/js/cookie/jquery.cookie.js",
                    "~/Content/js/md5/jquery.md5.js"
                ));
            //登录页framework*.css
            bundles.Add(new StyleBundle("~/Content/css/framework4Login").Include(
                "~/Content/css/framework-font.css",
                "~/Content/css/framework-login.css",
                "~/Content/User/User@login.css"
                ));

            //framework*.css
            bundles.Add(new StyleBundle("~/Content/css/framework").Include(
                "~/Content/css/framework-font.css",
                "~/Content/css/framework-theme.css",
                "~/Content/css/framework-ui.css"
                ));
            //_Index(列表页Layout)JS
            bundles.Add(new ScriptBundle("~/bundles/js_index").Include(
                    "~/Content/js/jquery/jquery-2.1.1.min.js",
                    "~/Content/js/bootstrap/bootstrap.js",
                    "~/Content/js/jqgrid/jqgrid.min.js",
                    "~/Content/js/framework-ui.js"
                ));

            //_form(表单页Layout)JS
            bundles.Add(new ScriptBundle("~/bundles/js_form").Include(
                "~/Content/js/jquery/jquery-2.1.1.min.js",
                "~/Content/js/bootstrap/bootstrap.js",
                 "~/Content/js/wizard/wizard.js", 
                 "~/Content/js/validate/jquery.validate.min.js", 
                 "~/Content/js/datepicker/WdatePicker.js",
                 "~/Content/js/framework-ui.js"
                ));
            BundleTable.EnableOptimizations = true;
        }
    }
}
