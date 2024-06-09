using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class Review
{
    [Key]
    [Column("ReviewID")]
    public int ReviewId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("CourierID")]
    public int? CourierId { get; set; }

    [Column("OrderID")]
    public int? OrderId { get; set; }

    public int? Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("CourierId")]
    [InverseProperty("Reviews")]
    public virtual Courier? Courier { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Reviews")]
    public virtual Order? Order { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Reviews")]
    public virtual User User { get; set; } = null!;
}
