namespace SalesOrderApp.Models
{
    public class SoItemViewModel
    {
        public long SoItemId { get; set; }

        public long SoOrderId { get; set; }

        public string ItemName { get; set; } = null!;

        public int Quantity { get; set; }

        public double Price { get; set; }
    }
}
