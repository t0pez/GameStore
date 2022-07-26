using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.PlatformTypes;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.SharedKernel.Specifications.Filters;
using GameStore.Web.Extensions;
using GameStore.Web.Filters;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Game;
using GameStore.Web.ViewModels.Games;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.Controllers;

[ServiceFilter(typeof(WorkTimeTrackingFilter))]
[Route("games")]
public class GamesController : Controller
{
    private readonly IGameService _gameService;
    private readonly IGenreService _genreService;
    private readonly IMapper _mapper;
    private readonly IOrderService _orderService;
    private readonly IPlatformTypeService _platformTypeService;
    private readonly IPublisherService _publisherService;
    private readonly ISearchService _searchService;
    private readonly IUserCookieService _userCookieService;

    public GamesController(ISearchService searchService, IGameService gameService, IPublisherService publisherService,
                           IGenreService genreService, IPlatformTypeService platformTypeService, IMapper mapper,
                           IOrderService orderService, IUserCookieService userCookieService)
    {
        _searchService = searchService;
        _gameService = gameService;
        _publisherService = publisherService;
        _genreService = genreService;
        _platformTypeService = platformTypeService;
        _mapper = mapper;
        _orderService = orderService;
        _userCookieService = userCookieService;
    }

    [HttpGet("count")]
    [ResponseCache(Duration = 60)]
    public async Task<int> GetTotalGamesCount()
    {
        return await _gameService.GetTotalCountAsync();
    }

    [HttpGet]
    public async Task<ActionResult<GamesGetAllViewModel>> GetAllAsync(GamesFilterRequestModel filterRequest,
                                                                      int? currentPage, int? pageSize)
    {
        filterRequest ??= new GamesFilterRequestModel();
        filterRequest.CurrentPage = currentPage ?? PaginationFilter.DefaultCurrentPage;
        filterRequest.PageSize = pageSize ?? PaginationFilter.DefaultPageSize;

        var filter = _mapper.Map<AllProductsFilter>(filterRequest);
        var games = await _searchService.GetProductDtosByFilterAsync(filter);

        await FillFilterData(filterRequest);

        var result = new GamesGetAllViewModel
        {
            GamesPaged = games,
            Filter = filterRequest
        };

        return View(result);
    }

    [HttpGet("{gameKey}")]
    public async Task<ActionResult<GameViewModel>> GetWithDetailsAsync([FromRoute] string gameKey)
    {
        var game = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(gameKey);

        await IncreaseViews(game);

        ViewData["CustomerHasActiveOrder"] = await IsCustomerHasActiveOrder();

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
        if (await _gameService.IsGameKeyAlreadyExists(request.Key))
        {
            ModelState.AddModelError(nameof(request.Key), "Same game key already exists");
        }

        if (ModelState.IsValid == false)
        {
            return View(request);
        }

        var createModel = _mapper.Map<GameCreateModel>(request);

        var game = await _gameService.CreateAsync(createModel);

        return RedirectToAction("GetWithDetails", new { gameKey = game.Key });
    }

    [HttpGet("{gameKey}/update")]
    public async Task<ActionResult> UpdateAsync([FromRoute] string gameKey)
    {
        await FillViewData();

        var gameToUpdate = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(gameKey);
        var mapped = _mapper.Map<GameUpdateRequestModel>(gameToUpdate);

        return View(mapped);
    }

    [HttpPost("{gameKey}/update")]
    public async Task<ActionResult> UpdateAsync(GameUpdateRequestModel request, string gameKey)
    {
        if (await _gameService.IsGameKeyAlreadyExists(request.Key))
        {
            ModelState.AddModelError(nameof(request.Key), "Same game key already exists");
        }

        if (ModelState.IsValid == false)
        {
            await FillViewData();
            return View(request);
        }

        var updateModel = _mapper.Map<GameUpdateModel>(request);
        updateModel.OldGameKey = gameKey;

        await _gameService.UpdateFromEndpointAsync(updateModel);

        return RedirectToAction("GetWithDetails", "Games", new { gameKey = request.Key });
    }

    [HttpPost("{gameKey}/remove")]
    public async Task<ActionResult> DeleteAsync(string gameKey, int database)
    {
        await _gameService.DeleteAsync(gameKey, (Database)database);

        return RedirectToAction("GetAll", "Games");
    }

    [HttpPost("key/{name}")]
    public async Task<ActionResult> GenerateKeyAsync([FromRoute] string name)
    {
        var key = Regex.Replace(name.Trim().ToLower(), @"\s{2,}", " ").Replace(' ', '-');
        return new JsonResult(new { key });
    }

    private async Task IncreaseViews(ProductDto product)
    {
        product.Views++;

        var updateModel = _mapper.Map<GameUpdateModel>(product);

        await _gameService.UpdateAsync(updateModel);
    }

    private async Task<bool> IsCustomerHasActiveOrder()
    {
        if (_userCookieService.TryGetCookiesUserId(HttpContext.Request.Cookies, out var customerId))
        {
            return await _orderService.IsCustomerHasActiveOrder(customerId);
        }

        return false;
    }

    private async Task FillViewData()
    {
        var genresSelectList = await GetGenresSelectList();
        ViewData["Genres"] = genresSelectList;

        var platformsSelectList = await GetPlatformsSelectList();
        ViewData["Platforms"] = platformsSelectList;

        var publishersSelectList = await GetPublishersSelectList();
        ViewData["Publishers"] = publishersSelectList;
    }

    private async Task FillFilterData(GamesFilterRequestModel filterRequest)
    {
        filterRequest.Genres = await GetGenresSelectList();
        foreach (var genre in filterRequest.Genres)
        {
            genre.Selected = filterRequest.SelectedGenres.Contains(genre.Value);
        }

        filterRequest.Platforms = await GetPlatformsSelectList();
        foreach (var platform in filterRequest.Platforms)
        {
            platform.Selected = filterRequest.SelectedPlatforms.Contains(platform.Value);
        }

        filterRequest.Publishers = await GetPublishersSelectList();
        foreach (var publisher in filterRequest.Publishers)
        {
            publisher.Selected = filterRequest.SelectedPublishers.Contains(publisher.Value);
        }

        var orderBySelectList = new SelectList(
            Enum.GetValues(typeof(GameSearchFilterOrderByState)).OfType<Enum>()
                .Select(enumElement => new SelectListItem
                {
                    Value = Convert.ToInt32(enumElement).ToString(),
                    Text = enumElement.GetDisplayName()
                }),
            nameof(SelectListItem.Value), nameof(SelectListItem.Text),
            filterRequest.OrderByState);

        ViewData["OrderBy"] = orderBySelectList;
    }

    private async Task<SelectList> GetGenresSelectList()
    {
        var genres = await _genreService.GetAllAsync();
        var genresSelectList = new SelectList(genres, nameof(Genre.Id), nameof(Genre.Name));

        return genresSelectList;
    }

    private async Task<SelectList> GetPlatformsSelectList()
    {
        var platforms = await _platformTypeService.GetAllAsync();
        var platformsSelectList = new SelectList(platforms, nameof(PlatformType.Id), nameof(PlatformType.Name));

        return platformsSelectList;
    }

    private async Task<SelectList> GetPublishersSelectList()
    {
        var publishers = await _publisherService.GetAllAsync();
        var publishersSelectList = new SelectList(publishers, nameof(Publisher.Name), nameof(Publisher.Name));

        return publishersSelectList;
    }
}