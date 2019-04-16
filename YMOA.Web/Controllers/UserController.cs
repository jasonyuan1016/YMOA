using YMOA.DALFactory;
using YMOA.Comm;
using YMOA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace YMOA.Web.Controllers
{
    [App_Start.JudgmentLogin]
    public class UserController : BaseController
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="UserPwd"></param>
        /// <param name="NewPwd"></param>
        /// <param name="ConfirmPwd"></param>
        /// <returns></returns>
        public ActionResult UpdatePwd(string UserPwd, string NewPwd, string ConfirmPwd)
        {
            //string result = "";
            UserEntity uInfo = ViewData["Account"] as UserEntity;

            UserEntity userChangePwd = new UserEntity();
            userChangePwd.ID = uInfo.ID;
            userChangePwd.Password = Md5.GetMD5String(NewPwd);   //md5加密

            if (Md5.GetMD5String(UserPwd) == uInfo.Password)
            {
                if (DALUtility.User.ChangePwd(userChangePwd))
                {
                    return OperationReturn(true, "修改成功，请重新登录！");
                }
                else
                {
                    return OperationReturn(false, "修改失败！");
                }
            }
            else
            {
                return OperationReturn(false, "原密码不正确！");
            }
            //return Content(result);
        }

        public ActionResult ChangePwd()
        {
            return View();
        }

        public ActionResult GetAllUserInfo()
        {
            string sort = Request["sort"] == null ? "ID" : Request["sort"];
            string order = Request["order"] == null ? "asc" : Request["order"];

            //首先获取前台传递过来的参数
            int pageindex = Request["page"] == null ? 1 : Convert.ToInt32(Request["page"]);
            int pagesize = Request["rows"] == null ? 10 : Convert.ToInt32(Request["rows"]);
            string userid = Request["accountid"] == null ? "" : Request["accountid"];
            string username = Request["username"] == null ? "" : Request["username"];
            string isable = Request["isable"] == null ? "" : Request["isable"];
            string ifchangepwd = Request["ifchangepwd"] == null ? "" : Request["ifchangepwd"];
            string userperson = Request["userperson"] == null ? "" : Request["userperson"];
            string adddatestart = Request["adddatestart"] == null ? "" : Request["adddatestart"];
            string adddateend = Request["adddateend"] == null ? "" : Request["adddateend"];
            int roleid = Request["roleid"] == null ? 0 :int.Parse(Request["roleid"]);
            
            int totalCount;   //输出参数
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["pi"] = pageindex;
            paras["pageSize"] = pagesize;
            paras["userid"] = userid;
            paras["username"] = username;
            paras["IsAble"] = true;
            paras["IfChangePwd"] = true;
            paras["adddatestart"] = adddatestart;
            paras["adddateend"] = adddateend;
            paras["sort"] = sort;
            paras["order"] = order;
            if (roleid > 0)
            {
                paras["RoleID"] = roleid;
            }
            var users = DALUtility.User.QryUsers<UserEntity>(paras, out totalCount);
            return PagerData(totalCount, users);
        }

        /// <summary>
        /// 新增页面展示
        /// </summary>
        /// <returns></returns>
        public ActionResult UserAdd()
        {
            return View();
        }

        /// <summary>
        /// 新增 用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUser()
        {
            return SaveUser();

        }

        /// <summary>
        /// 编辑页面展示
        /// </summary>
        /// <returns></returns>
        public ActionResult UserEdit(int ID = 0)
        {
            var userInfo = new UserEntity();
            if (ID > 0)
            {
                Dictionary<string, object> paras = new Dictionary<string, object>();
                paras["ID"] = ID;
                userInfo = DALUtility.User.QryUserInfo<UserEntity>(paras);
            }
            else
            {
                userInfo.IfChangePwd = true;
                userInfo.IsAble = true;
            }
            return PartialView("_UserEdit", userInfo);
        }

        /// <summary>
        /// 编辑 用户
        /// </summary>
        /// <returns></returns>
        public ActionResult EditUser()
        {
            
            return SaveUser();
        }

        private ActionResult SaveUser()
        {
            int id = Convert.ToInt32(Request["id"]);
            string userid = Request["UserID"];
            string username = Request["UserName"];
            bool isable = bool.Parse(Request["Isable"]);
            bool ifchangepwd = bool.Parse(Request["IfChangepwd"]);
            string description = Request["Description"];

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras["ID"] = id;
            paras["AccountName"] = userid;
            paras["Email"] = Request["Email"];
            int iCheck = DALUtility.User.CheckUseridAndEmail(paras);
            paras["RealName"] = username;
            paras["RoleID"] = Convert.ToInt32(Request["RoleID"]);
            paras["MobilePhone"] = Request["MobilePhone"];
            paras["IsAble"] = isable;
            paras["IfChangePwd"] = ifchangepwd;
            paras["Description"] = description.Trim();
            paras["UpdateBy"] = "admin";
#warning 获取当前用户名
            paras["UpdateTime"] = DateTime.Now;
            if (id == 0)
            {
                paras["Password"] = Md5.GetMD5String("q123456");   //md5加密
                paras["CreateBy"] = paras["UpdateBy"];
                paras["CreateTime"] = paras["UpdateTime"];
            }
            
            if (iCheck > 0)
            {
                return OperationReturn(false, iCheck == 1 ? "用户名重复" : "邮箱重复");
            }
            else
            {
                return OperationReturn(DALUtility.User.Save(paras) > 0);
            }
        }

        public ActionResult DelUserByIDs()
        {
            string Ids = Request["IDs"] == null ? "" : Request["IDs"];
            if (!string.IsNullOrEmpty(Ids))
            {
                return OperationReturn(DALUtility.User.DeleteUser(Ids));
            }
            else
            {
                return OperationReturn(false);
            }
        }

        /// <summary>
        /// 用户角色权限页面展示
        /// </summary>
        /// <returns></returns>
        public ActionResult UserRole()
        {
            return View();
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllRoleInfo()
        {
            string roleJson = JsonHelper.ToJson(DALUtility.Role.GetAllRole("1=1"));
            return Content(roleJson);
        }


        public ActionResult SetUserDept()
        {
            return View();
        }

        public ActionResult UserDeptSet()
        {
            string UserIds = Request["UserIds"];
            string DeptIds = Request["DeptIds"];

            if (UserIds.IndexOf(",") == -1)  //单个用户设置部门
            {
                if (UserIds != "" && SetDepartmentSingle(Convert.ToInt32(UserIds), DeptIds))
                {
                    return Content("{\"msg\":\"设置成功！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"设置失败！\",\"success\":true}");
                }
            }
            else   //批量设置用户部门
            {
                if (UserIds != "" && SetDepartmentBatch(UserIds, DeptIds))
                {
                    return Content("{\"msg\":\"设置成功！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"设置失败！\",\"success\":true}");
                }
            }
        }

        /// <summary>
        /// 设置用户部门（单个用户）
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="depIds">部门id，多个用逗号隔开</param>
        bool SetDepartmentSingle(int userId, string depIds)
        {
            DataTable dt_user_dep_old = DALUtility.Department.GetDepartmentByUserId(userId);  //用户之前拥有的部门
            List<UserDepartmentEntity> dep_addList = new List<UserDepartmentEntity>();     //需要插入部门的sql语句集合
            List<UserDepartmentEntity> dep_deleteList = new List<UserDepartmentEntity>();     //需要删除部门的sql语句集合

            string[] str_dep = depIds.Trim(',').Split(',');    //传过来用户勾选的部门（有去勾的也有新勾选的）

            UserDepartmentEntity userdepdelete = null;
            UserDepartmentEntity userdepadd = null;
            for (int i = 0; i < dt_user_dep_old.Rows.Count; i++)
            {
                //等于-1说明用户去掉勾选了某个部门 需要删除
                if (Array.IndexOf(str_dep, dt_user_dep_old.Rows[i]["departmentid"].ToString()) == -1)
                {
                    userdepdelete = new UserDepartmentEntity();
                    userdepdelete.DepartmentId = Convert.ToInt32(dt_user_dep_old.Rows[i]["departmentid"].ToString());
                    userdepdelete.UserId = userId;
                    dep_deleteList.Add(userdepdelete);
                }
            }

            if (!string.IsNullOrEmpty(depIds))
            {
                for (int j = 0; j < str_dep.Length; j++)
                {
                    //等于0那么原来的部门没有 是用户新勾选的
                    if (dt_user_dep_old.Select("departmentid = '" + str_dep[j] + "'").Length == 0)
                    {
                        userdepadd = new UserDepartmentEntity();
                        userdepadd.UserId = userId;
                        userdepadd.DepartmentId = Convert.ToInt32(str_dep[j]);
                        dep_addList.Add(userdepadd);
                    }
                }
            }
            if (dep_addList.Count == 0 && dep_deleteList.Count == 0)
                return true;
            else
                return DALUtility.UserDepartment.SetDepartmentSingle(dep_addList, dep_deleteList);
        }

        /// <summary>
        /// 设置用户部门（批量设置）
        /// </summary>
        /// <param name="userIds">用户主键，多个用逗号隔开</param>
        /// <param name="depIds">部门id，多个用逗号隔开</param>
        bool SetDepartmentBatch(string userIds, string depIds)
        {
            List<UserDepartmentEntity> dep_addList = new List<UserDepartmentEntity>();     //需要插入部门的sql语句集合
            List<UserDepartmentEntity> dep_deleteList = new List<UserDepartmentEntity>();     //需要删除部门的sql语句集合
            string[] str_userid = userIds.Trim(',').Split(',');
            string[] str_dep = depIds.Trim(',').Split(',');

            UserDepartmentEntity userdepdelete = null;
            UserDepartmentEntity userdepadd = null;
            for (int i = 0; i < str_userid.Length; i++)
            {
                //批量设置先删除当前用户的所有部门
                userdepdelete = new UserDepartmentEntity();
                userdepdelete.UserId = Convert.ToInt32(str_userid[i]);
                dep_deleteList.Add(userdepdelete);

                if (!string.IsNullOrEmpty(depIds))
                {
                    //再添加设置的部门
                    for (int j = 0; j < str_dep.Length; j++)
                    {
                        userdepadd = new UserDepartmentEntity();
                        userdepadd.UserId = Convert.ToInt32(str_userid[i]);
                        userdepadd.DepartmentId = Convert.ToInt32(str_dep[j]);
                        dep_addList.Add(userdepadd);
                    }
                }
            }
            return DALUtility.UserDepartment.SetDepartmentBatch(dep_addList, dep_deleteList);
        }
    }
}
