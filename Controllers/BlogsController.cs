using AutoMapper;
using KiddieParadies.Core.Helpers;
using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using KiddieParadies.Extensions;
using KiddieParadies.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KiddieParadies.Controllers
{
    [Route("news")]
    public class BlogsController : Controller
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Year> _yearRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _host;

        public BlogsController(IRepository<Blog> blogRepository, IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment host, IRepository<Year> yearRepository)
        {
            _blogRepository = blogRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _host = host;
            _yearRepository = yearRepository;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var blogs = await _blogRepository.GetAsync(null, q => q.OrderByDescending(y => y.Time));
            return View(blogs);
        }

        [HttpGet("add")]
        public IActionResult New()
        {
            ViewData["Title"] = "خبر جديد";
            return View("BlogForm", new BlogFormViewModel());
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveAsync(BlogFormViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new BlogFormViewModel();
                ModelState.AddModelError(viewModel.GetPropertyDisplayName(v => v.Title), "يوجد خطأ بالمدخلات");
                return View("BlogForm", viewModel);
            }

            if (!ModelState.IsValid)
            {
                var keys = new List<KeyStore>();
                foreach (var key in ModelState.Keys)
                {
                    var arabicKey = viewModel.GetType().GetProperties().Single(p => p.Name == key)
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

                return View("BlogForm", viewModel);
            }

            Blog blog;
            if (viewModel.Id == 0)
            {
                blog = _mapper.Map<Blog>(viewModel);
                if (Request.GetDisplayUrl().ToLower().Contains("somee"))
                    blog.Time = DateTime.Now.AddHours(8);
                else
                    blog.Time = DateTime.Now;

                var year = (await _yearRepository.GetAsync(y => y.FromDate < blog.Time && y.ToDate > blog.Time)).FirstOrDefault();
                if (year != null)
                    blog.YearId = year.Id;
            }
            else
            {
                blog = await _blogRepository.GetByIdAsync(viewModel.Id);
                if (blog == null)
                {
                    ModelState.AddModelError(viewModel.GetPropertyDisplayName(v => v.Title), "أنت تحاول التعديل على خبر غير موجود");
                    return View("BlogForm", viewModel);
                }
            }

            string previousImageName = blog.MainImageName;
            if (viewModel.Image != null && viewModel.Image.Length != 0)
            {
                string[] acceptedFileTypes = new string[] { ".jpg", ".jpeg", ".png" };
                var ex = Path.GetExtension(viewModel.Image.FileName).ToLower();
                if (acceptedFileTypes.All(s => s != ex))
                {
                    ModelState.AddModelError(viewModel.GetPropertyDisplayName(v => v.Image), "لاحقة الصورة يجب أن تكون jpg أو jpeg أو png");
                    return View("BlogForm", viewModel);
                }

                try
                {
                    using (var image = Image.FromStream(viewModel.Image.OpenReadStream()))
                    {
                        if (image.Width != image.Height)
                        {
                            ModelState.AddModelError(viewModel.GetPropertyDisplayName(v => v.Image), "يجب أن تكون الأبعاد متساوية (صورة مربعة)");
                            return View("BlogForm", viewModel);
                        }
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError(viewModel.GetPropertyDisplayName(v => v.Image), "الملف المحمل يجب أن يكون صورة");
                    return View("BlogForm", viewModel);
                }
                var newsImagesFolderPath = Path.Combine(_host.ContentRootPath + "\\wwwroot", "images", "news");
                if (!Directory.Exists(newsImagesFolderPath))
                    Directory.CreateDirectory(newsImagesFolderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.Image.FileName);
                var filePath = Path.Combine(newsImagesFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.Image.CopyToAsync(stream);
                }
                blog.MainImageName = fileName;
            }

            if (viewModel.Id == 0)
            {
                await _blogRepository.AddAsync(blog);
                if (await _unitOfWork.SaveChangesAsync() <= 0)
                {
                    previousImageName = Path.Combine(_host.ContentRootPath + "\\wwwroot", "images", "news", blog.MainImageName);
                    System.IO.File.Delete(previousImageName);
                    ModelState.AddModelError(viewModel.GetPropertyDisplayName(v => v.Title), "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً");
                    return View("BlogForm", viewModel);
                }
                return RedirectToAction("New");
            }

            _mapper.Map(viewModel, blog);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
            {
                if (!string.IsNullOrWhiteSpace(blog.MainImageName))
                {
                    previousImageName = Path.Combine(_host.ContentRootPath + "\\wwwroot", "images", "news",
                        blog.MainImageName);
                    System.IO.File.Delete(previousImageName);
                }
                ModelState.AddModelError(viewModel.GetPropertyDisplayName(v => v.Title), "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً");
                return View("BlogForm", viewModel);
            }

            if (!string.IsNullOrWhiteSpace(previousImageName))
            {
                previousImageName = Path.Combine(_host.ContentRootPath + "\\wwwroot", "images", "news",
                    previousImageName);
                System.IO.File.Delete(previousImageName);
            }
            return RedirectToAction("New");
        }

        [HttpGet("read/{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
                return View("NotFound");

            return View("Preview", blog);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> EditAsync(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
                return View("NotFound");

            var viewModel = _mapper.Map<BlogFormViewModel>(blog);
            ViewData["Title"] = "تعديل خبر";
            return View("BlogForm", viewModel);
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
                return View("NotFound");

            var imageName = blog.MainImageName;

            _blogRepository.Delete(blog);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
                return View("InternalServerError");

            if (!string.IsNullOrWhiteSpace(imageName))
            {
                var imagePath = Path.Combine(_host.ContentRootPath + "\\wwwroot", "images", "news",
                    blog.MainImageName);
                System.IO.File.Delete(imagePath);
            }
            return RedirectToAction("List");
        }
    }
}