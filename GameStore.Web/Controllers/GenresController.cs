using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.ServiceModels.Genres;
using GameStore.Web.Helpers;
using GameStore.Web.Infrastructure.Authorization;
using GameStore.Web.Models.Genre;
using GameStore.Web.ViewModels.Genres;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.Controllers;

[Route("genres")]
public class GenresController : Controller
{
    private readonly IGenreService _genreService;
    private readonly IMapper _mapper;

    public GenresController(IGenreService genreService, IMapper mapper)
    {
        _genreService = genreService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreListViewModel>>> GetAllAsync()
    {
        var genres = await _genreService.GetAllAsync();
        var result = _mapper.Map<IEnumerable<GenreListViewModel>>(genres);

        return View(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GenreViewModel>> GetWithDetailsAsync([FromRoute] Guid id)
    {
        var genre = await _genreService.GetByIdAsync(id);
        var result = _mapper.Map<GenreViewModel>(genre);

        return View(result);
    }

    [HttpGet("new")]
    [Authorize(Roles = ApiRoles.Manager)]
    public async Task<ActionResult> CreateAsync()
    {
        var genres = await _genreService.GetAllAsync();
        var genresSelectList = genres.Select(genre => new SelectListItem(genre.Name, genre.Id.ToString())).ToList();
        genresSelectList.Add(new SelectListItem("", Guid.Empty.ToString()));

        ViewData[ViewKeys.Genres.ParentGenres] = genresSelectList;

        return View(new GenreCreateRequestModel());
    }

    [HttpPost("new")]
    [Authorize(Roles = ApiRoles.Manager)]
    public async Task<ActionResult> CreateAsync(GenreCreateRequestModel model)
    {
        var createModel = _mapper.Map<GenreCreateModel>(model);
        await _genreService.CreateAsync(createModel);

        return RedirectToAction("GetAll", "Genres");
    }

    [HttpGet("update/{id}")]
    [Authorize(Roles = ApiRoles.Manager)]
    public async Task<ActionResult<GenreUpdateRequestModel>> UpdateAsync([FromRoute] Guid id)
    {
        var genres = await _genreService.GetAllAsync();
        var currentGenre = genres.FirstOrDefault(genre => genre.Id == id);

        if (currentGenre is null)
        {
            return BadRequest();
        }

        var genresSelectList = genres.Except(new[] { currentGenre })
                                     .Select(genre => new SelectListItem(genre.Name, genre.Id.ToString()))
                                     .ToList();

        genresSelectList.Add(new SelectListItem("", Guid.Empty.ToString()));

        SetParentGenreAsSelected(currentGenre.ParentId, genresSelectList);

        ViewData[ViewKeys.Genres.ParentGenres] = genresSelectList;

        return View(new GenreUpdateRequestModel
        {
            Id = id,
            Name = currentGenre.Name
        });
    }

    [HttpPost("update/{id}")]
    [Authorize(Roles = ApiRoles.Manager)]
    public async Task<ActionResult> UpdateAsync(GenreUpdateRequestModel model)
    {
        var updateModel = _mapper.Map<GenreUpdateModel>(model);
        await _genreService.UpdateAsync(updateModel);

        return RedirectToAction("GetAll", "Genres");
    }

    [HttpPost("delete")]
    [Authorize(Roles = ApiRoles.Manager)]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await _genreService.DeleteAsync(id);

        return RedirectToAction("GetAll", "Genres");
    }

    private void SetParentGenreAsSelected(Guid? parentGenreId, IEnumerable<SelectListItem> genresSelectList)
    {
        if (parentGenreId is not null)
        {
            genresSelectList.First(item => item.Value == parentGenreId.ToString()).Selected = true;
        }
        else
        {
            genresSelectList.First(item => item.Value == Guid.Empty.ToString()).Selected = true;
        }
    }
}