using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.Core.Models.Server.Publishers;

public class Publisher : ISafeDelete
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string HomePage { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string ContactName { get; set; }

    public string ContactTitle { get; set; }

    public string Phone { get; set; }

    public string Fax { get; set; }

    public string Country { get; set; }

    public string PostalCode { get; set; }

    public string Region { get; set; }

    public ICollection<Game> Games { get; set; }

    [NotMapped]
    public Database Database { get; set; } = Database.Server;

    public bool IsDeleted { get; set; }
}