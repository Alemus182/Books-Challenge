
namespace Application.Dtos.Auth
{
    public record SingInResponse
    {
        public bool valid { get; set; }
        public string id { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
