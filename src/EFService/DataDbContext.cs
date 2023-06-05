using Microsoft.EntityFrameworkCore;
using SimpleWarehouse.Common;

namespace SimpleWarehouse.EFService
{
  public class DataDbContext : DbContext
  {
    public DbSet<Product> Products { get; set; }
    public DbSet<Filter> Filters { get; set; }
    public DbSet<ReportInfo> ReportsInfos { get; set; }
    public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
    {
      try
      {
        Database.EnsureCreated();
      } catch (ArgumentException ex)
      {
        throw new InvalidOperationException($"Invalid database operation(wrong parameter {ex.ParamName})", ex);
      }
    }
  }
}