using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UserAvatar.Dal.Entities
{
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

        [ForeignKey("EventName")]
        public virtual Event Event { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
        
        public bool Calculated { get; set; }
    }
}