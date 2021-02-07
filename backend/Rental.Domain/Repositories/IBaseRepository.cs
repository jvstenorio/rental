using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Domain.Repositories
{
    public interface IBaseRepository<T> where T : new()
    {
        Task AddAsync(object entity, CancellationToken cancellationToken);
        Task<T> GetByIdentifierAsync(Guid identifier, CancellationToken cancellationToken);
        Task<List<T>> ListAllAsync(CancellationToken cancellationToken);
    }
}
