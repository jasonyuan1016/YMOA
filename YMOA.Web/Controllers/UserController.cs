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
    public class UserController : Controller
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
            try
            {
                string result = "";
                UserEntity uInfo = ViewData["Account"] as UserEntity;

                UserEntity userChangePwd = new UserEntity();
                userChangePwd.ID = uInfo.ID;
                userChangePwd.Password = Md5.GetMD5String(NewPwd);   //md5加密

                if (Md5.GetMD5String(UserPwd) == uInfo.Password)
                {
                    if (DALCore.GetUserDAL().ChangePwd(userChangePwd))
                    {
                        result = "{\"msg\":\"修改成功，请重新登录！\",\"success\":true}";
                    }
                    else
                    {
                        result = "{\"msg\":\"修改失败！\",\"success\":false}";
                    }
                }
                else
                {
                    result = "{\"msg\":\"原密码不正确！\",\"success\":false}";
                }
                return Content(result);
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"修改失败," + ex.Message + "\",\"success\":false}");
            }
        }

        public ActionResult ChangePwd()
        {
            return View();
        }

        public ActionResult GetAllUserInfo()
        {
            string strWhere = "1=1";
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

            if (userid.Trim() != "" && !SqlInjection.GetString(userid))   //防止sql注入
                strWhere += string.Format(" and AccountName like '%{0}%'", userid.Trim());
            if (username.Trim() != "" && !SqlInjection.GetString(username))
                strWhere += string.Format(" and RealName like '%{0}%'", username.Trim());
            if (isable.Trim() != "select" && isable.Trim() != "")
                strWhere += " and IsAble = '" + isable.Trim() + "'";
            if (ifchangepwd.Trim() != "select" && ifchangepwd.Trim() != "")
                strWhere += " and IfChangePwd = '" + ifchangepwd.Trim() + "'";
            if (adddatestart.Trim() != "")
                strWhere += " and CreateTime > '" + adddatestart.Trim() + "'";
            if (adddateend.Trim() != "")
                strWhere += " and CreateTime < '" + adddateend.Trim() + "'";

            int totalCount;   //输出参数
            DataTable dt = SqlPagerHelper.GetPager("tbUser", "ID,AccountName,[Password],RealName,MobilePhone,Email,IsAble,IfChangePwd,[Description],CreateTime,CreateBy,UpdateTime,UpdateBy", sort + " " + order, pagesize, pageindex, strWhere, out totalCount);
            dt.Columns.Add(new DataColumn("UserRoleId"));
            dt.Columns.Add(new DataColumn("UserRole"));
            dt.Columns.Add(new DataColumn("UserDepartmentId"));
            dt.Columns.Add(new DataColumn("UserDepartment"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataTable dtrole = DALCore.GetRoleDAL().GetRoleByUserId(Convert.ToInt32(dt.Rows[i]["ID"]));
                DataTable dtdepartment = DALCore.GetDepartmentDAL().GetDepartmentByUserId(Convert.ToInt32(dt.Rows[i]["ID"]));
                dt.Rows[i]["UserRoleId"] = JsonHelper.ColumnToJson(dtrole, 0);
                dt.Rows[i]["UserRole"] = JsonHelper.ColumnToJson(dtrole, 1);
                dt.Rows[i]["UserDepartmentId"] = JsonHelper.ColumnToJson(dtdepartment, 0);
                dt.Rows[i]["UserDepartment"] = JsonHelper.ColumnToJson(dtdepartment, 1);
            }
            #warning 待优化，通过视图关联角色，部门信息
            string strJson = JsonHelper.ToJson(dt);
            var jsonResult = new { total = totalCount.ToString(), rows = strJson };

            //string strJson = JsonHelper.ToJson(SqlPagerHelper.GetPager("tbUser", "ID,AccountName,[Password],RealName,MobilePhone,Email,IsAble,IfChangePwd,[Description],CreateTime,CreateBy,UpdateTime,UpdateBy", sort + " " + order, pagesize, pageindex, strWhere, out totalCount));
            //var jsonResult = new { total = totalCount.ToString(), rows = strJson };
            return Content("{\"total\": " + totalCount.ToString() + ",\"rows\":" + strJson + "}");
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
            try
            {
                UserEntity uInfo = ViewData["Account"] as UserEntity;
                string userid = Request["UserID"];
                string username = Request["UserName"];
                bool isable = bool.Parse(Request["Isable"]);
                bool ifchangepwd = bool.Parse(Request["IfChangepwd"]);
                string description = Request["Description"];

                UserEntity userAdd = new UserEntity();
                userAdd.AccountName = userid.Trim();
                userAdd.RealName = username.Trim();
                userAdd.Password = Md5.GetMD5String("q123456");   //md5加密
                userAdd.IsAble = isable;
                userAdd.IfChangePwd = ifchangepwd;
                userAdd.Description = description.Trim();
                userAdd.MobilePhone = Request["MobilePhone"];
                userAdd.Email = Request["Email"];
                userAdd.CreateTime = DateTime.Now;
                userAdd.CreateBy = uInfo.AccountName;
                userAdd.UpdateTime = DateTime.Now;
                userAdd.UpdateBy = uInfo.AccountName;
                int userId = DALCore.GetUserDAL().AddUser(userAdd);
                if (userId > 0)
                {
                    return Content("{\"msg\":\"添加成功！默认密码是【q123456】！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"添加失败！\",\"success\":false}");
                }
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"添加失败," + ex.Message + "\",\"success\":false}");
            }
        }

        /// <summary>
        /// 编辑页面展示
        /// </summary>
        /// <returns></returns>
        public ActionResult UserEdit()
        {
            return View();
        }

        /// <summary>
        /// 编辑 用户
        /// </summary>
        /// <returns></returns>
        public ActionResult EditUser()
        {
            try
            {
                int id = Convert.ToInt32(Request["id"]);
                string originalName = Request["originalName"];
                string userid = Request["UserID"];
                string username = Request["UserName"];
                bool isable = bool.Parse(Request["Isable"]);
                bool ifchangepwd = bool.Parse(Request["IfChangepwd"]);
                string description = Request["Description"];

                UserEntity userEdit = new UserEntity();
                userEdit.ID = id;
                userEdit.AccountName = userid.Trim();
                userEdit.RealName = username.Trim();
                userEdit.IsAble = isable;
                userEdit.IfChangePwd = ifchangepwd;
                userEdit.Description = description.Trim();
                userEdit.MobilePhone = Request["MobilePhone"];
                userEdit.Email = Request["Email"];
                userEdit.UpdateTime = DateTime.Now;
                if (userEdit.AccountName != originalName && DALCore.GetUserDAL().GetUserByUserId(userEdit.AccountName) != null)
                {
                    throw new Exception("已经存在此用户！");
                }
                if (DALCore.GetUserDAL().EditUser(userEdit))
                {
                    return Content("{\"msg\":\"修改成功！\",\"success\":true}");
                }
                else
                {
                    return Content("{\"msg\":\"修改失败！\",\"success\":true}");
                }

            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"修改失败," + ex.Message + "\",\"success\":false}");
            }
        }

        public ActionResult DelUserByIDs()
        {
            try
            {
                string Ids = Request["IDs"] == null ? "" : Request["IDs"];
                if (!string.IsNullOrEmpty(Ids))
                {
                    if (DALCore.GetUserDAL().DeleteUser(Ids))
                    {
                        return Content("{\"msg\":\"删除成功！\",\"success\":true}");
                    }
                    else
                    {
                        return Content("{\"msg\":\"删除失败！\",\"success\":false}");
                    }
                }
                else
                {
                    return Content("{\"msg\":\"删除失败！\",\"success\":false}");
                }
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"删除失败," + ex.Message + "\",\"success\":false}");
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
        /// 新增 用户角色权限
        /// </summary>
        /// <returns></returns>
        public ActionResult SetUserRole()
        {
            try
            {
                string UserIDs = Request["UserIDs"] ?? "";  //用户id，可能是多个 
                string RoleIDs = Request["RoleIDs"] ?? "";  //角色id，可能是多个

                if (UserIDs.IndexOf(",") == -1)  //单个用户分配角色
                {
                    if (UserIDs != "" && SetRoleSingle(Convert.ToInt32(UserIDs), RoleIDs))
                    {
                        return Content("{\"msg\":\"设置成功！\",\"success\":true}");
                    }
                    else
                    {
                        return Content("{\"msg\":\"设置失败！\",\"success\":true}");
                    }
                }
                else   //批量设置用户角色
                {
                    if (UserIDs != "" && SetRoleBatch(UserIDs, RoleIDs))
                    {
                        return Content("{\"msg\":\"设置成功！\",\"success\":true}");
                    }
                    else
                    {
                        return Content("{\"msg\":\"设置失败！\",\"success\":true}");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("{\"msg\":\"设置失败," + ex.Message + "\",\"success\":false}");
            }
        }
        

        /// <summary>
        /// 设置用户角色（单个用户）
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="roleIds">角色id，多个逗号隔开</param>
        bool SetRoleSingle(int userId, string roleIds)
        {
            DataTable dt_user_role_old = DALCore.GetRoleDAL().GetRoleByUserId(userId);  //用户之前拥有的角色
            List<UserRoleEntity> role_addList = new List<UserRoleEntity>();     //需要插入角色的sql语句集合
            List<UserRoleEntity> role_deleteList = new List<UserRoleEntity>();     //需要删除角色的sql语句集合

            string[] str_role = roleIds.Trim(',').Split(',');    //传过来用户勾选的角色（有去勾的也有新勾选的）

            UserRoleEntity userroledelete = null;
            UserRoleEntity userroleadd = null;
            //用户去掉勾选的角色（要删除本用户的角色）
            for (int i = 0; i < dt_user_role_old.Rows.Count; i++)
            {
                //等于-1说明用户去掉勾选了某个角色 需要删除
                if (Array.IndexOf(str_role, dt_user_role_old.Rows[i]["roleid"].ToString()) == -1)
                {
                    userroledelete = new UserRoleEntity();
                    userroledelete.RoleId = Convert.ToInt32(dt_user_role_old.Rows[i]["roleid"].ToString());
                    userroledelete.UserId = userId;
                    role_deleteList.Add(userroledelete);
                }
            }

            //用户新勾选的角色（要添加本用户的角色）
            if (!string.IsNullOrEmpty(roleIds))
            {
                for (int j = 0; j < str_role.Length; j++)
                {
                    //等于0那么原来的角色没有 是用户新勾选的
                    if (dt_user_role_old.Select("roleid = '" + str_role[j] + "'").Length == 0)
                    {
                        userroleadd = new UserRoleEntity();
                        userroleadd.UserId = userId;
                        userroleadd.RoleId = Convert.ToInt32(str_role[j]);
                        role_addList.Add(userroleadd);
                    }
                }
            }
            if (role_addList.Count == 0 && role_deleteList.Count == 0)
                return true;
            else
                return DALCore.GetUserRoleDAL().SetRoleSingle(role_addList, role_deleteList);
        }

        /// <summary>
        /// 设置用户角色（批量设置）
        /// </summary>
        /// <param name="userIds">用户主键，多个逗号隔开</param>
        /// <param name="roleIds">角色id，多个逗号隔开</param>
        bool SetRoleBatch(string userIds, string roleIds)
        {
            List<UserRoleEntity> role_addList = new List<UserRoleEntity>();     //需要插入角色的sql语句集合
            List<UserRoleEntity> role_deleteList = new List<UserRoleEntity>();     //需要删除角色的sql语句集合
            string[] str_userid = userIds.Trim(',').Split(',');
            string[] str_role = roleIds.Trim(',').Split(',');

            UserRoleEntity userroledelete = null;
            UserRoleEntity userroleadd = null;
            for (int i = 0; i < str_userid.Length; i++)
            {
                //批量设置先删除当前用户的所有角色
                userroledelete = new UserRoleEntity();
                userroledelete.UserId = Convert.ToInt32(str_userid[i]);
                role_deleteList.Add(userroledelete);

                if (!string.IsNullOrEmpty(roleIds))
                {
                    //再添加设置的角色
                    for (int j = 0; j < str_role.Length; j++)
                    {
                        userroleadd = new UserRoleEntity();
                        userroleadd.UserId = Convert.ToInt32(str_userid[i]);
                        userroleadd.RoleId = Convert.ToInt32(str_role[j]);
                        role_addList.Add(userroleadd);
                    }
                }
            }
            return DALCore.GetUserRoleDAL().SetRoleBatch(role_addList, role_deleteList);
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllRoleInfo()
        {
            string roleJson = JsonHelper.ToJson(DALCore.GetRoleDAL().GetAllRole("1=1"));
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
            DataTable dt_user_dep_old = DALCore.GetDepartmentDAL().GetDepartmentByUserId(userId);  //用户之前拥有的部门
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
                return DALCore.GetUserDepartmentDAL().SetDepartmentSingle(dep_addList, dep_deleteList);
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
            return DALCore.GetUserDepartmentDAL().SetDepartmentBatch(dep_addList, dep_deleteList);
        }
    }
}
