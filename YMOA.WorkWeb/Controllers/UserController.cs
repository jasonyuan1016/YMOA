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
        [PermissionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionFilter("user","index")]
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

        [PermissionFilter("user", "index")]
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

        [PermissionFilter("user", "index", Operationype.Add)]
        public ActionResult Add(UserEntity userEntity)
        {
            return SubmitForm(userEntity);
        }

        [PermissionFilter("user", "index", Operationype.Update)]
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
            paras["RealName"] = userEntity.RealName;
            paras["RoleId"] = userEntity.RoleId;
            paras["DepartmentId"] = userEntity.DepartmentId;
            paras["MobilePhone"] = userEntity.MobilePhone;
            paras["Birthday"] = userEntity.Birthday;
            paras["Entrydate"] = userEntity.Entrydate;
            paras["IsAble"] = userEntity.IsAble;
            paras["Description"] = userEntity.Description == null ? "" : userEntity.Description;
            // 改变部门时初始化职务
            if (userEntity.ID != 0)
            {
                Dictionary<string, object> para = new Dictionary<string, object>();
                para["ID"] = userEntity.ID;
                // 获取原部门
                int departmentId = DALUtility.UserCore.QryUserInfo<UserEntity>(paras).DepartmentId;
                if (departmentId != userEntity.DepartmentId)
                {
                    paras["DutyId"] = 1;
                }
            }
            bool boo = DALUtility.UserCore.Save(paras) > 0;
            return OperationReturn(boo);
        }

        /// <summary>
        ///  修改用户是否启用
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="IsAble"></param>
        /// <returns></returns>
        [PermissionFilter("user", "index", Operationype.Update)]
        public ActionResult UpdateIsAble(int ID, bool IsAble)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = ID;
            paras["IsAble"] = IsAble;
            return OperationReturn(DALUtility.UserCore.Save(paras) > 0);
        }
        /// <summary>
        ///  用户删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>

        [PermissionFilter("user", "index", Operationype.Delete)]
        public ActionResult Delete(int ID)
        {
            return OperationReturn(DALUtility.UserCore.OnlyDeleteUser(ID.ToString()));
        }
    }
}