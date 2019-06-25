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
        public IEnumerable<T> QryAllUser<T>()
        {
            string strSql = "select * from v_user_list";
            using (IDbConnection dbConnection = GetConnection())
            {
                var retObj = dbConnection.Query<T>(strSql);
                return retObj;
            }
        }

        /// <summary>
        ///  查询真实姓名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<T> QryRealName<T>(Dictionary<string, object> paras)
        {
            string sql = "SELECT AccountName,RealName FROM tbUser";
            WhereBuilder builder = new WhereBuilder();
            if (paras != null)
            {
                builder.AddWhereAndParameter(paras, "names", "AccountName", "IN");
                builder.AddWhereAndParameter(paras, "DepartmentId");
            }
            return QueryList<T>(sql, builder);
        }

        /// <summary>
        ///  查询部门主管
        /// </summary>
        /// <param name="departmentId">部门编号</param>
        /// <returns></returns>
        public string GetCharge(int departmentId)
        {
            return QuerySingle<string>("P_User_GetCharge", new { DepartmentId = departmentId }, CommandType.StoredProcedure);
        }

        /// <summary>
        ///  设置部门主管(每个部门只存在一个主管)
        /// </summary>
        /// <param name="departmentId">部门编号</param>
        /// <param name="accountName">部门主管登录名</param>
        /// <returns></returns>
        public bool SetCharge(int departmentId, string accountName)
        {
            return Execute("P_User_SetCharge", new { DepartmentId = departmentId, AccountName = accountName }, CommandType.StoredProcedure) > 0;
        }



    }
}
