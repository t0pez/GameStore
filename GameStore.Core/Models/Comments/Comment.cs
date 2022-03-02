using System;
using System.Collections.Generic;
using GameStore.Core.Models.Games;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.Core.Models.Comments
{
    public class Comment : BaseEntity, ISafeDelete
    {
        public Comment(string name, string body, Game game, Comment parent = null)
        {
            Name = name;
            Body = body;
            Game = game;
            Parent = parent;
            Date = DateTime.Now;
        }

        private Comment(Guid id, string name, string body, DateTime date, Game game, Comment parent, bool isDeleted)
        {
            Id = id;
            Name = name;
            Body = body;
            Date = date;
            Game = game;
            Parent = parent;
            IsDeleted = isDeleted;
        }

        public Comment()
        {

        }

        public string Name { get; set; }

        public string Body { get; set; }
        public DateTime Date { get; set; }

        public Game Game { get; set; }
        public Comment Parent { get; set; }

        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

        public bool IsDeleted { get; set; }
    }
}
