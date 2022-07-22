using System.Threading.Tasks;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;

namespace GameStore.Core.Interfaces;

public interface ISearchService
{
    public Task<PagedResult.PagedResult<ProductDto>> GetProductDtosByFilterAsync(AllProductsFilter filter);
    public Task<ProductDto> GetProductDtoByGameKeyOrDefaultAsync(string gameKey);
    public Task<PublisherDto> GetPublisherDtoByCompanyNameOrDefaultAsync(string companyName);
    public Task<bool> IsGameKeyExistsAsync(string gameKey);
}