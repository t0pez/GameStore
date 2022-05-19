using System;
using GameStore.Core.Models.Comments;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.ServiceModels.Comments;

namespace GameStore.Core.Interfaces;

public interface ICommentService
{
    public Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey);
    public Task CommentGameAsync(CommentCreateModel model);
    public Task ReplyCommentAsync(ReplyCreateModel createModel);
    public Task UpdateAsync(CommentUpdateModel updateModel);
    public Task DeleteAsync(Guid id);
}

