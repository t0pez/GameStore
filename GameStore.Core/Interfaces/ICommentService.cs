using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Records;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Core.Interfaces
{
    public interface ICommentService
    {
        Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey);
        Task CommentGameAsync(CreateCommentModel model);
        Task ReplyCommentAsync(Guid parentId, string authorName, string message);
    }
}
