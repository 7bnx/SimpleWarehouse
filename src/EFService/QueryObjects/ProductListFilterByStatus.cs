using SimpleWarehouse.Common;

namespace SimpleWarehouse.EFService
{
  public static class ProductFilterByStatus
  {
    public static IQueryable<Product> FilterProductsByStatus(this IQueryable<Product> products, ProductStatus status)
      => status switch
      {
        ProductStatus.None => products,
        _ => products.Where(p => (p.Status & status) != 0)
      };
  }
}
