namespace SimpleWarehouse.Common
{
  public interface IRepository
  {
    Task AddAsync(Product? product);
    Task AddAsync(ReportInfo? reportInfo);
    Task AddAsync(Filter? filter);
    Task DeleteAsync(Product? product);
    Task UpdateAsync(Product? product);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<IEnumerable<Product>> FilterByAsync(Filter? filter);
    Task<Filter> GetLastFilterAsync();
  }
}
