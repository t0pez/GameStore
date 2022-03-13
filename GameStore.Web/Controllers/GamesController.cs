using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Records;
using GameStore.Web.Models;
using HybridModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Web.ViewModels;

namespace GameStore.Web.Controllers;

[ApiController]
public class GamesController : Controller
{
    private readonly IGameService _gameService;
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;
    private readonly ILogger<GamesController> _logger;

    public GamesController(IGameService gameService, ICommentService commentService, IMapper mapper, ILogger<GamesController> logger)
    {
        _gameService = gameService;
        _commentService = commentService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("games")]
    public async Task<ICollection<GameViewModel>> GetAll()
    {
        var games = await _gameService.GetAllAsync();
        var result = _mapper.Map<ICollection<GameViewModel>>(games);

        return result;
    }

    [HttpGet("games/{gameKey}")]
    public async Task<ActionResult<GameViewModel>> GetWithDetails([FromRoute] string gameKey)
    {
        var game = await _gameService.GetByKeyAsync(gameKey);
        var result = _mapper.Map<GameViewModel>(game);

        return Ok(result);
    }

    [HttpPost("games/{gameKey}/download")]
    public async Task<ActionResult<byte[]>> GetFile([FromRoute] string gameKey)
    {
        var result = await _gameService.GetFileAsync(gameKey);

        return Ok(result);
    }

    [HttpPost("games/new")]
    public async Task<ActionResult<GameViewModel>> Create([FromBody] GameCreateRequestModel request)
    {
        var createModel = _mapper.Map<GameCreateModel>(request);

        var game = await _gameService.CreateAsync(createModel);
        var result = _mapper.Map<GameViewModel>(game);
        
        _logger.LogInformation($"Game created. {nameof(result.Id)} = {result.Id}");
        
        return Ok(result);
    }

    [HttpPost("games/{gameKey}/newcomment")]
    public async Task<ActionResult> CommentGame([FromHybrid] CommentCreateRequestModel request)
    {
        var createModel = _mapper.Map<CommentCreateModel>(request);

        await _commentService.CommentGameAsync(createModel);

        _logger.LogInformation($"Game commented. {nameof(createModel.GameKey)} = {createModel.GameKey}");

        return Ok();
    }

    [HttpGet("games/{gameKey}/comments")]
    public async Task<ICollection<Comment>> GetComments([FromRoute] string gameKey)
    {
        var result = await _commentService.GetCommentsByGameKeyAsync(gameKey);

        return result;
    }

    [HttpPost("games/update")]
    public async Task<ActionResult> Edit([FromBody] GameEditRequestModel request)
    {
        var game = _mapper.Map<GameUpdateModel>(request);
        await _gameService.UpdateAsync(game);

        return Ok();
    }

    [HttpPost("games/remove")]
    public async Task<ActionResult> Delete([FromBody] Guid id)
    {
        await _gameService.DeleteAsync(id);

        return Ok();
    }
}