using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using YMOA.IDAL;
using YMOA.Model;

namespace YMOA.DAL
{
    /// <summary>
    /// 系统设置    
    /// </summary>
    public class SystemDAL : BaseDal,ISystemDAL
    {
        #region 选单权限相关
        /// <summary>
        /// 获得对应角色的选单权限
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> RoleMenuGetListByRoleId<T>(Dictionary<string, object> paras)
        {
            return QueryList<T>("P_RoleMenu_GetListByRoleId", paras, CommandType.StoredProcedure, true);
        }

        /// <summary>
        /// 获得权限
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> RoleGetList<T>(Dictionary<string, object> paras)
        {
            string sql = "SELECT * FROM tbRole WHERE 1=1";
            if (paras != null && paras.ContainsKey("id"))
            {
                sql += " AND id=@id";
            }
            return QueryList<T>(sql, paras);
        }

        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public int RoleSave(Dictionary<string, object> paras)
        {
            DataTable dtRolememu = paras["rolemenu"] as DataTable;
            paras["rolemenu"] = dtRolememu.AsTableValuedParameter();
            return QuerySingle<int>("P_Role_Save", paras, CommandType.StoredProcedure);
        }

        /// <summary>
        ///  根据Id删除角色
        /// </summary>
        /// <param name="paras"></param>
        /// <returns>0成功 1角色不存在 2角色被用户引用</returns>
        public int RoleDelete(Dictionary<string, object> paras)
        {
            return QuerySingle<int>("P_Role_Delete", paras, CommandType.StoredProcedure);
        }

        #endregion

        /// <summary>
        /// 系统数据加载
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="RoleId"></param>
        /// <param name="groups"></param>
        /// <param name="departments"></param>
        /// <param name="menuPermissions"></param>
        public void SystemDataInit<T1, T2, T3>(int RoleId, ref List<T1> groups, ref List<T2> departments, ref List<T3> menuPermissions)
        {
            using (var connection = GetConnection())
            {
                using (var multi = connection.QueryMultiple("P_SystemData_Init", new { id = RoleId }, null, null, CommandType.StoredProcedure))
                {
                    groups = multi.Read<T1>().ToList();
                    departments = multi.Read<T2>().ToList();
                    menuPermissions = multi.Read<T3>().ToList();
                }
            }
        }

        /// <summary>
        /// 新增/修改 公用数据类型
        /// </summary>
        /// <param name="libraryEntity"></param>
        /// <returns></returns>
        public int LibrarySave(LibraryEntity libraryEntity)
        {
            return QuerySingle<int>("P_Library_Save", libraryEntity, CommandType.StoredProcedure);
        }

        #region 选单管理
        
        /// <summary>
        /// 新增/修改 菜单
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public int MemuSave(Dictionary<string, object> paras)
        {
            return StandardInsertOrUpdate("tbMenu", paras);
        }

        /// <summary>
        ///  删除菜单 (可批量删除)
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public bool DeleteMemu(string idList)
        {
            return Execute("delete from tbRoleMenu where m_id in (" + idList + ")"+" delete from tbMenu where id in (" + idList + ")", null, CommandType.Text) > 0;
        }

        /// <summary>
        ///  查询选单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        public IEnumerable<T> MenuGetList<T>(Dictionary<string, object> paras)
        {
            string sql = "SELECT * FROM tbMenu";
            if (paras != null)
            {
                WhereBuilder builder = new WhereBuilder();
                builder.AddWhereAndParameter(paras, "id");
                builder.AddWhereAndParameter(paras, "noState","state","!=");
                if (builder.Wheres.Count > 0)
                {
                    sql += " WHERE " + String.Join(" and ", builder.Wheres);
                }
            }
            sql += " ORDER BY sortvalue DESC";

            return QueryList<T>(sql, paras);
        }

        #endregion



    }
}
