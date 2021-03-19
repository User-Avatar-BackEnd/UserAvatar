using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using SQLite;

namespace UserAvatar.DAL.Entities
{
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

        [Required] 
        public string PasswordHash { get; set; }

        public int Score { get; set; }

        //todo: change into constants
        public string Role { get; set; }

        public ICollection<History> Histories { get; set; }
        public ICollection<Board> Boards { get; set; }
        public ICollection<Comment> Comments { get; set; }
        /*[InverseProperty("Invited")]
        public ICollection<Invite> Invited { get; set; }
        [InverseProperty("Inviter")]
        public ICollection<Invite> Inviter { get; set; }*/

    }
}