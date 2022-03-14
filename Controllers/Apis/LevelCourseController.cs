using KiddieParadies.Controllers.Apis.Dtos;
using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers.Apis
{
    [Route("api/levelCourses")]
    public class LevelCourseController : Controller
    {
        private readonly IRepository<Level> _levelRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Year> _yearRepository;
        private readonly IRepository<LevelCourse> _levelCourseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LevelCourseController(IRepository<Level> levelsRepository, IRepository<Course> coursesRepository,
            IRepository<Year> yearRepository, IRepository<LevelCourse> levelCourseRepository, IUnitOfWork unitOfWork)
        {
            _levelRepository = levelsRepository;
            _courseRepository = coursesRepository;
            _yearRepository = yearRepository;
            _levelCourseRepository = levelCourseRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetLevelCoursesAsync()
        {
            var levelCourses = await _levelCourseRepository
                .GetAsync(l => l.Year.FromDate < DateTime.Now
                && l.Year.ToDate > DateTime.Now);

            return Ok(levelCourses);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveLevelCoursesAsync([FromBody] LevelCoursesDto dto)
        {
            var year = (await _yearRepository
                .GetAsync(y => y.FromDate < DateTime.Now && y.ToDate > DateTime.Now)).FirstOrDefault();

            var dbLevelCourses = await _levelCourseRepository
                .GetAsync(l => l.YearId == year.Id);

            if (dto.LevelCourses.Count == dbLevelCourses.Count())
            {
                var changes = false;
                foreach (var levelCourse in dto.LevelCourses)
                {
                    if (!dbLevelCourses
                        .Any(l => l.LevelId == levelCourse.LevelId && l.CourseId == levelCourse.CourseId))
                    {
                        changes = true;
                        break;
                    }
                }

                if (!changes)
                    return NoContent();
            }

            foreach (var levelCourse in dto.LevelCourses)
            {
                if (!dbLevelCourses
                    .Any(l => l.LevelId == levelCourse.LevelId && l.CourseId == levelCourse.CourseId))
                    await _levelCourseRepository.AddAsync(new LevelCourse
                    {
                        CourseId = levelCourse.CourseId,
                        LevelId = levelCourse.LevelId,
                        YearId = year.Id
                    });
            }

            foreach (var levelCourse in dbLevelCourses)
            {
                if (!dto.LevelCourses
                    .Any(l => l.LevelId == levelCourse.LevelId && l.CourseId == levelCourse.CourseId))
                    _levelCourseRepository.Delete(levelCourse);
            }

            return await _unitOfWork.SaveChangesAsync() > 0 ? Ok() : BadRequest();
        }
    }
}
