using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using KiddieParadies.Areas.Identity.Pages.Account;
using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.Extensions;
using KiddieParadies.Helpers;
using KiddieParadies.Services;
using KiddieParadies.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KiddieParadies.Controllers
{
    [Route("parentInfo")]
    [Authorize]
    public class ParentsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Parent> _parentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationUser _loggedUser;
        private readonly IWebHostEnvironment _host;

        public ParentsController(IHttpContextAccessor httpContextAccessor,
            IRepository<Parent> parentRepository, IUnitOfWork unitOfWork, IMapper mapper,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment host)
        {
            _httpContextAccessor = httpContextAccessor;
            _parentRepository = parentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _loggedUser = _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name).Result;
            _host = host;
        }

        [HttpGet("add/{neededUserId?}")]
        public async Task<IActionResult> New(int? neededUserId)
        {
            var userRoles = await _userManager.GetRolesAsync(_loggedUser);
            if (userRoles.Where(r => r == "Admin").Count() == 0 && userRoles.Count > 0)
                return View("NotFound");
            
            var viewModel = new ParentFormViewModel();
            // if ((await _userManager.GetRolesAsync(_loggedUser)).Count == 0)
            //     viewModel.UserId = _loggedUser.Id;
            
            if (neededUserId != null)
            {
                if (userRoles.Count == 0)
                {
                    viewModel.UserId = _loggedUser.Id;
                    return View("ParentForm", viewModel);
                }
                
                var user = await _userManager.FindByIdAsync(neededUserId.ToString());
                if (user == null)
                    return View("NotFound");
                
                viewModel.UserId = user.Id;
            }
            else
            {
                if (userRoles.Any(r => r == "Admin"))
                    return View("NotFound");
            }

            ViewData["Title"] = "إضافة بيانات الأهل";
            viewModel.UserId = _loggedUser.Id;
            return View("ParentForm", viewModel);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save(ParentFormViewModel viewModel)
        {
            if (viewModel == null)
            {
                ModelState.AddModelError(viewModel.GetPropertyDisplayName(v => v.FatherName), 
                    "يوجد خطأ بالمدخلات");
                return View("ParentForm", viewModel);
            }

            if (!ModelState.IsValid)
                return View("ParentForm", viewModel);
            
            var userRoles = await _userManager.GetRolesAsync(_loggedUser);
            string[] acceptedFileTypes = new string[] { ".jpg", ".jpeg", ".png" };
            var imageExtension = string.Empty;

            if (viewModel.Id == 0)
            {
                if (viewModel.FatherIdentityImage == null || viewModel.FatherIdentityImage.Length == 0)
                {
                    ModelState.AddModelError("FatherIdentityImage", 
                        "صورة صفحة الأب بدفتر العائلة إجبارية");
                    return View("ParentForm", viewModel);
                }
                if (viewModel.MotherIdentityImage == null || viewModel.MotherIdentityImage.Length == 0)
                {
                    ModelState.AddModelError("MotherIdentityImage", 
                        "صورة صفحة الأم بدفتر العائلة إجبارية");
                    return View("ParentForm", viewModel);
                }
                imageExtension = Path.GetExtension(viewModel.FatherIdentityImage.FileName)
                    .ToLower();
                if (!acceptedFileTypes.Any(s => s == imageExtension))
                {
                    ModelState.AddModelError("FatherIdentityImage", "لاحقة الصورة يجب أن تكون jpg أو jpeg أو png");
                    return View("ParentForm", viewModel);
                }
                imageExtension = Path.GetExtension(viewModel.MotherIdentityImage.FileName)
                    .ToLower();
                if (!acceptedFileTypes.Any(s => s == imageExtension))
                {
                    ModelState.AddModelError("MotherIdentityImage", "لاحقة الصورة يجب أن تكون jpg أو jpeg أو png");
                    return View("ParentForm", viewModel);
                }

                if (userRoles.Any(r => r == "Admin") || userRoles.Count == 0)
                {
                    if (userRoles.Count == 0 && _loggedUser.Id == viewModel.UserId)
                    {
                        var parent = new Parent();
                        parent = _mapper.Map<Parent>(viewModel);

                        var parentsIdentityImagesFolderPath = Path
                            .Combine(_host.ContentRootPath + "\\wwwroot", "images", "parentsIdentity");
                        if (!Directory.Exists(parentsIdentityImagesFolderPath))
                            Directory.CreateDirectory(parentsIdentityImagesFolderPath);

                        var fatherFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.FatherIdentityImage.FileName);
                        var fatherFilePath = Path.Combine(parentsIdentityImagesFolderPath, fatherFileName);

                        using (var stream = new FileStream(fatherFilePath, FileMode.Create))
                        {
                            await viewModel.FatherIdentityImage.CopyToAsync(stream);
                        }
                        parent.FatherIdentityImagePath = fatherFileName;

                        var motherFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.MotherIdentityImage.FileName);
                        var motherFilePath = Path.Combine(parentsIdentityImagesFolderPath, motherFileName);

                        using (var stream = new FileStream(motherFilePath, FileMode.Create))
                        {
                            await viewModel.MotherIdentityImage.CopyToAsync(stream);
                        }
                        parent.MotherIdentityImagePath= motherFileName;

                        await _parentRepository.AddAsync(parent);
                        if (!userRoles.Any(r => r == "Admin"))
                            await _userManager.AddToRoleAsync(_loggedUser, "Parent");
                        if (await _unitOfWork.SaveChangesAsync() <= 0)
                        {
                            parent.FatherIdentityImagePath = Path
                                .Combine(_host.ContentRootPath + "\\wwwroot", "images", "parentsIdentity", 
                                parent.FatherIdentityImagePath);
                            System.IO.File.Delete(parent.FatherIdentityImagePath);
                            parent.MotherIdentityImagePath = Path
                                .Combine(_host.ContentRootPath + "\\wwwroot", "images", "parentsIdentity", 
                                parent.MotherIdentityImagePath);
                            System.IO.File.Delete(parent.MotherIdentityImagePath);
                            ModelState.AddModelError("FatherName", "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً");
                            return View("ParentForm", viewModel);
                        }
                        return RedirectToAction("New");
                    }
                }

                return View("NotFound");
            }

            if (viewModel.FatherIdentityImage != null && viewModel.FatherIdentityImage.Length != 0)
            {
                imageExtension = Path.GetExtension(viewModel.FatherIdentityImage.FileName)
                    .ToLower();
                if (!acceptedFileTypes.Any(s => s == imageExtension))
                {
                    ModelState.AddModelError("FatherIdentityImage", "لاحقة الصورة يجب أن تكون jpg أو jpeg أو png");
                    return View("ParentForm", viewModel);
                }
            }
            if (viewModel.MotherIdentityImage != null && viewModel.MotherIdentityImage.Length != 0)
            {
                imageExtension = Path.GetExtension(viewModel.MotherIdentityImage.FileName)
                    .ToLower();
                if (!acceptedFileTypes.Any(s => s == imageExtension))
                {
                    ModelState.AddModelError("MotherIdentityImage", "لاحقة الصورة يجب أن تكون jpg أو jpeg أو png");
                    return View("ParentForm", viewModel);
                }
            }
            

            var parentToUpdate = await _parentRepository.GetByIdAsync(viewModel.Id);
            if (parentToUpdate == null)
                return View("NotFound");
            
            if (userRoles.Any(r => r == "Admin") 
                || (userRoles.Any(r => r == "Parent") && (_loggedUser.Id == viewModel.UserId)))
            {
                var previousFatherImageName = parentToUpdate.FatherIdentityImagePath;
                var previousMotherImageName = parentToUpdate.MotherIdentityImagePath;

                _mapper.Map<ParentFormViewModel, Parent>(viewModel, parentToUpdate);
                var parentsIdentityImagesFolderPath = Path
                    .Combine(_host.ContentRootPath + "\\wwwroot", "images", "parentsIdentity");
                if (viewModel.FatherIdentityImage != null && viewModel.FatherIdentityImage.Length != 0)
                {
                    var fatherFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.FatherIdentityImage.FileName);
                    var fatherFilePath = Path.Combine(parentsIdentityImagesFolderPath, fatherFileName);

                    using (var stream = new FileStream(fatherFilePath, FileMode.Create))
                    {
                        await viewModel.FatherIdentityImage.CopyToAsync(stream);
                    }
                    parentToUpdate.FatherIdentityImagePath= fatherFileName;
                }
                if (viewModel.MotherIdentityImage != null && viewModel.MotherIdentityImage.Length != 0)
                {
                    var motherFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.MotherIdentityImage.FileName);
                    var motherFilePath = Path.Combine(parentsIdentityImagesFolderPath, motherFileName);

                    using (var stream = new FileStream(motherFilePath, FileMode.Create))
                    {
                        await viewModel.MotherIdentityImage.CopyToAsync(stream);
                    }
                    parentToUpdate.MotherIdentityImagePath = motherFileName;
                }
                if (await _unitOfWork.SaveChangesAsync() <= 0)
                {
                    parentToUpdate.FatherIdentityImagePath = Path
                        .Combine(_host.ContentRootPath + "\\wwwroot", "images", "parentsIdentity", 
                        parentToUpdate.FatherIdentityImagePath);
                    System.IO.File.Delete(parentToUpdate.FatherIdentityImagePath);
                    parentToUpdate.MotherIdentityImagePath = Path
                        .Combine(_host.ContentRootPath + "\\wwwroot", "images", "parentsIdentity", 
                        parentToUpdate.MotherIdentityImagePath);
                    System.IO.File.Delete(parentToUpdate.MotherIdentityImagePath);
                    ModelState.AddModelError("FatherName", "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً");
                    return View("ParentForm", viewModel);
                }

                if (viewModel.FatherIdentityImage != null && viewModel.FatherIdentityImage.Length != 0)
                {
                    previousFatherImageName = Path
                        .Combine(_host.ContentRootPath + "\\wwwroot", "images", "parentsIdentity", 
                        previousFatherImageName);
                    System.IO.File.Delete(previousFatherImageName);
                }
                if (viewModel.MotherIdentityImage != null && viewModel.MotherIdentityImage.Length != 0)
                {
                    previousMotherImageName = Path
                        .Combine(_host.ContentRootPath + "\\wwwroot", "images", "parentsIdentity", 
                        previousMotherImageName);
                    System.IO.File.Delete(previousMotherImageName);
                }
                
                return RedirectToAction("New");
            }
            
            return View("NotFound");
        }

        [HttpGet("edit/{id}")]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<IActionResult> Edit(int id)
        {
            var userRoles = await _userManager.GetRolesAsync(_loggedUser);
            if (userRoles.Any(r => r == "Admin"))
            {
                var parent = await _parentRepository.GetByIdAsync(id);
                if (parent == null)
                    return View("NotFound");

                var viewModel = _mapper.Map<ParentFormViewModel>(parent);
                ViewData["Title"] = "تعديل بيانات الأهل";
                return View("ParentForm", viewModel);
            }
            else if (userRoles.Any(r => r == "Parent"))
            {
                var parent = await _parentRepository.GetByIdAsync(id);
                if (parent == null)
                    return View("NotFound");
                
                if (_loggedUser.Id == parent.UserId)
                {
                    var viewModel = _mapper.Map<ParentFormViewModel>(parent);
                    ViewData["Title"] = "تعديل بيانات الأهل";
                    return View("ParentForm", viewModel);
                }

                return View("NotFound");
            }

            return View("NotFound");
        }
    
        [HttpGet("preview/{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var parent = await _parentRepository.GetByIdAsync(id);
            if (parent == null)
                return View(SharedViews.NotFound);

            return View("Preview", parent);
        }
    }
}