namespace LogApp.Database.Configuration.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        Task<T> EntityFindByAsync(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        T Delete(T entity);
        void Edit(T entity);
        void Save();
        IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters);
        void ExecCommand(string query, params object[] parameters);

        void DisableChangeTracking();
        void EnableChangeTracking();
    }
}
