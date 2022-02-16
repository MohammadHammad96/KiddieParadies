using System.Collections;
using System;
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
    [Route("api/years")]
    public class YearsController : Controller
    {
        private readonly IRepository<Year> _yearRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public YearsController(IRepository<Year> yearRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _yearRepository = yearRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string query = null)
        {
            IEnumerable<Year> years = new List<Year>();
            if (string.IsNullOrWhiteSpace(query))
                years = await _yearRepository.GetAsync();
            else
                years = await _yearRepository.GetAsync(y => y.Name.Contains(query), 
                    qy => qy.OrderByDescending(y => y.Name));

            return Ok(years);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] YearDto dto)
        {
            if (dto == null)
                return StatusCode(StatusCodes.Status400BadRequest, "يوجد خطأ بالمدخلات.");

            dto.Name = string.Concat(dto.ToDate.Year, "/", dto.FromDate.Year);
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

            if (dto.FromDate.Year == dto.ToDate.Year)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(y => y.ToDate), "لا يمكن بدء وإنهاء العام الدراسي بنفس العام الميلادي");
                return BadRequest(ModelState);
            }

            if (dto.FromDate >= dto.ToDate)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(y => y.ToDate), "لا يمكن إنهاء العام الدراسي قبل أن يبدأ");
                return BadRequest(ModelState);
            }

            if ((dto.FromDate.Year + 1) != dto.ToDate.Year)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(y => y.ToDate), "لا يمكن إنهاء العام الدراسي بعد أكثر من عام ميلادي");
                return BadRequest(ModelState);
            }

            var year = (await _yearRepository.GetAsync(y => y.Name == dto.Name)).FirstOrDefault();
            if (year != null)
            {
                ModelState.AddModelError(string.Empty, "يوجد عام دراسي آخر بالأعوام الميلادية المدخلة.");
                return BadRequest(ModelState);
            }

            year = _mapper.Map<Year>(dto);
            await _yearRepository.AddAsync(year);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة مرة أخرى");

            return Ok(year);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var year = await _yearRepository.GetByIdAsync(id);
            if (year == null)
                return NotFound("العام الدراسي غير موجود.");

            _yearRepository.Delete(year);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة مرة أخرى");

            return StatusCode(StatusCodes.Status202Accepted, "تم حذف العام الدراسي بنجاح.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] YearDto dto)
        {
            if (dto == null)
                return StatusCode(StatusCodes.Status400BadRequest, "يوجد خطأ بالمدخلات.");
            
            dto.Name = string.Concat(dto.ToDate.Year, "/", dto.FromDate.Year);
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

            if (dto.FromDate.Year == dto.ToDate.Year)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(y => y.ToDate), "لا يمكن بدء وإنهاء العام الدراسي بنفس العام الميلادي");
                return BadRequest(ModelState);
            }

            if (dto.FromDate >= dto.ToDate)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(y => y.ToDate), "لا يمكن إنهاء العام الدراسي قبل أن يبدأ");
                return BadRequest(ModelState);
            }

            if ((dto.FromDate.Year + 1) != dto.ToDate.Year)
            {
                ModelState.AddModelError(dto.GetPropertyDisplayName(y => y.ToDate), "لا يمكن إنهاء العام الدراسي بعد أكثر من عام ميلادي");
                return BadRequest(ModelState);
            }

            var yearToUpdate = await _yearRepository.GetByIdAsync(dto.Id);
            if (yearToUpdate == null)
                return NotFound("العام الدراسي غير موجود.");

            var year = (await _yearRepository.GetAsync(y => y.Name == dto.Name && y.Id != dto.Id)).FirstOrDefault();
            if (year != null)
            {
                ModelState.AddModelError(string.Empty, "يوجد عام دراسي آخر بالأعوام الميلادية المدخلة.");
                return BadRequest(ModelState);
            }

            _mapper.Map(dto, yearToUpdate);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "يوجد خطأ بالمخدم، يرجى المحاولة مرة أخرى");

            return Ok(yearToUpdate);
        }
    }
}