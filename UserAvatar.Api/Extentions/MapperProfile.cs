using AutoMapper;
using System.Linq;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Dal.Entities;
using UserAvatar.Api.Contracts.ViewModels;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Api.Contracts.Dtos;
namespace UserAvatar.Api.Extentions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<Member, MemberModel>();

            CreateMap<MemberModel, UserShortVm>()
                .ForMember(x => x.Login, y => y.MapFrom(z => z.User.Login))
                .ForMember(x=> x.Id, y=> y.MapFrom(z=> z.User.Id))
                .ForMember(x=> x.Rank,y=> y.MapFrom(z=> z.Rank))
                .ReverseMap();
            

            CreateMap<Column, ColumnModel>();
            CreateMap<Event, EventModel>();
            CreateMap<History, HistoryModel>();
            CreateMap<Invite, InviteModel>();
            CreateMap<Card, CardModel>();

            CreateMap<Comment, CommentModel>();
            CreateMap<History, HistoryModel>();
            
            CreateMap<UserModel, User>();
            CreateMap<MemberModel, Member>();
            
            CreateMap<MemberModel, UserWithRankModel>()
                .ForMember(x=> x.Login,y=> y.MapFrom(z=> z.User.Login))
                .ForMember(x=> x.UserId, y=> y.MapFrom(z=> z.User.Id))
                .ForMember(x=> x.Role, y=> y.MapFrom(z=> z.User.Role))
                .ForMember(x=> x.Score,y=> y.MapFrom(z=> z.User.Score))
                .ForMember(x=> x.Rank, y=> y.MapFrom(z=> z.User.Role))
                .ReverseMap();
            
            /*CreateMap<UserWithRankModel, MemberModel>()
                .ForMember(x=> x.User.Id, y=> y.MapFrom(z=> z.UserId))
                .ForMember(x=> x.User.Login, y=> y.MapFrom(z=> z.Login))
                .ForMember(x=> x.User.Role,y=> y.MapFrom(z=> z.Role))
                .ForMember(x=> x.User.Score,y=> y.MapFrom(z=> z.Score));*/
            
            CreateMap<HistoryModel, HistoryVm>();

            CreateMap<InviteModel, Invite>();
            CreateMap<CardModel, Card>();
            CreateMap<CommentModel, Comment>();
            CreateMap<EventModel, Event>();

            CreateMap<EventDto, EventModel>();

            CreateMap<BoardModel, BoardShortVm>();

            CreateMap<Board, BoardModel>();

            CreateMap<UserModel, UserShortVm>();

            CreateMap<ColumnModel, ColumnVm>()
                .ForMember(x => x.Order, y => y.MapFrom(z => z.Index));

            CreateMap<UserModel, UserShortVm>();

            CreateMap<BoardModel, BoardVm>();
                /*.ForMember(x => x.Members, opt => opt.MapFrom(src => src.Members.Select(x => x.User)));*/

            CreateMap<CardModel, CardDetailedVm>();

            CreateMap<CardModel, CardShortVm>()
                .ForMember(x => x.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count));

            CreateMap<CommentModel, CommentVm>()
                .ForMember(x => x.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<CardModel, CardVm>()
                .ForMember(x => x.CommentsCount, y => y.MapFrom(x => x.Comments.Count));

            CreateMap<ColumnModel, FullColumnVm>()
                .ForMember(x => x.Order, y => y.MapFrom(z => z.Index));

            CreateMap<UpdateCardDto, CardModel>();

            CreateMap<Invite, InviteModel>()
                .ForMember(x => x.Inviter,
                    y => y.MapFrom(z => z.Inviter))
                .ForMember(x => x.InvitedId,
                    y => y.MapFrom(z => z.Invited.Id));

            CreateMap<InviteModel, InviteVm>()
                .ForMember(x => x.Board, y => y.MapFrom(z => z.Board));
            CreateMap<EventModel, EventVm>();

            CreateMap<User, RateModel>();

            CreateMap<UpdateCardDto, CardModel>();

            CreateMap<Rank, RankDataModel>();

            CreateMap<RateModel, RateDataVm>();

            CreateMap<FullRateModel, FullRateVm>();
        }
    }
}