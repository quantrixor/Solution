namespace ModelDeliverySystemData.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OperationLog
    {
        [Key]
        public int LogID { get; set; }

        public int? UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Operation { get; set; }

        [StringLength(1000)]
        public string Details { get; set; }

        public DateTime? Timestamp { get; set; }

        public virtual User User { get; set; }
    }
}
