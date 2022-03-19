using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.Extensions;
using KiddieParadies.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers
{
    public class TripController : Controller
    {
        private readonly IRepository<Trip> _tripRepository;
        private readonly IRepository<YearEmployee> _yearEmployeeRepository;
        private readonly HttpContext _httpContext;
        private readonly IRepository<Student> _studentRepository;

        public TripController(IRepository<Trip> tripRepository, IRepository<YearEmployee> employeeRepository, IHttpContextAccessor httpContextAccessor, IRepository<Student> studentRepository)
        {
            _tripRepository = tripRepository;
            _yearEmployeeRepository = employeeRepository;
            _studentRepository = studentRepository;
            _httpContext = httpContextAccessor.HttpContext;
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

        public async Task<IActionResult> Track()
        {
            var userId = _httpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value.ToInt();
            var trips = await _tripRepository.GetAsync(t => t.Driver.Employee.UserId == userId
            && t.Students.Any());
            return View(trips);
        }

        public async Task<IActionResult> EnrollStudentTrip()
        {
            //var ignoredId = (await _tripRepository.GetAsync()).FirstOrDefault()?.Id;
            var trips = await _tripRepository.GetAsync(/*t => t.Id != ignoredId.ToInt()*/null,
                null, t => t.Driver.Employee, t => t.Escort.Employee);
            return View("TripStudents", trips);
        }

        public async Task<IActionResult> Attendance()
        {
            var userId = _httpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value.ToInt();
            var trip = (await _tripRepository.GetAsync(t => t.Escort.Employee.UserId == userId
            && t.IsActive == true, null, t => t.Students)).FirstOrDefault();
            if (trip == null)
                return View("Error", new ErrorViewModel("لا توجد رحلة حالياً"));

            var students = await _studentRepository.GetAsync(s => s.TripId == trip.Id,
                null, s => s.Parent);
            trip.Students = students.ToList();
            return View(trip);
        }
    }

    public class TrackViewModel
    {
        public string Type { get; set; }

        public Property Properties { get; set; }

        public Geometry Geometry { get; set; }
    }

    public class Property
    {
        public string Title { get; set; }
    }

    public class Geometry
    {
        public string Type { get; set; }

        public double[] Coordinates { get; set; }
    }
}
