using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using YMOA.IDAL;
using YMOA.Model;

namespace YMOA.DAL
{
    /// <summary>
    /// 角色
    /// </summary>
    public class RoleDAL : BaseDal,IRoleDAL
    {
        /// <summary>
        /// 获得对应角色的选单权限
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> GetMenuPermissionByGroupID<T>(Dictionary<string,object> paras)
        {
            return QueryList<T>("P_Menu_GetMemuByGroup", paras, CommandType.StoredProcedure, true);
        }

        public IEnumerable<T> GetRoles<T>(Dictionary<string, object> paras)
        {
            string sql = "SELECT * FROM tbGroup WHERE 1=1";
            if (paras!=null && paras.ContainsKey("GroupId"))
            {
                sql += " AND id=@GroupId";
            }
            return QueryList<T>(sql, paras);
        }

        public IEnumerable<T> GetMenus<T>(Dictionary<string, object> paras)
        {
            string sql = "SELECT * FROM tbMemu WHERE 1=1";
            if (paras != null && paras.ContainsKey("id"))
            {
                sql += " AND id=@id";
            }
            sql += " ORDER BY sortvalue DESC";
            return QueryList<T>(sql, paras);
        }

        public int Save(Dictionary<string, object> paras)
        {
            DataTable dtRolememu = paras["rolemenu"] as DataTable;
            paras["rolemenu"] = dtRolememu.AsTableValuedParameter();
            return QuerySingle<int>("P_Role_Save", paras, CommandType.StoredProcedure);
        }
    }
}
