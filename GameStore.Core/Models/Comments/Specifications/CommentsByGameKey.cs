using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Core.Models.Comments.Specifications
{
    internal class CommentsByGameKey : Specification<Comment>
    {
        public CommentsByGameKey(string gameKey)
        {
            //Query.
            //    Where(c => c.Game.Key == gameKey).
            //    Include(c => c.Replies);
        }
    }
}
