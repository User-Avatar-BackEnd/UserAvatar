using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.Dal.Entities
{
    [Table("Boards")]
    public class Board
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
          
        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual User User { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ModifiedAt { get; set; }
        
        public virtual ICollection<Column> Columns { get; set; }

        public virtual ICollection<Member> Members { get; set; }

        public bool IsDeleted { get; set; }
        
        public int ModifiedBy { get; set; }
    }
}