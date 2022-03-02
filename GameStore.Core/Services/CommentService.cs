using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Comments.Specifications;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private IRepository<Comment> CommentRepository => _unitOfWork.GetRepository<Comment>();

        public async Task CommentGameAsync(string gameKey, string authorName, string message)
        {
            var game = (await _unitOfWork.GetRepository<Game>().GetBySpecificationAsync(new GameByKeySpec(gameKey))).First();
            var comment = new Comment(authorName, message, game);

            await CommentRepository.AddAsync(comment);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey)
        {
            return await CommentRepository.GetBySpecificationAsync(new CommentsByGameKey(gameKey));
        }

        public async Task ReplyCommentAsync(Guid parentId, string authorName, string message)
        {
            var parent = await CommentRepository.GetByIdAsync(parentId);

            Comment reply = new Comment(authorName, message, parent.Game, parent);

            await CommentRepository.AddAsync(reply);

            parent.Replies.Add(reply);

            CommentRepository.Update(parent);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
