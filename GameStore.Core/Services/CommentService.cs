using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Comments.Specifications;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.Records;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public async Task CommentGameAsync(CreateCommentModel model)
        {
            var game = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(model.GameKey));

            if (game is null)
            {
                _logger.LogInformation("Add comment to game failed - no game with key {0}", model.GameKey);
                throw new ArgumentException("Game with such id doesn't exist");
            }

            var comment = new Comment(model.AuthorName, model.Message, game);

            await CommentRepository.AddAsync(comment);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey)
        {
            return await CommentRepository.GetBySpecAsync(new CommentsByGameKey(gameKey));
        }

        public async Task ReplyCommentAsync(Guid parentId, string authorName, string message)
        {
            var parent = await CommentRepository.GetByIdAsync(parentId);

            if(parent is null)
            {
                _logger.LogInformation("Reply comment failed - parent comment not found {0}", parentId);
                throw new ArgumentException("Parent comment with such id doesn't exists");
            }

            Comment reply = new Comment(authorName, message, parent.Game, parent);

            await CommentRepository.AddAsync(reply);

            parent.Replies.Add(reply);

            CommentRepository.Update(parent);

            await _unitOfWork.SaveChangesAsync();   
        }
    }
}
