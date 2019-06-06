using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.IDAL;
using YMOA.Model;

namespace YMOA.DAL
{
    public class TeamDAL : BaseDal, ITeamDAL
    {
        public IEnumerable<T> QryTeam<T>(Dictionary<string, object> paras)
        {
            return QueryList<T>("P_Select_Team", paras, CommandType.StoredProcedure);
        }

        public int Save(List<TeamEntity> listModel)
        {
            int ret = 0;
            try
            {
                if (listModel != null && listModel.Count > 0)
                {
                    TeamEntity model = listModel.FirstOrDefault();
                    var ps = model.GetType().GetProperties();
                    List<string> @colms = new List<string>();
                    List<string> @params = new List<string>();

                    foreach (var p in ps)
                    {
                            @colms.Add(string.Format("[{0}]", p.Name));
                            @params.Add(string.Format("@{0}", p.Name));
                    }
                    var sql = string.Format("INSERT INTO [{0}] ({1}) VALUES({2})", "tbTeam", string.Join(", ", @colms), string.Join(", ", @params));
                    using (var _conn = GetConnection())
                    {
                        _conn.Execute("delete from tbTeam where ProjectId = @ProjectId and TaskId='0'",new { ProjectId = listModel[0].ProjectId });
                         ret = _conn.Execute(sql, listModel);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }
    }
}
