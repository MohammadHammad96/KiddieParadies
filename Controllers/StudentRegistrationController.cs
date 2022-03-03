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

namespace KiddieParadies.Controllers
{
    [Route("registerStudent")]
    [Authorize]
    public class StudentRegistrationController : Controller
    {
        private readonly IRepository<StudentRegistrationInfo> _studentRegistrationInfoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationUser _loggedUser;
        private readonly IList<string> _userRoles;
        private readonly IRepository<Year> _yearRepository;
        private readonly IRepository<YearStudent> _yearStudentRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Parent> _parentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImagesRepository _imagesRepository;
        private const string ParentsImagesFolderName = "parentsIdentity";
        private const string StudentsImagesFolderName = "students";

        public StudentRegistrationController(IRepository<StudentRegistrationInfo> studentRegistrationInfoRepository,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor,
            IRepository<Year> yearRepository, IRepository<YearStudent> yearStudentRepository,
            IRepository<Parent> parentRepository, IMapper mapper, IUnitOfWork unitOfWork,
            IRepository<Student> studentRepository, IImagesRepository imagesRepository)
        {
            _studentRegistrationInfoRepository = studentRegistrationInfoRepository;
            _userManager = userManager;
            _yearRepository = yearRepository;
            _yearStudentRepository = yearStudentRepository;
            _parentRepository = parentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _studentRepository = studentRepository;
            _imagesRepository = imagesRepository;
            _loggedUser = _userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            _userRoles = _userManager.GetRolesAsync(_loggedUser).Result;
        }

        [HttpGet("parentsInfo/{neededUserId?}")]
        public async Task<IActionResult> NewParentsProfile(int? neededUserId)
        {
            if (_userRoles.All(r => r != "Admin") && _userRoles.Count > 0)
                return View("NotFound");

            var viewModel = new ParentFormViewModel();

            var year = await ThisAcademicYear();
            if (year == null)
                return View("NotFound");

            if (_userRoles.Count == 0)
            {
                if (await RegistrationAvailable(year.Id) == 0)
                    return View("NotFound");

                viewModel.UserId = _loggedUser.Id;
                return View("ParentForm", viewModel);
            }

            if (neededUserId == null)
                return View("NotFound");

            var user = await _userManager.FindByIdAsync(neededUserId.ToString());
            if (user == null)
                return View("NotFound");

            viewModel.UserId = user.Id;
            return View("ParentForm", viewModel);
        }

        [HttpGet("parentsInfo/edit/{id}")]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<IActionResult> EditParentsProfile(int id)
        {
            var parent = await _parentRepository.GetByIdAsync(id);
            if (parent == null)
                return View("NotFound");

            ParentFormViewModel viewModel;

            if (_userRoles.Contains("Admin"))
            {
                viewModel = _mapper.Map<ParentFormViewModel>(parent);
                return View("ParentForm", viewModel);
            }

            if (parent.UserId != _loggedUser.Id)
                return View("NotFound");

            var year = await ThisAcademicYear();
            if (year == null)
                return View("NotFound");

            if (await RegistrationAvailable(year.Id) == 0)
                return View("NotFound");

            if (parent.IsValid)
                return View("Error",
                    new ErrorViewModel(" „  √ﬂÌœ «·»Ì«‰«  Ê·„ Ì⁄œ »«·≈„ﬂ«‰ «· ⁄œÌ· ⁄·ÌÂ« ÌœÊÌ«" +
                                       "° Ì—ÃÏ „—«”·… „œÌ— «·—Ê÷… ·√Ã· –·ﬂ."));

            viewModel = _mapper.Map<ParentFormViewModel>(parent);
            return View("ParentForm", viewModel);
        }

        [HttpPost("parentsInfo/save")]
        public async Task<IActionResult> SaveParentsProfile(ParentFormViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ParentFormViewModel();
                ModelState.AddModelError("FatherName", "ÌÊÃœ Œÿ√ »«·„œŒ·« ");
                return View("ParentForm", viewModel);
            }

