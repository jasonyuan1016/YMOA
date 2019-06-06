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
    /// 创建时间：2019/05/29
    /// 项目接口
    /// </summary>
    public interface IProjectDAL
    {
        IEnumerable<T> QryProjects<T>(DynamicParameters dp, Pagination pagination);
        int Save(Dictionary<string, object> paras);
        bool DeleteProject(Dictionary<string,object>paras);
        T QryProjectInfo<T>(DynamicParameters dp);
    }
}
