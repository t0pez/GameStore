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

namespace GameStore.Web.Controllers
{
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

        [HttpGet("games/")]
        public async Task<ICollection<Game>> GetAll()
        {
            var result = await _gameService.GetAllAsync();

            _logger.LogInformation($"GetAll request done. Result.Count() = {result.Count}");

            return result;
        }

        [HttpGet("games/{gameKey}")]
        public async Task<ActionResult<Game>> GetWithDetails(string gameKey)
        {
            var result = await _gameService.GetByKeyAsync(gameKey);

            _logger.LogInformation($"GetWithDetails request done");


            return Ok(result);
        }

        [HttpPost("games/{gameKey}/download")]
        public async Task<ActionResult<byte[]>> GetFile([FromRoute(Name = "gameKey")] string gameKey)
        {
            var result = await _gameService.GetFileAsync(gameKey);

            _logger.LogInformation($"GetFile request done");

            return Ok(result);
        }

        [HttpPost("games/new")]
        public async Task<ActionResult<Game>> Create(GameCreateRequestModel request)
        {
            var createModel = _mapper.Map<GameCreateModel>(request);

            var result = await _gameService.CreateAsync(createModel);

            _logger.LogInformation("Create request done");

            return Ok(result);
        }

        [HttpPost("games/{gameKey}/newcomment")]
        public async Task<ActionResult> CommentGame([FromHybrid] CommentCreateRequestModel request)
        {
            var createModel = _mapper.Map<CommentCreateModel>(request);

            await _commentService.CommentGameAsync(createModel);

            _logger.LogInformation("CommentGame request done.");

            return Ok();
        }

        [HttpGet("games/{gameKey}/comments")]
        public async Task<ICollection<Comment>> GetComments([FromRoute(Name = "gameKey")] string gameKey)
        {
            var result = await _commentService.GetCommentsByGameKeyAsync(gameKey);

            _logger.LogInformation("GetComments request done.");

            return result;
        }

        [HttpPost("games/update")]
        public async Task<ActionResult<Game>> Edit([FromBody] GameEditRequestModel request)
        {
            var game = _mapper.Map<GameUpdateModel>(request);
            await _gameService.UpdateAsync(game);

            _logger.LogInformation("Edit request done.");

            return Ok();
        }

        [HttpPost("games/remove")]
        public async Task<ActionResult> Delete([FromBody] Guid id)
        {
            await _gameService.DeleteAsync(id);

            _logger.LogInformation("Delete request done.");

            return Ok();
        }
    }
}
