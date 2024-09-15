using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.Dal.Entities;

[Table("Histories")]
public class History
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    [Required]
    public string EventName { get; set; }

    [Required]
    public int Score { get; set; }

    [Required]
    public DateTimeOffset DateTime { get; set; }

    public bool Calculated { get; set; }
}
