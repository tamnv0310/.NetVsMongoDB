using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface ICarInformation<TDocument>
    {
        void Create(TDocument document);
        TDocument Get(string id);
        IEnumerable<TDocument> GetAll();
        /// <summary>
        /// Delete document by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(string id);
        /// <summary>
        /// Update one document
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Update(TDocument obj);
    }
}
