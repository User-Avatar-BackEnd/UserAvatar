using System.ComponentModel.DataAnnotations;

namespace UserAvatar.API.Contracts
{
    public class BoardRequest
    {
        public int Id { get; set; }
        
        [StringLength(64, MinimumLength = 1)]
        public string Title { get; set; }
    }
}