            if (!ModelState.IsValid)
                return View("ParentForm", viewModel);

            ApplicationUser user;
            Parent parent;

            if (_userRoles.Contains("Admin"))
            {
                if (viewModel.UserId == _loggedUser.Id)
                    return View("Error",
                        new ErrorViewModel("·« Ì„ﬂ‰ﬂ «” Œœ«„ Õ”«» «·≈œ«—… ﬂÊ·Ì √„—° ÌÃ» «” Œœ«„ Õ”«» ¬Œ—"));

                user = await _userManager.FindByIdAsync(viewModel.UserId.ToString());
                if (user == null)
                    return View("NotFound");

                if (viewModel.Id == 0)
                {
                    parent = _mapper.Map<Parent>(viewModel);

                    parent.FatherIdentityImageName = await _imagesRepository
                        .Save(viewModel.FatherIdentityImage, ParentsImagesFolderName);

                    parent.MotherIdentityImageName = await _imagesRepository
                        .Save(viewModel.MotherIdentityImage, ParentsImagesFolderName);
                    parent.IsValid = true;

                    await _parentRepository.AddAsync(parent);
                    await _userManager.AddToRoleAsync(user, "Parent");
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return RedirectToAction("NewParentsProfile");

                    _imagesRepository.Delete(parent.FatherIdentityImageName, ParentsImagesFolderName);
                    _imagesRepository.Delete(parent.MotherIdentityImageName, ParentsImagesFolderName);

                    ModelState.AddModelError("FatherName", "ÌÊÃœ Œÿ√ »«·„Œœ„° Ì—ÃÏ «·„Õ«Ê·… ·«Õﬁ«");
                    return View("ParentForm", viewModel);
                }

                var parentToUpdate = await _parentRepository.GetByIdAsync(viewModel.Id);
                if (parentToUpdate == null)
                    return View("NotFound");

                var previousFatherImageName = parentToUpdate.FatherIdentityImageName;
                var previousMotherImageName = parentToUpdate.MotherIdentityImageName;
                parentToUpdate.IsValid = true;

                _mapper.Map(viewModel, parentToUpdate);

                if (viewModel.FatherIdentityImage != null && viewModel.FatherIdentityImage.Length != 0)
                    parentToUpdate.FatherIdentityImageName = await _imagesRepository
                        .Save(viewModel.FatherIdentityImage, ParentsImagesFolderName);

                if (viewModel.MotherIdentityImage != null && viewModel.MotherIdentityImage.Length != 0)
                    parentToUpdate.MotherIdentityImageName = await _imagesRepository
                        .Save(viewModel.MotherIdentityImage, ParentsImagesFolderName);

                if (await _unitOfWork.SaveChangesAsync() <= 0)
                {
                    _imagesRepository.Delete(parentToUpdate.FatherIdentityImageName, ParentsImagesFolderName);
                    _imagesRepository.Delete(parentToUpdate.MotherIdentityImageName, ParentsImagesFolderName);

                    ModelState.AddModelError("FatherName", "ÌÊÃœ Œÿ√ »«·„Œœ„° Ì—ÃÏ «·„Õ«Ê·… ·«Õﬁ«");
                    return View("ParentForm", viewModel);
                }

                if (viewModel.FatherIdentityImage != null && viewModel.FatherIdentityImage.Length != 0)
                    _imagesRepository.Delete(previousFatherImageName, ParentsImagesFolderName);

                if (viewModel.MotherIdentityImage != null && viewModel.MotherIdentityImage.Length != 0)
                    _imagesRepository.Delete(previousMotherImageName, ParentsImagesFolderName);

                return RedirectToAction("NewParentsProfile");
            }

            Year year;

