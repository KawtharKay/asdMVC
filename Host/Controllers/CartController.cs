using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Helpers;
using static Application.Commands.AddToCart;
using static Application.Commands.RemoveFromCart;
using static Application.Commands.GetCart;
using static Application.Commands.UpdateCartItem;

namespace Host.Controllers
{
    [Authorize]
    public class CartController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var customerId = ClaimsHelper.GetCustomerId(User);
            var result = await mediator.Send(new GetCartQuery(customerId));

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                return View(null);
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity = 1)
        {
            var customerId = ClaimsHelper.GetCustomerId(User);
            var result = await mediator.Send(
                new AddToCartCommand(customerId, productId, quantity));

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateItem(Guid cartItemId, int quantity)
        {
            var result = await mediator.Send(
                new UpdateCartItemCommand(cartItemId, quantity));

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(Guid cartItemId)
        {
            var result = await mediator.Send(
                new RemoveFromCartCommand(cartItemId));

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}