using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class CourierDocument
{
    [Key]
    [Column("DocumentID")]
    public int DocumentId { get; set; }

    [Column("CourierID")]
    public int CourierId { get; set; }

    public byte[]? Passport { get; set; }

    [Column("INN")]
    public byte[]? Inn { get; set; }

    [Column("SNILS")]
    public byte[]? Snils { get; set; }

    public byte[]? DriverLicense { get; set; }

    public byte[]? ContractCopy { get; set; }

    public byte[]? BankDetails { get; set; }

    [StringLength(255)]
    public string? DocumentType { get; set; }

    [StringLength(255)]
    public string? DisplayName { get; set; }

    [ForeignKey("CourierId")]
    [InverseProperty("CourierDocuments")]
    public virtual Courier Courier { get; set; } = null!;
}
