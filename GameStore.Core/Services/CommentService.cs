using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Comments.Specifications;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.SharedKernel.Specifications.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace GameStore.Core.Services;

public class CommentService : ICommentService
{
    private readonly ILogger<CommentService> _logger;
    private readonly IMapper _mapper;
    private readonly IMongoLogger _mongoLogger;
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(ILogger<CommentService> logger, IMongoLogger mongoLogger, IUnitOfWork unitOfWork,
                          IMapper mapper)
    {
        _logger = logger;
        _mongoLogger = mongoLogger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<Comment> CommentRepository => _unitOfWork.GetEfRepository<Comment>();
    private IRepository<Game> GameRepository => _unitOfWork.GetEfRepository<Game>();

    public async Task CommentGameAsync(CommentCreateModel model)
    {
        var game = await GameRepository.GetSingleOrDefaultBySpecAsync(new GameByKeySpec(model.GameKey))
                   ?? throw new ItemNotFoundException(typeof(Game), model.GameKey, nameof(model.GameKey));

        var comment = _mapper.Map<Comment>(model);
        comment.GameId = game.Id;
        comment.DateOfCreation = DateTime.UtcNow;
        comment.State = CommentState.Head;

        await CommentRepository.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogCreateAsync(comment);
        _logger.LogInformation("Comment successfully added for game. " +
                               $@"{nameof(game.Id)} = {game.Id}, CommentId = {comment.Id}");
    }

    public async Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey)
    {
        if (await GameRepository.AnyAsync(new GameByKeySpec(gameKey)) == false)
        {
            throw new ItemNotFoundException(typeof(Game), gameKey);
        }

        var result = await CommentRepository.GetBySpecAsync(new CommentsWithoutParentByGameKeySpec(gameKey)
                                                                .IncludeDeleted());

        return result;
    }

    public async Task ReplyCommentAsync(ReplyCreateModel createModel)
    {
        if (await CommentRepository.AnyAsync(new CommentByIdSpec(createModel.ParentId)) == false)
        {
            throw new ItemNotFoundException(typeof(Comment), createModel.ParentId, nameof(createModel.ParentId));
        }

        var game = await GameRepository.GetSingleOrDefaultBySpecAsync(new GameByKeySpec(createModel.GameKey))
                   ?? throw new ItemNotFoundException(typeof(Game), createModel.GameKey, nameof(createModel.GameKey));

        var reply = _mapper.Map<Comment>(createModel);
        reply.GameId = game.Id;
        reply.DateOfCreation = DateTime.UtcNow;

        await CommentRepository.AddAsync(reply);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogCreateAsync(reply);
        _logger.LogInformation("Reply successfully added for comment. " +
                               "ParentId = {ParentId}, ReplyId = {ReplyId}",
                               reply.ParentId, reply.Id);
    }

    public async Task UpdateAsync(CommentUpdateModel updateModel)
    {
        var comment = await CommentRepository.GetSingleOrDefaultBySpecAsync(new CommentByIdSpec(updateModel.Id));

        var oldCommentVersion = comment.ToBsonDocument();

        UpdateValues(comment, updateModel);

        await CommentRepository.UpdateAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogUpdateAsync(typeof(Comment), oldCommentVersion, comment.ToBsonDocument());
    }

    public async Task DeleteAsync(Guid id)
    {
        var comment = await CommentRepository.GetSingleOrDefaultBySpecAsync(new CommentByIdSpec(id));

        comment.IsDeleted = true;

        await CommentRepository.UpdateAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogDeleteAsync(comment);
    }

    private void UpdateValues(Comment comment, CommentUpdateModel updateModel)
    {
        comment.Body = updateModel.Body;
        comment.Name = updateModel.AuthorName;
    }
}