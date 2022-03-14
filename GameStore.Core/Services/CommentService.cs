using GameStore.Core.Exceptions;
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

namespace GameStore.Core.Services;

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

    public async Task CommentGameAsync(CommentCreateModel model)
    {
        var game = await GameRepository.GetSingleBySpecAsync(new GameByKeySpec(model.GameKey));

        if (game is null) // TODO: change to ItemNotFoundException
        {
            throw new ArgumentException("Game with such key doesn't exist. " +
                                        $"{nameof(model.GameKey)} = {model.GameKey}");
        }

        var comment = new Comment(model.AuthorName, model.Message, game);

        await CommentRepository.AddAsync(comment);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Comment successfully added for game. " +
                               $"{nameof(game.Id)} = {game.Id}, {nameof(comment.Id)} = {comment.Id}");
    }

    public async Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey)
    {
        if(await GameRepository.AnyAsync(new GameByKeySpec(gameKey)) == false)
        {
            throw new ItemNotFoundException($"Game not found. " +
                                            $"{nameof(gameKey)} = {gameKey}");
        }

        var result = await CommentRepository.GetBySpecAsync(new CommentsByGameKey(gameKey));

        return result;
    }

    public async Task ReplyCommentAsync(Guid parentId, string authorName, string message)
    {
        var parent = await CommentRepository.GetByIdAsync(parentId);

        if(parent is null) // TODO: change to ItemNotFoundException, probably 
        {
            throw new ArgumentException("Parent comment with such id doesn't exists." +
                                        $"{nameof(parent.Id)} = {parentId}");
        }

        var reply = new Comment(authorName, message, parent.Game, parent);

        await CommentRepository.AddAsync(reply);

        parent.Replies.Add(reply);

        await CommentRepository.UpdateAsync(parent);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Reply successfully added for comment. " +
                               $"{nameof(parent.Id)} = {parent.Id}, {nameof(reply.Id)} = {reply.Id}");
    }
}