using AutoMapper;
using KiddieParadies.Controllers.Apis.Dtos;
using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.Extensions;
using KiddieParadies.Helpers;
using KiddieParadies.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers.Apis
{
    [Route("api/employeeRegistrationInfos")]
    public class EmployeeRegistrationInfosController : Controller
    {
        private readonly IRepository<EmployeeRegistrationInfo> _employeeRegistrationInfoRepository;
        private readonly IRepository<Year> _yearRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeRegistrationInfosController(
            IRepository<EmployeeRegistrationInfo> employeeRegistrationInfoRepository,
            IMapper mapper, IUnitOfWork unitOfWork, IRepository<Year> yearRepository)
        {
            _employeeRegistrationInfoRepository = employeeRegistrationInfoRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _yearRepository = yearRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var employeeRegistrationInfos = await _employeeRegistrationInfoRepository
                .GetAsync(null, null, e => e.Year);

            var employeeRegistrationInfosDto = _mapper.Map<IEnumerable<EmployeeRegistrationInfoDto>>(
                employeeRegistrationInfos);

            return Ok(employeeRegistrationInfosDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] EmployeeRegistrationInfoDto dto)
        {
            if (dto == null)
                return StatusCode(StatusCodes.Status400BadRequest, "يوجد خطأ بالمدخلات.");

            if (!ModelState.IsValid)
            {
                var keys = new List<KeyStore>();
                foreach (var key in ModelState.Keys)
                {

                    var arabicKey = dto.GetType().GetProperties().Single(p => p.Name == key)
                        .CustomAttributes.Single(ca => ca.AttributeType == typeof(DisplayAttribute))
                        .NamedArguments[0].TypedValue.Value.ToString();

                    keys.Add(new KeyStore(key, arabicKey));
                }

                for (int i = 0; i < keys.Count(); i++)
                {
                    var dictionaryKeyErrors = new List<ModelError>(ModelState[keys[i].EnglishKey].Errors.ToList());
                    ModelState.Remove(keys[i].EnglishKey);

                    foreach (var error in dictionaryKeyErrors)
                    {
                        ModelState.AddModelError(keys[i].ArabicKey, error.ErrorMessage);
                    }
                }

                return BadRequest(ModelState);
            }

            if (dto.FromDate >= dto.ToDate)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(e => e.ToDate), "لا يمكن إنهاء التسجيل قبل أن يبدأ");
                return BadRequest(ModelState);
            }

            var year = await _yearRepository.GetByIdAsync(dto.YearId);
            if (year == null)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(e => e.YearName), "العام الدراسي غير صحيح، يرجى عدم الكتابة بعد اختيار العام من القائمة");
                return BadRequest(ModelState);
            }
            if (year.Name != dto.YearName)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(e => e.YearName), "العام الدراسي غير صحيح، يرجى عدم الكتابة بعد اختيار العام من القائمة");
                return BadRequest(ModelState);
            }

            if ((dto.FromDate < year.FromDate))
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(e => e.FromDate), "لا يمكن بدء التسجيل قبل بداية العام الدراسي");
                return BadRequest(ModelState);
            }
            if ((dto.ToDate > year.ToDate))
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(e => e.ToDate), "لا يمكن إنهاء التسجيل بعد نهاية العام الدراسي");
                return BadRequest(ModelState);
            }

            var employeeRegistrationInfo = _mapper.Map<EmployeeRegistrationInfo>(dto);
            await _employeeRegistrationInfoRepository.AddAsync(employeeRegistrationInfo);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة مرة أخرى");

            _mapper.Map(employeeRegistrationInfo, dto);
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var employeeRegistrationInfo = await _employeeRegistrationInfoRepository.GetByIdAsync(id);
            if (employeeRegistrationInfo == null)
                return NotFound("الموعد غير موجود.");

            _employeeRegistrationInfoRepository.Delete(employeeRegistrationInfo);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة مرة أخرى");

            return StatusCode(StatusCodes.Status202Accepted, "تم حذف الموعد بنجاح.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] EmployeeRegistrationInfoDto dto)
        {
            if (dto == null)
                return StatusCode(StatusCodes.Status400BadRequest, "يوجد خطأ بالمدخلات.");

            if (!ModelState.IsValid)
            {
                var keys = new List<KeyStore>();
                foreach (var key in ModelState.Keys)
                {

                    var arabicKey = dto.GetType().GetProperties().Single(p => p.Name == key)
                        .CustomAttributes.Single(ca => ca.AttributeType == typeof(DisplayAttribute))
                        .NamedArguments[0].TypedValue.Value.ToString();

                    keys.Add(new KeyStore(key, arabicKey));
                }

                for (int i = 0; i < keys.Count(); i++)
                {
                    var dictionaryKeyErrors = new List<ModelError>(ModelState[keys[i].EnglishKey].Errors.ToList());
                    ModelState.Remove(keys[i].EnglishKey);

                    foreach (var error in dictionaryKeyErrors)
                    {
                        ModelState.AddModelError(keys[i].ArabicKey, error.ErrorMessage);
                    }
                }

                return BadRequest(ModelState);
            }

            if (dto.FromDate >= dto.ToDate)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(e => e.ToDate), "لا يمكن إنهاء التسجيل قبل أن يبدأ");
                return BadRequest(ModelState);
            }

            var year = await _yearRepository.GetByIdAsync(dto.YearId);
            if (year == null)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(e => e.YearName), "العام الدراسي غير صحيح، يرجى عدم الكتابة بعد اختيار العام من القائمة");
                return BadRequest(ModelState);
            }
            if (year.Name != dto.YearName)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(e => e.YearName), "العام الدراسي غير صحيح، يرجى عدم الكتابة بعد اختيار العام من القائمة");
                return BadRequest(ModelState);
            }

            if ((dto.FromDate < year.FromDate))
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(e => e.FromDate), "لا يمكن بدء التسجيل قبل بداية العام الدراسي");
                return BadRequest(ModelState);
            }
            if ((dto.ToDate > year.ToDate))
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(e => e.ToDate), "لا يمكن إنهاء التسجيل بعد نهاية العام الدراسي");
                return BadRequest(ModelState);
            }

            var employeeRegistrationInfoToUpdate = await _employeeRegistrationInfoRepository.GetByIdAsync(dto.Id);
            if (employeeRegistrationInfoToUpdate == null)
                return NotFound("الموعد غير موجود.");

            _mapper.Map(dto, employeeRegistrationInfoToUpdate);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة مرة أخرى");

            _mapper.Map(employeeRegistrationInfoToUpdate, dto);
            return Ok(dto);
        }
    }
}