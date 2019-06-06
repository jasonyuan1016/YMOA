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



        public ISystemDAL SystemCore
        {
            get { return LoadAssamblyType<ISystemDAL>("SystemDAL"); }
        }

        public IUserDAL UserCore
        {
            get { return LoadAssamblyType<IUserDAL>("UserDAL"); }
        }
        
        public IProjectDAL ProjectCore
        {
            get { return LoadAssamblyType<IProjectDAL>("ProjectDAL"); }
        }

        public ITaskDAL TaskCore
        {
            get { return LoadAssamblyType<ITaskDAL>("TaskDAL"); }
        }
        public ITeamDAL TeamCore
        {
            get { return LoadAssamblyType<ITeamDAL>("TeamDAL"); }
        }
    }
}
