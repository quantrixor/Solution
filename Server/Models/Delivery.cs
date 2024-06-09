using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class Delivery
{
    [Key]
    [Column("DeliveryID")]
    public int DeliveryId { get; set; }

    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("CourierID")]
    public int CourierId { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public DateOnly? ActualDeliveryDate { get; set; }

    [Column("StatusID")]
    public int StatusId { get; set; }

    [ForeignKey("CourierId")]
    [InverseProperty("Deliveries")]
    public virtual Courier Courier { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("Deliveries")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("StatusId")]
    [InverseProperty("Deliveries")]
    public virtual DeliveryStatus Status { get; set; } = null!;
}
