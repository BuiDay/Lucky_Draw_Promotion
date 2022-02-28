using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lucky_Draw_Promotion.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lucky_Draw_Promotion.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ReCaptcha _captcha;
        private readonly SignInManager<IdentityUser> signInManager;

        [BindProperty]

        public Login Model { get; set; }

        public LoginModel(SignInManager<IdentityUser> signInManager, ReCaptcha captcha)
        {
            this.signInManager = signInManager;
            _captcha = captcha;
        }



        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if (!Request.Form.ContainsKey("g-recaptcha-response")) 
                    return Page();
                var captcha = Request.Form["g-recaptcha-response"].ToString();
                if (await _captcha.IsValid(captcha))
                {
                    
                    var identityResult = await signInManager.PasswordSignInAsync(Model.Email, Model.Password, Model.RememberMe, false);

                    if (identityResult.Succeeded)
                    {
                        if (returnUrl == null || returnUrl == "/")
                        {
                            return RedirectToPage("Home");
                        }
                        else
                        {
                            return RedirectToPage(returnUrl);
                        }
                    }

                    ModelState.AddModelError("", "User or pass incorrect");
                }else return Page();
            }
            return Page();
        }
    }
}

