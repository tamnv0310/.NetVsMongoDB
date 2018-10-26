using Core.Interfaces;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Infrastructure.Data
{
    public class CarInformationServices<CarModel> : ICarInformation<CarModel> where CarModel : class, new()
    {
        private MongoDbContext<CarModel> _dbContext;
        public CarInformationServices(MongoDbContext<CarModel> mongoDbContext)
        {
            var connectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            var databaseName = ConfigurationManager.AppSettings["MongoDatabaseName"];
            _dbContext = new MongoDbContext<CarModel>(connectionString, databaseName);
        }
        public void Create(CarModel document)
        {
            _dbContext.Create(document);
        }

        public bool Delete(string id)
        {
            return _dbContext.Delete(id);
        }

        public CarModel Get(string id)
        {
            return _dbContext.Get(id);
        }

        public IEnumerable<CarModel> GetAll()
        {
            return _dbContext.GetAll();
        }

        public bool Update(CarModel document)
        {
            return _dbContext.Update(document);
        }
    }
}
