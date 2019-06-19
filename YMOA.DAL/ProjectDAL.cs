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

        public T QryProjectInfo<T>(DynamicParameters dp)
        {
            using (IDbConnection conn = GetConnection())
            {
                dp.Add("Count", null, DbType.Int32, ParameterDirection.Output);
                var objRet = conn.QuerySingleOrDefault<T>("P_Select_Project", dp, null, null, CommandType.StoredProcedure);
                return objRet;
            }
        }

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

        public int Save(Dictionary<string, object> paras)
        {
            if (paras.ContainsKey("Team"))
            {
                DataTable dtTeam = paras["Team"] as DataTable;
                paras["Team"] = dtTeam.AsTableValuedParameter();
            }
            return QuerySingle<int>("P_Product_Save", paras, CommandType.StoredProcedure);
        }
    }
}
