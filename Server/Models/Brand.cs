using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class Brand
{
    [Key]
    [Column("BrandID")]
    public int BrandId { get; set; }

    [StringLength(100)]
    public string BrandName { get; set; } = null!;

    [InverseProperty("Brand")]
    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
