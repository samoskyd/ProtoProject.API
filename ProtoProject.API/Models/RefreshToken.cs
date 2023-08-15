namespace ProtoProject.API.Models
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; } = DateTime.Now.AddDays(1);
    }
}
