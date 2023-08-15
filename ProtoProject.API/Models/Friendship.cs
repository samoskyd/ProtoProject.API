namespace ProtoProject.API.Models
{
    public class Friendship
    {
        public int FriendshipId { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public int FUserId { get; set; }
        public virtual User? FUser { get; set; }
        public int SUserId { get; set; }
        public virtual User? SUser { get; set; }
    }
}
