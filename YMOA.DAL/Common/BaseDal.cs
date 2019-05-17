﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using YMOA.Comm;
using YMOA.Model;
using YMOA.MongoDB;

namespace YMOA.DAL
{
    public class BaseDal
    {
        private static int CommandTimeout
        {
            get
            {
                int timeoutSecond = 0;

                if (int.TryParse(ConfigurationManager.AppSettings["CommandTimeout"], out timeoutSecond))
                {
                    return timeoutSecond;
                }

                return 30;
            } 
        }

        /// <summary>
        /// MongoDB表名
        /// </summary>
        private const string MongoDBCollection = "DBLog";

        protected string ConnString { get; set; } = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected IDbConnection GetConnection()
        {
            return new SqlConnection(ConnString);
        }

        protected T QuerySingle<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (IDbConnection conn = GetConnection())
            {
                var q = conn.Query<T>(sql, param, null, true, CommandTimeout, commandType);
                if (q != null)
                {
                    return q.FirstOrDefault();
                }
                return default(T);
            }
        }

        /// <summary>
        /// 列表查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="buffered">是否将查询结果存放到内存中</param>
        /// <returns></returns>
        protected IEnumerable<T> QueryList<T>(string sql, object param = null, CommandType commandType = CommandType.Text, bool buffered = true)
        {
            using (IDbConnection conn = GetConnection())
            {
                return conn.Query<T>(sql, param, null, buffered, CommandTimeout, commandType);
            }
        }

