using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UserAvatar.API.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [NotNull]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [NotNull]
        public string Email { get; set; }
        [NotNull]
        [StringLength(64, MinimumLength = 6)]
        public string Login { get; set; }
        [NotNull]
        public string PasswordHash { get; set; }
        
        public int Score { get; set; }
        
        //todo: change into constants
        public string Role { get; set; }

    }
}