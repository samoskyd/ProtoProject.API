using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoProject.API.Models
{
    public class UserGroup
    {
        public UserGroup()
        {
            Users = new HashSet<User>();
        }

        public int UserGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AdminId { get; set; }
        public User? Admin { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }
}
