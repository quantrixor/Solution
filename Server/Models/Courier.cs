using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class Courier
{
    [Key]
    [Column("CourierID")]
    public int CourierId { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(20)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(50)]
    public string LicenseNumber { get; set; } = null!;

    [Column("VehicleID")]
    public int? VehicleId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Courier")]
    public virtual ICollection<CourierDocument> CourierDocuments { get; set; } = new List<CourierDocument>();

    [InverseProperty("Courier")]
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    [InverseProperty("Courier")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("VehicleId")]
    [InverseProperty("Couriers")]
    public virtual Vehicle? Vehicle { get; set; }
}
