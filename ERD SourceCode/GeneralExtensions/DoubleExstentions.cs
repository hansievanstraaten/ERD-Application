namespace GeneralExtensions
{
  public static class DoubleExstentions
  {
    public static bool IsNan(this double value)
    {
      return double.IsNaN(value);
    }

    public static bool IsNan(this double? value)
    {
      return (!value.HasValue || double.IsNaN(value.Value));
    }
  }
}
