namespace ProtoProject.API.Models
{
    public class CardSequence
    {
        public CardSequence()
        {
            Cards = new HashSet<Card>();
        }

        public int CardSequenceId { get; set; }
        public string CardSequenceName { get; set;}
        public string? CardSequenceDescription { get; set;}
        public int StartCardId { get; set; }
        public virtual Card? StartCard { get; set; }
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Card>? Cards { get; set; }
    }
}
