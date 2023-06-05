using SimpleWarehouse.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleWarehouse.WPFView
{
  public partial class AddProductWindow : Window
  {
    public Product Product { get; private set; } = null!;
    private readonly Dictionary<Func<bool>, Label> _userInputItems;
    private readonly ProductStatus _productStatus;
    private static readonly string[] _arrayOfColors = new[]
      {"Красный", "Оранжевый", "Желтый", "Зеленый", "Голубой", "Синий", "Фиолетовый", "Черный", "Коричневый" };
    private static readonly string[] _arrayOfNames = new[]
      {"Транспортир", "Карандаш", "Ластик", "Пенал", "Корректор", "Фломастер", "Портфель", "Альбом", "Пластилин" };
    private static readonly Random _random = new();
    public AddProductWindow(ProductStatus productStatus)
    {
      InitializeComponent();
      ProductAddDatePicker.SelectedDate = RandomDate();
      TxtBoxProductName.Text = RandomName();
      _userInputItems = new()
      {
        { () => !string.IsNullOrEmpty(TxtBoxProductName.Text), lblInvalidProductName },
        { () => ProductAddDatePicker.SelectedDate is not null, lblInvalidDate}
      };
      _productStatus = productStatus;
      PreviewKeyDown += (sender, e) =>
      {
        if (e.Key == Key.Enter)
          BtnAdd_Click(sender, e);
      };
    }
    private static DateTime RandomDate()
    {
      DateTime start = new(1995, 1, 1);
      int range = (DateTime.Today - start).Days;
      return start.AddDays(_random.Next(range));
    }
    private void ProductAddDate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        => e.Handled = !(char.IsNumber(e.Text[^1]) || e.Text[^1] == '.');

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
      if (!ValidateUserInput())
        return;
      Product = new(TxtBoxProductName.Text, _productStatus, (DateTime)ProductAddDatePicker.SelectedDate!);
      DialogResult = true;
    }

    private static string RandomName()
    {
      var colorIndex = _random.Next(0, _arrayOfColors.Length);
      var colorName = _random.Next(0, _arrayOfNames.Length);
      return $"{_arrayOfColors[colorIndex]} {_arrayOfNames[colorName]}";
    }

    private bool ValidateUserInput()
    {
      bool isValidInput = true;
      foreach (var item in _userInputItems)
      {
        var isInputValid = item.Key.Invoke();
        item.Value.Visibility = isInputValid ? Visibility.Hidden : Visibility.Visible;
        isValidInput &= isInputValid;
      }
      return isValidInput;
    }
  }
}
