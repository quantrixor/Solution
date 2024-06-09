namespace ModelDeliverySystemData.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Invoice
    {
        public int InvoiceID { get; set; }

        public int? ProductID { get; set; }

        public byte[] InvoiceDocument { get; set; }

        public DateTime? UploadedAt { get; set; }

        public virtual Product Product { get; set; }
    }
}
