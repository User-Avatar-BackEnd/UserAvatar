using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UserAvatar.DAL.Entities
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
        public string EventId { get; set; }

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
        
        public bool Calculated { get; set; }
    }
}