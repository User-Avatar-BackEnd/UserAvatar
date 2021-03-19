using System;
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
        
        //Fk
        [Required]
        public int OwnerId { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime ModifiedAt { get; set; }
    }
}