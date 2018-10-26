﻿using Core.Builders;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Infrastructure.Data
{
    public class MongoDbContext<TDocument> : IMongoCRUD<TDocument>
        where TDocument : class, new()
    {
        public IMongoClient MongoClient { get; private set; }

        /// <summary>
        /// MongoDatabase from mongo driver
        /// </summary>
        private IMongoDatabase Database { get; set; }

        /// <summary>
        /// MongoCollection<T> from mongo driver
        /// </summary>
        private IMongoCollection<TDocument> Collection { get; set; }

        /// <summary>
        /// MongoCRUD configuration private object
        /// </summary>
        private MongoConfiguration _configuration { get; set; }

        /// <summary>
        /// MongoCRUD configuration with connection string and database name
        /// </summary>
        public MongoConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                this.SetupMongoConfiguration(value);
            }
        }
        /// <summary>
        /// Empty constructor
        /// </summary>
        public MongoDbContext() { }

        /// <summary>
        /// Contruct MongoCRUD with mongo client and database name
        /// </summary>
        public MongoDbContext(IMongoClient client, string database)
        {
            this.SetupMongoClient(client, database);
        }

        /// <summary>
        /// Construct MongoCRUD with connection string and databasse name
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="database"></param>
        public MongoDbContext(string connectionString, string database)
        {
            this.Configuration = new MongoConfiguration
            {
                ConnectionString = connectionString,
                Database = database
            };
        }

        /// <summary>
        /// Create new document
        /// </summary>
        /// <param name="document"></param>
        public void Create(TDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            this.Collection.InsertOne(document);
        }

        /// <summary>
        /// Create many new documents
        /// </summary>
        /// <param name="obj"></param>
        public void Create(IEnumerable<TDocument> documents)
        {
            if (documents == null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            this.Collection.InsertMany(documents);
        }

        /// <summary>
        /// Update one document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool Update(TDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var id = this.GetDocumentId(document);

            var filter = Builders<TDocument>.Filter.Eq("_id", id);
            var options = new UpdateOptions() { IsUpsert = false };

            return this.Collection.ReplaceOne(filter, document, options).IsAcknowledged;
        }

        /// <summary>
        /// Update one or more documents partially by filter
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="partialDocument"></param>
        /// <returns></returns>
        public bool UpdateByQuery(FilterDefinition<TDocument> filters, object partialDocument)
        {
            filters = filters ?? FilterBuilder.GetFilterBuilder<TDocument>().Empty;

            if (partialDocument == null)
            {
                throw new ArgumentNullException(nameof(partialDocument));
            }

            var updateMapped = new BsonDocument { { "$set", partialDocument.ToBsonDocument() } };

            var update = new BsonDocumentUpdateDefinition<TDocument>(updateMapped);
            return this.Collection.UpdateMany(filters, update).IsAcknowledged;
        }

        /// <summary>
        /// Update if exists or create new document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool Upsert(TDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var id = this.GetDocumentId(document);

            var filter = Builders<TDocument>.Filter.Eq("_id", id);
            var options = new UpdateOptions { IsUpsert = true };

            return this.Collection.ReplaceOne(filter, document, options).IsAcknowledged;
        }

        /// <summary>
        /// Delete document by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var filter = Builders<TDocument>.Filter.Eq("_id", new ObjectId(id));
            return this.Collection.DeleteOne(filter).IsAcknowledged;
        }

        /// <summary>
        /// Delete by query
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public bool DeleteByQuery(FilterDefinition<TDocument> filters)
        {
            filters = filters ?? FilterBuilder.GetFilterBuilder<TDocument>().Empty;

            return this.Collection.DeleteMany(filters).IsAcknowledged;
        }

        /// <summary>
        /// Search documents by filters, with paging and sorting
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public SearchResult<TDocument> Search(FilterDefinition<TDocument> filters, SearchOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            filters = filters ?? FilterBuilder.GetFilterBuilder<TDocument>().Empty;

            var documents = this.Collection.Find(filters)
                .WithPaging(options)
                .WithSorting(options);

            var count = this.Collection.CountDocuments(filters);

            return new SearchResult<TDocument>
            {
                Count = count,
                Documents = documents.ToEnumerable()
            };
        }

        /// <summary>
        /// Get document by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TDocument Get(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var filter = Builders<TDocument>.Filter.Eq("_id", new ObjectId(id));
            return this.Collection.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// Get document id by property or annotation
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private object GetDocumentId(TDocument obj)
        {
            var resultByAttribute = obj.GetIndividualPropertyDetailsByAttribute<TDocument, BsonIdAttribute>();
            if (resultByAttribute != null)
            {
                return resultByAttribute.Value;
            }

            var resultByPropertyName = obj.GetIndividualPropertyDetailsByPropertyName<TDocument>("id");
            if (resultByPropertyName != null)
            {
                return resultByPropertyName.Value;
            }

            throw new InvalidOperationException("Model must have a property called 'Id' or a property with BsonId attribute");
        }

        /// <summary>
        /// Setup instance configuration
        /// </summary>
        /// <param name="configuration"></param>
        private void SetupMongoConfiguration(MongoConfiguration configuration)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var mongoClient = new MongoClient(configuration.ConnectionString);
            this.SetupMongoClient(mongoClient, configuration.Database);
        }

        /// <summary>
        /// Setup instance configuration
        /// </summary>
        /// <param name="configuration"></param>
        private void SetupMongoClient(IMongoClient mongoClient, string database)
        {
            if (string.IsNullOrWhiteSpace(database))
            {
                throw new ArgumentNullException(nameof(database));
            }

            this.MongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
            this.Database = this.MongoClient.GetDatabase(database);
            this.Collection = this.Database.GetCollection<TDocument>(typeof(TDocument).Name);
        }

        public IEnumerable<TDocument> GetAll()
        {
            return this.Collection.Find(_ => true).ToList();
        }
    }
    public static class MongoCRUD
    {
        /// <summary>
        /// Default convention pack name
        /// </summary>
        public static readonly string DEFAULT_CONVENTION_PACK_NAME = "Default MongoCRUD Convention";

        /// <summary>
        /// Default convention pack
        /// - Guid as string
        /// - Enum as string
        /// - CamelCase element name
        /// - Ignore extra elements
        /// Please, call this convention pack registration only one time in your application.
        /// </summary>
        public static void RegisterDefaultConventionPack(Func<Type, bool> filter)
        {
            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));

            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String)

            };

            ConventionRegistry.Register(DEFAULT_CONVENTION_PACK_NAME, conventionPack, filter);
        }
    }
}