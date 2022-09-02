using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Helpers.GameKeyGeneration;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Filters;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.SharedKernel.Specifications.Filters;
using GameStore.Web.Extensions;
using GameStore.Web.Filters;
using GameStore.Web.Helpers;
using GameStore.Web.Infrastructure.Authorization;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Game;
using GameStore.Web.ViewModels.Games;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IPublisherAuthHelper _publisherAuth;
    private readonly IPublisherService _publisherService;
    private readonly ISearchService _searchService;
    private readonly IUserCookieService _userCookieService;

    public GamesController(ISearchService searchService, IGameService gameService, IPublisherService publisherService,
                           IGenreService genreService, IPlatformTypeService platformTypeService, IMapper mapper,
                           IOrderService orderService, IUserCookieService userCookieService,
                           IPublisherAuthHelper publisherAuth)
    {
        _searchService = searchService;
        _gameService = gameService;
        _publisherService = publisherService;
        _genreService = genreService;
        _platformTypeService = platformTypeService;
        _mapper = mapper;
        _orderService = orderService;
        _userCookieService = userCookieService;
        _publisherAuth = publisherAuth;
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

        if (filterRequest.MinPrice.HasValue && filterRequest.MaxPrice.HasValue &&
            filterRequest.MinPrice > filterRequest.MaxPrice)
        {
            ModelState.AddModelError(nameof(filterRequest.MinPrice), "Min price can't be greater than max");
        }

        var filter = _mapper.Map<AllProductsFilter>(filterRequest);
        var games = await _searchService.GetProductDtosByFilterAsync(filter);

        await FillFilterDataAsync(filterRequest);

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
        var game = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(gameKey)
                   ?? throw new ItemNotFoundException(typeof(Game), gameKey);

        await IncreaseViews(game);

        ViewData[ViewKeys.Games.CustomerHasActiveOrder] = await IsCustomerHasActiveOrder();

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

    [Authorize(Roles = ApiRoles.Manager)]
    [HttpGet("new")]
    public async Task<ActionResult> CreateAsync()
    {
        await FillViewDataAsync();

        return View(new GameCreateRequestModel());
    }

    [Authorize(Roles = ApiRoles.Manager)]
    [HttpPost("new")]
    public async Task<ActionResult> CreateAsync(GameCreateRequestModel request)
    {
        if (await _gameService.IsGameKeyAlreadyExists(request.Key))
        {
            ModelState.AddModelError(nameof(request.Key), "Same game key already exists");
        }

        if (ModelState.IsValid == false)
        {
            await FillViewDataAsync();

            return View(request);
        }

        var createModel = _mapper.Map<GameCreateModel>(request);

        var game = await _gameService.CreateAsync(createModel);

        return RedirectToAction("GetWithDetails", new { gameKey = game.Key });
    }

    [Authorize(Roles = ApiRoles.Publisher)]
    [HttpGet("{gameKey}/update")]
    public async Task<ActionResult> UpdateAsync([FromRoute] string gameKey)
    {
        await FillViewDataAsync();

        var gameToUpdate = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(gameKey);

        var isCanEditAsync = await _publisherAuth.CanEditAsync(gameToUpdate.PublisherName);

        if (isCanEditAsync == false)
        {
            return Unauthorized();
        }

        var mapped = _mapper.Map<GameUpdateRequestModel>(gameToUpdate);

        return View(mapped);
    }

    [Authorize(Roles = ApiRoles.Publisher)]
    [HttpPost("{gameKey}/update")]
    public async Task<ActionResult> UpdateAsync(GameUpdateRequestModel request, string gameKey)
    {
        if (ModelState.IsValid == false)
        {
            await FillViewDataAsync();

            return View(request);
        }

        var game = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(gameKey);

        var canEditPublisher = await _publisherAuth.CanEditAsync(game.PublisherName);

        if (canEditPublisher == false)
        {
            return Unauthorized();
        }

        var updateModel = _mapper.Map<GameUpdateModel>(request);
        updateModel.OldGameKey = gameKey;

        await _gameService.UpdateFromEndpointAsync(updateModel);

        return RedirectToAction("GetWithDetails", "Games", new { gameKey = request.Key });
    }

    [Authorize(ApiRoles.Manager)]
    [HttpPost("{gameKey}/remove")]
    public async Task<ActionResult> DeleteAsync(string gameKey, int database)
    {
        await _gameService.DeleteAsync(gameKey, (Database)database);

        return RedirectToAction("GetAll", "Games");
    }

    [HttpPost("key/{name}")]
    public async Task<ActionResult> GenerateKeyAsync([FromRoute] string name)
    {
        var key = GameKeyGenerator.GenerateGameKey(name);

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
        var customerId = _userCookieService.GetCookiesUserId();

        return await _orderService.IsCustomerHasActiveOrderAsync(customerId);
    }

    private async Task FillViewDataAsync()
    {
        var genresSelectList = await GetGenresSelectList();
        ViewData[ViewKeys.Games.Genres] = genresSelectList;

        var platformsSelectList = await GetPlatformsSelectList();
        ViewData[ViewKeys.Games.Platforms] = platformsSelectList;

        var publishersSelectList = await GetPublishersSelectList();
        ViewData[ViewKeys.Games.Publishers] = publishersSelectList;
    }

    private async Task FillFilterDataAsync(GamesFilterRequestModel filterRequest)
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

        var publishedAtSelectList = new SelectList(
            Enum.GetValues(typeof(GameSearchFilterPublishedAtState)).OfType<Enum>()
                .Select(enumElement => new SelectListItem
                 {
                     Value = Convert.ToInt32(enumElement).ToString(),
                     Text = enumElement.GetDisplayName()
                 }),
            nameof(SelectListItem.Value), nameof(SelectListItem.Text),
            filterRequest.PublishedAtByState);

        ViewData[ViewKeys.Games.PublishedAt] = publishedAtSelectList;

        var orderBySelectList = new SelectList(
            Enum.GetValues(typeof(GameSearchFilterOrderByState)).OfType<Enum>()
                .Select(enumElement => new SelectListItem
                 {
                     Value = Convert.ToInt32(enumElement).ToString(),
                     Text = enumElement.GetDisplayName()
                 }),
            nameof(SelectListItem.Value), nameof(SelectListItem.Text),
            filterRequest.OrderByState);

        ViewData[ViewKeys.Games.OrderBy] = orderBySelectList;
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