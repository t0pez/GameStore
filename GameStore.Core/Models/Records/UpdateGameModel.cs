using GameStore.Core.Models.Games;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Core.Models.Records
{
    public class UpdateGameModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] File { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<PlatformType> PlatformTypes { get; set; } = new List<PlatformType>();

    }
}
