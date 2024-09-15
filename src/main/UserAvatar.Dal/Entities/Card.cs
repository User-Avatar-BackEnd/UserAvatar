using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.Dal.Entities;

[Table("Cards")]
public class Card
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public int ColumnId { get; set; }

    [ForeignKey("ColumnId")] public virtual Column Column { get; set; }

    [Required] [MaxLength(64)] public string Title { get; set; }

    [MaxLength(2048)] public string Description { get; set; }

    [Required] public int OwnerId { get; set; }

    [ForeignKey("OwnerId")] public virtual User Owner { get; set; }

    public int? ResponsibleId { get; set; }

    [ForeignKey("ResponsibleId")] public virtual User Responsible { get; set; }

    [Required] public DateTimeOffset CreatedAt { get; set; }

    [Required] public DateTimeOffset ModifiedAt { get; set; }

    public int? Priority { get; set; }

    public bool IsHidden { get; set; }

    public virtual ICollection<Comment> Comments { get; set; }

    public bool IsDeleted { get; set; }

    public int ModifiedBy { get; set; }
}
