using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers
{
    [Route("weekProgram")]
    public class WeekProgramController : Controller
    {
        private readonly IRepository<CourseClassRoom> _courseClassRoomRepository;

        public WeekProgramController(IRepository<CourseClassRoom> courseClassRoomRepository)
        {
            _courseClassRoomRepository = courseClassRoomRepository;
        }

        [HttpGet("manage")]
        public ActionResult Manage()
        {
            return View();
        }

        [HttpGet("preview/{neededLevelId}/{neededCourseNumber}")]
        public async Task<IActionResult> Preview(int neededLevelId, int neededCourseNumber)
        {
            var courseClassRooms = await _courseClassRoomRepository
                .GetAsync(c => c.Course.LevelId == neededLevelId
                 && c.ClassRoom == neededCourseNumber, null, c => c.Course.Course, c => c.Teacher.Employee);
            if (!courseClassRooms.Any())
                return View("NotFound");

            var result = courseClassRooms.Select(c => new WeekProgramPreviewViewModel
            {
                Course = c.Course.Course.Name,
                Teacher = c.Teacher.Employee.FirstName + " " + c.Teacher.Employee.LastName,
                Order = c.Order,
                Day = (int)c.Day
            }).ToList();

            var classRoom = neededLevelId switch
            {
                1 => "للمستوى الأول",
                2 => "للمستوى الثاني",
                3 => "للمستوى الثالث",
                _ => string.Empty
            };

            ViewData["title"] = "البرنامج الأسبوعي " + classRoom + " الشعبة " + neededCourseNumber;

            for (var i = 1; i <= 5; i++)
            {
                for (var j = 1; j <= 6; j++)
                {
                    if (result.FirstOrDefault(v => (int)v.Day == i && v.Order == j) == null)
                    {
                        result.Add(new WeekProgramPreviewViewModel
                        {
                            Course = string.Empty,
                            Teacher = string.Empty,
                            Day = i,
                            Order = j
                        });
                    }
                }
            }

            result = result.OrderBy(v => v.Day).ThenBy(v => v.Order).ToList();

            return View(result);
        }
    }
}
