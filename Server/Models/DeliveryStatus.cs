using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

[Table("DeliveryStatus")]
public partial class DeliveryStatus
{
    [Key]
    [Column("StatusID")]
    public int StatusId { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [InverseProperty("Status")]
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