        protected int Execute(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (IDbConnection conn = GetConnection())
            {
                return conn.Execute(sql, param, null, CommandTimeout, commandType);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="builder"></param>
        /// <param name="grid"></param>
        /// <param name="iCount"></param>
        /// <returns></returns>
        protected IEnumerable<object> SortAndPage(WhereBuilder builder, GridData grid, out int iCount)
        {
            iCount = 0;
            var sql = "";
            var countSql = "";
            FormartSqlToSortAndPage(grid, ref sql, ref countSql, ref builder);
            using (IDbConnection dbConnection = GetConnection())
            {
                var retObj = dbConnection.Query(sql, builder.Parameters);
                iCount = dbConnection.QuerySingleOrDefault<int>(countSql, builder.Parameters);
                return retObj;
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="builder"></param>
        /// <param name="grid"></param>
        /// <param name="iCount"></param>
        /// <returns></returns>
        protected IEnumerable<T> SortAndPage<T>(WhereBuilder builder, GridData grid, out int iCount)
        {
            iCount = 0;
            var sql = "";
            var countSql = "";
            FormartSqlToSortAndPage(grid, ref sql, ref countSql, ref builder);
            using (IDbConnection dbConnection = GetConnection())
            {
                var retObj = dbConnection.Query<T>(sql, builder.Parameters);
                iCount = dbConnection.QuerySingleOrDefault<int>(countSql, builder.Parameters);
                return retObj;
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="builder"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        protected IEnumerable<T> SortAndPage<T>(WhereBuilder builder, Pagination pagination)
        {
            var sql = "";
            var countSql = "";
            FormartSqlToSortAndPage(pagination, ref sql, ref countSql, ref builder);
            using (IDbConnection dbConnection = GetConnection())
            {
                var retObj = dbConnection.Query<T>(sql, builder.Parameters);
                pagination.records = dbConnection.QuerySingleOrDefault<int>(countSql, builder.Parameters);
                return retObj;
            }
        }


        /// <summary>
        /// 执行标准单表Insert&Update操作
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tabName">表名</param>
        /// <param name="paras">参数</param>
        /// <param name="keyFild">主键字段</param>
        /// <returns></returns>
        protected int StandardInsertOrUpdate(string tabName, Dictionary<string, object> paras, string keyFild = "ID", bool needLog = false, OperateType operateType = OperateType.None)
        {
            var fields = GetFieldsFromDictionary(paras, keyFild);
            var sql = "";
            if (paras[keyFild].ToString().Equals("0"))
            {
                var fieldsSql1 = String.Join(",", fields);
                var fieldsSql2 = String.Join(",", fields.Select(field => "@" + field));
                sql = String.Format("INSERT {0} ({1}) VALUES ({2});", tabName, fieldsSql1, fieldsSql2);
            }
            else
            {
                var fieldsSql = String.Join(",", fields.Select(field => field + " = @" + field));
                sql = String.Format("UPDATE {0} SET {1} WHERE {2} = @{2}", tabName, fieldsSql, keyFild);
            }

            using (IDbConnection dbConnection = GetConnection())
            {
                if (needLog)
                {
                    DBLogEntity entity = new DBLogEntity();
                    entity.tabName = tabName;
                    entity.tId = paras[keyFild].ToString();
                    entity.lType = (int)operateType;
                    entity.sql = sql;
                    entity.paras = paras.ToJson();
                    return InvokeMethodWithDB<int>(() =>
                    {
                        int i = dbConnection.Execute(sql, paras);
                        if (entity.tId.Equals("0"))
                        {
                            //获得刚新增的ID
                            entity.tId = dbConnection.QueryFirst<string>(string.Format("SELECT MAX({0}) FROM {1}", keyFild, tabName));
                        }
                        return i;
                    }, entity);
                }
                else
                {
                    return dbConnection.Execute(sql, paras);
                }
            }
        }

       

        private string[] GetFieldsFromDictionary(Dictionary<string, object> keyValues, string keyFild = "")
        {
            var result = new List<string>();
            foreach (var entry in keyValues)
            {
                if (entry.Key != keyFild)
                {
                    result.Add(entry.Key);
                }
            }
            return result.ToArray();
        }

        void FormartSqlToSortAndPage(GridData grid, ref string sql, ref string countSql, ref WhereBuilder builder)
        {
            sql = "";
            countSql = "";
            if (!sql.StartsWith("select", StringComparison.CurrentCultureIgnoreCase))
            {
                //sql = string.Format("SELECT * FROM(SELECT row=ROW_NUMBER() OVER(ORDER BY {0} {1}),* FROM {2}", grid.SortField, grid.SortDirection, builder.FromSql); //
                sql = "select * from " + builder.FromSql;                       //仅支持SQL Server 2012及以上
                countSql = "select count(*) from " + builder.FromSql;
            }
            if (builder.Wheres.Count > 0)
            {
                string strWhere = " where " + String.Join(" and ", builder.Wheres);
                sql += strWhere;
                countSql += strWhere;
            }
            //sql += ") AS A WHERE row between @PageStartIndex AND @PageSize";
            #region 仅支持SQL Server 2012及以上
            sql += " order by " + grid.SortField + " " + grid.SortDirection;
            sql += " OFFSET @PageStartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
            builder.Parameters.Add("PageSize", grid.PageSize);
            builder.Parameters.Add("PageStartIndex", grid.PageSize * (grid.PageIndex - 1));
            #endregion
            //builder.Parameters.Add("PageSize", grid.PageSize * grid.PageIndex);
            //builder.Parameters.Add("PageStartIndex", grid.PageSize * (grid.PageIndex - 1) + 1);
        }

        void FormartSqlToSortAndPage(Pagination pagination, ref string sql, ref string countSql, ref WhereBuilder builder)
        {
            sql = "";
            countSql = "";
            if (!sql.StartsWith("select", StringComparison.CurrentCultureIgnoreCase))
            {
                //sql = string.Format("SELECT * FROM(SELECT row=ROW_NUMBER() OVER(ORDER BY {0} {1}),* FROM {2}", grid.SortField, grid.SortDirection, builder.FromSql); //
                sql = "select * from " + builder.FromSql;                       //仅支持SQL Server 2012及以上
                countSql = "select count(*) from " + builder.FromSql;
            }
            if (builder.Wheres.Count > 0)
            {
                string strWhere = " where " + String.Join(" and ", builder.Wheres);
                sql += strWhere;
                countSql += strWhere;
            }
            //sql += ") AS A WHERE row between @PageStartIndex AND @PageSize";
            #region 仅支持SQL Server 2012及以上
            sql += " order by " + pagination.sidx + " " + pagination.sord;
            sql += " OFFSET @PageStartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
            builder.Parameters.Add("PageSize", pagination.rows);
            builder.Parameters.Add("PageStartIndex", pagination.rows * (pagination.page - 1));
            #endregion
            //builder.Parameters.Add("PageSize", grid.PageSize * grid.PageIndex);
            //builder.Parameters.Add("PageStartIndex", grid.PageSize * (grid.PageIndex - 1) + 1);
        }

        /// <summary>
        /// 执行DB操作
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="p">当前执行方法</param>
        /// <param name="entity">数据库操作日志</param>
        /// <returns></returns>
        private T InvokeMethodWithDB<T>(Func<T> p, DBLogEntity entity)
        {
            T retObj = default(T);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            retObj = p();
            sw.Stop();
            entity.ms = sw.ElapsedMilliseconds;
            entity.uId = HttpContext.Current.Session["UserId"].ToString();
            entity.ctime = DateTime.Now;
            Task.Factory.StartNew(() =>
            {
                new MongoDbService().Add<DBLogEntity>(ConfigurationManager.AppSettings["MongoDb_Name"], MongoDBCollection, entity);
            });
            return retObj;
        }
    }
}
