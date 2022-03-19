using System;
using AutoMapper;
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
    [Route("api/trip")]
    public class TripController : Controller
    {
        private readonly IRepository<Trip> _tripRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Student> _studentRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<Attendance> _attendanceRepository;

        public TripController(IRepository<Trip> tripRepository, IUnitOfWork unitOfWork, IRepository<Student> studentRepository, IMapper mapper, IRepository<Attendance> attendanceRepository)
        {
            _tripRepository = tripRepository;
            _unitOfWork = unitOfWork;
            _studentRepository = studentRepository;
            _mapper = mapper;
            _attendanceRepository = attendanceRepository;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAsync([FromBody] Trip trip)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _tripRepository.AddAsync(trip);
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok(new
                {
                    trip.Id,
                    trip.Type,
                    trip.Time,
                    trip.DriverId,
                    trip.EscortId
                });

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditAsync([FromBody] Trip trip)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tripToUpdate = await _tripRepository.GetByIdAsync(trip.Id);
            if (tripToUpdate == null)
                return NotFound();

            tripToUpdate.DriverId = trip.DriverId;
            tripToUpdate.EscortId = trip.EscortId;
            tripToUpdate.Type = trip.Type;
            tripToUpdate.Time = trip.Time;
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok(new
                {
                    trip.Id,
                    trip.Type,
                    trip.Time,
                    trip.DriverId,
                    trip.EscortId
                });

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("getStudentsLocation/{id}")]
        public async Task<IActionResult> GetStudentsHome(int id)
        {
            var students = new List<Student>();
            var trip = await _tripRepository.GetByIdAsync(id, t => t.Students);
            if (trip.Students.Any())
            {
                foreach (var tripStudent in trip.Students)
                {
                    students.Add(await _studentRepository.GetByIdAsync(tripStudent.Id, s => s.Parent.HomeLocation));
                }

            }

            var dto = new List<TrackViewModel>();
            if (students.Any())
            {
                foreach (var student in students)
                {
                    dto.Add(
                        new TrackViewModel
                        {
                            Type = "Feature",
                            Geometry = new Geometry
                            {
                                Type = "Point",
                                Coordinates = new[]
                                {
                                    double.Parse(student.Parent.HomeLocation.Longitude),
                                    double.Parse(student.Parent.HomeLocation.Latitude)
                                }
                            },
                            Properties = new Property
                            {
                                Title = string.Join(" ", student.FirstName, student.Parent.FatherLastName)
                            }
                        });
                }

                return Ok(dto);
            }

            /*dto = new List<TrackViewModel>()
            {
                new TrackViewModel
                {
                    Type = "Feature",
                    Geometry = new Geometry
                    {
                        Type = "Point",
                        Coordinates = new []
                        {
                            36.29212856087469,
                            35.29212856087469
                        }
                    },
                    Properties = new Property
                    {
                        Title = "علي علوش"
                    }
                },
                new TrackViewModel
                {
                    Type = "Feature",
                    Geometry = new Geometry
                    {
                        Type = "Point",
                        Coordinates = new []
                        {
                            32.29212856087469,
                            37.29212856087469
                        }
                    },
                    Properties = new Property
                    {
                        Title = "سليم سلوم"
                    }
                },
                new TrackViewModel
                {
                    Type = "Feature",
                    Geometry = new Geometry
                    {
                        Type = "Point",
                        Coordinates = new []
                        {
                            29.29212856087469,
                            33.29212856087469
                        }
                    },
                    Properties = new Property
                    {
                        Title = "أحمد حمود"
                    }
                }
            };*/
            return Ok(dto);
        }

        [HttpGet("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip.IsActive)
                trip.IsActive = false;
            else
                trip.IsActive = true;

            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("getTripStudents/{id}")]
        public async Task<IActionResult> GetTripStudents(int id)
        {
            var ignoredId = (await _tripRepository.GetAsync()).FirstOrDefault()?.Id;
            var students = await _studentRepository
                .GetAsync(s => s.TripId == id || s.TripId == ignoredId.ToInt(), null, s => s.Parent);

            var dtos = _mapper.Map<IEnumerable<GetTripStudentDto>>(students);
            return Ok(dtos);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] GetTripStudentSaveDto dto)
        {
            int changedRecords = 0;
            foreach (var getTripStudentDto in dto.TripStudents)
            {
                var student = await _studentRepository.GetByIdAsync(getTripStudentDto.StudentId);
                if (student == null)
                    continue;
                if (student.TripId == getTripStudentDto.TripId)
                    continue;
                student.TripId = getTripStudentDto.TripId;
                changedRecords++;
            }

            if (changedRecords == 0)
                return BadRequest();

            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("takeAttendance")]
        public async Task<IActionResult> TakeAttendance([FromBody] TakeAttendanceDto dto)
        {
            var trip = await _tripRepository.GetByIdAsync(dto.TripId);
            if (trip == null)
                return NotFound();

            var student = await _studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null)
                return NotFound();

            var attendance = new Attendance
            {
                IsAttend = true,
                StudentId = student.Id,
                TripId = trip.Id,
                Time = DateTime.Now
            };
            await _attendanceRepository.AddAsync(attendance);
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public class GetTripStudentDto
    {
        public int StudentId { get; set; }

        public int TripId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class GetTripStudentSaveDto
    {
        public ICollection<GetTripStudentDto> TripStudents { get; set; }

        public GetTripStudentSaveDto()
        {
            TripStudents = new List<GetTripStudentDto>();
        }
    }

    public class TakeAttendanceDto
    {
        public int StudentId { get; set; }

        public int TripId { get; set; }
    }
}
