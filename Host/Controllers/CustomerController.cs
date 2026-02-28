using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.RegisterCustomer;

namespace Host.Controllers
{
    public class CustomerController(IMediator mediator) : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterCustomerCommand command)
        {
            if (!ModelState.IsValid) return View(command);

            var result = await mediator.Send(command);

            if (!result.Status)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(command);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("VerifyEmail", new { email = result.Data!.Email });
        }

        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            ViewBag.Email = email;
            return View();
        }
    }
}
