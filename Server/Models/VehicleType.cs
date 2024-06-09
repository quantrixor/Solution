using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class VehicleType
{
    [Key]
    [Column("TypeID")]
    public int TypeId { get; set; }

    [StringLength(50)]
    public string TypeName { get; set; } = null!;

    [InverseProperty("Type")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
