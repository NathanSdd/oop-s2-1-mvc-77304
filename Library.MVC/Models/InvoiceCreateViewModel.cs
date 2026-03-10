using Library.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library.MVC.Models
{
    public class InvoiceCreateViewModel
    {
        public int CustomerId { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public List<SelectListItem>? Customers { get; set; }

        public List<SelectListItem>? Products { get; set; }
    }
}
