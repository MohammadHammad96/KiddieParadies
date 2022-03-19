using AutoMapper;
using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal;

namespace KiddieParadies.Controllers
{
    [Route("registerEmployee")]
    [Authorize]
    public class EmployeeRegistrationController : Controller
    {
        private readonly IRepository<EmployeeRegistrationInfo> _employeeRegistrationInfoRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationUser _loggedUser;
        private readonly IList<string> _userRoles;
        private readonly IRepository<Year> _yearRepository;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImagesRepository _imagesRepository;
        private readonly IFilesRepository _filesRepository;
        private const string EmployeesImagesFolderName = "employees";
        private const string EmployeesMainFolderName = "employees";
        private const string EmployeesResumesFolderName = "resumes";
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EmployeeRegistrationController(IRepository<EmployeeRegistrationInfo> employeeRegistrationRepository,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor,
            IRepository<Year> yearRepository,// IRepository<YearEmployee> yearEmployeeRepository,
             IMapper mapper, IUnitOfWork unitOfWork, IImagesRepository imagesRepository,
            SignInManager<ApplicationUser> signInManager, IRepository<Employee> employeeRepository,
            IFilesRepository filesRepository, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _yearRepository = yearRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imagesRepository = imagesRepository;
            _signInManager = signInManager;
            _employeeRepository = employeeRepository;
            _filesRepository = filesRepository;
            _roleManager = roleManager;
            _employeeRegistrationInfoRepository = employeeRegistrationRepository;
            //_yearEmployeeRepository = yearEmployeeRepository;
            _loggedUser = _userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            _userRoles = _userManager.GetRolesAsync(_loggedUser).Result;
        }

        [HttpGet("{neededUserId:int?}")]
        public async Task<IActionResult> NewEmployeeProfile(int? neededUserId)
        {
            if (_userRoles.All(r => r != "Admin") && _userRoles.Count > 0)
                return View("NotFound");

            var viewModel = new EmployeeFormViewModel();

            var year = await ThisAcademicYear();
            if (year == null)
                return View("NotFound");

            if (_userRoles.Count == 0)
            {
                await GetAvailableRoles(year.Id, viewModel.Roles);
                if (viewModel.Roles.Count == 0)
                    return View("NotFound");

                viewModel.UserId = _loggedUser.Id;
                return View("EmployeeForm", viewModel);
            }

            if (neededUserId == null)
                return View("NotFound");

            var user = await _userManager.FindByIdAsync(neededUserId.ToString());
            if (user == null)
                return View("NotFound");

            viewModel = new EmployeeFormViewModel
            {
                UserId = user.Id
            };
            return View("EmployeeForm", viewModel);
        }

