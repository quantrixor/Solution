using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class Model
{
    [Key]
    [Column("ModelID")]
    public int ModelId { get; set; }

    [StringLength(100)]
    public string ModelName { get; set; } = null!;

    [Column("BrandID")]
    public int BrandId { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Models")]
    public virtual Brand Brand { get; set; } = null!;

    [InverseProperty("Model")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
