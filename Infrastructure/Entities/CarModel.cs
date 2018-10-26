using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Entities
{
    public class CarModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement(nameof(CarName))]
        public string CarName { get; set; }
        [BsonElement(nameof(Color))]
        public string Color { get; set; }
        [BsonElement(nameof(Price))]
        public int Price { get; set; }
        [BsonElement(nameof(Engineno))]
        public string Engineno { get; set; }
        [BsonElement(nameof(Registrationno))]
        public string Registrationno { get; set; }
        [BsonElement(nameof(Model))]
        public string Model { get; set; }
        [BsonElement(nameof(Chassisno))]
        public string Chassisno { get; set; }
    }
}
