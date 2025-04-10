using Microsoft.AspNetCore.Mvc;
using SalesOrderApp.Models;
using System.Diagnostics;
using SalesOrderApp.Data;
using Microsoft.EntityFrameworkCore;
using SalesOrderApp.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;

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

        public async Task<IActionResult> Create()
        {
            //var customers = await _service.GetCustomersAsync();
            ViewBag.Customers = await PopulateCustomerList();

            return View("SalesOrderForm", new SoOrderViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("OrderNo,OrderDate,Address,Items,ComCustomerId")] SoOrderViewModel order)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = await PopulateCustomerList();
                return View(order);
            }

            SoOrder newOrder = new SoOrder()
            {
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
                if (!success) {
                    ViewBag.Customers = await PopulateCustomerList();
                    return View(order);
                }

                return RedirectToAction("Index");
            } catch (Exception ex)
            {
                ModelState.AddModelError(ex.Message, "An error occurred while saving the order. Please try again.");
            }

            ViewBag.Customers = await PopulateCustomerList();
            return View(order);

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
            ViewBag.Customers = await PopulateCustomerList();
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
                    if (!success)
                    {
                        ViewBag.Customers = await PopulateCustomerList();
                        return View(order);
                    }

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

            ViewBag.Customers = await PopulateCustomerList();
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(SoOrderPageViewModel viewModel)
        {
            var orders = await _service.GetAllOrdersAsync(viewModel.Keywords, viewModel.OrderDate);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sales Orders");

            // Headers
            worksheet.Cell(1, 1).Value = "Order No";
            worksheet.Cell(1, 2).Value = "Order Date";
            worksheet.Cell(1, 3).Value = "Customer";
            worksheet.Cell(1, 4).Value = "Address";

            // Data
            int row = 2;
            foreach (var order in orders)
            {
                worksheet.Cell(row, 1).Value = order.SoOrderId;
                worksheet.Cell(row, 2).Value = order.OrderNo;
                worksheet.Cell(row, 3).Value = order.OrderDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 4).Value = order.ComCustomer?.CustomerName ?? "N/A";
                worksheet.Cell(row, 5).Value = order.Address;
                row++;
            }

            // Optional: adjust column widths
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"SalesOrders_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
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

        private async Task<List<SelectListItem>> PopulateCustomerList()
        {
            var customers = await _service.GetCustomersAsync();
            return customers
                .Select(c => new SelectListItem
                {
                    Value = c.ComCustomerId.ToString(),
                    Text = c.CustomerName
                })
                .ToList();
        }
    }
}