using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcAuthDemo.Models;

namespace MvcAuthDemo.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Account/Login
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = InMemoryUserStore.Users.FirstOrDefault(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            //check if locked
            if (user.IsLocked)
            {
                ModelState.AddModelError("", "Your account has been locked due to 3 consecutive failed login attempts. Please contact support or reset your password.");
                return View(model);
            }

           
            if (user.Password != model.Password)
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 3)
                {
                    user.IsLocked = true;
                    ModelState.AddModelError("", "Your account has been locked due to 3 consecutive failed login attempts.");
                }
                else
                {
                    int remaining = 3 - user.FailedLoginAttempts;
                    ModelState.AddModelError("", $"Invalid email or password. You have {remaining} attempt(s) remaining before account lockout.");
                }

                return View(model);
            }

            // Login then reset failed attempts count
            user.FailedLoginAttempts = 0;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Portfolio");
        }

        // POST: /Account/Logout
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        // POST: /Account/ForgotPassword
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = InMemoryUserStore.Users.FirstOrDefault(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));

            if (user != null)
            {
                string tempPassword = "Temp" + Random.Shared.Next(1000, 9999) + "!";
                user.Password = tempPassword;
                user.FailedLoginAttempts = 0; 
                user.IsLocked = false;

                TempData["Notice"] = $"Password reset successfully. Your temporary password is: {tempPassword}";
            }
            else
            {
                TempData["Notice"] = "If an account exists with that email, a password reset has been generated.";
            }

            return RedirectToAction("ForgotPassword");
        }

        // GET: /Account/ChangePassword
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword() => View();

        // POST: /Account/ChangePassword
        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string? userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = InMemoryUserStore.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null || user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(model);
            }

            user.Password = model.NewPassword;
            ViewBag.Message = "Password successfully changed!";
            return View();
        }
    }
}