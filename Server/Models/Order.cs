using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class Order
{
    [Key]
    [Column("OrderID")]
    public int OrderId { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    [Column("ClientID")]
    public int? ClientId { get; set; }

    public int? CreatedBy { get; set; }

    public int? ManagedBy { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("Orders")]
    public virtual Client? Client { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("OrderCreatedByNavigations")]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    [InverseProperty("Order")]
    public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();

    [ForeignKey("ManagedBy")]
    [InverseProperty("OrderManagedByNavigations")]
    public virtual User? ManagedByNavigation { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();

    [InverseProperty("Order")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
