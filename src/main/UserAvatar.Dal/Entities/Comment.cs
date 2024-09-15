using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.Dal.Entities;

[Table("Comments")]
public class Comment
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public int CardId { get; set; }

    [ForeignKey("CardId")] public virtual Card Card { get; set; }

    [Required] public int UserId { get; set; }

    [ForeignKey("UserId")] public virtual User User { get; set; }

    [Required] [MaxLength(256)] public string Text { get; set; }

    [Required] public DateTimeOffset CreatedAt { get; set; }

    [Required] public DateTimeOffset ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }
}
