using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Extensions;
using GameStore.Core.Helpers.GameKeyGeneration;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Dto.Specifications;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Mongo.Products.Filters;
using GameStore.Core.Models.Mongo.Products.Specifications;
using GameStore.Core.Models.Mongo.Suppliers;
using GameStore.Core.Models.Mongo.Suppliers.Specifications;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Filters;
using GameStore.Core.Models.Server.Games.Specifications;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.Genres.Specifications;
using GameStore.Core.Models.Server.Publishers;
using GameStore.Core.Models.Server.Publishers.Specifications;
using GameStore.Core.PagedResult;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.SharedKernel.Specifications.Filters;
using SpecificationExtensions.Core.Extensions;

namespace GameStore.Core.Services;

public class SearchService : ISearchService
{
    private static readonly DateTime AddedToStoreAtForMongo = new(2022, 6, 2);
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public SearchService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<Game> GamesRepository => _unitOfWork.GetEfRepository<Game>();

    private IRepository<Product> ProductsRepository => _unitOfWork.GetMongoRepository<Product>();

    private IRepository<Genre> GenresRepository => _unitOfWork.GetEfRepository<Genre>();

    private IRepository<Publisher> PublishersRepository => _unitOfWork.GetEfRepository<Publisher>();

    private IRepository<Supplier> SuppliersRepository => _unitOfWork.GetMongoRepository<Supplier>();

    public async Task<PagedResult<ProductDto>> GetProductDtosByFilterAsync(AllProductsFilter filter)
    {
        filter.GenresIds = await GetGenresWithChildrenAsync(filter.GenresIds);
        var filteredGames = await GetFilteredGamesAsync(filter);
        var filteredProducts = await GetFilteredProductsAsync(filter);

        await SetProductsGameKeysAsync(filteredProducts);

        var mappedGames = _mapper.Map<IEnumerable<ProductDto>>(filteredGames);
        var mappedProducts = _mapper.Map<IEnumerable<ProductDto>>(filteredProducts);
        var mappedItems = mappedGames.Concat(mappedProducts);

        var uniqueItems = mappedItems.DistinctBy(item => item.Key).ToList();

        var sortedItems = SortItems(filter, uniqueItems);

        var paginationFilter = filter as PaginationFilter;
        var totalItemsCount = filteredGames.Count + filteredProducts.Count;
        var pagedItems = EnablePaging(sortedItems, paginationFilter);
        var result = new PagedResult<ProductDto>(pagedItems, totalItemsCount, paginationFilter);

        return result;
    }

    public async Task<ProductDto> GetProductDtoByGameKeyOrDefaultAsync(string gameKey)
    {
        ProductDto result = null;

        var gamesSpec = new GamesSpec().ByKey(gameKey).WithDetails();
        var game = await GamesRepository.GetSingleOrDefaultBySpecAsync(gamesSpec);

        if (IsGameFoundInServer(game))
        {
            result = _mapper.Map<ProductDto>(game);

            await IncludePublisher(game, result);
        }

        if (IsGameNotFoundInServer(game))
        {
            var productsSpec = new ProductsSpec().ByGameKey(gameKey).WithDetails();

            var product = await ProductsRepository.GetFirstOrDefaultBySpecAsync(productsSpec);

            if (IsProductFoundInMongo(product))
            {
                result = _mapper.Map<ProductDto>(product);
                result.AddedToStoreAt = AddedToStoreAtForMongo;

                await IncludeGenre(product, result);
            }
        }

        return result;
    }

    public async Task<PublisherDto> GetPublisherDtoByCompanyNameOrDefaultAsync(string companyName)
    {
        PublisherDto result = null;

        var publishersSpec = new PublishersSpec().ByName(companyName);
        var publisher = await PublishersRepository.GetSingleOrDefaultBySpecAsync(publishersSpec);

        if (IsPublisherFoundInServer(publisher))
        {
            result = _mapper.Map<PublisherDto>(publisher);
        }

        if (IsPublisherNotFoundInServer(publisher))
        {
            var suppliersSpec = new SuppliersSpec().ByName(companyName);
            var supplier = await SuppliersRepository.GetFirstOrDefaultBySpecAsync(suppliersSpec);

            if (IsSupplierFoundInMongo(supplier))
            {
                result = _mapper.Map<PublisherDto>(supplier);
            }
        }

        return result;
    }

    public async Task<bool> IsGameKeyExistsAsync(string gameKey)
    {
        return await GamesRepository.AnyAsync(new GamesSpec().ByKey(gameKey)) ||
               await ProductsRepository.AnyAsync(new ProductsSpec().ByGameKey(gameKey));
    }

    private async Task<List<Game>> GetFilteredGamesAsync(AllProductsFilter filter)
    {
        var gamesFilter = _mapper.Map<GameSearchFilter>(filter);

        var spec = new GamesByFilterSpec(gamesFilter);
        var filteredGames = await GamesRepository.GetBySpecAsync(spec);

        return filteredGames;
    }

