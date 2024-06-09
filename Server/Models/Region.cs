using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class Region
{
    [Key]
    [Column("RegionID")]
    public int RegionId { get; set; }

    [StringLength(100)]
    public string RegionName { get; set; } = null!;

    [InverseProperty("Region")]
    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    [InverseProperty("Region")]
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
