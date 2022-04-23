using System;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Comments.Specifications;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Models.ServiceModels.Comments;

namespace GameStore.Core.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CommentService> _logger;
    private readonly IMapper _mapper;

    public CommentService(IUnitOfWork unitOfWork, ILogger<CommentService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    private IRepository<Comment> CommentRepository => _unitOfWork.GetRepository<Comment>();
    private IRepository<Game> GameRepository => _unitOfWork.GetRepository<Game>();

    public async Task CommentGameAsync(CommentCreateModel model)
    {
        var game = await GameRepository.GetSingleOrDefaultBySpecAsync(new GameByKeySpec(model.GameKey))
                   ?? throw new ItemNotFoundException(typeof(Game), model.GameKey, nameof(model.GameKey));
        
        var comment = _mapper.Map<Comment>(model);
        comment.GameId = game.Id;
        comment.DateOfCreation = DateTime.UtcNow;

        await CommentRepository.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Comment successfully added for game. " +
                               "GameId = {GameId}, CommentId = {CommentId}",
                               game.Id, comment.Id);
    }

    public async Task<ICollection<Comment>> GetCommentsByGameKeyAsync(string gameKey)
    {
        if (await GameRepository.AnyAsync(new GameByKeySpec(gameKey)) == false)
        {
            throw new ItemNotFoundException(typeof(Game), gameKey);
        }

        var result = await CommentRepository.GetBySpecAsync(new CommentsByGameKeySpec(gameKey));

        return result;
    }

    public async Task ReplyCommentAsync(ReplyCreateModel createModel)
    {
        if (await CommentRepository.AnyAsync(new CommentByIdSpec(createModel.ParentId)) == false)
        {
            throw new ItemNotFoundException(typeof(Comment), createModel.ParentId, nameof(createModel.ParentId));
        }

        if (await GameRepository.AnyAsync(new GameByIdSpec(createModel.GameId)) == false)
        {
            throw new ItemNotFoundException(typeof(Game), createModel.GameId, nameof(createModel.GameId));
        }

        var reply = _mapper.Map<Comment>(createModel);
        reply.DateOfCreation = DateTime.UtcNow;

        await CommentRepository.AddAsync(reply);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Reply successfully added for comment. " +
                               "ParentId = {ParentId}, ReplyId = {ReplyId}",
                               reply.ParentId, reply.Id);
    }
}