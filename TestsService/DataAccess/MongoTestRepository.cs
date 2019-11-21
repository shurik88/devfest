using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TestsService.Models;

namespace TestsService.DataAccess
{
    public class MongoTestRepository: IQueryable<Test>
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="MongoTestRepository"/>
        /// </summary>
        /// <param name="connectionString">Строка подключения к бд</param>
        /// <param name="collectionName">Имя коллекции</param>
        public MongoTestRepository(string connectionString, string collectionName = "tests") :
            this(new MongoUrl(connectionString).DatabaseName, collectionName, new MongoClient(new MongoUrl(connectionString)))
        {
        }

        private MongoTestRepository(string dbName, string collectionName, IMongoClient client)
        {
            if (string.IsNullOrEmpty(dbName))
                throw new ArgumentNullException(nameof(dbName));
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException(nameof(collectionName));
            Client = client ?? throw new ArgumentNullException(nameof(client));
            DataBase = Client.GetDatabase(dbName);
            TestsCollection = DataBase.GetCollection<Test>(collectionName);
        }

        /// <inheritdoc/>
        public async Task AddAsync(Test entity, CancellationToken token = default(CancellationToken))
        {
            await TestsCollection.InsertOneAsync(entity, new InsertOneOptions { BypassDocumentValidation = false }, token);
        }

        /// <inheritdoc/>
        public async Task AddManyAsync(IEnumerable<Test> entities, CancellationToken token = default(CancellationToken))
        {
            if (entities.Any())
                await TestsCollection.InsertManyAsync(entities, null, token);
        }

        /// <inheritdoc/>
        public async Task ClearAsync(CancellationToken token = default(CancellationToken))
        {
            await TestsCollection.DeleteManyAsync(x => true, token);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(Test entity, CancellationToken token = default(CancellationToken))
        {
            await TestsCollection.DeleteOneAsync(x => x.Id.Equals(entity.Id), new DeleteOptions { }, token);
        }

        /// <inheritdoc/>
        public async Task DeleteManyAsync(IEnumerable<Test> entities, CancellationToken token = default(CancellationToken))
        {
            var listIds = entities.Select(x => x.Id).ToList();
            await DeleteManyAsync(x => listIds.Contains(x.Id), token);
        }

        /// <inheritdoc/>
        public async Task DeleteManyAsync(Expression<Func<Test, bool>> criteria, CancellationToken token = default(CancellationToken))
        {
            await TestsCollection.DeleteManyAsync(criteria, token);
        }

        /// <inheritdoc/>
        public async Task SaveAsync(Test entity, bool isUpsert = false, CancellationToken token = default(CancellationToken))
        {
            await TestsCollection.ReplaceOneAsync(x => x.Id.Equals(entity.Id), entity, new UpdateOptions { IsUpsert = isUpsert }, token);
        }


        IMongoClient Client { get; }

        protected IMongoDatabase DataBase { get; }

        protected IMongoCollection<Test> TestsCollection { get; }

        #region IQueryable<T>

        /// <inheritdoc/>
        public Type ElementType => TestsCollection.AsQueryable().ElementType;

        /// <inheritdoc/>
        public Expression Expression => TestsCollection.AsQueryable().Expression;

        /// <inheritdoc/>
        public IQueryProvider Provider => TestsCollection.AsQueryable().Provider;

        /// <inheritdoc/>
        public IEnumerator<Test> GetEnumerator()
        {
            return TestsCollection.AsQueryable().GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
