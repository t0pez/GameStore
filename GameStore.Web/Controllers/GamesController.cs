﻿using AutoMapper;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Core.Helpers.AliasCrafting;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Web.Filters;
using GameStore.Web.Models.Comment;
using GameStore.Web.Models.Game;
using GameStore.Web.ViewModels.Comments;
using GameStore.Web.ViewModels.Games;
using HybridModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.Controllers;

[ServiceFilter(typeof(WorkTimeTrackingFilter))]
[Route("games")]
public class GamesController : Controller
{
    private readonly IGameService _gameService;
    private readonly ICommentService _commentService;
    private readonly IPublisherService _publisherService;
    private readonly IGenreService _genreService;
    private readonly IPlatformTypeService _platformTypeService;
    private readonly IAliasCraft _gameKeyAliasCraft;
    private readonly IMapper _mapper;

    public GamesController(IGameService gameService, ICommentService commentService, IPublisherService publisherService,
                           IGenreService genreService, IPlatformTypeService platformTypeService, IMapper mapper)
    {
        _gameService = gameService;
        _commentService = commentService;
        _publisherService = publisherService;
        _genreService = genreService;
        _platformTypeService = platformTypeService;
        _mapper = mapper;
        _gameKeyAliasCraft =
            new AliasCraftBuilder()
                .Values("_", " ").ReplaceWith("-")
                .Values(",", ".", ":", "?").Delete()
                .Build();
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
    public async Task<FileContentResult> GetFileAsync([FromRoute] string gameKey)
    {
        var bytes = await _gameService.GetFileAsync(gameKey);
        var fileName = gameKey + ".txt";

        return File(bytes, "application/force-download", fileName);
    }

    [HttpGet("new")]
    public async Task<ActionResult> CreateAsync()
    {
        await FillViewData();

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
    public async Task<ActionResult> CreateCommentAsync([FromHybrid] CommentCreateRequestModel request)
    {
        if (request.ParentId is null)
        {
            var createModel = _mapper.Map<CommentCreateModel>(request);
            await _commentService.CommentGameAsync(createModel);
        }
        else
        {
            var createModel = _mapper.Map<ReplyCreateModel>(request);
            await _commentService.ReplyCommentAsync(createModel);
        }

        return RedirectToAction("GetComments", new { gameKey = request.GameKey });
    }
    
    [HttpPost("{gameKey}/comment/update")]
    public async Task<ActionResult> UpdateCommentAsync([FromHybrid] CommentUpdateRequestModel request)
    {
        var updateModel = _mapper.Map<CommentUpdateModel>(request);
        
        await _commentService.UpdateAsync(updateModel);

        return RedirectToAction("GetComments", new { gameKey = request.GameKey });
    }
    
    [HttpPost("{gameKey}/comment/delete")]
    public async Task<ActionResult> DeleteCommentAsync(Guid id, string gameKey)
    {
        await _commentService.DeleteAsync(id);

        return RedirectToAction("GetComments", new { gameKey });
    }

    [HttpGet("{gameKey}/comments")]
    public async Task<ActionResult<ICollection<CommentViewModel>>> GetCommentsAsync([FromRoute] string gameKey)
    {
        var comments = await _commentService.GetCommentsByGameKeyAsync(gameKey);
        var result = _mapper.Map<ICollection<CommentViewModel>>(comments);

        return View(result);
    }

    [HttpGet("{gameKey}/update")]
    public async Task<ActionResult> UpdateAsync([FromRoute] string gameKey)
    {
        await FillViewData();

        var gameToUpdate = await _gameService.GetByKeyAsync(gameKey);
        var mapped = _mapper.Map<GameUpdateRequestModel>(gameToUpdate);
        
        return View(mapped);
    }

    [HttpPost("{gameKey}/update")]
    public async Task<ActionResult> UpdateAsync(GameUpdateRequestModel request)
    {
        var game = _mapper.Map<GameUpdateModel>(request);
        await _gameService.UpdateAsync(game);

        return RedirectToAction("GetWithDetails", "Games", new { gameKey = request.Key });
    }

    [HttpPost("remove")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await _gameService.DeleteAsync(id);

        return RedirectToAction("GetAll", "Games");
    }

    [HttpPost("key/{name}")]
    public async Task<ActionResult> GenerateKeyAsync([FromRoute] string name)
    {
        return new JsonResult(new { key = _gameKeyAliasCraft.CreateAlias(name) });
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

    private async Task FillViewData()
    {
        var publishers = await _publisherService.GetAllAsync();
        var publishersSelectList = new SelectList(publishers, nameof(Publisher.Id), nameof(Publisher.Name));
        ViewData["Publishers"] = publishersSelectList;

        var genres = await _genreService.GetAllAsync();
        var genresSelectList = new SelectList(genres, nameof(Genre.Id), nameof(Genre.Name));
        ViewData["Genres"] = genresSelectList;

        var platforms = await _platformTypeService.GetAllAsync();
        var platformsSelectList = new SelectList(platforms, nameof(PlatformType.Id), nameof(PlatformType.Name));
        ViewData["Platforms"] = platformsSelectList;
    }
}