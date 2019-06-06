using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Model;

namespace YMOA.IDAL
{
    public interface ITeamDAL
    {
        IEnumerable<T> QryTeam<T>(Dictionary<string, object> paras);
        int Save(List<TeamEntity>listModels);
    }
}
