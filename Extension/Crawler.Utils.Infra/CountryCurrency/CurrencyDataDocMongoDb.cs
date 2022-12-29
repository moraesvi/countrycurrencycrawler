using Crawler.Utils.Domain.Infra.CountryCurrency;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crawler.Utils.Infra.CountryCurrency
{
    public class CurrencyDataDocMongoDb
    {
        private readonly IMongoDatabase _mongoDb;
        private readonly IMongoCollection<CurrencyDataDocDb> _currencyDataDocDb;
        private readonly IMongoCollection<CurrencyDataDocDb> _mongoCollection;
        public CurrencyDataDocMongoDb(IConfiguration config)
        {
            var client = new MongoClient(config.GetSection("MongoConnectionString").Value);

            _mongoDb = client.GetDatabase(config.GetSection("MongoDataBaseName").Value);
            _mongoCollection = _mongoDb.GetCollection<CurrencyDataDocDb>(nameof(CurrencyDataDocDb));
        }
        public async Task<List<CurrencyDataDocDb>> Get()
        {
            return await _mongoCollection.Find(s => true)
                                         .ToListAsync();
        }
        public async Task<CurrencyDataDocDb> Add(CurrencyDataDocDb model)
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
