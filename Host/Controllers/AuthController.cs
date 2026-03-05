using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Application.Commands.ForgotPassword;
using static Application.Commands.LoginUser;
using static Application.Commands.RegisterCustomer;
using static Application.Commands.ResendVerification;
using static Application.Commands.ResetPassword;
using static Application.Commands.VerifyEmail;

namespace Host.Controllers
{
    public class AuthController(IMediator mediator) : Controller
    {
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterCustomerCommand command)
        {
            if (!ModelState.IsValid) return View(command);

            var result = await mediator.Send(command);

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                return View(command);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(
                nameof(VerifyEmail), new { email = result.Data!.Email });
        }

        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            var result = await mediator.Send(
                new VerifyEmailCommand(email, token));

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                ViewBag.Email = email;
                return View();
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendVerification(string email)
        {
            var result = await mediator.Send(
                new ResendVerificationCommand(email));

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            ViewBag.Email = email;
            return View(nameof(VerifyEmail));
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            if (!ModelState.IsValid) return View(command);

            var result = await mediator.Send(command);

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                return View(command);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, result.Data!.UserId.ToString()),
                new("CustomerId", result.Data.CustomerId.ToString()),
                new(ClaimTypes.Name, result.Data.FullName),
                new(ClaimTypes.Email, result.Data.Email),
                new(ClaimTypes.Role, result.Data.Role)
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = command.RememberMe,
                    ExpiresUtc = command.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(7)
                        : DateTimeOffset.UtcNow.AddHours(1)
                });

            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }

        
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordCommand command)
        {
            if (!ModelState.IsValid) return View(command);

            var result = await mediator.Send(command);

            TempData["Success"] = result.Message;
            return RedirectToAction(
                nameof(ResetPassword), new { email = command.Email });
        }

        
        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordCommand command)
        {
            if (!ModelState.IsValid) return View(command);

            var result = await mediator.Send(command);

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                ViewBag.Email = command.Email;
                return View(command);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Login));
        }

        // ACCESS DENIED

        public IActionResult AccessDenied() => View();

        // LOGOUT

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}