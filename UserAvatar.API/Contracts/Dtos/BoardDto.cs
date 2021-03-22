﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.API.Contracts.Dtos;

namespace UserAvatar.Api.Contracts.Dtos
{
    public class BoardDto
    {
        public int Id { get; set; }

        public bool IsOwner { get; set; }

        public string Title { get; set; }

        public List<UserShortDto> Members { get; set; }

        public List<ColumnDto> Columns { get; set; }
    }
}
