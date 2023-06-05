namespace SimpleWarehouse.Common
{
  public class AcceptedProductState : BaseProductState
  {
    public static AcceptedProductState Create()
      => new();
    public AcceptedProductState() : base(ProductStatus.Accepted, ProductStatus.MovedToWarehouse) { }
    public override IProductState SetNextState(Product product)
      => new MovedToWarehouseProductState();
  }
  public class MovedToWarehouseProductState : BaseProductState
  {
    public static MovedToWarehouseProductState Create()
      => new();
    public MovedToWarehouseProductState() : base(ProductStatus.MovedToWarehouse, ProductStatus.Sold) { }
    public override IProductState SetNextState(Product product)
      => new SoldProductState();
  }
  public class SoldProductState : BaseProductState
  {
    public static SoldProductState Create()
      => new();
    public SoldProductState() : base(ProductStatus.Sold, ProductStatus.Sold) { }
    public override IProductState SetNextState(Product product)
      => this;
  }

  abstract public class BaseProductState : IProductState
  {
    public ProductStatus CurrentStatus { get; init; }

    public ProductStatus NextStatus { get; init; }
    protected BaseProductState(ProductStatus currentStatus, ProductStatus nextStatus)
      => (CurrentStatus, NextStatus) = (currentStatus, nextStatus);
    public abstract IProductState SetNextState(Product product);
  }
}
