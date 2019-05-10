using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.Model;

namespace YMOA.WorkWeb.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetGridJson(Pagination pagination, string keyword)
        {
            Dictionary<string, object> pars = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(keyword))
            {
                pars["keyword"] = keyword;
            }
            var data = new
            {
                rows = DALUtility.User.QryUsers<UserEntity>(pagination, pars),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        public ActionResult UserEdit(int ID = 0)
        {
            var userInfo = new UserEntity();
            if (ID > 0)
            {
                Dictionary<string, object> paras = new Dictionary<string, object>();
                paras["ID"] = ID;
                userInfo = DALUtility.User.QryUserInfo<UserEntity>(paras);
            }
            return View(userInfo);
        }

        public ActionResult Add(UserEntity userEntity)
        {
            return SubmitForm(userEntity);
        }

        public ActionResult Update(UserEntity userEntity)
        {
            return SubmitForm(userEntity);
        }
        public ActionResult SubmitForm(UserEntity userEntity)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = userEntity.ID;
            if (userEntity.ID == 0)
            {
                //验证账号是否重复
                paras["AccountName"] = userEntity.AccountName;
            }
            if (userEntity.Password != "******")
            {
                paras["Password"] = userEntity.Password;
            }
            //验证手机号、邮箱是否重复
            paras["RealName"] = userEntity.RealName;
            paras["RoleId"] = userEntity.RoleId;
            paras["DepartmentId"] = userEntity.DepartmentId;
            paras["DutyId"] = userEntity.DutyId;
            paras["Birthday"] = userEntity.Birthday;
            paras["Entrydate"] = userEntity.Entrydate;
            paras["MobilePhone"] = userEntity.MobilePhone;
            paras["IsAble"] = userEntity.IsAble;
            //……待补充
            return OperationReturn(DALUtility.User.Save(paras) > 0);
        }
    }
}