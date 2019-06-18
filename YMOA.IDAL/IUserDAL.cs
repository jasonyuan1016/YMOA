using System.Collections.Generic;
using System.Data;
using YMOA.Comm;
using YMOA.Model;

namespace YMOA.IDAL
{
    /// <summary>
    /// 用户接口
    /// </summary>
    public interface IUserDAL
    {
        /// <summary>
        /// 根据用户id获取用户
        /// </summary>
        UserEntity GetUserByUserId(string userId);

        /// <summary>
        /// 根据id获取用户
        /// </summary>
        UserEntity GetUserById(string id);

        /// <summary>
        /// 首次登陆强制修改密码
        /// </summary>
        bool InitUserPwd(UserEntity user);


        /// <summary>
        /// 用户登录
        /// </summary>
        UserEntity UserLogin(Dictionary<string, object> paras);




        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <param name="iCount"></param>
        /// <returns></returns>
        IEnumerable<T> QryUsers<T>(Dictionary<string, object> paras, out int iCount);

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <typeparam name="T">数据集</typeparam>
        /// <param name="pagination">分页信息</param>
        /// <param name="paras">查询条件参数</param>
        /// <returns></returns>
        IEnumerable<T> QryUsers<T>(Pagination pagination, Dictionary<string, object> paras);
        IEnumerable<T> QryAllUser<T>();
        /// <summary>
        /// 查询用户资料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paras"></param>
        /// <returns></returns>
        T QryUserInfo<T>(Dictionary<string, object> paras);

        /// <summary>
        /// 检查账号、邮箱是否存在重复
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        int CheckUseridAndEmail(Dictionary<string, object> paras);

        /// <summary>
        /// 新增/修改用户
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        int Save(Dictionary<string, object> paras);
        /// <summary>
        /// 仅删除用户(可批量删除)
        /// </summary>
        /// <param name="idList">需要删除的id集</param>
        /// <returns></returns>
        bool OnlyDeleteUser(string idList);

        /// <summary>
        ///  查询真实姓名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<T> QryRealName<T>(Dictionary<string, object> paras);
    }
}
