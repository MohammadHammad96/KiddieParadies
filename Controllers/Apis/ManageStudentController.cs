using System.Linq;
using System.Threading.Tasks;
using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KiddieParadies.Controllers.Apis
{
    [Route("api/student")]
    public class ManageStudentController : Controller
    {
        private const string ParentsImagesFolderName = "parentsIdentity";
        private const string StudentsImagesFolderName = "students";
        private readonly IRepository<Parent> _parentRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImagesRepository _imagesRepository;
        private readonly IFilesRepository _filesRepository;

        public ManageStudentController(IUnitOfWork unitOfWork, IImagesRepository imagesRepository,
            IFilesRepository filesRepository, UserManager<ApplicationUser> userManager,
            IRepository<Parent> parentRepository, IRepository<Student> studentRepository)
        {
            _unitOfWork = unitOfWork;
            _imagesRepository = imagesRepository;
            _filesRepository = filesRepository;
            _userManager = userManager;
            _parentRepository = parentRepository;
            _studentRepository = studentRepository;
        }

        [HttpPut("validateStudentProfile/{id}")]
        public async Task<IActionResult> ValidateStudentProfile(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            student.IsValid = true;
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPut("validateParentProfile/{id}")]
        public async Task<IActionResult> ValidateParentProfile(int id)
        {
            var parent = await _parentRepository.GetByIdAsync(id);
            if (parent == null)
                return NotFound();

            parent.IsValid = true;
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("deleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudentProfile(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            _imagesRepository.Delete(student.ImageName, StudentsImagesFolderName);
            _studentRepository.Delete(student);
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("deleteParent/{id}")]
        public async Task<IActionResult> DeleteParentProfile(int id)
        {
            var parent = await _parentRepository.GetByIdAsync(id, p => p.Children);
            if (parent == null)
                return NotFound();

            foreach (var child in parent.Children)
            {
                _imagesRepository.Delete(child.ImageName, StudentsImagesFolderName);
            }

            _imagesRepository.Delete(parent.FatherIdentityImageName, ParentsImagesFolderName);
            _imagesRepository.Delete(parent.MotherIdentityImageName, ParentsImagesFolderName);
            _parentRepository.Delete(parent);

            var user = await _userManager.FindByIdAsync(parent.UserId.ToString());
            var parentRoles = await _userManager.GetRolesAsync(user);
            if (parentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, parentRoles);

            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}