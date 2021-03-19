using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.DAL.Entities
{
    [Table("Invites")]
    public class Invite
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        //FK
        [ForeignKey("InviterId")]
        public virtual User Inviter { get; set; }
        [Required]
        //FK
        [ForeignKey("InvitedId")]
        public virtual User Invited { get; set; }
        [Required]
        //Enum?
        public string Status { get; set; }
        public DateTime Issued { get; set; }
    }
}