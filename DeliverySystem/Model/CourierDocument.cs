namespace DeliverySystem.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CourierDocument
    {
        [Key]
        public int DocumentID { get; set; }

        public int CourierID { get; set; }

        public byte[] Passport { get; set; }

        public byte[] INN { get; set; }

        public byte[] SNILS { get; set; }

        public byte[] DriverLicense { get; set; }

        public byte[] ContractCopy { get; set; }

        public byte[] BankDetails { get; set; }

        public virtual Courier Courier { get; set; }
    }
}
