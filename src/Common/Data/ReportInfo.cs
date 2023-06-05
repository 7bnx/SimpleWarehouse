namespace SimpleWarehouse.Common
{
  public record ReportInfo
  (
    int ReportInfoId,
    Filter? Filter,
    DateTime? CreationDate,
    int ProductsCount
  )
  {
    public ReportInfo() : this(0, null, null, 0) { }
  }
}
