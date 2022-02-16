using KiddieParadies.Core.Models;
using KiddieParadies.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace KiddieParadies.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (await _unitOfWork.SaveChangesAsync() <= 0)
            {
                ModelState.AddModelError(string.Empty, "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً");
                return Page();
            }
            StatusMessage = result.Succeeded ? "شكراً لتأكيد حسابك." : "يوجد خطأ بالمخدم، يرجى المحاولة لاحقاً.";
            ViewData["StatusMessage"] = StatusMessage;
            return Page();
        }
    }
}
