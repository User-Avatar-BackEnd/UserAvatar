﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAvatar.API.Contracts.Dtos
{
    public class ColumnDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Order { get; set; }

        public List<TaskShortDto> Tasks { get; set; }
    }
}