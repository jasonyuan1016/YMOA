using System.Configuration;

namespace MongoDBHelper
{
    internal static class DbConfigParams
    {
        private static string _conntionString = ConfigurationManager.AppSettings["MongoDBConn"];

        /// <summary>
        /// 获取 数据库连接串
        /// </summary>
        public static string ConntionString
        {
            get { return _conntionString; }
        }

        private static string _dbName = ConfigurationManager.AppSettings["MongoDBName"];

        /// <summary>
        /// 获取 数据库名称
        /// </summary>
        public static string DbName
        {
            get { return _dbName; }
        }

        /// <summary>
        /// 获取 数据库连接串+数据库名称
        /// </summary>
        public static string conString
        {
            get { return _conntionString + "/" + _dbName; }
        }

    }
}
