namespace ProtoProject.API.Models
{
    public class ShareHash
    {
        public int ShareHashId { get; set; }
        public int CardId { get; set; }
        public virtual Card? Card { get; set; }
        public string Hash { get; set; }
    }
}
