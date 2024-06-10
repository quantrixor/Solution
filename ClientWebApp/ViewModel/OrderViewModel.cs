using System.ComponentModel.DataAnnotations;

namespace ClientWebApp.ViewModel
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public DateTime? DeliveryDate { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "The amount must be greater than 0")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string Comment { get; set; }
    }
}
