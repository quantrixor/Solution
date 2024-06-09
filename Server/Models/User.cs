using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

[Index("PhoneNumber", Name = "UQ__Users__85FB4E3817E198F2", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D1053494526D75", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(20)]
    public string PhoneNumber { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("RoleID")]
    public int? RoleId { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();

    [InverseProperty("User")]
    public virtual ICollection<OperationLog> OperationLogs { get; set; } = new List<OperationLog>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Order> OrderCreatedByNavigations { get; set; } = new List<Order>();

    [InverseProperty("ManagedByNavigation")]
    public virtual ICollection<Order> OrderManagedByNavigations { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role? Role { get; set; }
}
