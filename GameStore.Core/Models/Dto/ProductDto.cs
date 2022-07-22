using System;
using System.Collections.Generic;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.ServiceModels.Enums;

namespace GameStore.Core.Models.Dto;

public class ProductDto
{
    public string Id { get; set; }
    public int? ProductId { get; set; }

    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int UnitsInStock { get; set; }
    public string QuantityPerUnit { get; set; } = string.Empty;
    public bool Discontinued { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? AddedToStoreAt { get; set; }
    public byte[] File { get; set; }
    public int? Views { get; set; }

    public List<Genre> Genres { get; set; } = new();
    public List<PlatformType> Platforms { get; set; } = new();

    public string PublisherName { get; set; } = string.Empty;
    public PublisherDto Publisher { get; set; }
    public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();

    public Database Database { get; set; }
    public bool? IsDeleted { get; set; }
}