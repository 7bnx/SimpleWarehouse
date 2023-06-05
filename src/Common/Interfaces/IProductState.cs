namespace SimpleWarehouse.Common
{
  public interface IProductState
  {
    ProductStatus CurrentStatus { get; }
    ProductStatus NextStatus { get; }
    IProductState SetNextState(Product product);
  }
}
