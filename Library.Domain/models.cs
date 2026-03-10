using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{


    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public List<Invoice> Invoices { get; set; } = new();
    }

    public class InvoiceLine
    {
        public int Id { get; set; }

        // FK to Invoice
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        // FK to Product
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // snapshot price at time of invoice
    }

    public class Invoice
    {
        public int Id { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        // FK to Customer
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public List<InvoiceLine> Lines { get; set; } = new();
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public List<InvoiceLine> InvoiceLines { get; set; } = new();
    }
}
