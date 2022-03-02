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

        public async Task<TModel> AddAsync(TModel model)
        {
            await Set.AddAsync(model);

            return model;
        }

        public async Task<ICollection<TModel>> GetAllAsync()
        {
            return await Set.ToListAsync();
        }

        public async Task<TModel> GetByIdAsync(Guid id)
        {
            return await Set.SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<ICollection<TModel>> GetBySpecificationAsync(ISpecification<TModel> specification)
        {
            var specificationResult = ApplySpecifications(specification);

            return await specificationResult.ToListAsync();
        }

        public void Update(TModel updated)
        {
            Set.Update(updated);
        }

        public void Delete(TModel model)
        {
            if (model is ISafeDelete)
            {
                (model as ISafeDelete).IsDeleted = true;
                Update(model);
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
    }
}