        [HttpGet("edit/{id}")]
        [Authorize(Roles = "Admin,Teacher,Driver,Escort")]
        public async Task<IActionResult> EditEmployeeProfile(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return View("NotFound");

            EmployeeFormViewModel viewModel = new EmployeeFormViewModel();

            if (_userRoles.Contains("Admin"))
            {
                viewModel = _mapper.Map<EmployeeFormViewModel>(employee);

                return View("EmployeeForm", viewModel);
            }

            var year = await ThisAcademicYear();
            if (year == null)
                return View("NotFound");

            await GetAvailableRoles(year.Id, viewModel.Roles);
            if (viewModel.Roles.Count == 0)
                return View("NotFound");

            if (_loggedUser.Id != employee.UserId)
                return View("NotFound");

            if (employee.IsValid)
                return View("Error",
                    new ErrorViewModel("تم تأكيد البيانات ولم يعد بالإمكان التعديل عليها يدوياً" +
                                       "، يرجى مراسلة مدير الروضة لأجل ذلك."));

            viewModel = _mapper.Map<EmployeeFormViewModel>(employee);
            return View("EmployeeForm", viewModel);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveEmployeeProfile(EmployeeFormViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new EmployeeFormViewModel();
                ModelState.AddModelError("FirstName", "يوجد خطأ بالمدخلات");
                return View("EmployeeForm", viewModel);
            }

            if (!ModelState.IsValid)
                return View("EmployeeForm", viewModel);

            var user = await _userManager.FindByIdAsync(viewModel.UserId.ToString());
            if (user == null)
                return View("NotFound");

            Employee employee, employeeToUpdate;
            Year year;
            string previousImageName, previousResumeName;

            if (_userRoles.Contains("Admin"))
            {
                if (viewModel.Id == 0)
                {
                    year = await ThisAcademicYear();
                    if (year == null)
                        return View("NotFound");

                    employee = _mapper.Map<Employee>(viewModel);

                    employee.ImageName = await _imagesRepository
                        .Save(viewModel.Image, EmployeesImagesFolderName);
                    employee.ResumeName = await _filesRepository
                        .Save(viewModel.Resume, EmployeesMainFolderName, EmployeesResumesFolderName);
                    employee.IsValid = true;

                    employee.EmployeeYears.Add(new YearEmployee { YearId = year.Id });
                    await _employeeRepository.AddAsync(employee);
                    await _userManager.AddToRoleAsync(user, viewModel.Role);
                    //await _yearEmployeeRepository.AddAsync(new YearEmployee { EmployeeId = employee.Id, YearId = year.Id });
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return RedirectToAction("NewEmployeeProfile");

                    _imagesRepository.Delete(employee.ImageName, EmployeesImagesFolderName);
                    _filesRepository.Delete(employee.ResumeName, EmployeesMainFolderName,
                        EmployeesResumesFolderName);

                    ModelState.AddModelError("FirstName", "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً");
                    return View("EmployeeForm", viewModel);
                }

                employeeToUpdate = await _employeeRepository.GetByIdAsync(viewModel.Id);
                if (employeeToUpdate == null)
                    return View("NotFound");

                previousImageName = employeeToUpdate.ImageName;
                previousResumeName = employeeToUpdate.ResumeName;

                _mapper.Map(viewModel, employeeToUpdate);

                if (viewModel.Image != null && viewModel.Image.Length != 0)
                    employeeToUpdate.ImageName = await _imagesRepository
                        .Save(viewModel.Image, EmployeesImagesFolderName);
                if (viewModel.Resume != null && viewModel.Resume.Length != 0)
                    employeeToUpdate.ResumeName = await _filesRepository
                        .Save(viewModel.Resume, EmployeesMainFolderName, EmployeesResumesFolderName);
                employeeToUpdate.IsValid = true;
                await _userManager
                    .RemoveFromRoleAsync(user, (await _userManager.GetRolesAsync(user)).FirstOrDefault());

                if (await _unitOfWork.SaveChangesAsync() <= 0)
                {
                    if (viewModel.Image != null && viewModel.Image.Length != 0)
                        _imagesRepository.Delete(employeeToUpdate.ImageName, EmployeesImagesFolderName);
                    if (viewModel.Resume != null && viewModel.Resume.Length != 0)
                        _filesRepository.Delete(employeeToUpdate.ResumeName, EmployeesMainFolderName,
                        EmployeesResumesFolderName);

                    ModelState.AddModelError("FirstName", "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً");
                    return View("EmployeeForm", viewModel);
                }
                await _userManager.AddToRoleAsync(user, viewModel.Role);
                await _unitOfWork.SaveChangesAsync();

                if (viewModel.Image != null && viewModel.Image.Length != 0)
                    _imagesRepository.Delete(previousImageName, EmployeesImagesFolderName);
                if (viewModel.Resume != null && viewModel.Resume.Length != 0)
                    _filesRepository.Delete(previousResumeName, EmployeesMainFolderName,
                        EmployeesResumesFolderName);

                return RedirectToAction("NewEmployeeProfile");
            }

            if (viewModel.UserId != _loggedUser.Id)
                return View("NotFound");

            year = await ThisAcademicYear();
            if (year == null)
                return View("NotFound");

            await GetAvailableRoles(year.Id, viewModel.Roles);
            if (viewModel.Roles.Count == 0)
                return View("NotFound");

            if (!viewModel.Roles.ContainsKey(viewModel.Role))
                return View("NotFound");

            if (viewModel.UserId != _loggedUser.Id)
                return View("NotFound");

            if (viewModel.Id == 0)
            {
                employee = _mapper.Map<Employee>(viewModel);

                employee.ImageName = await _imagesRepository
                    .Save(viewModel.Image, EmployeesImagesFolderName);
                employee.ResumeName = await _filesRepository
                    .Save(viewModel.Resume, EmployeesMainFolderName, EmployeesResumesFolderName);
                employee.EmployeeYears.Add(new YearEmployee { YearId = year.Id });
                await _employeeRepository.AddAsync(employee);
                /*await _yearEmployeeRepository.AddAsync(new YearEmployee
                {
                    EmployeeId = employee.Id,
                    YearId = year.Id
                });*/
                await _userManager.AddToRoleAsync(user, viewModel.Role);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    await _signInManager.SignInAsync(user, true);
                    return RedirectToAction("NewEmployeeProfile");
                }

                _imagesRepository.Delete(employee.ImageName, EmployeesImagesFolderName);
                _filesRepository.Delete(employee.ResumeName, EmployeesMainFolderName,
                    EmployeesResumesFolderName);
                ModelState.AddModelError("FirstName", "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً");
                return View("EmployeeForm", viewModel);
            }

