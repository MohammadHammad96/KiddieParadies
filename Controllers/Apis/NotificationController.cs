using AutoMapper;
using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.Extensions;
using KiddieParadies.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers.Apis
{
    [Authorize]
    [Route("api/notification")]
    public class NotificationController : Controller
    {
        private readonly HttpContext _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Parent> _parentRepository;
        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
        private readonly IUserConnectionManager _userConnectionManager;

        public NotificationController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IRepository<Employee> employeeRepository, IRepository<Parent> parentRepository, IRepository<Message> messageRepository, IMapper mapper, IRepository<Notification> notificationRepository, IUnitOfWork unitOfWork, IHubContext<NotificationUserHub> notificationUserHubContext, IUserConnectionManager userConnectionManager)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _userManager = userManager;
            _employeeRepository = employeeRepository;
            _parentRepository = parentRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _notificationUserHubContext = notificationUserHubContext;
            _userConnectionManager = userConnectionManager;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = _httpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;

            var notifications = await _notificationRepository
                .GetAsync(n => n.UserId == userId.ToInt(),
                    o => o.OrderBy(n => n.Time));

            var result = notifications.Select(n => new
            {
                n.Title,
                n.Url,
                n.Time,
                n.IsRead,
                n.Id
            }).ToList();
            return Ok(result);
        }

        [HttpGet("markAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return NoContent();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
