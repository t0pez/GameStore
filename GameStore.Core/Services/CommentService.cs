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
using System.Threading;
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
        private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();

        public async Task CommentGameAsync(string gameKey, string authorName, string message, CancellationToken token = default)
        {
            try
            {
                var game = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(gameKey), token);

                var comment = new Comment(authorName, message, game);

                await CommentRepository.AddAsync(comment, token);

                await _unitOfWork.SaveChangesAsync(token);
            }
            catch (ItemNotFoundException<Game>)
            {
                _logger.LogInformation("Add comment to game failed - no game with key {0}", gameKey);
                throw new ArgumentException("Game with such id doesn't exist");
            }
        }

        public async Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey, CancellationToken token = default)
        {
            return await CommentRepository.GetBySpecAsync(new CommentsByGameKey(gameKey), token);
        }

        public async Task ReplyCommentAsync(Guid parentId, string authorName, string message, CancellationToken token = default)
        {
            try
            {
                var parent = await CommentRepository.GetByIdAsync(parentId, token);

                Comment reply = new Comment(authorName, message, parent.Game, parent);

                await CommentRepository.AddAsync(reply, token);

                parent.Replies.Add(reply);

                await CommentRepository.UpdateAsync(parent, token);

                await _unitOfWork.SaveChangesAsync(token);
            }
            catch (ItemNotFoundException<Comment>)
            {
                _logger.LogInformation("Reply comment failed - parent comment not found {0}", parentId);
                throw new ArgumentException("Parent comment with such id doesn't exists");
            }
        }
    }
}
