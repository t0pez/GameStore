using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Core.Interfaces
{
    public interface ICommentService
    {
        Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey);
        Task CommentGameAsync(string gameKey, string authorName, string message);
        Task ReplyCommentAsync(Guid parentId, string authorName, string message);
    }
}
