using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Comm;
using YMOA.IDAL;

namespace YMOA.DAL
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/06/25
    /// 报销单实现
    /// </summary>
    public class ReimbursementDAL : BaseDal, IReimbursementDAL
    {
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
