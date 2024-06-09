using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class Vehicle
{
    [Key]
    [Column("VehicleID")]
    public int VehicleId { get; set; }

    [StringLength(50)]
    public string LicensePlate { get; set; } = null!;

    [Column("ModelID")]
    public int ModelId { get; set; }

    [Column("TypeID")]
    public int TypeId { get; set; }

    public byte[]? InsuranceDocument { get; set; }

    public byte[]? TechnicalPassport { get; set; }

    [InverseProperty("Vehicle")]
    public virtual ICollection<Courier> Couriers { get; set; } = new List<Courier>();

    [ForeignKey("ModelId")]
    [InverseProperty("Vehicles")]
    public virtual Model Model { get; set; } = null!;

    [ForeignKey("TypeId")]
    [InverseProperty("Vehicles")]
    public virtual VehicleType Type { get; set; } = null!;
}
