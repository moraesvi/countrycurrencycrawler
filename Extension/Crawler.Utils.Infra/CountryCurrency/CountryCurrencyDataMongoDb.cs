using Crawler.Utils.Domain.Infra.CountryCurrency;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crawler.Utils.Infra.CountryCurrency
{
    public class CountryCurrencyDataMongoDb
    {
        private readonly IMongoDatabase _mongoDb;
        private readonly IMongoCollection<CountryCurrencyDataDb> _currencyDataDocDb;
        private readonly IMongoCollection<CountryCurrencyDataDb> _mongoCollection;
        public CountryCurrencyDataMongoDb(IConfiguration config)
        {
            var client = new MongoClient(config.GetSection("MongoConnectionString").Value);

            _mongoDb = client.GetDatabase(config.GetSection("MongoDataBaseName").Value);
            _mongoCollection = _mongoDb.GetCollection<CountryCurrencyDataDb>(nameof(CountryCurrencyDataDb));
        }
        public async Task<List<CountryCurrencyDataDb>> Get()
        {
            return await _mongoCollection.Find(s => true)
                                         .ToListAsync();
        }
        public async Task<CountryCurrencyDataDb> Add(CountryCurrencyDataDb model)
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