    private async Task<List<Product>> GetFilteredProductsAsync(AllProductsFilter filter)
    {
        var productsFilter = await GetProductsFilterAsync(filter);

        var allServerGameKeysSpec = new GamesSpec().LoadAll().Select(game => game.Key);

        var allServerGameKeys = await GamesRepository.SelectBySpecAsync(allServerGameKeysSpec);

        var productsByFilterExceptServerGameKeysSpec =
            new ProductsByFilterSpec(productsFilter).ExceptBy(allServerGameKeys, product => product.GameKey);

        var filteredProducts = await ProductsRepository.GetBySpecAsync(productsByFilterExceptServerGameKeysSpec);

        return filteredProducts;
    }

    private async Task<ProductFilter> GetProductsFilterAsync(AllProductsFilter filter)
    {
        var productsFilter = _mapper.Map<ProductFilter>(filter);
        productsFilter.IsCategoriesIdsFilterEnabled = filter.GenresIds.Any();
        productsFilter.IsSuppliersIdsFilterEnabled = filter.PublishersNames.Any();

        if (productsFilter.IsCategoriesIdsFilterEnabled)
        {
            var categoriesIdsSpec =
                new GenresSpec().ByIds(filter.GenresIds).WithCategoryId().Select(genre => genre.CategoryId.Value);

            var genresMongoIds = await GenresRepository.SelectBySpecAsync(categoriesIdsSpec);

            productsFilter.CategoriesIds = genresMongoIds;
        }

        if (productsFilter.IsSuppliersIdsFilterEnabled)
        {
            var suppliersIdsSpec =
                new SuppliersSpec().ByNames(filter.PublishersNames).Select(supplier => supplier.SupplierId);

            var suppliersMongoIds = await SuppliersRepository.SelectBySpecAsync(suppliersIdsSpec);

            productsFilter.SuppliersIds = suppliersMongoIds;
        }

        return productsFilter;
    }

    private async Task<IEnumerable<Guid>> GetGenresWithChildrenAsync(IEnumerable<Guid> genresIds)
    {
        var genresByIdsWithDetailsSpec = new GenresSpec().ByIds(genresIds).WithDetails();
        var genres = await GenresRepository.GetBySpecAsync(genresByIdsWithDetailsSpec);

        var allChildren = GetAllChildrenGenres(genres);

        var result = allChildren.Select(genre => genre.Id);

        return result;
    }

    private IEnumerable<Genre> GetAllChildrenGenres(IEnumerable<Genre> genres)
    {
        var result = genres.ToList();

        foreach (var genre in genres)
        {
            var children = GetAllChildrenGenres(genre.SubGenres);
            result.AddRange(children);
        }

        return result;
    }

    private IEnumerable<ProductDto> SortItems(AllProductsFilter filter, IEnumerable<ProductDto> uniqueItems)
    {
        var isPublishedAtFilterEnabled = filter.PublishedAtState != GameSearchFilterPublishedAtState.Default;
        var isPlatformFilterEnabled = filter.PlatformsIds.Any();

        var orderByDatabase = isPlatformFilterEnabled || isPublishedAtFilterEnabled;

        var sortedItems = new ProductDtoSortSpec(filter.OrderByState, orderByDatabase).Evaluate(uniqueItems);

        return sortedItems;
    }

    private async Task SetProductsGameKeysAsync(IEnumerable<Product> filteredProducts)
    {
        foreach (var product in filteredProducts.Where(product => product.GameKey == null))
        {
            var key = GameKeyGenerator.GenerateGameKey(product.ProductName);

            product.GameKey = key;
            await ProductsRepository.UpdateAsync(product);
        }
    }

    private async Task IncludeGenre(Product product, ProductDto result)
    {
        var genreByCategoryIdSpec = new GenresSpec().ByCategoryId(product.CategoryId);

        var genre = await GenresRepository.GetSingleOrDefaultBySpecAsync(genreByCategoryIdSpec);

        if (genre is not null)
        {
            result.Genres.Add(genre);
        }
    }

    private async Task IncludePublisher(Game game, ProductDto result)
    {
        var publisherDto = await GetPublisherDtoByCompanyNameOrDefaultAsync(game.PublisherName);

        if (publisherDto is not null)
        {
            result.Publisher = publisherDto;
        }
    }

    private static IEnumerable<ProductDto> EnablePaging(IEnumerable<ProductDto> items,
                                                        PaginationFilter paginationFilter)
    {
        var result = items.Skip(paginationFilter.Skip).Take(paginationFilter.Take).ToList();

        return result;
    }

    private bool IsProductFoundInMongo(Product product)
    {
        return product is not null;
    }

    private bool IsGameNotFoundInServer(Game game)
    {
        return game is null;
    }

    private bool IsGameFoundInServer(Game game)
    {
        return game is not null;
    }

    private bool IsPublisherFoundInServer(Publisher publisher)
    {
        return publisher is not null;
    }

    private bool IsPublisherNotFoundInServer(Publisher publisher)
    {
        return publisher is null;
    }

    private bool IsSupplierFoundInMongo(Supplier supplier)
    {
        return supplier is not null;
    }
}