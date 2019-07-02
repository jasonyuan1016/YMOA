using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mail;
using System.Web.Mvc;
using System.Web.Security;
using YMOA.Comm;

namespace YMOA.WorkWeb.Controllers
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/4/22
    /// 登录控制器
    /// </summary>
    public class LoginController : BaseController
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAuthCode()
        {
            return File(new VerifyCode().GetVerifyCode(), @"image/Gif");
        }

        /// <summary>
        /// 登录事件
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult CheckLogin(string username, string password, string code)
        {
            if (Session["session_verifycode"].IsEmpty() || Md5.md5(code.ToLower(), 16) != Session["session_verifycode"].ToString())
            {
                return AjaxReturn(ResultType.error, "验证码错误，请重新输入");
            }
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["UserId"] = username;
            paras["UserPwd"] = password;
            var currentUser = DALUtility.UserCore.UserLogin(paras);
            if (currentUser != null)
            {
                if (currentUser.IsAble == false)
                {
                    return AjaxReturn(ResultType.error, "用户已被禁用，请您联系管理员");
                }
                
                Session["UserId"] = currentUser.AccountName;
                Session["RealName"] = currentUser.RealName;
                DateTime dateTime = DateTime.Now;
                Session["LoginTime"] = dateTime;
                Session["RoleId"] = currentUser.RoleId;

                var CurrentOnline = System.Web.HttpContext.Current.Application["CurrentOnline"];
                
                if (CurrentOnline != null)
                {
                    Hashtable htOnline = (Hashtable)CurrentOnline;
                    htOnline[currentUser.AccountName] = dateTime;
                }
                else
                {
                    Hashtable htOnline = new Hashtable();
                    htOnline[currentUser.AccountName] = dateTime;
                    System.Web.HttpContext.Current.Application["CurrentOnline"] = htOnline;
                }
                return AjaxReturn(ResultType.success, "登录成功");
            }
            else
            {
                return AjaxReturn(ResultType.error, "用户名密码错误，请您检查");
            }
        }

        /// <summary>
        /// 忘记密码页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgetPwd()
        {
            return View();
        }

        /// <summary>
        /// 忘记密码操作
        /// </summary>
        /// <returns></returns>
        public ActionResult PwdForget()
        {
            //bool f = false;
            //string user = Request["user_AN"];
            //var iUserDal = DALUtility.User;
            //var currentUser = iUserDal.GetUserByAccountName(user);
            //Session["UserId"] = currentUser;
            ////链接地址必须是绝对地址
            //string mailContent = "<a href='http://172.16.31.234:6666/User/PwdUpdate'>修改密码</a>";
            //if (currentUser != null)
            //{
            //    f = EmailHelper.send(currentUser.UserEmail, "点击链接修改密码", mailContent);
            //}
            return OperationReturn(true, "邮件已发送！");
        }

        /// <summary>
        /// 安全退出
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OutLogin()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        /// <summary>
        ///  修改语系
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
            {
                cookie.Value = culture;
            }
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddDays(1);
            }
            Response.Cookies.Add(cookie);
            ClientCulture = culture;
            return Content("1");
        }
    }
}