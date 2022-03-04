using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Core.Interfaces
{
    public interface ICommentService
    {
        Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey, CancellationToken token = default);
        Task CommentGameAsync(string gameKey, string authorName, string message, CancellationToken token = default);
        Task ReplyCommentAsync(Guid parentId, string authorName, string message, CancellationToken token = default);
    }
}
