using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Comm;
using YMOA.IDAL;

namespace YMOA.DAL
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/05/29
    /// 项目实现
    public class ProjectDAL : BaseDal, IProjectDAL
    {
        /// <summary>
        /// 删除项目，任务以及团队
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public bool DeleteProject(Dictionary<string,object>paras)
        {
            return Execute("P_Delete_ProjectANDTaskANDTeam", paras, CommandType.StoredProcedure) > 0;
        }

        /// <summary>
        /// 查询单个项目详细信息
        /// </summary>
        /// <typeparam name="T">传入类型，返回类型</typeparam>
        /// <param name="dp">查询条件</param>
        /// <returns></returns>
        public T QryProjectInfo<T>(DynamicParameters dp)
        {
            using (IDbConnection conn = GetConnection())
            {
                dp.Add("Count", null, DbType.Int32, ParameterDirection.Output);
                var objRet = conn.QuerySingleOrDefault<T>("P_Select_Project", dp, null, null, CommandType.StoredProcedure);
                return objRet;
            }
        }
        
        /// <summary>
        /// 查询项目基本信息
        /// </summary>
        /// <typeparam name="T">传入类型，返回类型</typeparam>
        /// <param name="dp">查询条件</param>
        /// <param name="pagination">分页条件</param>
        /// <returns></returns>
        public IEnumerable<T> QryProjects<T>(DynamicParameters dp, Pagination pagination)
        {
            using (IDbConnection conn = GetConnection())
            {
                dp.Add("Count", null, DbType.Int32, ParameterDirection.Output);
                var objRet = conn.Query<T>("P_Select_Project", dp, null, true, null, CommandType.StoredProcedure);
                pagination.records = dp.Get<int>("Count");
                return objRet;
            }
        }
        
        /// <summary>
        /// 修改/保存项目基本信息
        /// </summary>
        /// <param name="paras">修改/保存内容</param>
        /// <returns></returns>
        public int Save(Dictionary<string, object> paras)
        {
            if (paras.ContainsKey("Team"))
            {
                DataTable dtTeam = paras["Team"] as DataTable;
                paras["Team"] = dtTeam.AsTableValuedParameter();
            }
            return QuerySingle<int>("P_Product_Save", paras, CommandType.StoredProcedure);
        }

        /// <summary>
        ///  查询用户可添加项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> QryInsertTask<T>(Dictionary<string, object> paras)
        {
            return QueryList<T>("P_Product_UserAdd", paras, CommandType.StoredProcedure);
        }
    }
}
