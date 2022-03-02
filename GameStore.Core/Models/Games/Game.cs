﻿using GameStore.Core.Models.Comments;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;

namespace GameStore.Core.Models.Games
{
    public class Game : BaseEntity, ISafeDelete
    {
        public Game(string key, string name, string description, byte[] file)
        {
            Id = Guid.NewGuid();
            Key = key;
            Name = name;
            Description = description;
            File = file;
            IsDeleted = false;
        }

        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] File { get; set; } // TODO: Change for smth else

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<PlatformType> PlatformTypes { get; set; } = new List<PlatformType>();

        public bool IsDeleted { get; set; }
    }
}
