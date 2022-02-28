using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucky_Draw_Promotion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Lucky_Draw_Promotion.Pages
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ReCaptcha _captcha;
        public ResetPasswordModel(UserManager<IdentityUser> userManager, ReCaptcha captcha)
        {
            _userManager = userManager;
            _captcha = captcha;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; } = "124321423";
        }

        //public IActionResult OnGet(string code = null)
        //{
        //    if (code == null)
        //    {
        //        return BadRequest("A code must be supplied for password reset.");
        //    }
        //    else
        //    {
        //        Input = new InputModel
        //        {
        //            Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
        //        };
        //        return Page();
        //    }
        //}

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (!Request.Form.ContainsKey("g-recaptcha-response"))
                return Page();
            var captcha = Request.Form["g-recaptcha-response"].ToString();
            if (await _captcha.IsValid(captcha))
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
                if (result.Succeeded)
                {
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
    }
}
