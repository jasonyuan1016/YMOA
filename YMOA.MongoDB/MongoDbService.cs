using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using YMOA.MongoDB.Extension;
using YMOA.MongoDB.Model;
using FrameWork.Extension;
using YMOA.MongoDB.MongoDbConfig;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace YMOA.MongoDB
{
    #region MongoDb操作封装
    /// <summary>
    /// MongoDb操作封装
    /// </summary>
    public class MongoDbService
    {
        #region 初始化

        private readonly string _connString = "MongoDb".ValueOfAppSetting();
        private readonly MongoClient _mongoClient;

        public MongoDbService()
        {
            ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack { new IgnoreExtraElementsConvention(true) }, type => true);
            _mongoClient = new MongoClient(_connString);
        }

        #endregion

        #region 增

        #region 增（同步）
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合（表）</param>
        /// <param name="entity">实体(文档)</param>
        public void Add<T>(string database, string collection, T entity)
        {
            AddAsync(database, collection, entity).Wait();
        }
        #endregion

        #region 增（异步）

        /// <summary>
        /// 增（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合（表）</param>
        /// <param name="entity">实体(文档)</param>
        /// <returns></returns>
        public async Task AddAsync<T>(string database, string collection, T entity)
        {
            var db = _mongoClient.GetDatabase(database);
            var coll = db.GetCollection<T>(collection);
            await coll.InsertOneAsync(entity).ConfigureAwait(false);
        }
        #endregion

        #endregion

        #region 批量增

        #region 批量增（异步）

        /// <summary>
        /// 批量增（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合（表）</param>
        /// <param name="entity">实体(文档)</param>
        public async Task BatchAddAsync<T>(string database, string collection, List<T> entity)
        {
            var db = _mongoClient.GetDatabase(database);
            var coll = db.GetCollection<T>(collection);
            await coll.InsertManyAsync(entity).ConfigureAwait(false);
        }
        #endregion

        #region 批量增（同步）

        /// <summary>
        /// 批量增（同步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合（表）</param>
        /// <param name="entity">实体(文档)</param>
        public void BatchAdd<T>(string database, string collection, List<T> entity)
        {
            BatchAddAsync(database, collection, entity).Wait();
        }
        #endregion

        #endregion

        #region 删

        #region 删（同步）

        /// <summary>
        /// 删
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public long Delete<T>(string database, string collection, Expression<Func<T, bool>> predicate)
          
        {
            return DeleteAsync(database, collection, predicate).Result;
        }
        #endregion

        #region 删（异步）
        
        /// <summary>
        /// 删
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">实体</param>
        /// <returns></returns>
        public async Task<long> DeleteAsync<T>(string database, string collection, Expression<Func<T, bool>> predicate)
        {
            var db = _mongoClient.GetDatabase(database);
            var coll = db.GetCollection<T>(collection);
            var result = await coll.DeleteManyAsync(predicate).ConfigureAwait(false);
            return result.DeletedCount;
        }
        #endregion

        #endregion

        #region 改

        #region 改（同步）
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">条件</param>
        /// <param name="entity">字段</param>
        /// <returns></returns>
        public long Update<T>(string database, string collection, Expression<Func<T, bool>> predicate, T entity)
        {
            return UpdateAsync(database, collection, predicate, entity).Result;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">条件</param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public long Update<T>(string database, string collection, Expression<Func<T, bool>> predicate, Expression<Func<T, T>> lambda)
        {
            return UpdateAsync(database, collection, predicate, lambda).Result;
        }

        #endregion

        #region 改（异步）
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="entity">实体（根据主键更新）</param>
        /// <returns></returns>
        public Task<long> UpdateAsync<T>(Expression<Func<T, bool>> predicate, T entity)
        {
            var mongoAttribute = typeof(T).GetMongoAttribute();
            if (mongoAttribute.IsNull())
                throw new ArgumentException("MongoAttribute不能为空");

            return UpdateAsync(mongoAttribute.Database, mongoAttribute.Collection, predicate, entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">条件</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<long> UpdateAsync<T>(string database, string collection,
           Expression<Func<T, bool>> predicate, T entity)
        {
            var db = _mongoClient.GetDatabase(database);
            var coll = db.GetCollection<T>(collection);

            var updateDefinitionList = entity.GetUpdateDefinition();

            var updateDefinitionBuilder = new UpdateDefinitionBuilder<T>().Combine(updateDefinitionList);

            var result = await coll.UpdateOneAsync<T>(predicate, updateDefinitionBuilder).ConfigureAwait(false);


            return result.ModifiedCount;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">条件</param>
        /// <param name="lambda">实体</param>
        /// <returns></returns>
        public async Task<long> UpdateAsync<T>(string database, string collection,
           Expression<Func<T, bool>> predicate, Expression<Func<T, T>> lambda)
        {
            var db = _mongoClient.GetDatabase(database);
            var coll = db.GetCollection<T>(collection);

            var updateDefinitionList = MongoDbExpression<T>.GetUpdateDefinition(lambda);

            var updateDefinitionBuilder = new UpdateDefinitionBuilder<T>().Combine(updateDefinitionList);

            var result = await coll.UpdateManyAsync<T>(predicate, updateDefinitionBuilder).ConfigureAwait(false);

            return result.ModifiedCount;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="lambda">实体</param>
        /// <returns></returns>
        public Task<long> UpdateAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> lambda)
        {
            var mongoAttribute = typeof(T).GetMongoAttribute();
            if (mongoAttribute.IsNull())
                throw new ArgumentException("MongoAttribute不能为空");

            return UpdateAsync(mongoAttribute.Database, mongoAttribute.Collection, predicate, lambda);
        }

        #endregion

        #endregion

        #region 查

        #region 查（同步）
        /// <summary>
        /// 查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="projector">查询字段</param>
        /// <returns></returns>
        public T Get<T>(string database, string collection, Expression<Func<T, bool>> predicate,
            Expression<Func<T, T>> projector = null)
        {
            return GetAsync(database, collection, predicate, projector).Result;
        }

        #endregion

        #region 查（异步）

        /// <summary>
        /// 查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="projector">查询字段</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string database, string collection,
            Expression<Func<T, bool>> predicate, Expression<Func<T, T>> projector)
        {
            var db = _mongoClient.GetDatabase(database);
            var coll = db.GetCollection<T>(collection);

            var find = coll.Find(predicate);
            if (!projector.IsNull())
                find = find.Project(projector);
            
            return await find.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region 列表

        #region 列表（同步）
        /// <summary>
        /// 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">过滤条件</param>
        /// <param name="projector">查询字段</param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<T> List<T>(string database, string collection,
           Expression<Func<T, bool>> predicate, Expression<Func<T, T>> projector, int? limit, bool ascending = true, Expression<Func<T, object>> sortfield = null)
        {
            return ListAsync(database, collection, predicate, projector, limit, ascending, sortfield).Result;
        }
        #endregion

        #region 列表（异步）
        /// <summary>
        /// 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="projector"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<List<T>> ListAsync<T>(string database, string collection,
           Expression<Func<T, bool>> predicate, Expression<Func<T, T>> projector, int? limit, bool ascending = true, Expression<Func<T, object>> sortfield = null)
        {
            var db = _mongoClient.GetDatabase(database);
            var coll = db.GetCollection<T>(collection);

            var find = coll.Find(predicate);

            if (projector.IsNotNull())
                find = find.Project(projector);

            if (sortfield.IsNotNull())
            {
                if (ascending)
                    find = find.SortBy(sortfield);
                else
                    find = find.SortByDescending(sortfield);
            }
            
            if (limit.IsNotNull())
                find = find.Limit(limit);

            return await find.ToListAsync().ConfigureAwait(false);
        }
        #endregion

        #endregion

        #region 分页

        #region 分页（同步）
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">过滤条件</param>
        /// <param name="projector">查询字段</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页项</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="desc">顺序、倒叙</param>
        /// <returns></returns>
        public PageList<T> PageList<T>(string database, string collection, Expression<Func<T, bool>> predicate, Expression<Func<T, T>> projector, int pageIndex = 1, int pageSize = 20, Expression<Func<T, object>> orderby = null, bool desc = false)
        {
            return PageListAsync(database, collection, predicate, projector, pageIndex, pageSize, orderby, desc).Result;
        }

        #endregion

        #region 分页（异步）
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合</param>
        /// <param name="predicate">过滤条件</param>
        /// <param name="projector">查询字段</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页项</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="desc">顺序、倒叙</param>
        /// <returns></returns>
        public async Task<PageList<T>> PageListAsync<T>(string database, string collection, Expression<Func<T, bool>> predicate, Expression<Func<T, T>> projector, int pageIndex = 1, int pageSize = 20, Expression<Func<T, object>> orderby = null, bool desc = false)
        {
            var db = _mongoClient.GetDatabase(database);
            var coll = db.GetCollection<T>(collection);

            var count = (int)await coll.CountAsync<T>(predicate).ConfigureAwait(false);

            var find = coll.Find(predicate);

            if (projector.IsNotNull())
                find = find.Project(projector);

            if (orderby != null)
            {
                find = desc ? find.SortByDescending(@orderby) : find.SortBy(@orderby);
            }

            find = find.Skip((pageIndex - 1) * pageSize).Limit(pageSize);

            var items = await find.ToListAsync().ConfigureAwait(false);

            return new PageList<T>(pageIndex, pageSize, count, items);
        }
        #endregion

        #endregion


        public List<BsonDocument> GetGroupData<T>(string database, string collection, List<string> _groupKeys, List<string> _groupFileds, List<string> _matchs = null)
        {
            var db = _mongoClient.GetDatabase(database);
            var coll = db.GetCollection<BsonDocument>(collection);

            IList<IPipelineStageDefinition> stages = new List<IPipelineStageDefinition>();
            string strTmp = string.Empty;
            if (_matchs != null && _matchs.Count > 0)
            {
                
                foreach (string _m in _matchs)
                {
                    strTmp = "," + _m;
                }
                stages.Add(new JsonPipelineStageDefinition<BsonDocument, BsonDocument>("{$match:{" + strTmp.Substring(1) + "}}"));
            }
            strTmp = string.Empty;
            foreach (string _k in _groupKeys)
            {
                strTmp += string.Format(",{0}:'${0}'", _k);
            }
            string strFileds = string.Empty;
            foreach (string _f in _groupFileds)
            {
                strFileds += "," + _f;
            }
            stages.Add(new JsonPipelineStageDefinition<BsonDocument, BsonDocument>("{$group:{_id:{" + strTmp.Substring(1) + "}" + strFileds + "}}"));
            PipelineDefinition<BsonDocument, BsonDocument> pipeline = new PipelineStagePipelineDefinition<BsonDocument, BsonDocument>(stages);
            return coll.Aggregate(pipeline).ToList();
        }
    }
    #endregion
}
