using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Genres;
using GameStore.Core.Models.Genres.Specifications;
using GameStore.Core.Models.ServiceModels.Genres;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Services;

public class GenreService : IGenreService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GenreService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<Genre> Repository => _unitOfWork.GetRepository<Genre>();
    
    public async Task<ICollection<Genre>> GetAllAsync()
    {
        var result = await Repository.GetBySpecAsync(new GenresListSpec());

        return result;
    }

    public async Task<Genre> GetByIdAsync(Guid id)
    {
        var result = await Repository.GetSingleOrDefaultBySpecAsync(new GenreByIdWithDetailsSpec(id))
                     ?? throw new ItemNotFoundException(typeof(Genre), id);

        return result;
    }

    public async Task CreateAsync(GenreCreateModel createModel)
    {
        var genre = _mapper.Map<Genre>(createModel);

        await Repository.AddAsync(genre);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(GenreUpdateModel updateModel)
    {
        var genre = await Repository.GetSingleOrDefaultBySpecAsync(new GenreByIdSpec(updateModel.Id))
                    ?? throw new ItemNotFoundException(typeof(Genre), updateModel.Id, nameof(updateModel.Id));

        UpdateValues(updateModel, genre);

        await Repository.UpdateAsync(genre);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var genre = await Repository.GetSingleOrDefaultBySpecAsync(new GenreByIdSpec(id))
                                 ?? throw new ItemNotFoundException(typeof(Genre), id);

        await SetNewParentForChildrenGenres(genre);
        genre.IsDeleted = true;
        
        await Repository.UpdateAsync(genre);
        await _unitOfWork.SaveChangesAsync();
    }

    private void UpdateValues(GenreUpdateModel updateModel, Genre genre)
    {
        genre.Name = updateModel.Name;
        genre.ParentId = updateModel.ParentId != Guid.Empty ? updateModel.ParentId : null;
    }


    private async Task SetNewParentForChildrenGenres(Genre genre)
    {
        var children = await Repository.GetBySpecAsync(new GenresByParentIdSpec(genre.Id));
        
        var newParentId = genre.ParentId;

        foreach (var child in children)
        {
            child.ParentId = newParentId;
            await Repository.UpdateAsync(child);
        }
    }
}