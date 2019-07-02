using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Comm;

namespace YMOA.IDAL
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/06/25
    /// 报销单接口
    /// </summary>
    public interface IReimbursementDAL
    {
        /// <summary>
        /// 保存/修改报销单
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        int Save(Dictionary<string, object> paras);

        /// <summary>
        /// 查询未处理的报销单
        /// </summary>
        /// <param name="dp">参数</param>
        /// <returns></returns>
        string QryUntreated(DynamicParameters dp);

        /// <summary>
        /// 按条件查询所有的报销单
        /// </summary>
        /// <typeparam name="T">传入类型，返回类型</typeparam>
        /// <param name="dp">条件</param>
        /// <param name="pagination">分页信息</param>
        /// <returns></returns>
        IEnumerable<T> QryAll<T>(DynamicParameters dp, Pagination pagination);

        /// <summary>
        /// 通过ID查询报销单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        T QryReimbursement<T>(string ID);
    }
}
