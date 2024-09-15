using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.Dal.Entities;

[Table("DailyEvents")]
public sealed class DailyEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string EventName { get; set; }

    public bool IsCompleted { get; set; }
}
