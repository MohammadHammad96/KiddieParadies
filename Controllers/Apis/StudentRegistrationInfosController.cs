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
    [Route("api/studentRegistrationInfos")]
    public class StudentRegistrationInfosController : Controller
    {
        private readonly IRepository<StudentRegistrationInfo> _studentRegistrationInfoRepository;
        private readonly IRepository<Year> _yearRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StudentRegistrationInfosController(
            IRepository<StudentRegistrationInfo> studentRegistrationInfoRepository,
            IMapper mapper, IUnitOfWork unitOfWork, IRepository<Year> yearRepository)
        {
            _studentRegistrationInfoRepository = studentRegistrationInfoRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _yearRepository = yearRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var studentRegistrationInfos = await _studentRegistrationInfoRepository
                .GetAsync(null, null, s => s.Year);

            var studentRegistrationInfosDto = _mapper.Map<IEnumerable<StudentRegistrationInfoDto>>(
                studentRegistrationInfos);

            return Ok(studentRegistrationInfosDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] StudentRegistrationInfoDto dto)
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
                ModelState.AddModelError(dto.GetPropertyDisplayName(s => s.ToDate), "لا يمكن إنهاء التسجيل قبل أن يبدأ");
                return BadRequest(ModelState);
            }

            var year = await _yearRepository.GetByIdAsync(dto.YearId);
            if (year == null)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(s => s.YearName), "العام الدراسي غير صحيح، يرجى عدم الكتابة بعد اختيار العام من القائمة");
                return BadRequest(ModelState);
            }
            if (year.Name != dto.YearName)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(s => s.YearName), "العام الدراسي غير صحيح، يرجى عدم الكتابة بعد اختيار العام من القائمة");
                return BadRequest(ModelState);
            }

            if ((dto.FromDate < year.FromDate))
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(s => s.FromDate), "لا يمكن بدء التسجيل قبل بداية العام الدراسي");
                return BadRequest(ModelState);
            }
            if ((dto.ToDate > year.ToDate))
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(s => s.ToDate), "لا يمكن إنهاء التسجيل بعد نهاية العام الدراسي");
                return BadRequest(ModelState);
            }

            var studentRegistrationInfo = _mapper.Map<StudentRegistrationInfo>(dto);
            await _studentRegistrationInfoRepository.AddAsync(studentRegistrationInfo);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة مرة أخرى");

            _mapper.Map(studentRegistrationInfo, dto);
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var studentRegistrationInfo = await _studentRegistrationInfoRepository.GetByIdAsync(id);
            if (studentRegistrationInfo == null)
                return NotFound("الموعد غير موجود.");

            _studentRegistrationInfoRepository.Delete(studentRegistrationInfo);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة مرة أخرى");

            return StatusCode(StatusCodes.Status202Accepted, "تم حذف الموعد بنجاح.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] StudentRegistrationInfoDto dto)
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
                ModelState.AddModelError(dto.GetPropertyDisplayName(s => s.ToDate), "لا يمكن إنهاء التسجيل قبل أن يبدأ");
                return BadRequest(ModelState);
            }

            var year = await _yearRepository.GetByIdAsync(dto.YearId);
            if (year == null)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(s => s.YearName), "العام الدراسي غير صحيح، يرجى عدم الكتابة بعد اختيار العام من القائمة");
                return BadRequest(ModelState);
            }
            if (year.Name != dto.YearName)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(s => s.YearName), "العام الدراسي غير صحيح، يرجى عدم الكتابة بعد اختيار العام من القائمة");
                return BadRequest(ModelState);
            }

            if ((dto.FromDate < year.FromDate))
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(s => s.FromDate), "لا يمكن بدء التسجيل قبل بداية العام الدراسي");
                return BadRequest(ModelState);
            }
            if ((dto.ToDate > year.ToDate))
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(s => s.ToDate), "لا يمكن إنهاء التسجيل بعد نهاية العام الدراسي");
                return BadRequest(ModelState);
            }

            var studentRegistrationInfoToUpdate = await _studentRegistrationInfoRepository.GetByIdAsync(dto.Id);
            if (studentRegistrationInfoToUpdate == null)
                return NotFound("الموعد غير موجود.");

            _mapper.Map(dto, studentRegistrationInfoToUpdate);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة مرة أخرى");

            _mapper.Map(studentRegistrationInfoToUpdate, dto);
            return Ok(dto);
        }
    }
}