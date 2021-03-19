using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.DAL.Entities
{
    [Table("Boards")]
    public class Board
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        
        [ForeignKey("OwnerId")]
        [Required]
        public virtual User User { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime ModifiedAt { get; set; }
        
        public ICollection<Column> Columns { get; set; }
        public ICollection<Member> Members { get; set; }
    }
}