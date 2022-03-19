using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers.Apis
{
    [Route("api/trip")]
    public class TripController : Controller
    {
        private readonly IRepository<Trip> _tripRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TripController(IRepository<Trip> tripRepository, IUnitOfWork unitOfWork)
        {
            _tripRepository = tripRepository;
            _unitOfWork = unitOfWork;
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
    }
}
