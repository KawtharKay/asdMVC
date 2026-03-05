using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Application.Queries.GetAllProducts;
using static Application.Queries.GetProductById;

namespace Host.Controllers
{
    public class ProductController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index(Guid? categoryId = null)
        {
            var result = await mediator.Send(
                new GetAllProductsQuery(categoryId));
            return View(result.Data);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var result = await mediator.Send(new GetProductByIdQuery(id));

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }
    }
}