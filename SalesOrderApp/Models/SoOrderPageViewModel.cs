namespace SalesOrderApp.Models
{
    public class SoOrderPageViewModel
    {
        public List<SoOrder> Orders { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string? Keywords { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
