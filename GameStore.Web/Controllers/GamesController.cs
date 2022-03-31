using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Records;
using GameStore.Web.Models;
using HybridModelBinding;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Web.Filters;
using GameStore.Web.ViewModels;

namespace GameStore.Web.Controllers;

[ApiController]
[ServiceFilter(typeof(WorkTimeTrackingFilter))]
[Route("games")]
public class GamesController : Controller
{
    private readonly IGameService _gameService;
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    public GamesController(IGameService gameService, ICommentService commentService, IMapper mapper)
    {
        _gameService = gameService;
        _commentService = commentService;
        _mapper = mapper;
    }

    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<int> GetTotalGamesCount()
    {
        return await _gameService.GetTotalCountAsync();
    }
    
    [HttpGet]
    public async Task<ICollection<GameViewModel>> GetAllAsync()
    {
        var games = await _gameService.GetAllAsync();
        var result = _mapper.Map<ICollection<GameViewModel>>(games);

        return result;
    }

    [HttpGet("{gameKey}")]
    public async Task<ActionResult<GameViewModel>> GetWithDetailsAsync([FromRoute] string gameKey)
    {
        var game = await _gameService.GetByKeyAsync(gameKey);
        var result = _mapper.Map<GameViewModel>(game);

        return Ok(result);
    }

    [HttpGet("{gameKey}/download")]
    public async Task<ActionResult<byte[]>> GetFileAsync([FromRoute] string gameKey)
    {
        var result = await _gameService.GetFileAsync(gameKey);

        return Ok(result);
    }

    [HttpPost("new")]
    public async Task<ActionResult<GameViewModel>> CreateAsync([FromBody] GameCreateRequestModel request)
    {
        var createModel = _mapper.Map<GameCreateModel>(request);

        var game = await _gameService.CreateAsync(createModel);
        var result = _mapper.Map<GameViewModel>(game);

        return Ok(result);
    }

    [HttpPost("{gameKey}/newcomment")]
    public async Task<ActionResult> CommentGameAsync([FromHybrid] CommentCreateRequestModel request)
    {
        var createModel = _mapper.Map<CommentCreateModel>(request);

        await _commentService.CommentGameAsync(createModel);

        return Ok();
    }

    [HttpGet("{gameKey}/comments")]
    public async Task<ICollection<CommentViewModel>> GetCommentsAsync([FromRoute] string gameKey)
    {
        var comments = await _commentService.GetCommentsByGameKeyAsync(gameKey);
        var result = _mapper.Map<ICollection<CommentViewModel>>(comments);
        
        return result;
    }

    [HttpPost("update")]
    public async Task<ActionResult> UpdateAsync([FromBody] GameEditRequestModel request)
    {
        var game = _mapper.Map<GameUpdateModel>(request);
        await _gameService.UpdateAsync(game);

        return Ok();
    }

    [HttpPost("remove")]
    public async Task<ActionResult> DeleteAsync([FromBody] Guid id)
    {
        await _gameService.DeleteAsync(id);

        return Ok();
    }
}