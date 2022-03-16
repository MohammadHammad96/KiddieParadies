using AutoMapper;
using KiddieParadies.Controllers.Apis.Dtos;
using KiddieParadies.Core.Helpers;
using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers.Apis
{
    [Route("api/weekProgram")]
    public class WeekProgramController : Controller
    {
        private readonly IRepository<Year> _yearRepository;
        private readonly IRepository<LevelCourse> _levelCourseRepository;
        private readonly IRepository<YearEmployee> _employeeRepository;
        private readonly IRepository<CourseClassRoom> _courseClassRoomRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public WeekProgramController(IRepository<Year> yearRepository, IRepository<LevelCourse> levelCourseRepository, IRepository<YearEmployee> employeeRepository, IRepository<CourseClassRoom> courseClassRoomRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _yearRepository = yearRepository;
            _levelCourseRepository = levelCourseRepository;
            _employeeRepository = employeeRepository;
            _courseClassRoomRepository = courseClassRoomRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("levelClassRoomCoursesAndTeacher")]
        public async Task<IActionResult> LevelClassRoomCoursesAndTeacher([FromHeader] LevelClassRoomQuery query)
        {
            var levelClassRooms = await _courseClassRoomRepository
                .GetAsync(c => c.Course.LevelId == query.LevelId && c.ClassRoom == query.ClassNumber);
            return Ok(levelClassRooms);
        }

        [HttpGet("getLevelCourses/{levelId}")]
        public async Task<IActionResult> GetLevelCourses(int levelId)
        {
            var year = await _yearRepository.GetThis();
            var levelCourses = await _levelCourseRepository
                .GetAsync(l => l.YearId == year.Id && l.LevelId == levelId, null, l => l.Course);
            var dto = levelCourses.Select(l => new IdNameDto
            {
                Id = l.Id,
                Name = l.Course.Name
            });
            return Ok(dto);
        }

        [HttpGet("getTeachers")]
        public async Task<IActionResult> GetTeachers()
        {
            var year = await _yearRepository.GetThis();
            var teachers = await _employeeRepository
                .GetAsync(e => e.YearId == year.Id && (e.Employee.User.UserRoles.Any(ur => ur.Role.NormalizedName == "Teacher".ToUpper()))
                    , null, e => e.Employee);
            //teachers.Where(t => t.Employee.User.UserRoles.First().)
            var dto = teachers.Select(t => new IdNameDto
            { Id = t.Id, Name = string.Join(" ", t.Employee.FirstName, t.Employee.LastName) }).ToList();
            return Ok(dto);
        }

        [HttpGet("getLevelClasses")]
        public async Task<IActionResult> GetLevelClasses()
        {
            var year = await _yearRepository.GetThis();
            var classes = await _courseClassRoomRepository.GetAsync(c => c.Course.YearId == year.Id, null, c => c.Course.Level);
            var result = classes.Select(c => new
            {
                c.Course.LevelId,
                c.ClassRoom
            }).Distinct().ToList();
            return Ok(result);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] List<ClassRoomCourseSaveDto> dto)
        {
            if (dto == null)
                return BadRequest("يوجد خطأ بالمدخلات");

            if (!dto.Any())
                return BadRequest("يجب تحديد مقررات الشعبة ومدرسيها");

            var classNumber = dto.First().ClassRoom;
            var levelId = (await _levelCourseRepository.GetByIdAsync(dto.First().CourseId)).LevelId;
            var courseClassRoomToUpdate = await _courseClassRoomRepository.GetAsync(c =>
                c.Course.LevelId == levelId && c.ClassRoom == classNumber);
            if (courseClassRoomToUpdate.Any())
            {
                foreach (var courseClassRoom in courseClassRoomToUpdate)
                    _courseClassRoomRepository.Delete(courseClassRoom);

                var ClassRooms = _mapper.Map<IEnumerable<CourseClassRoom>>(dto);
                foreach (var courseClassRoom in ClassRooms)
                    await _courseClassRoomRepository.AddAsync(courseClassRoom);

                return await _unitOfWork.SaveChangesAsync() > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً.");
            }


            var courseClassRooms = _mapper.Map<IEnumerable<CourseClassRoom>>(dto);
            foreach (var courseClassRoom in courseClassRooms)
                await _courseClassRoomRepository.AddAsync(courseClassRoom);

            return await _unitOfWork.SaveChangesAsync() > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً.");
        }
    }
}