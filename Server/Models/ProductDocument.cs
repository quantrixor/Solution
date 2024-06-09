using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public partial class ProductDocument
{
    [Key]
    [Column("DocumentID")]
    public int DocumentId { get; set; }

    [Column("ProductID")]
    public int? ProductId { get; set; }

    [StringLength(100)]
    public string DocumentType { get; set; } = null!;

    public byte[]? DocumentContent { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UploadedAt { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductDocuments")]
    public virtual Product? Product { get; set; }
}
