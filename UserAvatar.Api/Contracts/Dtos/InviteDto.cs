using System.ComponentModel.DataAnnotations;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class InviteDto
    {
        [Required]
        public int BoardId { get; set; }
        [Required]
        public string Payload { get; set; }
    }
}