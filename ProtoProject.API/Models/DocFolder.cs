using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoProject.API.Models
{
    public class DocFolder
    {
        public int DocFolderId { get; set; }
        public string? Link { get; set; }
        public string? ContainerName { get; set; }
        public string? BlobName { get; set; }
        [ForeignKey("Card")]
        public int CardId { get; set; }
        public virtual Card? Card { get; set; }
    }
}
