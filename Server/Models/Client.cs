using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

[Index("PhoneNumber", Name = "UQ__Clients__85FB4E38D23A77AD", IsUnique = true)]
[Index("Email", Name = "UQ__Clients__A9D105346639C0CD", IsUnique = true)]
public partial class Client
{
    [Key]
    [Column("ClientID")]
    public int ClientId { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(100)]
    public string MiddleName { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string PhoneNumber { get; set; } = null!;

    [Column("RegionID")]
    public int? RegionId { get; set; }

    [Column("CityID")]
    public int? CityId { get; set; }

    [StringLength(255)]
    public string StreetAddress { get; set; } = null!;

    [ForeignKey("CityId")]
    [InverseProperty("Clients")]
    public virtual City? City { get; set; }

    [InverseProperty("Client")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("RegionId")]
    [InverseProperty("Clients")]
    public virtual Region? Region { get; set; }
}
