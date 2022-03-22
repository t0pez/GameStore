using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Records;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Core.Interfaces;

public interface ICommentService
{
    public Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey);
    public Task CommentGameAsync(CommentCreateModel model);
    public Task ReplyCommentAsync(ReplyCreateModel createModel);
}