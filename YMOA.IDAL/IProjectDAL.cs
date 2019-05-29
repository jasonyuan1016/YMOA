using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Comm;

namespace YMOA.IDAL
{
    public interface IProjectDAL
    {
        IEnumerable<T> QryProjects<T>(Dictionary<string, object> paras, Pagination pagination);
        int Save(Dictionary<string, object> paras);
        bool DeleteProject(string idList);
    }
}
