namespace GeneralExtensions
{
  public static class Int64Exstentions
  {
    public static long ValueFromNullable(this long? value)
    {
      if (!value.HasValue)
      {
        return 0;
      }

      return value.Value;
    }
  }
}
