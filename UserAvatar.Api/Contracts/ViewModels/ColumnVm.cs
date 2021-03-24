using System.Collections.Generic;
namespace UserAvatar.Api.Contracts.ViewModels
{
    public class ColumnVm
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Order { get; set; }

        public List<CardShortVm> Cards { get; set; }
    }
}
