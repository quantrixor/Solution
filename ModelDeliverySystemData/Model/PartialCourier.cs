namespace ModelDeliverySystemData.Model
{
    public partial class Courier
    {
        public string FullName
        {
            get
            {
                return $"{LastName} {FirstName}";
            }
        }
    }
}
