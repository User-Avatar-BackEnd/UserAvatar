using System;
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
        [Required]
        public int Title { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        //FK
        public int CreatedById { get; set; }
        
        public DateTime ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
        public int Index { get; set; }
    }
}