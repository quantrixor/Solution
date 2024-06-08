namespace DeliverySystem.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vehicle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vehicle()
        {
            Couriers = new HashSet<Courier>();
        }

        public int VehicleID { get; set; }

        [Required]
        [StringLength(50)]
        public string LicensePlate { get; set; }

        public int ModelID { get; set; }

        public int TypeID { get; set; }

        public byte[] InsuranceDocument { get; set; }

        public byte[] TechnicalPassport { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Courier> Couriers { get; set; }

        public virtual Model Model { get; set; }

        public virtual VehicleType VehicleType { get; set; }
    }
}
