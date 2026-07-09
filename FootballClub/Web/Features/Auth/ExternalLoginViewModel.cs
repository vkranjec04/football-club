namespace FootballClub.Web.Features.Auth
{
    public class ExternalLoginViewModel
    {
        public string Token { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = "/";
    }
}
