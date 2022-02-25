using System.Threading.Tasks;

namespace KiddieParadies.Core.Services
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}