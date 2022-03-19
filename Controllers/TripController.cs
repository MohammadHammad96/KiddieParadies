using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers
{
    public class TripController : Controller
    {
        private readonly IRepository<Trip> _tripRepository;
        private readonly IRepository<YearEmployee> _yearEmployeeRepository;

        public TripController(IRepository<Trip> tripRepository, IRepository<YearEmployee> employeeRepository)
        {
            _tripRepository = tripRepository;
            _yearEmployeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var trips = await _tripRepository
                .GetAsync(null, null,
                    t => t.Driver.Employee, t => t.Escort.Employee);

            var drivers = await _yearEmployeeRepository
                .GetAsync(e => e.Employee.User.UserRoles.Any(ur => ur.Role.Name == "Driver"),
                    null, e => e.Employee);

            var escorts = await _yearEmployeeRepository
                .GetAsync(e => e.Employee.User.UserRoles.Any(ur => ur.Role.Name == "Escort"),
                    null, e => e.Employee);

            var viewModel = new TripViewModel
            {
                Trips = trips.ToList(),
                Drivers = drivers.ToList(),
                Escorts = escorts.ToList()
            };

            return View(viewModel);
        }
    }
}
