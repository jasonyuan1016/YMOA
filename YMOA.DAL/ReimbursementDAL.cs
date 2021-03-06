﻿using System;
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
        /// 查询所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pairs"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public IEnumerable<T> QryRei<T>(Dictionary<string,object>pairs,Pagination pagination)
        {
            WhereBuilder builder = new WhereBuilder();
            builder.FromSql = "tbReimbursement";
            builder.AddWhereAndParameter(pairs, "ID");
            builder.AddWhereAndParameter(pairs, "Department");
            builder.AddWhereAndParameter(pairs, "Applicant");
            return SortAndPage<T>(builder, pagination);
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

        /// <summary>
        /// 删除当前报销单
        /// </summary>
        /// <param name="ID">报销ID</param>
        /// <returns></returns>
        public int Delete(string ID)
        {
            string strSql = "delete from tbReimbursement where ID ='"+ID+"'";
            return Execute(strSql);
        }
    }
}
