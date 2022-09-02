using System;
using System.Collections.Generic;
using GameStore.Core.Models.Server.RelationalModels;
using GameStore.SharedKernel.Interfaces;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Core.Models.Server.Genres;

public class Genre : ISafeDelete
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int? CategoryId { get; set; }

    public Guid? ParentId { get; set; }

    [BsonIgnore]
    public Genre Parent { get; set; }

    [BsonIgnore]
    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();

    [BsonIgnore]
    public ICollection<GameGenre> Games { get; set; } = new List<GameGenre>();

    public bool IsDeleted { get; set; }
}