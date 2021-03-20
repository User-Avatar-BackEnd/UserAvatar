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
        public int InviterId { get; set; }

        [ForeignKey("InviterId")]
        public virtual User Inviter { get; set; }

        [Required]
        public int InvitedId { get; set; }

        [ForeignKey("InvitedId")]
        public virtual User Invited { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public DateTime Issued { get; set; }
    }
}