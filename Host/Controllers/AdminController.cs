using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.CreateCategory;
using static Application.Commands.CreateProduct;
using static Application.Commands.DeleteProduct;
using static Application.Commands.UpdateOrderStatus;
using static Application.Commands.UpdateProduct;
using static Application.Queries.GetAllCategories;
using static Application.Queries.GetAllCustomers;
using static Application.Queries.GetAllOrders;
using static Application.Queries.GetAllProducts;
using static Application.Queries.GetCustomerById;
using static Application.Queries.GetDashboardStats;
using static Application.Queries.GetOrderById;
using static Application.Queries.GetProductById;

namespace Host.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IMediator mediator) : Controller
    {
        // DASHBOARD

        public async Task<IActionResult> Index()
        {
            var result = await mediator.Send(new GetDashboardStatsQuery());
            return View(result.Data);
        }

        // CUSTOMERS

        public async Task<IActionResult> Customers()
        {
            var result = await mediator.Send(new GetAllCustomersQuery());
            return View(result.Data);
        }

        public async Task<IActionResult> CustomerDetails(Guid id)
        {
            var result = await mediator.Send(new GetCustomerByIdQuery(id));

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Customers));
            }

            return View(result.Data);
        }

        // ORDERS

        public async Task<IActionResult> Orders()
        {
            var result = await mediator.Send(new GetAllOrdersQuery());
            return View(result.Data);
        }

        public async Task<IActionResult> OrderDetails(Guid id)
        {
            var result = await mediator.Send(new GetOrderByIdQuery(id));

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Orders));
            }

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(
            Guid orderId, Domain.Enums.OrderStatus status)
        {
            var result = await mediator.Send(
                new UpdateOrderStatusCommand(orderId, status));

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(OrderDetails), new { id = orderId });
        }

        // PRODUCTS

        public async Task<IActionResult> Products()
        {
            var result = await mediator.Send(new GetAllProductsQuery());
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var categories = await mediator.Send(new GetAllCategoriesQuery());
            ViewBag.Categories = categories.Data;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            if (!ModelState.IsValid)
            {
                var categories = await mediator.Send(new GetAllCategoriesQuery());
                ViewBag.Categories = categories.Data;
                return View(command);
            }

            var result = await mediator.Send(command);

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                var categories = await mediator.Send(new GetAllCategoriesQuery());
                ViewBag.Categories = categories.Data;
                return View(command);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(Guid id)
        {
            var product = await mediator.Send(new GetProductByIdQuery(id));

            if (!product.Status)
            {
                TempData["Error"] = product.Message;
                return RedirectToAction(nameof(Products));
            }

            var categories = await mediator.Send(new GetAllCategoriesQuery());
            ViewBag.Categories = categories.Data;
            return View(product.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(UpdateProductCommand command)
        {
            if (!ModelState.IsValid)
            {
                var categories = await mediator.Send(new GetAllCategoriesQuery());
                ViewBag.Categories = categories.Data;
                return View(command);
            }

            var result = await mediator.Send(command);

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await mediator.Send(new DeleteProductCommand(id));

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Products));
        }

        // CATEGORIES

        public async Task<IActionResult> Categories()
        {
            var result = await mediator.Send(new GetAllCategoriesQuery());
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(
            CreateCategoryCommand command)
        {
            var result = await mediator.Send(command);

            TempData[result.Status ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Categories));
        }
    }
}