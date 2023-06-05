using Microsoft.EntityFrameworkCore;
using SimpleWarehouse.Common;
using System.Data;
using System.Runtime.CompilerServices;

namespace SimpleWarehouse.EFService
{
  public class EFRepository : IRepository
  {
    private readonly DataDbContext _context;
    public EFRepository(DataDbContext context)
      => _context = context;
    /// <exception cref="InvalidOperationException"/>
    public async Task AddAsync(Product? product)
      => await AddASync(product);
    /// <exception cref="InvalidOperationException"/>
    public async Task AddAsync(ReportInfo? reportInfo)
      => await AddASync(reportInfo);
    /// <exception cref="InvalidOperationException"/>
    public async Task AddAsync(Filter? filter)
      => await AddASync(filter);
    /// <exception cref="InvalidOperationException"/>
    public async Task DeleteAsync(Product? product)
    {
      if (product is null) return;
      _context.Products.Remove(product);
      await ExceptionCatchDB(async () => await _context.SaveChangesAsync());
    }
    /// <exception cref="InvalidOperationException"/>
    public async Task UpdateAsync(Product? product)
    {
      if (product is null) return;
      _context.Products.Update(product);
      await ExceptionCatchDB(async () => await _context.SaveChangesAsync());
    }
    /// <exception cref="InvalidOperationException"/>
    public async Task<IEnumerable<Product>> FilterByStatusAndChangeTime(
      ProductStatus status,
      DateTime? from,
      DateTime? to
    )
      => await GetAllQueryable(query => query.FilterProductsByStatus(status).FilterProductsByTime(from, to));
    /// <exception cref="InvalidOperationException"/>
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
      => await GetAllQueryable();
    /// <exception cref="InvalidOperationException"/>
    public async Task<IEnumerable<Product>> FilterByAsync(Filter? filter)
      => filter is null
          ? await GetAllProductsAsync()
          : await FilterByStatusAndChangeTime(filter.Status, filter.From, filter.To);
    /// <exception cref="InvalidOperationException"/>
    public async Task<Filter> GetLastFilterAsync()
    {
      Filter? filter = null;
      await ExceptionCatchDB(async () =>
      {
        filter = await _context.Filters.OrderBy(f => f.FilterId).LastOrDefaultAsync();
        if (filter == null)
        {
          filter = new Filter();
          await _context.Filters.AddAsync(filter);
          await _context.SaveChangesAsync();
        }
      });
      return filter!;
    }

    private async Task AddASync<T>(T? item)
    {
      if (item is null) return;
      await ExceptionCatchDB(async () =>
      {
        await _context.AddAsync(item);
        await _context.SaveChangesAsync();
      });
    }
    private async Task<IEnumerable<Product>> GetAllQueryable(
        Func<IQueryable<Product>,
        IQueryable<Product>>? filter = null
    )
    {
      IEnumerable<Product>? result = null;
      await ExceptionCatchDB(() =>
      {
        result = (filter is null ? _context.Products : filter(_context.Products)).ToList();
        return Task.FromResult(result);
      });
      return result ?? new List<Product>();
    }

    private static async Task ExceptionCatchDB(Func<Task> action, [CallerMemberName] string methodName = "")
    {
      try
      {
        await action();
      } catch (Exception ex) when (ex is OperationCanceledException
                                 || ex is DbUpdateException
                                 || ex is DBConcurrencyException)
      {
        throw new InvalidOperationException($"Invalid Database operation ({methodName})", ex);
      }
    }
  }
}
