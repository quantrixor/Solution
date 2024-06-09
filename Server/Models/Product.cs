using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class Product
{
    [Key]
    [Column("ProductID")]
    public int ProductId { get; set; }

    [StringLength(255)]
    public string ProductName { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    [Column("ProductTypeID")]
    public int? ProductTypeId { get; set; }

    public DateOnly? ProductionDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateAt { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("Product")]
    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductDocument> ProductDocuments { get; set; } = new List<ProductDocument>();

    [ForeignKey("ProductTypeId")]
    [InverseProperty("Products")]
    public virtual ProductType? ProductType { get; set; }
}
