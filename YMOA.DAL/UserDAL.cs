using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using YMOA.IDAL;
using YMOA.Model;
using Dapper;
using Newtonsoft.Json;
using System.Linq;
using YMOA.Comm;

namespace YMOA.DAL
{
    /// <summary>
    /// 用户（SQL Server数据库实现）
    /// </summary>
    public class UserDAL : BaseDal,IUserDAL
    {
        /// <summary>
        /// 根据用户id获取用户
        /// </summary>
        public UserEntity GetUserByUserId(string userId)
        {
            const string sql = "select top 1 ID,AccountName,[Password],RealName,MobilePhone,Email,IsAble,IfChangePwd,[Description],CreateTime,CreateBy,UpdateTime,UpdateBy from tbUser where AccountName = @UserId";
            return QuerySingle<UserEntity>(sql, new { UserId = userId });
        }

        /// <summary>
        /// 根据id获取用户
        /// </summary>
        public UserEntity GetUserById(string id)
        {
            string sql = "select ID,AccountName,[Password],RealName,MobilePhone,Email,IsAble,IfChangePwd,[Description],CreateTime,CreateBy,UpdateTime,UpdateBy from tbUser where ID = @ID";
            return QuerySingle<UserEntity>(sql, new { ID = id });
        }

        /// <summary>
        /// 首次登陆强制修改密码
        /// </summary>
        public bool InitUserPwd(UserEntity user)
        {
            string sql = "update tbUser set [Password] = @UserPwd,IfChangePwd = @IfChangePwd where ID = @ID";
            return Execute(sql, new { UserPwd = user.Password, IfChangePwd = true, ID = user.ID }) > 0;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public bool ChangePwd(UserEntity user)
        {
            string sql = "update tbUser set Password = @UserPwd,IfChangePwd=1 where ID = @Id";
            SqlParameter[] paras = { 
                                   new SqlParameter("@UserPwd",user.Password),
                                   new SqlParameter("@Id",user.ID)
                                   };
            object obj = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, sql, paras);
            if (Convert.ToInt32(obj) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        public UserEntity UserLogin(Dictionary<string, object> paras)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("select top 1 ID, AccountName, RealName, RoleId, MobilePhone, Email, IsAble, DepartmentId, DutyId from tbUser ");
            sbSql.Append("where AccountName=@UserId and Password=@UserPwd");
        
            return QuerySingle<UserEntity>(sbSql.ToString(), paras);
            
        }

        /// <summary>
        /// 根据用户id判断用户是否可用
        /// </summary>
        public UserEntity CheckLoginByUserId(string userId)
        {
            string sql = "select top 1 ID,AccountName,[Password],RealName,MobilePhone,Email,IsAble,IfChangePwd,[Description],CreateTime,CreateBy,UpdateTime,UpdateBy from tbUser where AccountName=@UserId";
            UserEntity user = null;
            DataTable dt = SqlHelper.GetDataTable(SqlHelper.connStr, CommandType.Text, sql, new SqlParameter("@UserId", userId));
            if (dt.Rows.Count > 0)
            {
                user = new UserEntity();
                DataRowToModel(user, dt.Rows[0]);
                return user;
            }
            else
                return null;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        public int AddUser(UserEntity user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into tbUser(AccountName,[Password],RealName,MobilePhone,Email,IsAble,IfChangePwd,[Description],CreateTime,CreateBy,UpdateTime,UpdateBy)");
            strSql.Append(" values ");
            strSql.Append("(@AccountName,@Password,@RealName,@MobilePhone,@Email,@IsAble,@IfChangePwd,@Description,@CreateTime,@CreateBy,@UpdateTime,@UpdateBy)");
            strSql.Append(";SELECT @@IDENTITY");   //返回插入用户的主键
            SqlParameter[] paras = { 
                                   new SqlParameter("@AccountName",user.AccountName),
                                   new SqlParameter("@Password",user.Password),
                                   new SqlParameter("@RealName",user.RealName),
                                   new SqlParameter("@MobilePhone",user.MobilePhone),
                                   new SqlParameter("@Email",user.Email),                           
                                   new SqlParameter("@IsAble",user.IsAble),
                                   new SqlParameter("@IfChangePwd",user.IfChangePwd),
                                   new SqlParameter("@Description",user.Description),
                                   new SqlParameter("@CreateTime",user.CreateTime),
                                   new SqlParameter("@CreateBy",user.CreateBy),
                                   new SqlParameter("@UpdateTime",user.UpdateTime),
                                   new SqlParameter("@UpdateBy",user.UpdateBy)
                                   };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connStr, CommandType.Text, strSql.ToString(), paras));
        }

