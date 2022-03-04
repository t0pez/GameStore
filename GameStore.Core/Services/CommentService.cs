using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Comments.Specifications;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CommentService> _logger;

        public CommentService(IUnitOfWork unitOfWork, ILogger<CommentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private IRepository<Comment> CommentRepository => _unitOfWork.GetRepository<Comment>();

        public async Task CommentGameAsync(string gameKey, string authorName, string message)
        {
            try
            {
                var game = await _unitOfWork.GetRepository<Game>().GetSingleBySpecAsync(new GameByKeySpec(gameKey));

                var comment = new Comment(authorName, message, game);

                await CommentRepository.AddAsync(comment);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (ItemNotFoundException<Game>)
            {
                _logger.LogInformation("Add comment to game failed - no game with key {0}", gameKey);
                throw new ArgumentException("Game with such id doesn't exist");
            }
        }

        public async Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey)
        {
            return await CommentRepository.GetBySpecAsync(new CommentsByGameKey(gameKey));
        }

        public async Task ReplyCommentAsync(Guid parentId, string authorName, string message)
        {
            try
            {
                var parent = await CommentRepository.GetByIdAsync(parentId);

                Comment reply = new Comment(authorName, message, parent.Game, parent);

                await CommentRepository.AddAsync(reply);

                parent.Replies.Add(reply);

                await CommentRepository.UpdateAsync(parent);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (ItemNotFoundException<Comment>)
            {
                _logger.LogInformation("Reply comment failed - parent comment not found {0}", parentId);
                throw new ArgumentException("Parent comment with such id doesn't exists");
            }
        }
    }
}
