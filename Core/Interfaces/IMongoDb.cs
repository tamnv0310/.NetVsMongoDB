using MongoDB.Bson;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IMongoDb<T> where T : class, new()
    {
        IEnumerable<T> Gets<T>(string partitionKey);
        T Get(string partitionKey, ObjectId Id);
        T Create(string partitionKey, T document);
        void Update(string partitionKey, ObjectId Id, T document);
        void Remove(string partitionKey, ObjectId Id);
    }
}
