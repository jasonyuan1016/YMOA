using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
