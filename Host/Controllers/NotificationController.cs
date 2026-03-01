using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Helpers;
using static Application.Commands.MarkNotificationAsRead;
using static Application.Queries.GetMyNotifications;

namespace Host.Controllers
{
    [Authorize]
    public class NotificationController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var result = await mediator.Send(
                new GetMyNotificationsQuery(userId));
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Unread()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var result = await mediator.Send(
                new GetMyNotificationsQuery(userId, UnreadOnly: true));
            return Json(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            var userId = ClaimsHelper.GetUserId(User);
            var result = await mediator.Send(
                new MarkNotificationAsReadCommand(notificationId, userId));
            return Json(new { success = result.Status });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var result = await mediator.Send(
                new MarkNotificationAsReadCommand(
                    Guid.Empty, userId, MarkAll: true));
            return Json(new { success = result.Status });
        }
    }
}