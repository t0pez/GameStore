using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Records;
using GameStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Web.Filters;
using GameStore.Web.ViewModels.Comments;
using GameStore.Web.ViewModels.Games;
using HybridModelBinding;

namespace GameStore.Web.Controllers;

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

    [HttpGet("count")]
    [ResponseCache(Duration = 60)]
    public async Task<int> GetTotalGamesCount()
    {
        return await _gameService.GetTotalCountAsync();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameListViewModel>>> GetAllAsync()
    {
        var games = await _gameService.GetAllAsync();
        var result = _mapper.Map<IEnumerable<GameListViewModel>>(games);

        return View(result);
    }

    [HttpGet("{gameKey}")]
    public async Task<ActionResult<GameViewModel>> GetWithDetailsAsync([FromRoute] string gameKey)
    {
        var game = await _gameService.GetByKeyAsync(gameKey);
        var result = _mapper.Map<GameViewModel>(game);

        return View(result);
    }

    [HttpGet("{gameKey}/download")]
    public async Task<ActionResult<byte[]>> GetFileAsync([FromRoute] string gameKey)
    {
        var bytes = await _gameService.GetFileAsync(gameKey);
        var fileName = gameKey;

        return File(bytes, "application/force-download", fileName);
    }

    [HttpGet("new")]
    public async Task<ActionResult> CreateAsync()
    {
        return View(new GameCreateRequestModel());
    }

    [HttpPost("new")]
    public async Task<ActionResult> CreateAsync(GameCreateRequestModel request)
    {
        if (ModelState.IsValid == false ||
            HttpContext.Request.Form.Files.Count == 0)
        {
            return View("Error");
        }

        var createModel = _mapper.Map<GameCreateModel>(request);
        createModel.File = await GetBytesFromFormFile();

        var game = await _gameService.CreateAsync(createModel);

        return RedirectToAction("GetWithDetails", new { gameKey = game.Key });
    }

    [HttpPost("{gameKey}/newcomment")]
    public async Task<ActionResult> CommentGameAsync([FromHybrid] CommentCreateRequestModel request)
    {
        var createModel = _mapper.Map<CommentCreateModel>(request);

        await _commentService.CommentGameAsync(createModel);

        return RedirectToAction("GetComments", new { request.GameKey });
    }

    [HttpGet("{gameKey}/comments")]
    public async Task<ActionResult<ICollection<CommentViewModel>>> GetCommentsAsync([FromRoute] string gameKey)
    {
        var comments = await _commentService.GetCommentsByGameKeyAsync(gameKey);
        var result = _mapper.Map<ICollection<CommentViewModel>>(comments);

        return View(result);
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

    private async Task<byte[]> GetBytesFromFormFile()
    {
        var file = HttpContext.Request.Form.Files.FirstOrDefault()
                   ?? throw new ArgumentException("Form file must be added");

        await using var bytes = new MemoryStream();
        await file.CopyToAsync(bytes);
        var fileBytes = bytes.ToArray();

        return fileBytes;
    }
}