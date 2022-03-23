using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using GameStore.Infrastructure.Data.Context;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Infrastructure.Data.Repositories;

public class Repository<TModel> : IRepository<TModel> where TModel : class
{
    private readonly ApplicationContext _context;

    public Repository(ApplicationContext context)
    {
        _context = context;
    }

    private DbSet<TModel> Set => _context.Set<TModel>();

    public Task<List<TModel>> GetBySpecAsync(ISpecification<TModel> spec = null)
    {
        if (spec is null)
            return Set.ToListAsync();

        var specificationResult = ApplySpecifications(spec);

        return specificationResult.ToListAsync();
    }
        
    public Task<TModel> GetSingleBySpecAsync(ISpecification<TModel> spec)
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

    private IQueryable<TModel> ApplySpecifications(ISpecification<TModel> specification)
    {
        var specEvaluator = new SpecificationEvaluator();

        return specEvaluator.GetQuery(Set.AsQueryable(), specification);
    }
}