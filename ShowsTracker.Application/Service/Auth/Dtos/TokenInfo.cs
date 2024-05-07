namespace ShowsTracker.Application.Service.Auth.Dtos
{
    public class TokenInfo
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }
    }
}
