namespace ProtoProject.API.Models
{
    public class User
    {
        public User()
        {
            Cards = new HashSet<Card>();
            CardGroups = new HashSet<CardGroup>();
            UserGroups = new HashSet<UserGroup>();
            Friendships = new HashSet<Friendship>();
            CardSequences = new HashSet<CardSequence>();
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? TokenCreated { get; set; }
        public DateTime? TokenExpires { get; set; }
        public virtual ICollection<Card>? Cards { get; set; }
        public virtual ICollection<UserGroup>? UserGroups { get; set; }
        public virtual ICollection<Friendship>? Friendships { get; set; }
        public virtual ICollection<CardGroup>? CardGroups { get; set; }
        public virtual ICollection<CardSequence>? CardSequences { get; set; }
    }
}
