using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class ProductType
{
    [Key]
    [Column("ProductTypeID")]
    public int ProductTypeId { get; set; }

    [StringLength(100)]
    public string ProductTypeName { get; set; } = null!;

    [InverseProperty("ProductType")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
