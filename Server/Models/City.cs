using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class City
{
    [Key]
    [Column("CityID")]
    public int CityId { get; set; }

    [StringLength(100)]
    public string CityName { get; set; } = null!;

    [Column("RegionID")]
    public int RegionId { get; set; }

    [InverseProperty("City")]
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    [ForeignKey("RegionId")]
    [InverseProperty("Cities")]
    public virtual Region Region { get; set; } = null!;
}
