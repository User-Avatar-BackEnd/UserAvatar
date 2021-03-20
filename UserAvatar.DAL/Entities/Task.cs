using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.DAL.Entities
{
    [Table("Tasks")]
    public class Task
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ColumnId { get; set; }

        [ForeignKey("ColumnId")]
        public virtual Column Column { get; set; }

        [Required]
        [MaxLength(64)]
        public string Title { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }

        public int ResponsibleId { get; set; }

        [ForeignKey("ResponsibleId")]
        public virtual User Responsible { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ModifiedAt { get; set; }

        public int Priority { get; set; }
        
        public bool IsHidden { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        
    }
}