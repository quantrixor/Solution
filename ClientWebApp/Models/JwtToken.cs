namespace ClientWebApp.Models
{
    public class JwtToken
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
