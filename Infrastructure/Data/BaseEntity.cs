using MongoDB.Bson;

namespace Infrastructure.Data
{
    public class BaseEntity
    {
        public ObjectId Id { get; set; }
    }
}