using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class MongoConfiguration
    {
        /// <summary>
        /// Full connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Database name
        /// </summary>
        public string Database { get; set; }
    }
}
