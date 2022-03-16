using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KiddieParadies.Extensions
{
    public static class YearRepositoryExtensions
    {
        public static async Task<Year> GetThis(this IRepository<Year> yearRepository)
        {
            var years = await yearRepository
                .GetAsync(y => y.FromDate < DateTime.Now && y.ToDate > DateTime.Now);
            return years.FirstOrDefault();
        }
    }
}