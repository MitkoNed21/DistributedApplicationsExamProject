using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    // Check:
    // ?? Adapted from
    // ?? https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#implement-a-generic-repository-and-a-unit-of-work-class
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal MessageBoardsDbContext dbContext;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(MessageBoardsDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetPagedAsync(
            int pageSize, int page,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = ""
        )
        {
            IQueryable<TEntity> entities = dbSet;

            if (filter is not null)
            {
                entities = entities.Where(filter);
            }

            foreach (var includeProperty in includeProperties
                .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()))
            {
                entities = entities.Include(includeProperty);
            }

            if (orderBy is not null)
            {
                return await orderBy(entities)
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize)
                    .ToListAsync();
            }

            return await entities
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToListAsync();
        }

        public virtual IEnumerable<TEntity> GetPaged(
            int pageSize, int page,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = ""
        )
        {
            IQueryable<TEntity> entities = dbSet;

            if (filter is not null)
            {
                entities = entities.Where(filter);
            }

            foreach (var includeProperty in includeProperties
                .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()))
            {
                entities = entities.Include(includeProperty);
            }

            if (orderBy is not null)
            {
                return orderBy(entities)
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize)
                    .ToList();
            }

            return entities
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = ""
        )
        {
            IQueryable<TEntity> entities = dbSet;

            if (filter is not null)
            {
                entities = entities.Where(filter);
            }

            foreach (var includeProperty in includeProperties
                .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()))
            {
                entities = entities.Include(includeProperty);
            }

            if (orderBy is not null)
            {
                return await orderBy(entities).ToListAsync();
            }

            return await entities.ToListAsync();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = ""
        )
        {
            IQueryable<TEntity> entities = dbSet;

            if (filter is not null)
            {
                entities = entities.Where(filter);
            }

            foreach (var includeProperty in includeProperties
                .Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                entities = entities.Include(includeProperty);
            }

            if (orderBy is not null)
            {
                return orderBy(entities).ToList();
            }

            return entities.ToList();
        }

        public virtual async Task<TEntity?> GetByIdAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual TEntity? GetById(object id)
        {
            return dbSet.Find(id);
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.AddAsync(entity);
        }

        public virtual void Delete(object id)
        {
            var entityToDelete = dbSet.Find(id);
            // TODO: if null, exit?, test if something needs to be done?
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }

            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
