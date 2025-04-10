using SalesOrderApp.Models;
using SalesOrderApp.Data;
using Microsoft.EntityFrameworkCore;

namespace SalesOrderApp.Services
{
    public class SalesOrderService: ISalesOrderService
    {
        private readonly SalesDbContext _dbContext;

        public SalesOrderService(SalesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SoOrder>> GetAllOrdersAsync()
        {
            var orders = from o in _dbContext.SoOrders select o;
            return await orders.Include(o => o.ComCustomer).ToListAsync();
        }

        public async Task<int> GetTotalOrders(string? keywords, DateTime? dateTime)
        {
            var orders = from o in _dbContext.SoOrders select o;
            if (!String.IsNullOrEmpty(keywords))
            {
                orders = orders.Where(o => o.OrderNo.Contains(keywords) || o.ComCustomer.CustomerName.Contains(keywords));
            }
            if (dateTime != null)
            {
                orders = orders.Where(o => o.OrderDate == dateTime);
            }
            return (await orders.ToListAsync()).Count();
        }

        public async Task<List<SoOrder>> GetAllOrdersAsync(string? keywords, DateTime? dateTime, int page=1, int pageSize=10)
        {
            var orders = from o in _dbContext.SoOrders select o;
            if (!String.IsNullOrEmpty(keywords))
            {
                orders = orders.Where(o => o.OrderNo.Contains(keywords) || o.ComCustomer.CustomerName.Contains(keywords));
            }
            if (dateTime != null)
            {
                orders = orders.Where(o => o.OrderDate == dateTime);
            }
            return await orders.Include(o => o.ComCustomer).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<SoOrder?> GetOrderAsync(long id)
        {
            var order = await _dbContext.SoOrders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.SoOrderId == id);
            
            return order;
        }

        public bool OrderExists(long id)
        {
            return (_dbContext.SoOrders?.Any(e => e.SoOrderId == id)).GetValueOrDefault();
        }

        public bool ItemExists(long id, string name)
        {
            return (_dbContext.SoItems?.Any(e => e.SoOrderId == id && e.ItemName == name)).GetValueOrDefault();
        }

        public bool CustomerExists(long id)
        {
            return (_dbContext.ComCustomers?.Any(e => e.ComCustomerId == id)).GetValueOrDefault();
        }

        public async Task<bool> SaveAsync(SoOrder newOrder)
        {
            SoOrder? order;
            if (newOrder.SoOrderId <= 0)
            {
                _dbContext.SoOrders.Add(newOrder);
            }
            else
            {
                order = await _dbContext.SoOrders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.SoOrderId == newOrder.SoOrderId);

                if (order == null)
                {
                    throw new Exception("order does not exist");
                }

                var isCustomerExist = CustomerExists(newOrder.ComCustomerId);
                if (!isCustomerExist)
                {
                    throw new Exception("customer does not exist");
                }

                order.OrderNo = newOrder.OrderNo;
                order.OrderDate = newOrder.OrderDate;
                order.ComCustomerId = newOrder.ComCustomerId;
                order.Address = newOrder.Address;
                _dbContext.SoItems.RemoveRange(order.Items);

                order.Items = newOrder.Items.ToList();

                _dbContext.SoOrders.Update(order);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long newOrder)
        {
            return false;
        }

        public async Task<List<ComCustomer>> GetCustomersAsync()
        {
            return await _dbContext.ComCustomers.ToListAsync();
        }
    }
}
