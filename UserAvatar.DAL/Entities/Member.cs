using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.DAL.Entities
{
    [Table("Members")]
    public class Member
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("UserId")]
        //FK
        public virtual User User { get; set; }
        [Required]
        [ForeignKey("BoardId")]
        //FK
        public virtual Board Board{ get; set; }
    }
}