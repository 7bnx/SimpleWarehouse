namespace SimpleWarehouse.Common
{
  public class Product
  {
    public int ProductId { get; private set; }
    public string Name { get; private set; }
    public ProductStatus Status { get; private set; }
    public ProductStatus NextStatus => _productState.NextStatus;
    public DateTime ModifiedDate { get; private set; }
    private IProductState _productState;
    private static readonly Dictionary<ProductStatus, Func<IProductState>> _initDict = new()
    {
      { ProductStatus.Accepted, AcceptedProductState.Create },
      { ProductStatus.MovedToWarehouse, MovedToWarehouseProductState.Create },
      { ProductStatus.Sold, SoldProductState.Create }
    };
    /// <exception cref="ArgumentException">Invalid status</exception>
    public Product(string name, ProductStatus status, DateTime modifiedDate)
    {
      if (!_initDict.TryGetValue(status, out var create))
        throw new ArgumentException("Value of init status not allowed", nameof(status));
      _productState = create();
      Name = name;
      SetStatusAndChangeTime(modifiedDate);
    }
    /// <exception cref="ArgumentException">Invalid status</exception>
    public Product(ProductStatus status) : this("", status, DateTime.Now) { }

    public override string ToString()
      => $"Product(Name = {Name}; Status = {Status}; Modified = {ModifiedDate}; Next status = {NextStatus})";
    public void SetNextState()
    {
      if (_productState.CurrentStatus == _productState.NextStatus)
        return;
      _productState = _productState.SetNextState(this);
      SetStatusAndChangeTime(DateTime.Now);
    }
    private void SetStatusAndChangeTime(DateTime changeDate)
      => (Status, ModifiedDate) = (_productState.CurrentStatus, changeDate);
  }
}
