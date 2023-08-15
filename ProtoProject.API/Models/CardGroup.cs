namespace ProtoProject.API.Models
{
    public class CardGroup
    {
        public CardGroup()
        {
            Cards = new HashSet<Card>();
        }
        public int CardGroupId { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Card>? Cards { get; set; }
    }
}
