namespace KiddieParadies.Extensions
{
    public static class StringExtensions
    {
        public static int ToInt(this string value)
        {
            try
            {
                var result = int.Parse(value);
                return result;
            }
            catch
            {
                return 0;
            }
        }
    }
}