        /// <summary>
        /// 删除用户（可批量删除，删除用户同时删除对应的权限和所处的部门）
        /// </summary>
        public bool DeleteUser(string idList)
        {
            List<string> list = new List<string>();
            list.Add("delete from tbUser where ID in (" + idList + ")");
            list.Add("delete from tbUserRole where UserId in (" + idList + ")");

            try
            {
                int count = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, list);
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        public bool EditUser(UserEntity user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update tbUser set ");
            strSql.Append("AccountName=@AccountName,RealName=@RealName,MobilePhone=@MobilePhone,Email=@Email,IsAble=@IsAble,IfChangePwd=@IfChangePwd,Description=@Description,UpdateTime=@UpdateTime,UpdateBy=@UpdateBy");
            strSql.Append(" where ID=@Id");

            SqlParameter[] paras = { 
                                   new SqlParameter("@AccountName",user.AccountName),
                                   new SqlParameter("@RealName",user.RealName),
                                   new SqlParameter("@MobilePhone",user.MobilePhone),
                                   new SqlParameter("@Email",user.Email),                          
                                   new SqlParameter("@IsAble",user.IsAble),
                                   new SqlParameter("@IfChangePwd",user.IfChangePwd),
                                   new SqlParameter("@UpdateTime",user.UpdateTime),
                                   new SqlParameter("@Description",user.Description),
                                   new SqlParameter("@Id",user.ID),
                                   new SqlParameter("@UpdateBy",user.UpdateBy)
                                   };
            object obj = SqlHelper.ExecuteNonQuery(SqlHelper.connStr, CommandType.Text, strSql.ToString(), paras);
            if (Convert.ToInt32(obj) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取用户信息（“我的信息”）
        /// </summary>
        public DataTable GetUserInfo(int userId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select u.AccountName,u.RealName,u.CreateTime,r.RoleName from tbUser u");
            strSql.Append(" left join tbUserRole ur on u.Id = ur.UserId");
            strSql.Append(" left join tbRole r on ur.RoleId = r.Id");
            strSql.Append(" where u.ID = @userId");
            return SqlHelper.GetDataTable(SqlHelper.connStr, CommandType.Text, strSql.ToString(), new SqlParameter("@userId", userId));
        }

        /// <summary>
        /// 把DataRow行转成实体类对象
        /// </summary>
        private void DataRowToModel(UserEntity model, DataRow dr)
        {
            if (!DBNull.Value.Equals(dr["ID"]))
                model.ID = int.Parse(dr["ID"].ToString());

            if (!DBNull.Value.Equals(dr["AccountName"]))
                model.AccountName = dr["AccountName"].ToString();

            if (!DBNull.Value.Equals(dr["Password"]))
                model.Password = dr["Password"].ToString();

            if (!DBNull.Value.Equals(dr["RealName"]))
                model.RealName = dr["RealName"].ToString();

            if (!DBNull.Value.Equals(dr["MobilePhone"]))
                model.MobilePhone = dr["MobilePhone"].ToString();

            if (!DBNull.Value.Equals(dr["Email"]))
                model.Email = dr["Email"].ToString();

            if (!DBNull.Value.Equals(dr["IsAble"]))
                model.IsAble = bool.Parse(dr["IsAble"].ToString());

            if (!DBNull.Value.Equals(dr["IfChangePwd"]))
                model.IfChangePwd = bool.Parse(dr["IfChangePwd"].ToString());

            if (!DBNull.Value.Equals(dr["Description"]))
                model.Description = dr["Description"].ToString();

            if (!DBNull.Value.Equals(dr["CreateTime"]))
                model.CreateTime = Convert.ToDateTime(dr["CreateTime"]);

            if (!DBNull.Value.Equals(dr["CreateBy"]))
                model.CreateBy = dr["CreateBy"].ToString();

            if (!DBNull.Value.Equals(dr["UpdateTime"]))
                model.UpdateTime = Convert.ToDateTime(dr["UpdateTime"]);

            if (!DBNull.Value.Equals(dr["UpdateBy"]))
                model.UpdateBy = dr["UpdateBy"].ToString();


        }

        public IEnumerable<T> QryUsers<T>(Dictionary<string, object> paras, out int iCount)
        {
            iCount = 0;
            WhereBuilder builder = new WhereBuilder();
            builder.FromSql = "v_user_list";
            GridData grid = new GridData()
            {
                PageIndex = Convert.ToInt32(paras["pi"]),
                PageSize = Convert.ToInt32(paras["pageSize"]),
                SortField = paras["sort"].ToString(),
                SortDirection = paras["order"].ToString()
            };
            builder.AddWhereAndParameter(paras, "userid", "AccountName", "LIKE", "'%'+@userid+'%'");
            builder.AddWhereAndParameter(paras, "username", "RealName", "LIKE", "'%'+@username+'%'");
            builder.AddWhereAndParameter(paras, "IsAble");
            builder.AddWhereAndParameter(paras, "IfChangePwd");
            builder.AddWhereAndParameter(paras, "RoleID");
            builder.AddWhereAndParameter(paras, "adddatestart", "CreateTime", ">");
            builder.AddWhereAndParameter(paras, "adddateend", "CreateTime", "<");
            return SortAndPage<T>(builder, grid, out iCount);
        }

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <typeparam name="T">数据集</typeparam>
        /// <param name="pagination">分页信息</param>
        /// <param name="paras">查询条件参数</param>
        /// <returns></returns>
        public IEnumerable<T> QryUsers<T>(Pagination pagination, Dictionary<string, object> paras)
        {
            WhereBuilder builder = new WhereBuilder();
            builder.FromSql = "v_user_list";
            if (paras.ContainsKey("keyword"))
            {
                builder.AddWhere(" (AccountName Like '%'+@keyword+'%' OR RealName Like '%'+@keyword+'%')");
                builder.AddParameter("keyword", paras["keyword"]);
            }
            
            var rows = SortAndPage<T>(builder, pagination);
            return rows;
        }

        /// <summary>
        /// 查询用户资料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public T QryUserInfo<T>(Dictionary<string, object> paras)
        {
            return QuerySingle<T>("SELECT * FROM v_user_info WHERE ID=@ID", paras, CommandType.Text);
        }

        /// <summary>
        /// 检查账号、邮箱是否存在重复
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public int CheckUseridAndEmail(Dictionary<string, object> paras)
        {
            return QuerySingle<int>("P_User_CheckUseridAndEmail", paras, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 新增/修改用户
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public int Save(Dictionary<string, object> paras)
        {
            return StandardInsertOrUpdate("tbUser", paras);
        }

        public void GetClientData<T1,T2,T3>(int GroupId,ref List<T1> groups, ref List<T2> departments, ref List<T3> menuPermissions)
        {
            using (var connection = GetConnection())
            {
                using (var multi = connection.QueryMultiple("P_Data_Init", new { GroupId = GroupId }, null, null, CommandType.StoredProcedure))
                {
                    groups = multi.Read<T1>().ToList();
                    departments = multi.Read<T2>().ToList();
                    menuPermissions = multi.Read<T3>().ToList();
                }
            }
        }
            
        
    }
}
