using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.DAL.Entities
{

    [Table("Comments")]
    public class Comment
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        //FK
        public int TaskId { get; set; }
        [Required]
        //FK
        public int UserId { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime ModifiedAt { get; set; }
    }
}