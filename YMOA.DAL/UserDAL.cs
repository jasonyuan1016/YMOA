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
        /// 用户登录
        /// </summary>
        public UserEntity UserLogin(Dictionary<string, object> paras)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("select top 1 ID, AccountName, RealName, RoleId, MobilePhone, Email, IsAble, DepartmentId, DutyId from tbUser ");
            sbSql.Append("where AccountName=@UserId and Password=@UserPwd");
        
            return QuerySingle<UserEntity>(sbSql.ToString(), paras);
            
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
            return StandardInsertOrUpdate("tbUser", paras,"ID", true, OperateType.User);
        }

        /// <summary>
        /// 仅删除用户(可批量删除)
        /// </summary>
        /// <param name="idList">需要删除的id集</param>
        /// <returns></returns>
        public bool OnlyDeleteUser(string idList)
        {
            return Execute("delete from tbUser where ID in (" + idList + ")", null, CommandType.Text) > 0;
        }
    }
}