            if (_userRoles.Contains("Parent"))
            {
                year = await ThisAcademicYear();
                if (year == null)
                    return View("NotFound");

                if (await RegistrationAvailable(year.Id) == 0)
                    return View("NotFound");

                var parentToUpdate = await _parentRepository.GetByIdAsync(viewModel.Id);
                if (parentToUpdate == null)
                    return View("NotFound");

                if (parentToUpdate.UserId != _loggedUser.Id)
                    return View("NotFound");

                if (parentToUpdate.Id != viewModel.Id)
                    return View("NotFound");

                if (parentToUpdate.IsValid) // should move to student registration
                    return View("Error",
                        new ErrorViewModel(" „  √ﬂÌœ «·»Ì«‰«  Ê·„ Ì⁄œ »«·≈„ﬂ«‰ «· ⁄œÌ· ⁄·ÌÂ« ÌœÊÌ«" +
                                           "° Ì—ÃÏ „—«”·… „œÌ— «·—Ê÷… ·√Ã· –·ﬂ."));

                var previousFatherImageName = parentToUpdate.FatherIdentityImageName;
                var previousMotherImageName = parentToUpdate.MotherIdentityImageName;

                _mapper.Map(viewModel, parentToUpdate);

                if (viewModel.FatherIdentityImage != null && viewModel.FatherIdentityImage.Length != 0)
                    parentToUpdate.FatherIdentityImageName = await _imagesRepository
                        .Save(viewModel.FatherIdentityImage, ParentsImagesFolderName);

                if (viewModel.MotherIdentityImage != null && viewModel.MotherIdentityImage.Length != 0)
                    parentToUpdate.MotherIdentityImageName = await _imagesRepository
                        .Save(viewModel.MotherIdentityImage, ParentsImagesFolderName);

                if (await _unitOfWork.SaveChangesAsync() <= 0)
                {
                    _imagesRepository.Delete(parentToUpdate.FatherIdentityImageName, ParentsImagesFolderName);
                    _imagesRepository.Delete(parentToUpdate.MotherIdentityImageName, ParentsImagesFolderName);

                    ModelState.AddModelError("FatherName", "ÌÊÃœ Œÿ√ »«·„Œœ„° Ì—ÃÏ «·„Õ«Ê·… ·«Õﬁ«");
                    return View("ParentForm", viewModel);
                }

                if (viewModel.FatherIdentityImage != null && viewModel.FatherIdentityImage.Length != 0)
                    _imagesRepository.Delete(previousFatherImageName, ParentsImagesFolderName);

                if (viewModel.MotherIdentityImage != null && viewModel.MotherIdentityImage.Length != 0)
                    _imagesRepository.Delete(previousMotherImageName, ParentsImagesFolderName);

                // move to student registration
                return Ok();
            }

            if (_userRoles.Count > 0)
                return View("NotFound");


            if (viewModel.Id != 0)
                return View("NotFound");

            if (viewModel.UserId != _loggedUser.Id)
                return View("NotFound");

            year = await ThisAcademicYear();
            if (year == null)
                return View("NotFound");

            if (await RegistrationAvailable(year.Id) == 0)
                return View("NotFound");

            parent = _mapper.Map<Parent>(viewModel);

            parent.FatherIdentityImageName = await _imagesRepository
                .Save(viewModel.FatherIdentityImage, ParentsImagesFolderName);

            parent.MotherIdentityImageName = await _imagesRepository
                .Save(viewModel.MotherIdentityImage, ParentsImagesFolderName);

            await _parentRepository.AddAsync(parent);
            user = await _userManager.FindByIdAsync(_loggedUser.Id.ToString());
            await _userManager.AddToRoleAsync(user, "Parent");
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return RedirectToAction("NewParentsProfile");

            _imagesRepository.Delete(parent.FatherIdentityImageName, ParentsImagesFolderName);
            _imagesRepository.Delete(parent.MotherIdentityImageName, ParentsImagesFolderName);

            ModelState.AddModelError("FatherName", "ÌÊÃœ Œÿ√ »«·„Œœ„° Ì—ÃÏ «·„Õ«Ê·… ·«Õﬁ«");
            return View("ParentForm", viewModel);
        }

