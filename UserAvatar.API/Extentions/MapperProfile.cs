using AutoMapper;
using UserAvatar.Api.Contracts;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.API.Contracts.Dtos;
using UserAvatar.Bll.Models;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Api.Extentions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<CommentModel, Comment>();
            CreateMap<Board, BoardModel>();
            CreateMap<BoardModel, BoardDto>();
            CreateMap<Event, EventModel>();
            CreateMap<History, HistoryModel>();
            CreateMap<Invite, InviteModel>();
            CreateMap<Rank, RankModel>();
            CreateMap<Task, TaskModel>();
            CreateMap<TaskModel, TaskDetailedDto>()
                .ForMember("ColumnId", opt => opt.MapFrom(src => src.Column.Id))
                .ForMember("ResponsibleId", opt => opt.MapFrom(src => src.Responsible.Id));
            CreateMap<TaskModel, TaskShortDto>()
                .ForMember("ColumnId", opt => opt.MapFrom(src => src.Column.Id))
                .ForMember("ResponsibleId", opt => opt.MapFrom(src => src.Responsible.Id))
                .ForMember("CommentsCount", opt => opt.MapFrom(src => src.Comments.Count));
            CreateMap<CommentModel, CommentDto>()
                .ForMember("UserId", opt => opt.MapFrom(src => src.User.Id));
        }
    }
}