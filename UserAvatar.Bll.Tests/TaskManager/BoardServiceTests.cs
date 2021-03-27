using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Api.Extentions;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager
{
    public class BoardServiceTests
    {
        private readonly Mock<IBoardStorage> _boardStorage;

        private readonly IMapper _mapper;
        private readonly IOptions<LimitationOptions> _limitations;

        private readonly Mock<IBoardChangesService> _boardChangesService;

        public BoardServiceTests()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }

            _limitations = Options.Create(new LimitationOptions());


            _boardStorage = new Mock<IBoardStorage>();
            _boardChangesService = new Mock<IBoardChangesService>();
        }

        #region GetAllBoards test cases

        [Fact]
        public async Task GetAllBoards_If_User_Has_No_Boards_Returns_Empty_List()
        {
             _boardStorage.Setup(x=>x.GetAllBoardsAsync(It.IsAny<int>())).ReturnsAsync(new List<Board>());

            var boardService = SetUpService();

            var result = await boardService.GetAllBoardsAsync(It.IsAny<int>());

            result.Value.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetAllBoards_If_User_Has_Boards_Returns_Boards_List()
        {
            _boardStorage.Setup(x => x.GetAllBoardsAsync(It.IsAny<int>())).ReturnsAsync(
                new List<Board>() {
                    new Board(),
                    new Board(),
                    new Board()
            });

            var boardService = SetUpService();

            var result = await boardService.GetAllBoardsAsync(It.IsAny<int>());

            result.Value.Count().Should().Be(3);
        }

        #endregion

        #region CreateBoard test cases

        [Fact]
        public async Task CreateBoard_If_Max_Board_Count_Is_Reached_Returns_ResultCode_MaxBoardCount()
        {
            _limitations.Value.MaxBoardCount = 10;
            _boardStorage.Setup(x => x.CountAllBoardsAsync(It.IsAny<int>())).ReturnsAsync(_limitations.Value.MaxBoardCount);
            
            var boardService = SetUpService();

            var result = await boardService.CreateBoardAsync(It.IsAny<int>(), It.IsAny<string>());

            result.Code.Should().Be(ResultCode.MaxBoardCount);
        }

        [Fact]
        public async Task CreateBoard_If_Board_Can_Be_Created_Returns_ResultCode_Success()
        {
            _limitations.Value.MaxBoardCount = 10;
            _boardStorage.Setup(x => x.CountAllBoardsAsync(It.IsAny<int>())).ReturnsAsync(It.Is<int>(x => x < _limitations.Value.MaxBoardCount));

            var boardService = SetUpService();

            var result = await boardService.CreateBoardAsync(It.IsAny<int>(), It.IsAny<string>());

            result.Code.Should().Be(ResultCode.Success);
            result.EventType.Should().Be(EventType.CreateBoard);
        }

        #endregion

        #region GetBoard test cases

        [Fact]
        public async Task GetBoard_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
        {
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync((Board)null);

            var boardService = SetUpService();

            var result = await boardService.GetBoardAsync(It.IsAny<int>(), It.IsAny<int>());
            
            result.Code.Should().Be(404);
        }

        [Fact]
        public async Task GetBoard_If_User_Does_Not_Relate_To_Board_Returns_ResultCode_Forbidden()
        {
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board());
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            var boardService = SetUpService();

            var result = await boardService.GetBoardAsync(It.IsAny<int>(), It.IsAny<int>());

            result.Code.Should().Be(403);
        }

        [Fact]
        public async Task GetBoard_If_User_Relate_To_Board_Returns_ResultCode_Success()
        {
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board());
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var boardService = SetUpService();

            var result = await boardService.GetBoardAsync(It.IsAny<int>(), It.IsAny<int>());

            result.Code.Should().Be(200);
        }
     
        #endregion

        #region RenameBoard test cases

        [Fact]
        public async Task RenameBoard_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
        {
           _boardStorage.Setup(x=> x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync((Board) null);

            var boardService = SetUpService();

            var result =await  boardService.RenameBoardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            result.Should().Be(404);
        }

        [Fact]
        public async Task RenameBoard_If_Is_Not_Owner_Returns_ResultCode_Forbidden()
        {
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board());
            _boardStorage.Setup(x => x.IsOwnerBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            var boardService = SetUpService();

            var result = await boardService.RenameBoardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            result.Should().Be(403);
        }

        [Fact]
        public async Task RenameBoard_If_Board_Successfully_Remaned_Returns_ResultCode_Success()
        {
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board());
            _boardStorage.Setup(x => x.IsOwnerBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var boardService = SetUpService();

            var result = await boardService.RenameBoardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            result.Should().Be(200);
        }

        #endregion

        #region DeleteBoard test cases

        [Fact]
        public async Task DeleteBoard_If_Board_Does_Not_Exist_Returns_ResultCode_NotFound()
        {
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>())).ReturnsAsync(false);

            var boardService = SetUpService();

            var result = await boardService.DeleteBoardAsync(It.IsAny<int>(), It.IsAny<int>());

            result.Should().Be(404);
        }

        [Fact]
        public async Task DeleteBoard_If_Is_Not_Member_Or_User_Returns_ResultCode_Forbidden()
        { 
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _boardStorage.Setup(x => x.IsOwnerBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            var boardService = SetUpService();

            var result = await boardService.DeleteBoardAsync(It.IsAny<int>(), It.IsAny<int>());

            result.Should().Be(403);
        }

        [Fact]
        public async Task DeleteBoard_If_Is_Member_Or_User_Returns_ResultCode_Succcess()
        {
            _boardStorage.Setup(x => x.IsBoardExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _boardStorage.Setup(x => x.IsOwnerBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var boardService = SetUpService();

            var result = await boardService.DeleteBoardAsync(It.IsAny<int>(), It.IsAny<int>());

            result.Should().Be(200);
        }

        #endregion

        #region IsUserBoard test cases

        [Fact]
        public async Task IsUserBoard_If_Users_Board_Returns_True()
        {
          _boardStorage.Setup(method => method.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var boardService = SetUpService();

            var result = await boardService.IsUserBoardAsync(4, 2);

            result.Should().Be(true);
        }

        [Fact]
        public async Task IsUserBoard_If_Not_Users_Board_Returns_False()
        {
            var userId = 1;
            var boardId = 1;

            _boardStorage.Setup(method => method.IsUserBoardAsync(userId, boardId)).ReturnsAsync(true);

            var boardService = SetUpService();

            var result1 = await boardService.IsUserBoardAsync(3, boardId);
            var result2 = await boardService.IsUserBoardAsync(userId, 3);
            var result3 = await boardService.IsUserBoardAsync(3, 3);

            result1.Should().Be(false);
            result2.Should().Be(false);
            result3.Should().Be(false);
        }

        #endregion

        #region DeleteMemberFromBoard

        [Fact]
        public async Task DeleteMemberFromBoard_If_User_Equals_ToDeleteUser_Returns_ResultCode_Forbidden()
        {
            var userId = 1;

            var boardService = SetUpService();

            var result = await boardService.DeleteMemberFromBoardAsync(userId, userId, It.IsAny<int>());

            result.Should().Be(403);
        }

        [Fact]
        public async Task DeleteMemberFromBoard_If_Board_Does_Not_Exit_Returns_ResultCode_NotFound()
        {
            var userId = 1;

            _boardStorage.Setup(method => method.GetBoardAsync(It.IsAny<int>())).ReturnsAsync((Board)null);

            var boardService = SetUpService();

            var result = await boardService.DeleteMemberFromBoardAsync(userId, It.Is<int>(toDeleteUserId => toDeleteUserId != userId), It.IsAny<int>());

            result.Should().Be(404);
        }

        [Fact]
        public async Task DeleteMemberFromBoard_If_User_Is_Not_Owner_Returns_ResultCode_Forbidden()
        {
            var userId = 1;

            _boardStorage.Setup(method => method.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board { OwnerId = It.Is<int>(ownerId => ownerId != userId) });

            var boardService = SetUpService();

            var result = await boardService.DeleteMemberFromBoardAsync(userId, It.Is<int>(toDeleteUserId => toDeleteUserId != userId), It.IsAny<int>());

            result.Should().Be(403);
        }

        [Fact]
        public async Task DeleteMemberFromBoard_If_User_Is_Not_Board_Member_Returns_ResultCode_NotFound()
        {
            var userId = 1;
            var deletedId = 2;

            _boardStorage.Setup(method => method.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board
            {
                OwnerId = userId,
                Members = new List<Member>(){
                new Member { UserId = It.Is<int>(x=> x != deletedId)}
                }
            });

            var boardService = SetUpService();

            var result = await boardService.DeleteMemberFromBoardAsync(userId, deletedId, It.IsAny<int>());

            result.Should().Be(404);
        }

        [Fact]
        public async Task DeleteMemberFromBoard_If_User_Is_Board_Member_Returns_ResultCode_Success()
        {
            var userId = 1;
            var deletedId = 2;
            var boardId = 3;

            _boardStorage.Setup(method => method.GetBoardAsync(It.IsAny<int>())).ReturnsAsync(new Board
            {
                OwnerId = userId,
                Members = new List<Member>(){
                new Member { UserId = deletedId}
                }
            });

            _boardStorage.Setup(x=> x.GetMemberByIdAsync(deletedId, boardId)).ReturnsAsync( new Member());

            var boardService = SetUpService();

            var result = await boardService.DeleteMemberFromBoardAsync(userId, deletedId, boardId);

            result.Should().Be(200);
        }

        #endregion

        private BoardService SetUpService()
        {
            return new BoardService(
                _boardStorage.Object, 
                _mapper, 
                _limitations, 
                _boardChangesService.Object);
        }
    }
}
