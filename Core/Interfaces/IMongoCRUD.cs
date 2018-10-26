using Core.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IMongoCRUD<TDocument> where TDocument : class, new()
    {
        MongoConfiguration Configuration { get; set; }

        /// <summary>
        /// MongoClient from mongo driver
        /// </summary>
        IMongoClient MongoClient { get; }

        /// <summary>
        /// Create new document
        /// </summary>
        /// <param name="obj"></param>
        void Create(TDocument obj);

        /// <summary>
        /// Create many new documents
        /// </summary>
        /// <param name="obj"></param>
        void Create(IEnumerable<TDocument> objs);

        /// <summary>
        /// Update one document
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Update(TDocument obj);

        /// <summary>
        /// Update one or more documents partially by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="partialObject"></param>
        /// <returns></returns>
        bool UpdateByQuery(FilterDefinition<TDocument> filter, object partialObject);

        /// <summary>
        /// Update if exists or create new document
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Upsert(TDocument obj);

        /// <summary>
        /// Delete document by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(string id);

        /// <summary>
        /// Delete by query
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool DeleteByQuery(FilterDefinition<TDocument> filter);

        /// <summary>
        /// Search documents by filters, with paging and sorting
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        SearchResult<TDocument> Search(FilterDefinition<TDocument> filters, SearchOptions options);

        /// <summary>
        /// Get document by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TDocument Get(string id);
        IEnumerable<TDocument> GetAll();
    }
}
