namespace ModelDeliverySystemData.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Review
    {
        public int ReviewID { get; set; }

        public int UserID { get; set; }

        public int? CourierID { get; set; }

        public int? OrderID { get; set; }

        public int? Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual Courier Courier { get; set; }

        public virtual Order Order { get; set; }

        public virtual User User { get; set; }
    }
}
