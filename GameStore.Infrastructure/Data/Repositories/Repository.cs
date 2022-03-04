using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using GameStore.Core.Exceptions;
using GameStore.Infrastructure.Data.Context;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Infrastructure.Data.Repositories
{
    public class Repository<TModel> : IRepository<TModel> where TModel : BaseEntity
    {
        private readonly ApplicationContext _context;

        public Repository(ApplicationContext context)
        {
            _context = context;
        }

        private DbSet<TModel> Set => _context.Set<TModel>();

        public async Task<TModel> AddAsync(TModel model, CancellationToken token = default)
        {
            if (await SetContainsModelOfId(model.Id, token))
                throw new ItemAlreadyExistsException();

            await Set.AddAsync(model, token);

            return model;
        }

        public async Task<ICollection<TModel>> GetAllAsync(CancellationToken token = default)
        {
            return await Set.ToListAsync(token);
        }

        public async Task<TModel> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await Set.SingleOrDefaultAsync(e => e.Id == id, token) ?? throw new ItemNotFoundException<TModel>();
        }

        public async Task<ICollection<TModel>> GetBySpecAsync(ISpecification<TModel> specification, CancellationToken token = default)
        {
            var specificationResult = ApplySpecifications(specification);

            return await specificationResult.ToListAsync(token);
        }
        
        public async Task<TModel> GetSingleBySpecAsync(ISpecification<TModel> specification, CancellationToken token = default)
        {
            var specificationResult = ApplySpecifications(specification);

            var result = await specificationResult.FirstOrDefaultAsync(token);

            if (result is null)
                throw new ItemNotFoundException<TModel>();

            return result;
        }

        public async Task UpdateAsync(TModel updated, CancellationToken token = default)
        {
            if (await SetContainsModelOfId(updated.Id, token) == false)
                throw new ItemNotFoundException<TModel>();

            Set.Update(updated);
        }

        public async Task DeleteAsync(TModel model, CancellationToken token = default)
        {
            if (await SetContainsModelOfId(model.Id, token) == false)
                throw new ItemNotFoundException<TModel>();

            if (model is ISafeDelete)
            {
                if ((model as ISafeDelete).IsDeleted)
                    throw new InvalidOperationException("Item already deleted");

                (model as ISafeDelete).IsDeleted = true;
                await UpdateAsync(model, token);
            }
            else
            {
                Set.Remove(model);
            }
        }

        private IQueryable<TModel> ApplySpecifications(ISpecification<TModel> specification)
        {
            var specEvaluator = new SpecificationEvaluator();

            return specEvaluator.GetQuery(Set.AsQueryable(), specification);
        }

        private async Task<bool> SetContainsModelOfId(Guid id, CancellationToken token)
        {
            return await Set.AnyAsync(m => m.Id == id, token);
        }
    }
}
