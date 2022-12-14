using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Server.Comments;
using GameStore.Core.Models.Server.RelationalModels;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.SharedKernel.Interfaces;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Core.Models.Server.Games;

public class Game : ISafeDelete
{
    public Guid Id { get; set; }

    public string Key { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string QuantityPerUnit { get; set; }

    public decimal Price { get; set; }

    public bool Discontinued { get; set; }

    public int UnitsInStock { get; set; }

    [BsonIgnore]
    public byte[] File { get; set; }

    public int Views { get; set; }

    public DateTime PublishedAt { get; set; }

    public DateTime AddedToStoreAt { get; set; }

    public string PublisherName { get; set; }

    [NotMapped]
    [BsonIgnore]
    public PublisherDto Publisher { get; set; }

    [BsonIgnore]
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [BsonIgnore]
    public ICollection<GameGenre> Genres { get; set; } = new List<GameGenre>();

    [BsonIgnore]
    public ICollection<GamePlatformType> Platforms { get; set; } = new List<GamePlatformType>();

    [NotMapped]
    public Database Database { get; set; } = Database.Server;

    public bool IsDeleted { get; set; }
}