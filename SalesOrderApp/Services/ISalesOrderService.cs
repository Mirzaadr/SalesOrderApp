using SalesOrderApp.Models;

namespace SalesOrderApp.Services
{
    public interface ISalesOrderService
    {
        Task<List<SoOrder>> GetAllOrdersAsync();
        Task<List<SoOrder>> GetAllOrdersAsync(string? keywords, DateTime? orderDate, int page, int pageSize);
        Task<int> GetTotalOrders(string? keywords, DateTime? dateTime);
        Task<SoOrder?> GetOrderAsync(long id);
        Task<bool> SaveAsync(SoOrder order);
        Task<bool> DeleteAsync(long orderId);
        Task<List<ComCustomer>> GetCustomersAsync();
        bool OrderExists(long id);
        bool ItemExists(long id, string name);

    }
}
