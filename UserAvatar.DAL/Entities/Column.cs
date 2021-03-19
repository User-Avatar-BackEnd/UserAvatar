using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.DAL.Entities
{
    [Table("Columns")]
    public class Column
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [ForeignKey("BoardId")]
        [Required]
        public virtual Board Board { get; set; }
        [Required]
        public int Title { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        //FK
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        public DateTime ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public int Index { get; set; }
        
        public ICollection<Task> Tasks { get; set; }
    }
}