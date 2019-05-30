using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Comm;
using YMOA.IDAL;

namespace YMOA.DAL
{
    public class ProjectDAL : BaseDal, IProjectDAL
    {
        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="idList">id</param>
        /// <returns></returns>
        public bool DeleteProject(string idList)
        {
            string strSql = "delete from tbUser where ID in (" + idList + ")";
            return Execute(strSql, null, CommandType.Text) > 0;
        }

        public IEnumerable<T> QryProjects<T>(Dictionary<string, object> paras, Pagination pagination)
        {
            
            WhereBuilder builder = new WhereBuilder();
            builder.FromSql = "tbProduct";
            builder.AddWhereAndParameter(paras, "Name", "Name", "LIKE", "'%'+@Name+'%'");
            builder.AddWhereAndParameter(paras, "DutyPerson", "DutyPerson", "LIKE", "'%'+@DutyPerson+'%'");
            builder.AddWhereAndParameter(paras, "CreateBy","CreateBy","LIKE","'%'+@CreateBy+'%'");
            return SortAndPage<T>(builder,pagination);
        }

        public int Save(Dictionary<string, object> paras)
        {
            DataTable dtTeam = paras["Team"] as DataTable;
            paras["Team"] = dtTeam.AsTableValuedParameter();
            return QuerySingle<int>("P_Product_Save", paras, CommandType.StoredProcedure);
        }
    }
}
