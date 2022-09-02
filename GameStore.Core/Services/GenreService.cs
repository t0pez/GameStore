using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.Server.Genres;
using GameStore.Core.Models.Server.Genres.Specifications;
using GameStore.Core.Models.ServiceModels.Genres;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MongoDB.Bson;

namespace GameStore.Core.Services;

public class GenreService : IGenreService
{
    private readonly IMapper _mapper;
    private readonly IMongoLogger _mongoLogger;
    private readonly IUnitOfWork _unitOfWork;

    public GenreService(IMongoLogger mongoLogger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mongoLogger = mongoLogger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<Genre> Repository => _unitOfWork.GetEfRepository<Genre>();

    public async Task<ICollection<Genre>> GetAllAsync()
    {
        var spec = new GenresSpec();
        var result = await Repository.GetBySpecAsync(spec);

        return result;
    }

    public async Task<Genre> GetByIdAsync(Guid id)
    {
        var spec = new GenresSpec().ById(id).WithDetails();

        var result = await Repository.GetSingleOrDefaultBySpecAsync(spec)
                     ?? throw new ItemNotFoundException(typeof(Genre), id);

        result.SubGenres = result.SubGenres.Where(subGenre => subGenre.IsDeleted == false).ToList();

        return result;
    }

    public async Task CreateAsync(GenreCreateModel createModel)
    {
        var genre = _mapper.Map<Genre>(createModel);

        if (createModel.ParentId == Guid.Empty)
        {
            genre.ParentId = null;
        }

        await Repository.AddAsync(genre);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogCreateAsync(genre);
    }

    public async Task UpdateAsync(GenreUpdateModel updateModel)
    {
        var spec = new GenresSpec().ById(updateModel.Id);

        var genre = await Repository.GetSingleOrDefaultBySpecAsync(spec)
                    ?? throw new ItemNotFoundException(typeof(Genre), updateModel.Id, nameof(updateModel.Id));

        var oldGenreVersion = genre.ToBsonDocument();

        UpdateValues(updateModel, genre);

        await Repository.UpdateAsync(genre);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogUpdateAsync(typeof(Genre), oldGenreVersion, genre.ToBsonDocument());
    }

    public async Task DeleteAsync(Guid id)
    {
        var spec = new GenresSpec().ById(id);

        var genre = await Repository.GetSingleOrDefaultBySpecAsync(spec)
                    ?? throw new ItemNotFoundException(typeof(Genre), id);

        await SetNewParentForChildrenGenres(genre);
        genre.IsDeleted = true;

        await Repository.UpdateAsync(genre);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogDeleteAsync(genre);
    }

    private void UpdateValues(GenreUpdateModel updateModel, Genre genre)
    {
        genre.Name = updateModel.Name;
        genre.ParentId = updateModel.ParentId != Guid.Empty ? updateModel.ParentId : null;
    }

    private async Task SetNewParentForChildrenGenres(Genre genre)
    {
        var subGenresByParentIdSpec = new GenresSpec().ByParentId(genre.Id);
        var children = await Repository.GetBySpecAsync(subGenresByParentIdSpec);

        var newParentId = genre.ParentId;

        foreach (var child in children)
        {
            var oldGenreVersion = child.ToBsonDocument();

            child.ParentId = newParentId;
            await Repository.UpdateAsync(child);

            await _mongoLogger.LogUpdateAsync(typeof(Genre), oldGenreVersion, child.ToBsonDocument());
        }
    }
}