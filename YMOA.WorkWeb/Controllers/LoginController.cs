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

        [HttpGet]
        public ActionResult GetAuthCode()
        {
            return File(new VerifyCode().GetVerifyCode(), @"image/Gif");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult CheckLogin(string username, string password, string code)
        {
            if (Session["nfine_session_verifycode"].IsEmpty() || Md5.md5(code.ToLower(), 16) != Session["nfine_session_verifycode"].ToString())
            {
                throw new Exception("验证码错误，请重新输入");
            }
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["UserId"] = username;
            paras["UserPwd"] = password;
            var currentUser = DALUtility.User.UserLogin(paras);
            if (currentUser != null)
            {
                if (currentUser.IsAble == false)
                {
                    throw new Exception("用户已被禁用，请您联系管理员");
                }
                OperatorModel operatorModel = new OperatorModel();
                operatorModel.UserId = currentUser.AccountName;
                operatorModel.UserName = currentUser.RealName;
                operatorModel.RoleId = currentUser.RoleId;
                operatorModel.LoginIPAddress = Net.Ip;
                operatorModel.LoginIPAddressName = Net.GetLocation(operatorModel.LoginIPAddress);
                operatorModel.LoginTime = DateTime.Now;
                operatorModel.LoginToken = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                if (currentUser.AccountName == "admin")
                {
                    operatorModel.IsSystem = true;
                }
                else
                {
                    operatorModel.IsSystem = false;
                }
                OperatorProvider.Provider.AddCurrent(operatorModel);

                //CookiesHelper.SetCookie("UserID", AES.EncryptStr(currentUser.ID.ToString()));
                //FormsAuthentication.SetAuthCookie(currentUser.AccountName.ToUpper(), false);
                //Session["User"] = currentUser.AccountName;
                //Session["LoginTime"] = DateTime.Now;
                //Session["RoleID"] = currentUser.RoleID;

                var CurrentOnline = System.Web.HttpContext.Current.Application["CurrentOnline"];
                if (CurrentOnline != null)
                {
                    Hashtable htOnline = (Hashtable)CurrentOnline;
                    htOnline[currentUser.AccountName] = DateTime.Now;
                }
                else
                {
                    Hashtable htOnline = new Hashtable();
                    htOnline[currentUser.AccountName] = DateTime.Now;
                    System.Web.HttpContext.Current.Application["CurrentOnline"] = htOnline;
                }
                return Content(new AjaxResult { state = ResultType.success.ToString(), message = "登录成功" }.ToJson());

            }
            else
            {
                throw new Exception("用户名密码错误，请您检查");
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
            //Session["User"] = currentUser;
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
        public ActionResult UserLoginOut()
        {
            //清空cookie
            CookiesHelper.AddCookie("UserID", System.DateTime.Now.AddDays(-1));
            Session.Clear();
            return OperationReturn(true,"退出成功！");
        }
    }
}