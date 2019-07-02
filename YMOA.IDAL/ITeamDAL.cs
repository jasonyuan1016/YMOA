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
        /// <summary>
        /// 查询成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        IEnumerable<T> QryTeam<T>(Dictionary<string, object> paras);

        /// <summary>
        /// 批量添加成员
        /// </summary>
        /// <param name="listModels"></param>
        /// <returns></returns>
        int Save(List<TeamEntity>listModels);
        
        /// <summary>
        ///  查询成员
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetTeams<T>(Dictionary<string, object> paras);

        /// <summary>
        ///  多任务查询成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        IEnumerable<T> GetTeams<T>(string tasks);
    }
}
