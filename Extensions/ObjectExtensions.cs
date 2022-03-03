#nullable enable
using System;

namespace KiddieParadies.Extensions
{
    public static class ObjectExtensions
    {
        public static int ToInt(this object? obj)
        {
            try
            {
                int result = int.Parse(obj?.ToString() ?? throw new Exception());
                return result;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}