using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Application.Queries.GetAllCategories;
using static Application.Queries.GetAllProducts;

namespace Host.Controllers
{
    public class HomeController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index(Guid? categoryId = null)
        {
            var products = await mediator.Send(
                new GetAllProductsQuery(categoryId));

            var categories = await mediator.Send(
                new GetAllCategoriesQuery());

            ViewBag.Categories = categories.Data;
            ViewBag.SelectedCategory = categoryId;

            return View(products.Data);
        }
    }
}