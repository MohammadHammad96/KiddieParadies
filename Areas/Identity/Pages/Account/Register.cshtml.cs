using KiddieParadies.Core.Models;
using KiddieParadies.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace KiddieParadies.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailService _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailService emailSender, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "هذا الحقل إجباري")]
            [EmailAddress(ErrorMessage = "ليس عنوان بريد إلكتروني صالح")]
            [Display(Name = "البريد الالكتروني")]
            public string Email { get; set; }

            [Required(ErrorMessage = "هذا الحقل إجباري")]
            [StringLength(100, ErrorMessage = "{0} يجب أن تكون على الأقل {2} وعلى الأكثر {1} محرف.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "كلمة السر")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تأكيد كلمة السر")]
            [Compare("Password", ErrorMessage = "الكلمة ليست نفس الكلمة في الحقل السابق.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                /*var keys = new List<KeyStore>();
                var model = new InputModel();
                foreach (var key in ModelState.Keys)
                {
                    var arabicKey = model.GetType().GetProperties().SingleOrDefault(p => p.Name == key.Split(".")[1])
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
                }*/

                // If we got this far, something failed, redisplay form
                return Page();
            }
            var isEmailExist = await _userManager.FindByEmailAsync(Input.Email);
            if (isEmailExist != null)
            {
                ModelState.AddModelError("البريد الالكتروني", "العنوان مستخدم مسبقاً");
                return Page();
            }

            var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                if (await _unitOfWork.SaveChangesAsync() <= 0)
                {
                    ModelState.AddModelError(string.Empty, "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً");
                    return Page();
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "تأكيد الحساب",
                    $"يرجى تأكيد حسابك <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>اضغط هنا</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }
            // var keys = new List<KeyStore>();
            // var model = new ApplicationUser();
            // foreach (var key in result.Errors)
            // {
            //     var propertyName = key.Description.Split(" ")[0];
            //     //propertyName = propertyName.Substring(1, propertyName.Length - 1);
            //     var x = model.GetType().GetProperties().Single(p => p.Name == "UserName")
            //         .CustomAttributes;
            //     var properties = model.GetType().GetProperties().Single(p => p.Name == "UserName")
            //         .CustomAttributes.SingleOrDefault(ca => ca.GetType() == typeof(DisplayNameAttribute));
            //     var arabicKey = model.GetType().GetProperties().SingleOrDefault(p => p.Name == propertyName)
            //         .CustomAttributes.Single(ca => ca.GetType() == typeof(DisplayNameAttribute))
            //         .NamedArguments[0].TypedValue.Value.ToString();

            //     keys.Add(new KeyStore(key.Description, arabicKey));
            // }

            // for (int i = 0; i < keys.Count(); i++)
            // {
            //     var dictionaryKeyErrors = new List<ModelError>(ModelState[keys[i].EnglishKey].Errors.ToList());
            //     ModelState.Remove(keys[i].EnglishKey);

            //     foreach (var error in dictionaryKeyErrors)
            //     {
            //         ModelState.AddModelError(keys[i].ArabicKey, error.ErrorMessage);
            //     }
            // }
            foreach (var error in result.Errors)
            {


                ModelState.AddModelError(error.Code, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
