using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using UserAvatar.Api.Contracts;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.Requests;
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
            CreateMap<Member, MemberModel>();
           
            CreateMap<Column, ColumnModel>();
            CreateMap<Event, EventModel>();
            CreateMap<History, HistoryModel>();
            CreateMap<Invite, InviteModel>();
            CreateMap<Rank, RankModel>();
            CreateMap<Task, TaskModel>();
            CreateMap<Comment, ColumnModel>();
            
            
            CreateMap<UserModel, User>();
            CreateMap<MemberModel, Member>();
           
            CreateMap<ColumnModel, Column>();
            CreateMap<EventModel, Event>();
            CreateMap<HistoryModel, History>();
            CreateMap<InviteModel, Invite>();
            CreateMap<RankModel, Rank>();
            CreateMap<TaskModel, Task>();
            CreateMap<CommentModel, Comment>();


            CreateMap<BoardModel, BoardShortDto>();

            CreateMap<Board, BoardModel>();
            CreateMap<UserModel, UserShortDto>();
            // .ForMember(x=> x.Rank, x=>x.MapFrom(x=>x.))


            CreateMap<ColumnModel, ColumnDto>()
                .ForMember(x => x.Order, y => y.MapFrom(z => z.Index));

            //CreateMap<IEnumerable<ColumnModel>, IEnumerable<ColumnDto>>();


            // CreateMap<IEnumerable<Member>>

            CreateMap<UserModel, UserShortDto>();

            CreateMap<BoardModel, BoardDto>()
                .ForMember(x=> x.Members, opt=> opt.MapFrom(src=> src.Members.Select(x=>x.User)));


            CreateMap<TaskModel, TaskDetailedDto>()
                .ForMember(x => x.ColumnId, x => x.MapFrom(x => x.Column.Id))
                .ForMember(x => x.ResponsibleId, opt => opt.MapFrom(src => src.Responsible.Id));
            
            CreateMap<TaskModel, TaskShortDto>()
                .ForMember("ColumnId", opt => opt.MapFrom(src => src.Column.Id))
                .ForMember("ResponsibleId", opt => opt.MapFrom(src => src.Responsible.Id))
                .ForMember("CommentsCount", opt => opt.MapFrom(src => src.Comments.Count));

            //CreateMap<IEnumerable<TaskModel>, IEnumerable<TaskShortDto>>();
           
           CreateMap<CommentModel, CommentDto>()
                .ForMember(x=> x.UserId, opt => opt.MapFrom(src => src.User.Id));
            
            CreateMap<TaskModel, TaskDto>()
                .ForMember(x => x.CommentsCount, y => y.MapFrom(x => x.Comments.Count))
                .ForMember(x=> x.ResponsibleId, y=> y.MapFrom(x=> x.Responsible.Id));

            CreateMap<ColumnModel, FullColumnDto>()
                .ForMember(x => x.Order, y => y.MapFrom(z => z.Index));

            CreateMap<UpdateTaskRequest, TaskModel>();
        }
    }
}