using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UberSystem.Domain.Interfaces;

namespace UberSystem.Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
   	{
        public DbSet<T> Entities => DbContext.Set<T>();
 
    	public DbContext DbContext { get; private set; }
 
        public Repository(DbContext dbContext)
        {
            DbContext = dbContext;
    	}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveChanges"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id, bool saveChanges = true)
        {
            var entity = await Entities.FindAsync(id);
            await DeleteAsync(entity);

            if (saveChanges)
            {
                await DbContext.SaveChangesAsync();
            }
        }
 
        public async Task DeleteAsync(T entity, bool saveChanges = true)
        {
            Entities.Remove(entity);
            if (saveChanges)
            {
                await DbContext.SaveChangesAsync();
            }
        }
 
        public async Task DeleteRangeAsync(IEnumerable<T> entities, bool saveChanges = true)
        {
            var enumerable = entities as T[] ?? entities.ToArray();
            if (enumerable.Any())
            {
                Entities.RemoveRange(enumerable);
            }

            if (saveChanges)
            {
                await DbContext.SaveChangesAsync();
        }
        }
 
    	public async Task<IList<T>> GetAllAsync()
        {
            return await Entities.ToListAsync();
        }
 
        public T Find(params object[] keyValues)
        {
            return Entities.Find(keyValues);
        }
 
        public virtual async Task<T> FindAsync(params object[] keyValues)
        {
            return await Entities.FindAsync(keyValues);
        }
 
        public async Task InsertAsync(T entity, bool saveChanges = true)
        {
            await Entities.AddAsync(entity);

            if (saveChanges)
            {
				try
				{
					await DbContext.SaveChangesAsync();
				}
				catch (DbUpdateException)
				{
                    throw;
				}
			}
        }
 
        public async Task InsertRangeAsync(IEnumerable<T> entities, bool saveChanges = true)
        {
            await DbContext.AddRangeAsync(entities);

            if (saveChanges)
            {
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateAsync(T entity, bool saveChanges = true)
		{
			DbContext.Attach(entity); // Attach the entity to the context
			DbContext.Entry(entity).State = EntityState.Modified;
			if (saveChanges)
            {
                var result = await DbContext.SaveChangesAsync();
				return result > 0 ? true : false;
			}
            return false;
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, bool saveChanges = true)
        {
            if (entities.Any())
                DbContext.UpdateRange(entities);
            if (saveChanges) await DbContext.SaveChangesAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await Entities.FirstOrDefaultAsync(predicate, cancellationToken);

        public IQueryable<T> GetAll()
            => Entities.Where(x => true).AsQueryable();
    }
}