using AutoMapper;
using KiddieParadies.Controllers.Apis.Dtos;
using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.Extensions;
using KiddieParadies.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers.Apis
{
    [Route("api/chat")]
    [Authorize(Roles = "Admin,Teacher,Driver,Escort,Parent")]
    public class MessageController : Controller
    {
        private readonly HttpContext _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Parent> _parentRepository;
        private readonly IRepository<Message> _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
        private readonly IUserConnectionManager _userConnectionManager;

        public MessageController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IRepository<Employee> employeeRepository, IRepository<Parent> parentRepository, IRepository<Message> messageRepository, IMapper mapper, IUnitOfWork unitOfWork, IHubContext<NotificationUserHub> notificationUserHubContext, IUserConnectionManager userConnectionManager)
        {
            _userManager = userManager;
            _employeeRepository = employeeRepository;
            _parentRepository = parentRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _notificationUserHubContext = notificationUserHubContext;
            _userConnectionManager = userConnectionManager;
            _httpContext = httpContextAccessor.HttpContext;
        }

        [HttpGet("getUsers/{name?}")]
        public async Task<IActionResult> GetUsers(string name)
        {
            var userId = _httpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;

            Expression<Func<Employee, bool>> employeeFilters = null;
            Expression<Func<Parent, bool>> parentFilters = null;

            if (!string.IsNullOrWhiteSpace(name))
            {
                employeeFilters = e => (e.FirstName + " " + e.LastName).Contains(name);
                parentFilters = p => (p.FatherName + " " + p.FatherLastName).Contains(name);
            }

            var employees = await _employeeRepository
                .GetAsync(employeeFilters, null, e => e.User);
            var parents = await _parentRepository
                .GetAsync(parentFilters, null, p => p.User, p => p.Children);

            var result = employees.Select(e => new UserChatDto
            {
                Name = e.FirstName + " " + e.LastName,
                Id = e.UserId,
                ImagePath = string.Concat("/images/employees/", e.ImageName)
            }).ToList();

            result.AddRange(parents.Select(p => new UserChatDto
            {
                Name = p.FatherName + " " + p.FatherLastName,
                Id = p.UserId,
                ImagePath = string.Concat("/images/students/", p.Children.FirstOrDefault()?.ImageName)
            }));

            if (!string.IsNullOrWhiteSpace(name))
            {
                if (new string("مدير الروضة").Contains(name))
                    result.Add(new UserChatDto
                    {
                        Id = 1,
                        Name = "مدير الروضة",
                        ImagePath = "/Logo.png"
                    });
            }
            else
            {
                result.Add(new UserChatDto
                {
                    Id = 1,
                    Name = "مدير الروضة",
                    ImagePath = "/Logo.png"
                });
            }

            if (result.Any(u => u.Id.ToString() == userId))
                result.Remove(result.First(u => u.Id.ToString() == userId));

            return Ok(result);
        }

        [HttpGet("getChat/{secondUserId}")]
        [Authorize]
        public async Task<IActionResult> GetChat(int secondUserId)
        {
            var firstUserId = _httpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value.ToInt();
            var usersId = new[] { firstUserId, secondUserId };

            var chat = await _messageRepository
                .GetAsync(m => usersId.Contains(m.SenderId) && usersId.Contains(m.ReceiverId),
                    m => m.OrderByDescending(chat => chat.Time));

            var dto = _mapper.Map<IEnumerable<MessageDto>>(chat);
            return Ok(dto);
        }

        [HttpPost("send")]
        public async Task<ActionResult> Send([FromBody] MessageDto messageDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            messageDto.Time = DateTime.Now;

            var message = _mapper.Map<Message>(messageDto);

            await _messageRepository.AddAsync(message);

            //var x = await _dbContext.SaveChangesAsync();

            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError);

            _mapper.Map(message, messageDto);

            try
            {
                var connections = _userConnectionManager.GetUserConnections(messageDto.ReceiverId.ToString());

                if (connections == null || connections.Count == 0)
                    return NoContent();

                var errors = new List<string>();
                foreach (var connectionId in connections)
                {
                    try
                    {
                        await _notificationUserHubContext.Clients.Client(connectionId)
                            .SendAsync("sendMessage", messageDto);

                    }
                    catch (Exception)
                    {
                        errors.Add($"Sorry, Can not send this message to connection id: {connectionId}");
                    }
                }

                return errors.Count > 0 ? BadRequest(errors) : Ok(messageDto);
            }
            catch (Exception)
            {
                return Ok(messageDto);
            }

        }

        [HttpGet("getuserInfo/{id}")]
        public async Task<IActionResult> GetUserInfo(int id)
        {
            if (id == 1)
                return Ok(new
                {
                    Name = "مدير الروضة",
                    ImagePath = "/Logo.png"
                });

            var employee = (await _employeeRepository.GetAsync(e => e.UserId == id))
                .FirstOrDefault();
            if (employee != null)
                return Ok(new
                {
                    Name = string.Join(" ", employee.FirstName, employee.LastName),
                    ImagePath = string.Concat("/images/employees/", employee.ImageName)
                });

            var parent = (await _parentRepository
                    .GetAsync(p => p.UserId == id, null, p => p.Children))
                .FirstOrDefault();
            return Ok(new
            {
                Name = parent.FatherName + " " + parent.FatherLastName,
                ImagePath = parent.Children.FirstOrDefault()?.ImageName
            });
        }

        /*[HttpPost("SendToAllUsers")]
        public async Task<IActionResult> SendToAllUsers([FromBody] Article article)
        {
            try
            {
                await _notificationUserHubContext.Clients.All.SendAsync("sendToUser",
                    article.ArticleHeading, article.ArticleContent);

                return Ok(article);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }*/
    }
}
