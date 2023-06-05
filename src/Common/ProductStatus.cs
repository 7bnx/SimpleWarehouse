using System.ComponentModel;

namespace SimpleWarehouse.Common
{
  [TypeConverter(typeof(EnumToStringUsingDescription<ProductStatus>))]
  public enum ProductStatus
  {
    None = 0,
    [Description("Принято")]
    Accepted = 1 << 1,
    [Description("Склад")]
    MovedToWarehouse = 1 << 2,
    [Description("Продано")]
    Sold = 1 << 3
  }

  class EnumToStringUsingDescription<T> : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
      => (sourceType.Equals(typeof(Enum)));

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
      => destinationType is not null && destinationType.Equals(typeof(String));

    public override object ConvertFrom(ITypeDescriptorContext? context,
                                       System.Globalization.CultureInfo? culture,
                                       object value)
    {
      if (Enum.TryParse(typeof(T), value as string, out var name))
        return name;
      throw new ArgumentException($"Wrong argument {value}", nameof(value));
    }

    public override object ConvertTo(ITypeDescriptorContext? context,
                                     System.Globalization.CultureInfo? culture,
                                     object? value,
                                     Type? destinationType)
    {
      var name = value?.ToString()!;
      var attrs =
          value?.GetType()?.GetField(name)?.GetCustomAttributes(typeof(DescriptionAttribute), false);
      return (attrs is not null && attrs.Length > 0) ? ((DescriptionAttribute)attrs[0]).Description : name;
    }
  }
}
