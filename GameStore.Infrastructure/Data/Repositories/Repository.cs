﻿using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using GameStore.Core.Exceptions;
using GameStore.Infrastructure.Data.Context;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        public void Update(TModel updated)
        {
            Set.Update(updated);
        }

        public void Delete(TModel model)
        {
            if (model is ISafeDelete)
            {
                (model as ISafeDelete).IsDeleted = true;
                Set.Update(model);
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