            employeeToUpdate = await _employeeRepository.GetByIdAsync(viewModel.Id);
            if (employeeToUpdate == null)
                return View("NotFound");

            if (employeeToUpdate.IsValid)
                return View("Error",
                    new ErrorViewModel("تم تأكيد البيانات ولم يعد بالإمكان التعديل عليها يدوياً" +
                                       "، يرجى مراسلة مدير الروضة لأجل ذلك."));

            previousImageName = employeeToUpdate.ImageName;
            previousResumeName = employeeToUpdate.ResumeName;

            _mapper.Map(viewModel, employeeToUpdate);
            if (viewModel.Image != null && viewModel.Image.Length != 0)
                employeeToUpdate.ImageName = await _imagesRepository
                    .Save(viewModel.Image, EmployeesImagesFolderName);
            if (viewModel.Resume != null && viewModel.Resume.Length != 0)
                employeeToUpdate.ResumeName = await _filesRepository
                    .Save(viewModel.Resume, EmployeesMainFolderName, EmployeesResumesFolderName);

            await _userManager.AddToRoleAsync(user, viewModel.Role);


            await _userManager.GetRolesAsync(user);
            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, _userRoles);
                await _unitOfWork.SaveChangesAsync();
                if (viewModel.Image != null && viewModel.Image.Length != 0)
                    _imagesRepository.Delete(previousImageName, EmployeesImagesFolderName);
                if (viewModel.Resume != null && viewModel.Resume.Length != 0)
                    _filesRepository.Delete(previousResumeName, EmployeesMainFolderName,
                        EmployeesResumesFolderName);

                await _signInManager.SignInAsync(user, true);
                return RedirectToAction("NewEmployeeProfile");
            }

            if (viewModel.Image != null && viewModel.Image.Length != 0)
                _imagesRepository.Delete(employeeToUpdate.ImageName, EmployeesImagesFolderName);
            if (viewModel.Resume != null && viewModel.Resume.Length != 0)
                _filesRepository.Delete(employeeToUpdate.ResumeName, EmployeesMainFolderName,
                    EmployeesResumesFolderName);

            ModelState.AddModelError("FirstName", "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً");
            return View("EmployeeForm", viewModel);
        }

        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var roles = _roleManager.Roles.ToList();
            var employees = await _employeeRepository
                .GetAsync(null, null, e => e.User.UserRoles);

            foreach (var employee in employees)
            {
                var employeeRoleId = employee.User.UserRoles.First().RoleId;
                if (roles.Any(r => r.Id == employeeRoleId))
                {
                    var roleName = roles.First(r => r.Id == employeeRoleId).Name;
                    if (roleName == "Teacher")
                        roleName = "معلم";
                    else if (roleName == "Driver")
                        roleName = "سائق";
                    else if (roleName == "Escort")
                        roleName = "مرافق سائق";

                    employee.User.UserRoles = new List<ApplicationUserRole>
                        {
                            new ApplicationUserRole
                            {
                                Role = new ApplicationRole
                                {
                                    Name = roleName
                                }
                            }
                        };
                }
            }
            return View(employees);
        }

        private async Task GetAvailableRoles(int yearId, Dictionary<string, string> roles)
        {
            var count = (await _employeeRegistrationInfoRepository
                    .GetAsync(info => info.FromDate <= DateTime.Now
                                      && info.ToDate > DateTime.Now
                                      && info.YearId == yearId))
                .Aggregate(
                    new EmployeeRegistrationInfo(), (info, registrationInfo) =>
                    {
                        info.Teacher += registrationInfo.Teacher;
                        info.Driver += registrationInfo.Driver;
                        info.Escort += registrationInfo.Escort;
                        return info;
                    });

            if (count.Teacher == 0)
                roles.Remove("Teacher");
            if (count.Driver == 0)
                roles.Remove("Driver");
            if (count.Escort == 0)
                roles.Remove("Escort");
        }

        private async Task<Year> ThisAcademicYear()
        {
            return (await _yearRepository
                    .GetAsync(y => y.FromDate < DateTime.Now && y.ToDate > DateTime.Now))
                .FirstOrDefault();
        }
    }
}