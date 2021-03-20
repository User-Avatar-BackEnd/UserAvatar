using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.DAL.Entities
{
    [Table("Events")]
    public class Event
    {
        [Key] 
        [Required] 
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Name { get; set; }

        [Required]
        public int Score { get; set; }
    }
}