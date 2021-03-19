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
        
        [ForeignKey("ColumnId")]
        public virtual Column Column { get; set; }
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public int Priority { get; set; }
        
        public bool IsHidden { get; set; }
        
    }
}