using System;
using System.Data;
using System.Collections.Generic;
using YMOA.Model;

namespace YMOA.IDAL
{
    /// <summary>
    /// 系统设置
    /// </summary>
    public interface ISystemDAL
    {
        #region 选单权限相关
        /// <summary>
        /// 获得对应角色的选单权限
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        IEnumerable<T> RoleMenuGetListByRoleId<T>(Dictionary<string, object> paras);

        /// <summary>
        /// 获得角色
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> RoleGetList<T>(Dictionary<string, object> paras);

        /// <summary>
        /// 获得选单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        IEnumerable<T> MenuGetList<T>(Dictionary<string, object> paras);

        int RoleSave(Dictionary<string, object> paras);
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
        void SystemDataInit<T1, T2, T3>(int RoleId, ref List<T1> groups, ref List<T2> departments, ref List<T3> menuPermissions);

        /// <summary>
        /// 新增/修改 公用数据类型
        /// </summary>
        /// <param name="libraryEntity"></param>
        /// <returns></returns>
        int LibrarySave(LibraryEntity libraryEntity);
    }
}
