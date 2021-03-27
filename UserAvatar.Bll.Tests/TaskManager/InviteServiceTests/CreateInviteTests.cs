using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager.InviteServiceTests
{
    public class InviteServiceTest
    {
        private readonly Mock<IInviteStorage> _inviteStorage;
        private readonly Mock<IUserStorage> _userStorage;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IBoardStorage> _boardStorage;
        private InviteService _service;

        public InviteServiceTest()
        {
            _inviteStorage = new Mock<IInviteStorage>();
            _mapper = new Mock<IMapper>();
            _userStorage = new Mock<IUserStorage>();
            _boardStorage = new Mock<IBoardStorage>();
        }

        private InviteService SetupInviteService()
        {
            return new(
                _inviteStorage.Object,
                _mapper.Object,
                _userStorage.Object,
                _boardStorage.Object);
        }

        private void SetupDependencies()
        {
            _boardStorage.Setup(x => x.GetBoardAsync(It.IsAny<int>()))
                .ReturnsAsync(new Board());
            _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new User());
            _userStorage.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
        }

        [Fact]
        public async Task CreateInvite_If_Payload_Null_Returns_NotFound()
        {
            //Arrange
            _service = SetupInviteService();

            //Act
            var resulted = await _service.CreateInviteAsync(It.IsAny<int>(), It.IsAny<int>(), null);

            //Assert
            resulted.Code.Should().Be(ResultCode.NotFound);
        }

        [Theory]
        [InlineData("payload1")]
        [InlineData("1")]
        [InlineData("Vabadama@gmail.com")]
        [InlineData("Ababagalamaga@ababagalamaga.com")]
        public async Task CreateInvite_If_Board_Not_Exists_Returns_NotFound(string payload)
        {
            //Arrange
            _boardStorage.Setup(x => x.GetBoardAsync(It.Is<int>(boardId => boardId > 10)))
                .ReturnsAsync((Board) null);
            _service = SetupInviteService();

            //Act
            var resultedError = 
                await _service.CreateInviteAsync(It.Is<int>(x=> x<10),It.IsAny<int>(), payload);

            //Assert
            resultedError.Code.Should().Be(ResultCode.NotFound);
        }
        
        [Theory]
        [InlineData("444")]
        [InlineData("1")]
        [InlineData("Vabadama@gmail.com")]
        [InlineData("Ababagalamaga@ababagalamaga.com")]
        public async Task CreateInvite_If_User_Not_Exists_Returns_NotFound(string payload)
        {
            //Arrange
            _boardStorage.Setup(x => x.GetBoardAsync(It.Is<int>(boardId => boardId > 10)))
                .ReturnsAsync(new Board());
            

            _userStorage.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User) null);
            _service = SetupInviteService();

            //Act
            var resultedError = 
                await _service.CreateInviteAsync(It.IsAny<int>(),It.IsAny<int>(), payload);

            //Assert
            resultedError.Code.Should().Be(ResultCode.NotFound);
        }

        [Fact]
        public async Task CreateInvite_If_Inviter_and_Invited_Equals_Returns_Forbidden()
        {
            var inviter = 1;
            var invited = "1";
            //Arrange
            SetupDependencies();
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            _service = SetupInviteService();
            
            //Act
            var result = await _service
                .CreateInviteAsync(It.IsAny<int>(), inviter, invited);
            
            //Assert
            result.Code.Should().Be(ResultCode.Forbidden);
        }

        [Theory]
        [InlineData("payload1")]
        [InlineData("1")]
        [InlineData("Vabadama@gmail.com")]
        [InlineData("Ababagalamaga@ababagalamaga.com")]
        public async Task CreateInvite_If_User_Not_Member_Returns_Forbidden(string payload)
        {
            //Arrange
            SetupDependencies();
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);
            _service = SetupInviteService();
            
            //Act
            var result = await _service.CreateInviteAsync(It.IsAny<int>(), It.IsAny<int>(), payload);
            
            //Assert
            result.Code.Should().Be(ResultCode.Forbidden);
        }
        
        [Theory]
        [InlineData("2")]
        [InlineData("1")]
        [InlineData("4")]
        [InlineData("7")]
        public async Task CreateInvite_If_User_Is_Member_Returns_Forbidden(string payload)
        {
            //Arrange
            SetupDependencies();
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _service = SetupInviteService();
            
            //Act
            var result = await _service.CreateInviteAsync(It.IsAny<int>(), It.IsAny<int>(), payload);
            
            //Assert
            result.Code.Should().Be(ResultCode.BadRequest);
        }

        [Fact]
        public async Task CreateInvite_Returns_Invite()
        {
            //Arrange
            SetupDependencies();
            var payload = "12";
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.Is<int>(inviterId=> inviterId<10), It.IsAny<int>()))
                .ReturnsAsync(true);
            _boardStorage.Setup(x => x.IsUserBoardAsync(It.Is<int>(invitedId=> invitedId>10), It.IsAny<int>()))
                .ReturnsAsync(false);
            _inviteStorage.Setup(x => x.GetInviteByBoardAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Invite) null);
            
            _service = SetupInviteService();

            //Act
            var result = await _service.CreateInviteAsync(It.IsAny<int>(), It.IsAny<int>(), payload);
            
            //Assert
            result.Code.Should().Be(ResultCode.Success);
            result.EventType.Should().Be(EventType.SendInvite);
            
        }
    }
}