using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class OrderedItem
{
    [Key]
    [Column("OrderedItemID")]
    public int OrderedItemId { get; set; }

    [Column("OrderID")]
    public int OrderId { get; set; }

    [StringLength(255)]
    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderedItems")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("OrderedItems")]
    public virtual Product? Product { get; set; }
}
