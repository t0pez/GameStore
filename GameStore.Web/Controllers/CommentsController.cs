using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.ServiceModels.Comments;
using GameStore.Web.Models.Comment;
using GameStore.Web.ViewModels.Comments;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

[Route("games")]
public class CommentsController : Controller
{
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    public CommentsController(ICommentService commentService, IMapper mapper)
    {
        _commentService = commentService;
        _mapper = mapper;
    }

    [HttpPost("{gameKey}/newcomment")]
    public async Task<ActionResult> CreateCommentAsync(CommentCreateRequestModel request)
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
    public async Task<ActionResult> UpdateCommentAsync(CommentUpdateRequestModel request)
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
}