namespace ModelDeliverySystemData.Model
{
    public partial class Client
    {
        public string FullName
        {
            get
            {
                return $"{ClientID} {FirstName} {LastName} {PhoneNumber}";
            }
        }
    }
}
