namespace SimpleWarehouse.Common
{
  public record Filter
  (
    int FilterId,
    DateTime? From,
    DateTime? To,
    ProductStatus Status
  )
  {
    public Filter() : this(0, null, null, ProductStatus.None) { }
  }
}
