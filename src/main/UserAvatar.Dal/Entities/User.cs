using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.Dal.Entities;

[Table("Users")]
public class User
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    //todo: add here validation attribute to check valid email
    public string Email { get; set; }

    [Required]
    [StringLength(64, MinimumLength = 6)]
    public string Login { get; set; }

    [Required] public string PasswordHash { get; set; }

    [Required] public int Score { get; set; }

    [Required]
    //todo: change into constants
    public string Role { get; set; }

    public virtual ICollection<History> Histories { get; set; }

    public virtual ICollection<Board> Boards { get; set; }

    public virtual ICollection<Comment> Comments { get; set; }

    public virtual ICollection<Invite> Invited { get; set; }

    public virtual ICollection<Invite> Inviter { get; set; }
}
