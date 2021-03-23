using System.Collections.Generic;
using UserAvatar.Api.Contracts.Dtos;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class FullColumnVm
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public List<CardVm> Cards { get; set; }
    }
}