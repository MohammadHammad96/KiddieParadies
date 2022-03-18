using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers.Apis
{
    [Route("api/employee")]
    public class ManageEmployeeController : Controller
    {
        private const string EmployeesImagesFolderName = "employees";
        private const string EmployeesMainFolderName = "employees";
        private const string EmployeesResumesFolderName = "resumes";
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImagesRepository _imagesRepository;
        private readonly IFilesRepository _filesRepository;

        public ManageEmployeeController(IRepository<Employee> employeeRepository, IUnitOfWork unitOfWork, IImagesRepository imagesRepository, IFilesRepository filesRepository, UserManager<ApplicationUser> userManager)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _imagesRepository = imagesRepository;
            _filesRepository = filesRepository;
            _userManager = userManager;
        }

        [HttpPut("validateProfile/{id}")]
        public async Task<IActionResult> ValidateProfile(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            employee.IsValid = true;
            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            _imagesRepository.Delete(employee.ImageName, EmployeesImagesFolderName);
            _filesRepository.Delete(employee.ResumeName, EmployeesMainFolderName,
                EmployeesResumesFolderName);
            _employeeRepository.Delete(employee);

            var user = await _userManager.FindByIdAsync(employee.UserId.ToString());
            var employeeRoles = await _userManager.GetRolesAsync(user);
            if (employeeRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, employeeRoles);

            if (await _unitOfWork.SaveChangesAsync() > 0)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
