using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalesOrderApp.Models
{
    public class SoOrderViewModel
    {
        public long SoOrderId { get; set; }

        public string OrderNo { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public int ComCustomerId { get; set; }

        public string Address { get; set; } = null!;

        public List<SoItemViewModel> Items { get; set; } = new List<SoItemViewModel>();

        //public List<SelectListItem> Customers { get; set; } // For dropdown

    }
}
