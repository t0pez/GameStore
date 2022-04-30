using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.ServiceModels.Genres;

namespace GameStore.Core.Interfaces;

public interface IGenreService
{
    public Task<ICollection<Genre>> GetAllAsync();
    public Task<Genre> GetByIdAsync(Guid id);
    public Task CreateAsync(GenreCreateModel createModel);
    public Task UpdateAsync(GenreUpdateModel updateModel);
    public Task DeleteAsync(Guid id);
}