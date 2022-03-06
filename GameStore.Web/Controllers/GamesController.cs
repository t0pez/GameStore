using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Records;
using GameStore.Web.Models;
using HybridModelBinding;
using Microsoft.AspNetCore.Mvc;
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

        public GamesController(IGameService gameService, ICommentService commentService, IMapper mapper)
        {
            _gameService = gameService;
            _commentService = commentService;
            _mapper = mapper;
        }

        [HttpGet("games")]
        public async Task<ICollection<Game>> GetAll()
        {
            var result = await _gameService.GetAllAsync();

            return result;
        }

        [HttpGet("games/{gameKey}")]
        public async Task<ActionResult<Game>> GetWithDetails(string gameKey)
        {
            var result = await _gameService.GetByKeyAsync(gameKey);

            return Ok(result);
        }

        [HttpPost("games/{gameKey}/download")]
        public async Task<ActionResult<byte[]>> GetFile([FromRoute(Name = "gameKey")] string gameKey)
        {
            var result = await _gameService.GetFileAsync(gameKey);

            return Ok(result);
        }

        [HttpPost("games/new")]
        public async Task<ActionResult<Game>> Create(CreateGameRequest request)
        {
            var createModel = _mapper.Map<GameCreateModel>(request);

            var result = await _gameService.CreateAsync(createModel);

            return Ok(result);
        }

        [HttpPost("games/{gameKey}/newcomment")]
        public async Task<ActionResult> CommentGame([FromHybrid] CreateCommentRequest request)
        {
            var createModel = _mapper.Map<CommentCreateModel>(request);

            await _commentService.CommentGameAsync(createModel);

            return Ok();
        }

        [HttpGet("games/{gameKey}/comments")]
        public async Task<ICollection<Comment>> GetComments([FromRoute(Name = "gameKey")] string gameKey)
        {
            var result = await _commentService.GetCommentsByGameKeyAsync(gameKey);

            return result;
        }

        [HttpPost("games/update")]
        public async Task<ActionResult<Game>> Edit([FromBody] Game game)
        {
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
}
