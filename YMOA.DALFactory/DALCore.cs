using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.IDAL;

namespace YMOA.DALFactory
{
    /// <summary>
    /// 工厂类：创建访问数据库的实例对象
    /// </summary>
    public class DALCore
    {
        private static DALCore singleInstance;
        #region public static DALCore LoadAssamblyType<T>()
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DALCore GetInstance()
        {
            if (singleInstance == null)
            {
                singleInstance = new DALCore();
            }
            return singleInstance;
        }
        #endregion

        #region internal static T LoadAssamblyType<T>(string _type)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullType"></param>
        /// <returns></returns>
        internal static T LoadAssamblyType<T>(string _type) where T : class
        {
            string configName = System.Configuration.ConfigurationManager.AppSettings["DataAccess"];
            if (string.IsNullOrEmpty(configName))
            {
                throw new InvalidOperationException();    //抛错，代码不会向下执行了
            }
            return LoadAssamblyType<T>(configName, _type);

        }
        #endregion

        #region internal static T LoadAssamblyType<T>(string fullType)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName"></param>
        /// <param name="_type"></param>
        /// <returns></returns>
        internal static T LoadAssamblyType<T>(string assemblyName, string _type) where T : class
        {

            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(assemblyName);

            if (assembly != null)
            {
                Type loadType = assembly.GetType(assemblyName + "." + _type);

                if (loadType != null)
                {
                    return (T)Activator.CreateInstance(loadType);
                }
            }

            return default(T);
        }
        #endregion


        public IAuthorityDAL Authority
        {
            get {  return LoadAssamblyType<IAuthorityDAL>("AuthorityDAL");}
        }

        public IMenuDAL Menu
        {
            get { return LoadAssamblyType<IMenuDAL>("MenuDAL"); }
        }


        public IRoleDAL Role
        {
            get { return LoadAssamblyType<IRoleDAL>("RoleDAL"); }
        }

        public IUserDAL User
        {
            get { return LoadAssamblyType<IUserDAL>("UserDAL"); }
        }

        public IUserRoleDAL UserRole
        {
            get { return LoadAssamblyType<IUserRoleDAL>("UserRoleDAL"); }
        }

        public IRequestionTypeDAL RequestionType
        {
            get { return LoadAssamblyType<IRequestionTypeDAL>("RequestionTypeDAL"); }
        }

        public IRequestionDAL Requestion
        {
            get { return LoadAssamblyType<IRequestionDAL>("RequestionDAL"); }
        }

        public IButtonDAL Button
        {
            get { return LoadAssamblyType<IButtonDAL>("ButtonDAL"); }
        }

        public IMenuButtonDAL MenuButton
        {
            get { return LoadAssamblyType<IMenuButtonDAL>("MenuButtonDAL"); }
        }

        public IRoleMenuButtonDAL RoleMenuButton
        {
            get { return LoadAssamblyType<IRoleMenuButtonDAL>("RoleMenuButtonDAL"); }
        }

        public IDepartmentDAL Department
        {
            get
            { return LoadAssamblyType<IDepartmentDAL>("DepartmentDAL"); }
        }

        public IUserDepartmentDAL UserDepartment
        {
            get
            { return LoadAssamblyType<IUserDepartmentDAL>("UserDepartmentDAL"); }
        }

        public INewsTypeDAL NewsType
        {
            get
            { return LoadAssamblyType<INewsTypeDAL>("NewsTypeDAL"); }
        }

        public INewsDAL News
        {
            get
            { return LoadAssamblyType<INewsDAL>("NewsDAL"); }
        }

        public IHtmlTypeDAL HtmlType
        {
            get
            { return LoadAssamblyType<IHtmlTypeDAL>("HtmlTypeDAL"); }
        }

        public IDataTypeDAL DataType
        {
            get
            { return LoadAssamblyType<IDataTypeDAL>("DataTypeDAL"); }
        }

        public ITableDAL Table
        {
            get
            { return LoadAssamblyType<ITableDAL>("TableDAL"); }
        }
        public IFieldsDAL Fields
        {
            get
            { return LoadAssamblyType<IFieldsDAL>("FieldsDAL"); }
        }

        public IIconsDAL Icons
        {
            get
            { return LoadAssamblyType<IIconsDAL>("IconsDAL"); }
        }

        public ILoginIpLogDAL LoginIpLog
        {
            get
            { return LoadAssamblyType<ILoginIpLogDAL>("LoginIpLogDAL"); }
        }
    }
}
