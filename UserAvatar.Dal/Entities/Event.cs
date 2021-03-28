using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAvatar.Dal.Entities
{
    [Table("Events")]
    public class Event
    {
        [Key] 
        [Required] 
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Name { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Index { get; set; }

        [Required]
        public int Score { get; set; }
    }
}