namespace ModelDeliverySystemData.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductDocument
    {
        [Key]
        public int DocumentID { get; set; }

        public int? ProductID { get; set; }

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; }

        public byte[] DocumentContent { get; set; }

        public DateTime? UploadedAt { get; set; }

        public virtual Product Product { get; set; }
    }
}
