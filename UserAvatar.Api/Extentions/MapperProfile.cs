using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using UserAvatar.Api.Contracts;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Dal.Entities;
using UserAvatar.Api.Contracts.ViewModels;

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
            CreateMap<Card, CardModel>();
            CreateMap<Comment, CommentModel>();
            
            CreateMap<UserModel, User>();
            CreateMap<MemberModel, Member>();
           
            CreateMap<ColumnModel, Column>();
            CreateMap<EventModel, Event>();
            CreateMap<HistoryModel, History>();
            CreateMap<InviteModel, Invite>();
            CreateMap<RankModel, Rank>();
            CreateMap<CardModel, Card>();
            CreateMap<CommentModel, Comment>();


            CreateMap<BoardModel, BoardShortVm>();

            CreateMap<Board, BoardModel>();
            CreateMap<UserModel, UserShortVm>();
            // .ForMember(x=> x.Rank, x=>x.MapFrom(x=>x.))


            CreateMap<ColumnModel, ColumnVm>()
                .ForMember(x => x.Order, y => y.MapFrom(z => z.Index));

            //CreateMap<IEnumerable<ColumnModel>, IEnumerable<ColumnDto>>();


            // CreateMap<IEnumerable<Member>>

            CreateMap<UserModel, UserShortVm>();

            CreateMap<BoardModel, BoardVm>()
                .ForMember(x=> x.Members, opt=> opt.MapFrom(src=> src.Members.Select(x=>x.User)));


            CreateMap<CardModel, CardDetailedVm>()
                .ForMember(x => x.ColumnId, x => x.MapFrom(x => x.Column.Id))
                .ForMember(x => x.ResponsibleId, opt => opt.MapFrom(src => src.Responsible.Id));
            
            CreateMap<CardModel, CardShortVm>()
                .ForMember(x=> x.ColumnId, opt => opt.MapFrom(src => src.Column.Id))
                .ForMember(x=> x.ResponsibleId, opt => opt.MapFrom(src => src.Responsible.Id))
                .ForMember(x=> x.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count));

            //CreateMap<IEnumerable<CardModel>, IEnumerable<CardShortDto>>();
           
           CreateMap<CommentModel, CommentVm>()
                .ForMember(x=> x.UserId, opt => opt.MapFrom(src => src.User.Id));
            
            CreateMap<CardModel, CardVm>()
                .ForMember(x => x.CommentsCount, y => y.MapFrom(x => x.Comments.Count))
                .ForMember(x=> x.ResponsibleId, y=> y.MapFrom(x=> x.Responsible.Id));

            CreateMap<ColumnModel, FullColumnVm>()
                .ForMember(x => x.Order, y => y.MapFrom(z => z.Index));

            CreateMap<UpdateCardDto, CardModel>();
        }
    }
}