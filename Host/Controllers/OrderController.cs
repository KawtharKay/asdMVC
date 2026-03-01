using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Helpers;
using static Application.Commands.CancelOrder;
using static Application.Commands.PlaceOrder;
using static Application.Queries.GetMyOrders;
using static Application.Queries.GetOrderById;

namespace Host.Controllers
{
    [Authorize]
    public class OrderController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var customerId = ClaimsHelper.GetCustomerId(User);
            var result = await mediator.Send(new GetMyOrdersQuery(customerId));
            return View(result.Data);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var result = await mediator.Send(new GetOrderByIdQuery(id));

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string deliveryAddress)
        {
            var customerId = ClaimsHelper.GetCustomerId(User);
            var result = await mediator.Send(
                new PlaceOrderCommand(customerId, deliveryAddress));

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index", "Cart");
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(
                nameof(Details), new { id = result.Data!.OrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid orderId)
        {
            var customerId = ClaimsHelper.GetCustomerId(User);
            var result = await mediator.Send(
                new CancelOrderCommand(orderId, customerId));

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}