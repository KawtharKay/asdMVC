using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Helpers;
using static Application.Commands.AddBankAccount;
using static Application.Commands.DeleteBankAccount;
using static Application.Commands.SetDefaultBankAccount;
using static Application.Queries.GetMyBankAccounts;

namespace Host.Controllers
{
    [Authorize]
    public class BankAccountController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var customerId = ClaimsHelper.GetCustomerId(User);
            var result = await mediator.Send(
                new GetMyBankAccountsQuery(customerId));
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddBankAccountCommand command)
        {
            var customerId = ClaimsHelper.GetCustomerId(User);
            var result = await mediator.Send(
                command with { CustomerId = customerId });

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid bankAccountId)
        {
            var customerId = ClaimsHelper.GetCustomerId(User);
            var result = await mediator.Send(
                new DeleteBankAccountCommand(bankAccountId, customerId));

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(Guid bankAccountId)
        {
            var customerId = ClaimsHelper.GetCustomerId(User);
            var result = await mediator.Send(
                new SetDefaultBankAccountCommand(bankAccountId, customerId));

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}