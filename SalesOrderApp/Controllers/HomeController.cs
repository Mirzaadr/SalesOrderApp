using Microsoft.AspNetCore.Mvc;
using SalesOrderApp.Models;
using System.Diagnostics;
using SalesOrderApp.Data;
using Microsoft.EntityFrameworkCore;
using SalesOrderApp.Services;

namespace SalesOrderApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISalesOrderService _service;

        public HomeController(ILogger<HomeController> logger, ISalesOrderService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<IActionResult> Index(string? keywords, DateTime? orderDate, int page = 1, int pageSize = 10)
        {
            var orders = await _service.GetAllOrdersAsync(keywords, orderDate, page, pageSize);
            var totalOrders = await _service.GetTotalOrders(keywords, orderDate);

            var viewModel = new SoOrderPageViewModel
            {
                Orders = orders,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalOrders,
                Keywords = keywords,
                OrderDate = orderDate,
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(long id)
        {
            var order = await _service.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            SoOrderViewModel viewModel = new SoOrderViewModel()
            {
                SoOrderId = order.SoOrderId,
                OrderNo = order.OrderNo,
                OrderDate = order.OrderDate,
                ComCustomerId = order.ComCustomerId,
                Address = order.Address,
                Items = order.Items.Select(i => new SoItemViewModel
                {
                    SoItemId = i.SoItemId,
                    SoOrderId = i.SoOrderId,
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
            };
            //return View("SalesOrderForm", viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SoOrderId,OrderNo,OrderDate,Address,Items,ComCustomerId")] SoOrderViewModel order)
        {
            if (id != order.SoOrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                SoOrder newOrder = new SoOrder() { 
                    SoOrderId = order.SoOrderId,
                    OrderNo = order.OrderNo,
                    OrderDate = order.OrderDate,
                    ComCustomerId = order.ComCustomerId,
                    Address = order.Address,
                    Items = order.Items.Select(e => new SoItem()
                    {
                        ItemName = e.ItemName,
                        Quantity = e.Quantity,
                        Price = e.Price,
                        SoOrderId = order.SoOrderId,
                    }).ToList(),
                };

                try
                {
                    var success = await _service.SaveAsync(newOrder);
                    if (!success) return View(order);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (!_service.OrderExists(order.SoOrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An error occurred while saving the order. Please try again.");
                    }
                }
            }
            return View(order);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //private void PopulateCustomerList(SoOrderViewModel model)
        //{
        //    model.Customers = _dbContext.ComCustomers
        //        .Select(c => new Selec
        //        {
        //            Value = c.Id.ToString(),
        //            Text = c.Name
        //        })
        //        .ToList();
        //}
    }
}