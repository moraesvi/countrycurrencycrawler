using Crawler.Utils.Domain.Infra.CountryCurrency;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crawler.Utils.Infra.CountryCurrency
{
    public class CountryCurrencyMongoDb
    {
        private readonly IMongoDatabase _mongoDb;
        private readonly IMongoCollection<CountryCurrencyDb> _currencyDataDocDb;
        private readonly IMongoCollection<CountryCurrencyDb> _mongoCollection;
        public CountryCurrencyMongoDb(IConfiguration config)
        {
            var client = new MongoClient(config.GetSection("MongoConnectionString").Value);

            _mongoDb = client.GetDatabase(config.GetSection("MongoDataBaseName").Value);
            _mongoCollection = _mongoDb.GetCollection<CountryCurrencyDb>(nameof(CountryCurrencyDb));
        }
        public async Task<List<CountryCurrencyDb>> Get()
        {
            return await _mongoCollection.Find(s => true)
                                         .ToListAsync();
        }
        public async Task<CountryCurrencyDb> Add(CountryCurrencyDb model)
        {
            await _mongoCollection.InsertOneAsync(model);
            return model;
        }
        public async Task RemoveAll()
        {
            await _mongoCollection.DeleteManyAsync(s => true);
        }
    }
}
