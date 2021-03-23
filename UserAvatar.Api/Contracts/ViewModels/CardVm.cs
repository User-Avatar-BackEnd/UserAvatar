﻿namespace UserAvatar.Api.Contracts.ViewModel
{
    public class CardVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public int ResponsibleId { get; set; }
        public bool IsHidden { get; set; }
        public int CommentsCount { get; set; }
    }
}