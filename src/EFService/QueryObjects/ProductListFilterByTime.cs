using SimpleWarehouse.Common;

namespace SimpleWarehouse.EFService
{
  public static class ProductListFilterByTime
  {
    public static IQueryable<Product> FilterProductsByTime(this IQueryable<Product> query, DateTime? from, DateTime? to)
    {
      if (from is null && to is null) return query;
      return query
        .Where(p => p.ModifiedDate >= (from ?? DateTime.MinValue) && p.ModifiedDate <= (to ?? DateTime.MaxValue));
    }
  }
}
