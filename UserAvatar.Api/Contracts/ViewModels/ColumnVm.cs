using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class ColumnVm
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Order { get; set; }

        public List<CardShortVm> Cards { get; set; }
    }
}
