using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using YMOA.Comm;
using YMOA.IDAL;
using YMOA.Model;

namespace YMOA.DAL
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/06/25
    /// 报销单实现
    /// </summary>
    public class ReimbursementDAL : BaseDal, IReimbursementDAL
    {
        /// <summary>
        /// 查询未处理的报销单
        /// </summary>
        /// <param name="dp">参数</param>
        /// <returns></returns>
        public string QryUntreated(DynamicParameters dp)
        {
            using(IDbConnection conn = GetConnection())
            {
                dp.Add("Count", null, DbType.Int32, ParameterDirection.Output);
                var objRet = conn.Query<ReimbursementEntity>("P_Select_Reimbursement", dp, null, true, null, CommandType.StoredProcedure);
                int Count = dp.Get<int>("Count");
                return JsonConvert.SerializeObject(new { rows = objRet, Count = Count });
            }
        }

        /// <summary>
        /// 按条件查询所有的报销单
        /// </summary>
        /// <typeparam name="T">传入类型，返回类型</typeparam>
        /// <param name="dp">条件</param>
        /// <param name="pagination">分页信息</param>
        /// <returns></returns>
        public IEnumerable<T> QryAll<T>(DynamicParameters dp,Pagination pagination)
        {
            using(IDbConnection conn = GetConnection())
            {
                dp.Add("Count", null, DbType.Int32, ParameterDirection.Output);
                var objRet = conn.Query<T>("P_Select_AllReimbursement", dp, null, true, null, CommandType.StoredProcedure);
                pagination.records = dp.Get<int>("Count");
                return objRet;
            }
        }

        /// <summary>
        /// 通过ID查询报销单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        public T QryReimbursement <T>(string ID)
        {
            DynamicParameters dp = new DynamicParameters();
            using (IDbConnection conn = GetConnection())
            {
                dp.Add("ID", ID);
                dp.Add("Count", null, DbType.Int32, ParameterDirection.Output);
                var objRet = conn.QuerySingle<T>("P_Select_AllReimbursement", dp, null, null, CommandType.StoredProcedure);
                return objRet;
            }
        }
        
        /// <summary>
        /// 保存/修改报销单
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        public int Save(Dictionary<string, object> paras)
        {
            if (paras.ContainsKey("ID"))
            {
                return StandardUpdate("tbReimbursement", paras);
            }
            else
            {
                paras["ID"] = Guid.NewGuid().To16String();
                return StandardInsert("tbReimbursement", paras,"");
            }
        }
    }
}
