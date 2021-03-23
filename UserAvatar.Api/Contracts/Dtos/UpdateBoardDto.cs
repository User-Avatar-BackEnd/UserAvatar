using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Requests
{
    public class UpdateBoardDto
    {
        public int Id { get; set; }
        
        [StringLength(64, MinimumLength = 1)]
        public string Title { get; set; }
    }
}