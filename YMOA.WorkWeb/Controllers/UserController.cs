using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.Model;
using YMOA.WorkWeb.Domain;
using YMOA.WorkWeb.Resources;

namespace YMOA.WorkWeb.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        [PermissionFilter("user", "Index")]
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
                rows = DALUtility.UserCore.QryUsers<UserEntity>(pagination, pars),
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
                userInfo = DALUtility.UserCore.QryUserInfo<UserEntity>(paras);
            }
            return View(userInfo);
        }
        [PermissionFilter("user", "Index", Operationype.Add)]
        public ActionResult Add(UserEntity userEntity)
        {
            return SubmitForm(userEntity);
        }
        [PermissionFilter("user", "Index", Operationype.Update)]
        public ActionResult Update(UserEntity userEntity)
        {
            return SubmitForm(userEntity);
        }
        private ActionResult SubmitForm(UserEntity userEntity)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = userEntity.ID;
            paras["AccountName"] = userEntity.AccountName;
            paras["Email"] = userEntity.Email;
            //验证账号、邮箱是否重复
            int result = DALUtility.UserCore.CheckUseridAndEmail(paras);
            if (result > 0)
            {
                return OperationReturn(false, Resource.ResourceManager.GetString("ormsg_user_" + result));
            }
            if (userEntity.Password != "******")
            {
                paras["Password"] = userEntity.Password;
            }
            // 用户是否修改密码
            if (userEntity.ID == 0)
            {
                paras["IfChangePwd"] = false;
            }
            // 职务编号
            paras["DutyId"] = 1;
            paras["RealName"] = userEntity.RealName;
            paras["RoleId"] = userEntity.RoleId;
            paras["DepartmentId"] = userEntity.DepartmentId;
            paras["MobilePhone"] = userEntity.MobilePhone;
            paras["Birthday"] = userEntity.Birthday;
            paras["Entrydate"] = userEntity.Entrydate;
            paras["IsAble"] = userEntity.IsAble;
            paras["Description"] = userEntity.Description == null ? "" : userEntity.Description;
            return OperationReturn(DALUtility.UserCore.Save(paras) > 0);
        }

        [PermissionFilter("user", "Index", Operationype.Update)]
        /// <summary>
        ///  修改用户是否启用
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="IsAble"></param>
        /// <returns></returns>
        public ActionResult UpdateIsAble(int ID, bool IsAble)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = ID;
            paras["IsAble"] = IsAble;
            return OperationReturn(DALUtility.UserCore.Save(paras) > 0);
        }

        [PermissionFilter("user", "Index", Operationype.Delete)]
        /// <summary>
        ///  用户删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Delete(int ID)
        {
            return OperationReturn(DALUtility.UserCore.OnlyDeleteUser(ID.ToString()));
        }
    }
}