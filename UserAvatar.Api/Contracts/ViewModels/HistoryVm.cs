﻿using System;
namespace UserAvatar.Api.Contracts.ViewModels
{
    public class HistoryVm
    {
        public string EventName { get; set; }

        public int Sore { get; set; }

        public DateTimeOffset DateTime { get; set; }
    }
}
