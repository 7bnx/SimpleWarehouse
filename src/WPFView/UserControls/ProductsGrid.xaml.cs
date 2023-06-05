using Microsoft.Extensions.DependencyInjection;
using SimpleWarehouse.Common;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimpleWarehouse.WPFView
{
  public partial class ProductsGrid : UserControl
  {
    public Filter? Filter { get; set; }
    public int ProductsCount => _products.Count;
    public bool RefreshOnReload { get; set; } = true;
    public bool EnableContextMenu { get; set; } = true;
    private readonly ObservableCollection<Product> _products = new();
    private readonly IRepository _repository = null!;
    private readonly ILogger _log;
    public ProductsGrid()
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
      GridWithProducts.ItemsSource = _products;
      Loaded += ProductsGrid_LoadedFirst;
    }
    private void ProductsGrid_LoadedFirst(object sender, RoutedEventArgs e)
    {
      Loaded -= ProductsGrid_LoadedFirst;
      if (!EnableContextMenu) Menu.Visibility = Visibility.Hidden;
      else
      {
        var dummyProduct = new Product(Filter!.Status);
        if (dummyProduct.Status == dummyProduct.NextStatus)
          Menu.Items.Remove(MenuItemNext);
        else
          MenuItemNext.Header = dummyProduct.NextStatus;
      }
      Loaded += ProductsGrid_LoadedRegularAsync;
      ProductsGrid_LoadedRegularAsync(sender, e);
    }
    private async void ProductsGrid_LoadedRegularAsync(object sender, RoutedEventArgs e)
    {
      if (!RefreshOnReload) return;
      await RefreshGrid();
      SetDeleteNextMenuItemsEnableState();
    }
    public async Task RefreshGrid()
    {
      _products.Clear();
      try
      {
        foreach (var product in await _repository.FilterByAsync(Filter))
          _products.Add(product);
      } catch (InvalidOperationException ex)
      {
        _log.Write(LogType.Error, $"Error filtering product({Filter})", ex);
      }
    }

    private async void Add_ClickAsync(object sender, RoutedEventArgs e)
    {
      var addProductWindow = new AddProductWindow(Filter!.Status);
      if (addProductWindow.ShowDialog() == false) return;
      var product = addProductWindow.Product;
      try
      {
        await _repository.AddAsync(product);
      } catch (InvalidOperationException ex)
      {
        _log.Write(LogType.Error, $"Error add product({product}); Message{ex.Message}", ex);
      }
      _products.Add(addProductWindow.Product);
      SetDeleteNextMenuItemsEnableState();
    }

    private async void Delete_ClickAsync(object sender, RoutedEventArgs e)
      => await ClickHandlerAsync(async product =>
      {
        try
        {
          await _repository.DeleteAsync(product);
        } catch (InvalidOperationException ex)
        {
          _log.Write(LogType.Error, $"Error delete product({product}); Message{ex.Message}", ex);
        }
      });

    private async void Next_ClickAsync(object sender, RoutedEventArgs e)
    {
      await ClickHandlerAsync(async product =>
      {
        product.SetNextState();
        try
        {
          await _repository.UpdateAsync(product);
        } catch (InvalidOperationException ex)
        {
          _log.Write(LogType.Error, $"Error updating product({product}); Message{ex.Message}", ex);
        }
      });
    }
    private Task ClickHandlerAsync(Action<Product> action)
    {
      if (GridWithProducts.SelectedItem is Product product)
      {
        action?.Invoke(product);
        _products.Remove(product);
        SetDeleteNextMenuItemsEnableState();
      }
      return Task.CompletedTask;
    }
    private void SetDeleteNextMenuItemsEnableState()
      => MenuItemDelete.IsEnabled = MenuItemNext.IsEnabled = _products.Count > 0;
  }
}
