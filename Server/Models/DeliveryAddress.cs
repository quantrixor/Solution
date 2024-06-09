using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class DeliveryAddress
{
    [Key]
    [Column("AddressID")]
    public int AddressId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("OrderID")]
    public int OrderId { get; set; }

    [StringLength(255)]
    public string Address { get; set; } = null!;

    [StringLength(100)]
    public string City { get; set; } = null!;

    [StringLength(20)]
    public string PostalCode { get; set; } = null!;

    [StringLength(100)]
    public string Country { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("DeliveryAddresses")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("DeliveryAddresses")]
    public virtual User User { get; set; } = null!;
}
