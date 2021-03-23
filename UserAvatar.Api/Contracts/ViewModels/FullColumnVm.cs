using System.Collections.Generic;
namespace UserAvatar.Api.Contracts.ViewModel
{
    public class FullColumnVm
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public List<CardVm> Cards { get; set; }
    }
}