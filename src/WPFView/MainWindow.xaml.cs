using Microsoft.Extensions.DependencyInjection;
using SimpleWarehouse.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SimpleWarehouse.WPFView
{
  public partial class MainWindow : Window
  {
    private readonly ILogger _log;
    public MainWindow()
    {
      InitializeComponent();
      _log = App.Host.Services.GetRequiredService<ILogger>();
      _log.Write(LogType.Info, "Init MainWindow");
      foreach (var statusName in Enum.GetValues(typeof(ProductStatus)))
      {
        var status = (ProductStatus)statusName;
        if (status == ProductStatus.None) continue;
        TabItem item = new()
        {
          Header = statusName,
          Content = new ProductsGrid() { Filter = new() { Status = status } }
        };
        TabCntrl.Items.Add(item);
        _log.Write(LogType.Info, $"Tab {item.Header} at MainWindow created");
      }

      TabItem report = new()
      {
        Header = "Отчет",
        Content = new ReportGrid()
      };

      TabCntrl.Items.Add(report);
      _log.Write(LogType.Info, $"Tab {report.Header} at MainWindow created");
    }
  }
}
