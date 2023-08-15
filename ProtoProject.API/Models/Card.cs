using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtoProject.API.Models
{
    public enum Completion
    {
        VeryLow,
        Low,
        Average,
        AboveAverage,
        AlmostDone,
        Done
    }

    public enum Priority
    {
        Low,
        Average,
        High
    }


    public class Card
    {
        public int CardId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Info { get; set; }
        [ForeignKey("CardGroup")]
        public int? CardGroupId { get; set; }
        public virtual CardGroup? CardGroup { get; set; }
        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public string? Tag { get; set; }
        public int? Grade { get; set; }
        public Completion? Completion { get; set; }
        public Priority? Priority { get; set; }
        public int? PreviousCardId { get; set; }
        public virtual Card? PreviousCard { get; set; }
        public int? NextCardId { get; set; }
        public virtual Card? NextCard { get; set; }
        [ForeignKey("CardSequence")]
        public int? CardSequenceId { get; set; }
        public ShareHash? ShareHash { get; set; }
        public int? DocFolderId { get; set; }
        public virtual DocFolder? DocFolder { get; set; }
        public int? LinkFolderId { get; set; }
        public virtual LinkFolder? LinkFolder { get; set; }
        public int? ImageFolderId { get; set; }
        public virtual ImageFolder? ImageFolder { get; set; }
    }
}
