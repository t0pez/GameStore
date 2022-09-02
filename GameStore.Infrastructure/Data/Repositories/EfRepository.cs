using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using GameStore.Infrastructure.Data.Context;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Data.Repositories;

public class EfRepository<TModel> : IRepository<TModel> where TModel : class
{
    private readonly ApplicationContext _context;

    public EfRepository(ApplicationContext context)
    {
        _context = context;
    }

    private DbSet<TModel> Set => _context.Set<TModel>();

    public Task<List<TModel>> GetBySpecAsync(ISpecification<TModel> spec = null)
    {
        if (spec is null)
        {
            return Set.ToListAsync();
        }

        var specificationResult = ApplySpecifications(spec);

        return specificationResult.ToListAsync();
    }

    public Task<List<TResult>> SelectBySpecAsync<TResult>(ISpecification<TModel, TResult> spec)
    {
        var specificationResult = ApplySelectSpecifications(spec);

        return specificationResult.ToListAsync();
    }

    public Task<TModel> GetFirstOrDefaultBySpecAsync(ISpecification<TModel> spec)
    {
        var specificationResult = ApplySpecifications(spec);

        return specificationResult.FirstOrDefaultAsync();
    }

    public Task<TModel> GetSingleOrDefaultBySpecAsync(ISpecification<TModel> spec)
    {
        var specificationResult = ApplySpecifications(spec);

        return specificationResult.SingleOrDefaultAsync();
    }

    public Task AddAsync(TModel model)
    {
        return Set.AddAsync(model).AsTask();
    }

    public Task AddRangeAsync(IEnumerable<TModel> models)
    {
        return Set.AddRangeAsync(models);
    }

    public Task UpdateAsync(TModel updated)
    {
        return Task.Run(() => Set.Update(updated));
    }

    public Task DeleteAsync(TModel model)
    {
        return Task.Run(() => Set.Remove(model));
    }

    public Task DeleteRangeAsync(IEnumerable<TModel> models)
    {
        return Task.Run(() => Set.RemoveRange(models));
    }

    public Task<bool> AnyAsync(ISpecification<TModel> spec)
    {
        var specResult = ApplySpecifications(spec);

        return specResult.AnyAsync();
    }

    public Task<int> CountAsync(ISpecification<TModel> spec = null)
    {
        if (spec is null)
        {
            return Set.CountAsync();
        }

        var specResult = ApplySpecifications(spec);

        return specResult.CountAsync();
    }

    private IQueryable<TModel> ApplySpecifications(ISpecification<TModel> specification)
    {
        var specEvaluator = new SpecificationEvaluator();

        return specEvaluator.GetQuery(Set.AsQueryable(), specification);
    }

    private IQueryable<TResult> ApplySelectSpecifications<TResult>(ISpecification<TModel, TResult> specification)
    {
        var specEvaluator = new SpecificationEvaluator();

        return specEvaluator.GetQuery(Set.AsQueryable(), specification);
    }
}