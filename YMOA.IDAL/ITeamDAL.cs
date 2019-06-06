using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Model;

namespace YMOA.IDAL
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/06/04
    /// 团队接口
    /// </summary>
    public interface ITeamDAL
    {
        IEnumerable<T> QryTeam<T>(Dictionary<string, object> paras);
        int Save(List<TeamEntity>listModels);
    }
}
