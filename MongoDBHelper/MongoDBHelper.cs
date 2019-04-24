using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq.Expressions;

namespace MongoDBHelper
{
    /// <summary>
    /// MongoDb 数据库操作类
    /// </summary>
    public class MongoDBHelper<T> where T : BaseEntity
    {
        /// <summary>
        /// 数据库对象
        /// </summary>
        private IMongoDatabase database;
        /// <summary>
        /// 连接字符串
        /// </summary>
        static string _conString=DbConfigParams.conString;
        static string _contionString = DbConfigParams.ConntionString;
        /// <summary>y
        /// 构造函数
        /// </summary>
        /// <param name="conString">连接字符串</param>
        public MongoDBHelper()
        {
            var url = new MongoUrl(_conString);
            var client = new MongoClient(DbConfigParams.ConntionString);
            this.database = client.GetDatabase(url.DatabaseName);
        }
        /// <summary>
        /// 创建集合对象
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        ///<returns>集合对象</returns>
        private IMongoCollection<T> getCollection(String collectionName)
        {
            return database.GetCollection<T>(collectionName);
        }

        #region 新增
        /// <summary>
        /// 插入一个对象
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>  
        /// <param name="collectionName">集合名称</param>
        /// <param name="t">插入的对象</param>
        public void insertOne(String collectionName, T t)
        {
            var coll = getCollection(collectionName);
            coll.InsertOne(t);
        }
        /// <summary>
        /// 插入多个数据
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="ts">要插入的对象集合</param>
        public void insertMany(String collectionName, IEnumerable<T> ts)
        {
            var coll = getCollection(collectionName);
            coll.InsertMany(ts);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 按BsonDocument条件删除
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="document">文档</param>
        /// <returns></returns>
        public Int64 deleteByQuery(String collectionName, QueryDocument document)
        {
            var coll = getCollection(collectionName);
            var result = coll.DeleteMany(document);
            return result.DeletedCount;
        }

        /// <summary>
        /// 按json字符串删除
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public Int64 deleteByJson(String collectionName, String json)
        {
            var coll = getCollection(collectionName);
            var result = coll.DeleteMany(json);
            return result.DeletedCount;
        }      

        /// <summary>
        /// 按检索条件删除
        /// 建议用Builders<T>构建复杂的查询条件
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public Int64 deleteByFilter(String collectionName, FilterDefinition<T> filter)
        {
            var coll = getCollection(collectionName);
            var result = coll.DeleteMany(filter);
            return result.DeletedCount;
        }       
        #endregion

        #region 修改
        /// <summary>
        /// 修改文档
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">修改条件</param>
        /// <param name="update">修改结果</param>
        /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
        /// <returns></returns>
        public Int64 update(String collectionName, Expression<Func<T, Boolean>> filter, UpdateDefinition<T> update, Boolean upsert = false)
        {
            var coll = getCollection(collectionName);
            var result = coll.UpdateMany(filter, update, new UpdateOptions { IsUpsert = upsert });
            return result.ModifiedCount;
        }
        /// <summary>
        /// 用新对象替换-更新单个
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">修改条件</param>
        /// <param name="t">新对象</param>
        /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则true插入新文档）</param>
        /// <returns>修改影响文档数</returns>
        public Int64 updateOne(String collectionName, FilterDefinition<T> filter, T t, Boolean upsert = false)
        {
            var coll = getCollection(collectionName);
            BsonDocument document = t.ToBsonDocument<T>();
            document.Remove("_id");
            UpdateDocument update = new UpdateDocument("$set", document);
            var result = coll.UpdateOne(filter, update, new UpdateOptions { IsUpsert = upsert });
            return result.ModifiedCount;
        }
        /// <summary>
        /// 用新对象替换-更新批量
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">修改条件</param>
        /// <param name="t">新对象</param>
        /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则true插入新文档）</param>
        /// <returns>修改影响文档数</returns>
        public Int64 updateMany(String collectionName, FilterDefinition<T> filter, T t, Boolean upsert = false)
        {
            var coll = getCollection(collectionName);
            BsonDocument document = t.ToBsonDocument<T>();
            document.Remove("_id");
            UpdateDocument update = new UpdateDocument("$set", document);
            var result = coll.UpdateMany(filter, update, new UpdateOptions { IsUpsert = upsert });
            return result.ModifiedCount;
        }

        #endregion

        #region 查询 
        /// <summary>
        /// 根据条件查询数据库,并返回对象集合
        /// </summary>
        /// <param name="match">条件表达式</param>
        /// <param name="sortPropertyName">排序字段</param>
        /// <param name="isDescending">如果为true则为降序，否则为升序</param>
        /// <returns></returns>
        public IList<T> Find(String collectionName, Expression<Func<T, bool>> match, string sortPropertyName, bool isDescending = true)
        {
            return getQueryable(collectionName, match, sortPropertyName, isDescending).ToList();
        }
        /// <summary>
        /// 根据条件查询数据库,并返回对象集合
        /// </summary>
        /// <param name="filter">条件表达式</param>
        /// <param name="sortPropertyName">排序字段</param>
        /// <param name="isDescending">如果为true则为降序，否则为升序</param>
        /// <returns></returns>
        public IList<T> Find(String collectionName, FilterDefinition<T> filter, string sortPropertyName, bool isDescending = true)
        {
            return getQueryable(collectionName, filter, sortPropertyName, isDescending).ToList();
        }
        /// <summary>
        /// 根据条件查询数据库,并返回对象集合
        /// </summary>
        /// <param name="match">条件表达式</param>
        /// <param name="orderByProperty">排序表达式</param>
        /// <param name="isDescending">如果为true则为降序，否则为升序</param>
        /// <returns></returns>
        public IList<T> Find<TKey>(String collectionName, Expression<Func<T, bool>> match, Expression<Func<T, TKey>> orderByProperty, bool isDescending = true)
        {
            return getQueryable<TKey>(collectionName, match, orderByProperty, isDescending).ToList();
        }
        /// <summary>
        /// 根据条件查询数据库,如果存在返回第一个对象
        /// </summary>
        /// <param name="filter">条件表达式</param>
        /// <returns>存在则返回指定的第一个对象,否则返回默认值</returns>
        public T FindSingle(String collectionName, FilterDefinition<T> filter)
        {
            var coll = getCollection(collectionName);
            return coll.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="query">条件表达式</param>
        /// <param name="info">分页实体</param>
        /// <returns>指定对象的集合</returns>
        public IList<T> FindWithPager(String collectionName, FilterDefinition<T> query, PagerInfo info, string orderByProperty, bool isDescending = true)
        {
            int pageindex = (info.CurrenetPageIndex < 1) ? 1 : info.CurrenetPageIndex;
            int pageSize = (info.PageSize <= 0) ? 20 : info.PageSize;
            int excludedRows = (pageindex - 1) * pageSize;
            var find = getQueryable(collectionName, query, orderByProperty, isDescending);
            info.RecordCount = (int)find.Count();
            return find.Skip(excludedRows).Limit(pageSize).ToList();
        }
        /// <summary>
        /// 查询，复杂查询直接用Linq处理
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <returns>要查询的对象</returns>
        private IQueryable<T> getQueryable(String collectionName)
        {
            var coll = getCollection(collectionName);
            return coll.AsQueryable<T>();
        }
        /// <summary>
        /// 根据条件表达式返回可查询的记录源
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="sortPropertyName">排序表达式</param>
        /// <param name="isDescending">如果为true则为降序，否则为升序</param>
        /// <returns></returns>
        private IFindFluent<T, T> getQueryable(String collectionName,FilterDefinition<T> query, string sortPropertyName, bool isDescending = true)
        {
            IMongoCollection<T> collection = getCollection(collectionName);
            IFindFluent<T, T> queryable = collection.Find(query);
            var sort = isDescending ? Builders<T>.Sort.Descending(sortPropertyName) : Builders<T>.Sort.Ascending(sortPropertyName);
            return queryable.Sort(sort);
        }
        /// <summary>
        /// 根据条件表达式返回可查询的记录源
        /// </summary>
        /// <param name="match">查询条件</param>
        /// <param name="orderByProperty">排序表达式</param>
        /// <param name="isDescending">如果为true则为降序，否则为升序</param>
        /// <returns></returns>
        private IQueryable<T> getQueryable<TKey>(String collectionName, Expression<Func<T, bool>> match, Expression<Func<T, TKey>> orderByProperty,  bool isDescending = true)
        {
            IMongoCollection<T> collection = getCollection(collectionName);
            IQueryable<T> query = collection.AsQueryable();

            if (match != null)
            {
                query = query.Where(match);
            }

            if (orderByProperty != null)
            {
                query = isDescending ? query.OrderByDescending(orderByProperty) : query.OrderBy(orderByProperty);
            }
            else
            {
               // query = query.OrderBy(sortPropertyName, isDescending);
            }
            return query;
        }
        
        #endregion
    }

    /// <summary>
    /// 实体基类，方便生成_id
    /// </summary>
    public class BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }
        /// <summary>
        /// 给对象初值
        /// </summary>
        public BaseEntity()
        {
            this.Id = ObjectId.GenerateNewId().ToString();
        }
    }
}