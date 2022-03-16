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

        public MessageController(HttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IRepository<Employee> employeeRepository, IRepository<Parent> parentRepository, IRepository<Message> messageRepository, IMapper mapper, IUnitOfWork unitOfWork, IHubContext<NotificationUserHub> notificationUserHubContext, IUserConnectionManager userConnectionManager)
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

        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var userId = _httpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;

            var employees = await _employeeRepository.GetAsync(null, null, e => e.User);
            var parents = await _parentRepository.GetAsync(null, null, e => e.User);

            var result = employees.Select(e => new IdNameDto
            {
                Name = e.FirstName + " " + e.LastName,
                Id = e.UserId
            }).ToList();

            result.AddRange(parents.Select(e => new IdNameDto
            {
                Name = e.FatherName + " " + e.FatherLastName,
                Id = e.UserId
            }));

            result.Add(new IdNameDto { Id = 1, Name = "مدير الروضة" });

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

            return Ok(chat);
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
                            .SendAsync("sendToUser", messageDto);

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
