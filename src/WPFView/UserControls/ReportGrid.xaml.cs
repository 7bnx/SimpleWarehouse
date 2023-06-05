using Microsoft.Extensions.DependencyInjection;
using SimpleWarehouse.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleWarehouse.WPFView
{
  public partial class ReportGrid : UserControl
  {
    private readonly Dictionary<Func<bool>, ProductStatus> _userInputItems = new();
    private readonly IRepository _repository = null!;
    private readonly ILogger _log;
    public ReportGrid()
    {
      InitializeComponent();
      _log = App.Host.Services.GetRequiredService<ILogger>();
      try
      {
        _repository = App.Host.Services.GetRequiredService<IRepository>();
      } catch (InvalidOperationException ex)
      {
        _log.Write(LogType.Error, ex.Message, ex);
        MessageBox.Show("Ошибка доступа к репозиторию. Проверьте настройки подключения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown();
      }
      Loaded += Report_LoadedAsync;
      PreviewKeyDown += (sender, e) =>
      {
        if (e.Key == Key.Enter)
          Apply_ClickAsync(sender, e);
      };
    }

    private async void Report_LoadedAsync(object sender, RoutedEventArgs e)
    {
      try
      {
        ProductsGrid.Filter = await _repository.GetLastFilterAsync();
      } catch (InvalidOperationException ex)
      {
        _log.Write(LogType.Error, ex.Message, ex);
      }
      ProductsGrid.Filter ??= new();
      DatePicker_From.SelectedDate = ProductsGrid.Filter.From;
      DatePicker_To.SelectedDate = ProductsGrid.Filter.To;
      foreach (var statusName in Enum.GetValues(typeof(ProductStatus)))
      {
        var status = (ProductStatus)statusName;
        if (status == ProductStatus.None) continue;
        CheckBox item = new()
        {
          Content = statusName,
          IsChecked = (status & ProductsGrid.Filter.Status) != 0
        };
        _userInputItems[() => (bool)item.IsChecked!] = status;
        ChckBxStPanel.Children.Add(item);
        _log.Write(LogType.Info, $"Report filter {statusName} created");
      }
      Loaded -= Report_LoadedAsync;
    }

    private ProductStatus GetStatusFilter()
      => _userInputItems.Aggregate(ProductStatus.None, (x, y) => x | (y.Key() ? y.Value : ProductStatus.None));

    private async void Apply_ClickAsync(object sender, RoutedEventArgs e)
    {
      if (!ValidateInput()) return;
      var filter = new Filter()
      {
        From = DatePicker_From.SelectedDate,
        To = DatePicker_To.SelectedDate,
        Status = GetStatusFilter()
      };
      ProductsGrid.Filter = filter;
      await ProductsGrid.RefreshGrid();
      LblCount.Content = ProductsGrid.ProductsCount;
      var report = new ReportInfo()
      {
        ProductsCount = ProductsGrid.ProductsCount,
        Filter = filter,
        CreationDate = DateTime.Now
      };
      try
      {
        await _repository.AddAsync(report);
        _log.Write(LogType.Info, report.ToString());
      } catch (InvalidOperationException ex)
      {
        _log.Write(LogType.Error, $"{ex.Message}{report}", ex);
      }
    }

    private bool ValidateInput()
    {
      var from = DatePicker_From.SelectedDate;
      var to = DatePicker_To.SelectedDate;
      if (from is not null && to is not null && from > to)
      {
        var messageText = "Дата 'От' не может быть меньше даты 'До'";
        MessageBox.Show(messageText, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return false;
      }
      return true;
    }
  }
}
