using AutoMapper;
using UserAvatar.API.Contracts;
using UserAvatar.API.Contracts.Dtos;
using UserAvatar.BLL.Models;
using UserAvatar.DAL.Entities;

namespace UserAvatar.API.Options
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
            CreateMap<CommentModel, CommentDto>()
                .ForMember("UserId", opt => opt.MapFrom(src => src.User.Id));
        }
    }
}