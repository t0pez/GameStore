using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using GameStore.Infrastructure.Data.Context;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Infrastructure.Data.Repositories;

public class Repository<TModel> : IRepository<TModel> where TModel : BaseEntity
{
    private readonly ApplicationContext _context;

    public Repository(ApplicationContext context)
    {
        _context = context;
    }

    private DbSet<TModel> Set => _context.Set<TModel>();

    public Task<TModel> GetByIdAsync(Guid id)
    {
        return Set.SingleOrDefaultAsync(e => e.Id == id);
    }

    public Task<List<TModel>> GetBySpecAsync(ISpecification<TModel> specification = null)
    {
        if (specification is null)
            return Set.ToListAsync();

        var specificationResult = ApplySpecifications(specification);

        return specificationResult.ToListAsync();
    }
        
    public Task<TModel> GetSingleBySpecAsync(ISpecification<TModel> specification)
    {
        var specificationResult = ApplySpecifications(specification);

        return specificationResult.SingleOrDefaultAsync();
    }

    public Task AddAsync(TModel model)
    {
        return Set.AddAsync(model).AsTask();
    }

    public Task UpdateAsync(TModel updated) // TODO: make method async
    {
        return Task.Run(() => Set.Update(updated));
    }

    public Task DeleteAsync(TModel model)
    {
       return Task.Run(() => Set.Remove(model));
    }

    public Task<bool> AnyAsync(Guid id)
    {
        return Set.AnyAsync(m => m.Id == id);
    }

    public Task<bool> AnyAsync(ISpecification<TModel> specification)
    {
        var specResult = ApplySpecifications(specification);

        return specResult.AnyAsync();
    }

    private IQueryable<TModel> ApplySpecifications(ISpecification<TModel> specification)
    {
        var specEvaluator = new SpecificationEvaluator();

        return specEvaluator.GetQuery(Set.AsQueryable(), specification);
    }
}