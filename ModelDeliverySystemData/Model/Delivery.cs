namespace ModelDeliverySystemData.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Delivery
    {
        public int DeliveryID { get; set; }

        public int OrderID { get; set; }

        public int CourierID { get; set; }

        [Column(TypeName = "date")]
        public DateTime DeliveryDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ActualDeliveryDate { get; set; }

        public int StatusID { get; set; }

        public virtual Courier Courier { get; set; }

        public virtual Order Order { get; set; }

        public virtual DeliveryStatu DeliveryStatu { get; set; }
    }
}
