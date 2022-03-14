using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers
{
    public class LevelCourseController : Controller
    {
        private readonly IRepository<Level> _levelRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Year> _yearRepository;

        public LevelCourseController(IRepository<Level> levelRepository, IRepository<Course> courseRepository, IRepository<Year> yearRepository, IRepository<LevelCourse> levelCourseRepository, IUnitOfWork unitOfWork)
        {
            _levelRepository = levelRepository;
            _courseRepository = courseRepository;
            _yearRepository = yearRepository;
        }

        public async Task<IActionResult> Index()
        {
            var year = (await _yearRepository
                .GetAsync(y => y.FromDate < DateTime.Now && y.ToDate > DateTime.Now)).FirstOrDefault();
            if (year == null)
                return View("NotFound");

            var levels = await _levelRepository.GetAsync();
            var courses = await _courseRepository.GetAsync();
            var viewModel = new LevelCourseViewModel
            {
                Levels = levels.ToList(),
                Courses = courses.ToList()
            };

            return View(viewModel);
        }
    }
}