        [HttpGet("studentInfo/{neededParentId?}")]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<IActionResult> NewStudentProfile(int? neededParentId)
        {
            var viewModel = new StudentFormViewModel { IsMale = true };

            var year = await ThisAcademicYear();
            if (year == null)
                return View("NotFound");

            Parent parent;

            if (_userRoles.Contains("Parent"))
            {
                var availableLevels = await RegistrationAvailable(year.Id);
                if (availableLevels == 0)
                    return View("NotFound");

                parent = (await _parentRepository.GetAsync(p => p.UserId == _loggedUser.Id))
                    .First();
                viewModel.ParentId = parent.Id;

                switch (availableLevels)
                {
                    case 3:
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(3, "√Ê· (À·«À ”‰Ê« )"));
                        break;
                    case 4:
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(4, "À«‰Ì (√—»⁄ ”‰Ê« )"));
                        break;
                    case 5:
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(5, "À«·À (Œ„” ”‰Ê« )"));
                        break;
                    case 7:
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(3, "√Ê· (À·«À ”‰Ê« )"));
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(4, "À«‰Ì (√—»⁄ ”‰Ê« )"));
                        break;
                    case 8:
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(3, "√Ê· (À·«À ”‰Ê« )"));
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(5, "À«·À (Œ„” ”‰Ê« )"));
                        break;
                    case 9:
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(4, "À«‰Ì (√—»⁄ ”‰Ê« )"));
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(5, "À«·À (Œ„” ”‰Ê« )"));
                        break;
                    case 12:
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(3, "√Ê· (À·«À ”‰Ê« )"));
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(4, "À«‰Ì (√—»⁄ ”‰Ê« )"));
                        viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(5, "À«·À (Œ„” ”‰Ê« )"));
                        break;
                }

                return View("StudentForm", viewModel);
            }

            if (neededParentId == null)
                return View("NotFound");

            parent = await _parentRepository.GetByIdAsync((int)neededParentId);
            if (parent == null)
                return View("NotFound");

            viewModel.ParentId = parent.Id;
            viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(3, "√Ê· (À·«À ”‰Ê« )"));
            viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(4, "À«‰Ì (√—»⁄ ”‰Ê« )"));
            viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(5, "À«·À (Œ„” ”‰Ê« )"));
            return View("StudentForm", viewModel);
        }

        [HttpGet("studentInfo/edit/{id}")]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<IActionResult> EditStudentProfile(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return View("NotFound");

            StudentFormViewModel viewModel;

            if (_userRoles.Contains("Admin"))
            {
                viewModel = _mapper.Map<StudentFormViewModel>(student);

                viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(3, "√Ê· (À·«À ”‰Ê« )"));
                viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(4, "À«‰Ì (√—»⁄ ”‰Ê« )"));
                viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(5, "À«·À (Œ„” ”‰Ê« )"));

                return View("StudentForm", viewModel);
            }

            var year = await ThisAcademicYear();
            if (year == null)
                return View("NotFound");

            var availableLevels = await RegistrationAvailable(year.Id);
            if (availableLevels == 0)
                return View("NotFound");

            var parent = (await _parentRepository.GetAsync(p => p.UserId == _loggedUser.Id))
                .First();
            if (student.ParentId != parent.Id)
                return View("NotFound");

            if (student.IsValid)
                return View("Error",
                    new ErrorViewModel(" „  √ﬂÌœ «·»Ì«‰«  Ê·„ Ì⁄œ »«·≈„ﬂ«‰ «· ⁄œÌ· ⁄·ÌÂ« ÌœÊÌ«" +
                                       "° Ì—ÃÏ „—«”·… „œÌ— «·—Ê÷… ·√Ã· –·ﬂ."));

            viewModel = _mapper.Map<StudentFormViewModel>(student);
            switch (availableLevels)
            {
                case 3:
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(3, "√Ê· (À·«À ”‰Ê« )"));
                    break;
                case 4:
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(4, "À«‰Ì (√—»⁄ ”‰Ê« )"));
                    break;
                case 5:
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(5, "À«·À (Œ„” ”‰Ê« )"));
                    break;
                case 7:
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(3, "√Ê· (À·«À ”‰Ê« )"));
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(4, "À«‰Ì (√—»⁄ ”‰Ê« )"));
                    break;
                case 8:
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(3, "√Ê· (À·«À ”‰Ê« )"));
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(5, "À«·À (Œ„” ”‰Ê« )"));
                    break;
                case 9:
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(4, "À«‰Ì (√—»⁄ ”‰Ê« )"));
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(5, "À«·À (Œ„” ”‰Ê« )"));
                    break;
                case 12:
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(3, "√Ê· (À·«À ”‰Ê« )"));
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(4, "À«‰Ì (√—»⁄ ”‰Ê« )"));
                    viewModel.AvailableLevels.Add(new KeyValuePair<int, string>(5, "À«·À (Œ„” ”‰Ê« )"));
                    break;
            }

            return View("StudentForm", viewModel);
        }

        [HttpPost("studentInfo/save")]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<IActionResult> SaveStudentProfile(StudentFormViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new StudentFormViewModel();
                ModelState.AddModelError("FirstName", "ÌÊÃœ Œÿ√ »«·„œŒ·« ");
                return View("StudentForm", viewModel);
            }

            if (!ModelState.IsValid)
                return View("StudentForm", viewModel);

            Parent parent;
            Student student, studentToUpdate;
            Year year;
            string previousImageName;

            var existStudent = await _studentRepository
                    .GetAsync(s => s.FirstName == viewModel.FirstName && s.Id != viewModel.Id);
            if (existStudent != null)
            {
                ModelState
                    .AddModelError("FirstName", "·ﬁœ  „  ”ÃÌ· ÿ«·» »Â–« «·«”„ „”»ﬁ« ·‰›” √Ê·Ì«¡ «·√„—");
                return View("StudentForm", viewModel);
            }

            if (_userRoles.Contains("Admin"))
            {
                parent = await _parentRepository.GetByIdAsync(viewModel.ParentId);
                if (parent == null)
                    return View("NotFound");

                if (viewModel.Id == 0)
                {
                    year = await ThisAcademicYear();
                    if (year == null)
                        return View("NotFound");

                    student = _mapper.Map<Student>(viewModel);

                    student.ImageName = await _imagesRepository
                        .Save(viewModel.Image, StudentsImagesFolderName);
                    student.IsValid = true;

                    await _studentRepository.AddAsync(student);
                    await _yearStudentRepository.AddAsync(new YearStudent { StudentId = student.Id, YearId = year.Id });
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return RedirectToAction("NewStudentProfile");

                    _imagesRepository.Delete(student.ImageName, StudentsImagesFolderName);

                    ModelState.AddModelError("FirstName", "ÌÊÃœ Œÿ√ »«·„Œœ„° Ì—ÃÏ «·„Õ«Ê·… ·«Õﬁ«");
                    return View("StudentForm", viewModel);
                }

                studentToUpdate = await _studentRepository.GetByIdAsync(viewModel.Id);
                if (studentToUpdate == null)
                    return View("NotFound");

                previousImageName = studentToUpdate.ImageName;

                _mapper.Map(viewModel, studentToUpdate);

                if (viewModel.Image != null && viewModel.Image.Length != 0)
                    studentToUpdate.ImageName = await _imagesRepository
                        .Save(viewModel.Image, StudentsImagesFolderName);
                studentToUpdate.IsValid = true;

                if (await _unitOfWork.SaveChangesAsync() <= 0)
                {
                    _imagesRepository.Delete(studentToUpdate.ImageName, StudentsImagesFolderName);

                    ModelState.AddModelError("FirstName", "ÌÊÃœ Œÿ√ »«·„Œœ„° Ì—ÃÏ «·„Õ«Ê·… ·«Õﬁ«");
                    return View("StudentForm", viewModel);
                }

                if (viewModel.Image != null && viewModel.Image.Length != 0)
                    _imagesRepository.Delete(previousImageName, StudentsImagesFolderName);

                return RedirectToAction("NewStudentProfile");
            }

            parent = (await _parentRepository.GetAsync(p => p.UserId == _loggedUser.Id)).First();
            if (viewModel.ParentId != parent.Id)
                return View("NotFound");

            year = await ThisAcademicYear();
            if (year == null)
                return View("NotFound");

            if (await RegistrationAvailable(year.Id) == 0)
                return View("NotFound");

            if (viewModel.Id == 0)
            {
                student = _mapper.Map<Student>(viewModel);

                student.ImageName = await _imagesRepository
                    .Save(viewModel.Image, StudentsImagesFolderName);
                await _studentRepository.AddAsync(student);
                await _yearStudentRepository.AddAsync(new YearStudent { StudentId = student.Id, YearId = year.Id });
                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return RedirectToAction("NewParentsProfile");

                _imagesRepository.Delete(student.ImageName, StudentsImagesFolderName);
                ModelState.AddModelError("FirstName", "ÌÊÃœ Œÿ√ »«·„Œœ„° Ì—ÃÏ «·„Õ«Ê·… ·«Õﬁ«");
                return View("StudentForm", viewModel);
            }

            studentToUpdate = await _studentRepository.GetByIdAsync(viewModel.Id);
            if (studentToUpdate == null)
                return View("NotFound");

            if (studentToUpdate.IsValid)
                return View("Error",
                    new ErrorViewModel(" „  √ﬂÌœ «·»Ì«‰«  Ê·„ Ì⁄œ »«·≈„ﬂ«‰ «· ⁄œÌ· ⁄·ÌÂ« ÌœÊÌ«" +
                                       "° Ì—ÃÏ „—«”·… „œÌ— «·—Ê÷… ·√Ã· –·ﬂ."));

            _mapper.Map(viewModel, studentToUpdate);
            previousImageName = studentToUpdate.ImageName;
            if (viewModel.Image != null && viewModel.Image.Length != 0)
                studentToUpdate.ImageName = await _imagesRepository
                    .Save(viewModel.Image, StudentsImagesFolderName);
            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                _imagesRepository.Delete(previousImageName, StudentsImagesFolderName);
                return RedirectToAction("NewParentsProfile");
            }

            _imagesRepository.Delete(studentToUpdate.ImageName, StudentsImagesFolderName);
            ModelState.AddModelError("FirstName", "ÌÊÃœ Œÿ√ »«·„Œœ„° Ì—ÃÏ «·„Õ«Ê·… ·«Õﬁ«");
            return View("StudentForm", viewModel);
        }

        private async Task<int> RegistrationAvailable(int yearId)
        {
            var count = (await _studentRegistrationInfoRepository
                .GetAsync(info => info.FromDate <= DateTime.Now
                                  && info.ToDate > DateTime.Now
                                  && info.YearId == yearId))
                .Aggregate(
                new StudentRegistrationInfo(), (info, registrationInfo) =>
                {
                    info.LevelOne += registrationInfo.LevelOne;
                    info.LevelTwo += registrationInfo.LevelTwo;
                    info.LevelThree += registrationInfo.LevelThree;
                    return info;
                });

            if (count.LevelOne == 0 && count.LevelTwo == 0 && count.LevelThree == 0)
                return 0;

            var enrollment = (await _yearStudentRepository
                    .GetAsync(ys => ys.YearId == yearId && ys.FromDate != null))
                .Count();
            if (enrollment >= (count.LevelOne + count.LevelTwo + count.LevelThree))
                return 0;

            int result = 0;
            if (count.LevelOne > 0)
                result += 3;

            if (count.LevelTwo > 0)
                result += 4;

            if (count.LevelThree > 0)
                result += 5;

            return result;
        }

        private async Task<Year> ThisAcademicYear()
        {
            return (await _yearRepository
                    .GetAsync(y => y.FromDate < DateTime.Now && y.ToDate > DateTime.Now))
                .FirstOrDefault();
        }
    }
}