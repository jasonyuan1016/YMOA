using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.DALFactory;
using YMOA.Model;

namespace YMOA.Web.Controllers
{
    public class LoginController : BaseController
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 处理登录的信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="CookieExpires">cookie有效期</param>
        /// <returns></returns>
        public ActionResult CheckUserLogin(UserEntity userInfo, string CookieExpires)
        {
            try
            {
                var iUserDal = DALUtility.User;
                var currentUser = iUserDal.UserLogin(userInfo.AccountName, Md5.md5(userInfo.Password));
                if (currentUser != null)
                {
                    if (currentUser.IsAble == false)
                    {
                        return Content("用户已被禁用，请您联系管理员");
                    }
                    //记录登录cookie
                    CookiesHelper.SetCookie("UserID", AES.EncryptStr(currentUser.ID.ToString()));
                    //记录用户登录所在IP
                    LoginIpLogEntity logEntity = new LoginIpLogEntity();
                    string ip = CommFunc.Get_ClientIP();
                    if (string.IsNullOrEmpty(ip))
                    {
                        logEntity.IpAddress = "localhost";
                    }
                    else
                    {
                        logEntity.IpAddress = ip;
                    }
                    logEntity.CreateBy = currentUser.AccountName;
                    logEntity.CreateTime = DateTime.Now;
                    logEntity.UpdateBy = currentUser.AccountName;
                    logEntity.UpdateTime = DateTime.Now;
                    DALUtility.LoginIpLog.Add(logEntity);

                    return Content("OK");
                }
                else
                {
                    return Content("用户名密码错误，请您检查");
                }
            }
            catch (Exception ex)
            {
                return Content("登录异常," + ex.Message);
            }
        }

        public ActionResult UserLoginOut()
        {
            //清空cookie
            CookiesHelper.AddCookie("UserID", System.DateTime.Now.AddDays(-1));
            return Content("{\"msg\":\"退出成功！\",\"success\":true}");
        }
    }
}
