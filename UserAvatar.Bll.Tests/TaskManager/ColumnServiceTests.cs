using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Options;
using UserAvatar.Bll.TaskManager.Services;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Storages;
using UserAvatar.Dal.Storages.Interfaces;
using FluentAssertions;
using Xunit;

namespace UserAvatar.Bll.Tests.TaskManager
{
    public class ColumnServiceTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IColumnStorage> _columnStorage;
        private readonly Mock<IBoardStorage> _boardStorage;
        private readonly Mock<IBoardChangesService> _boardChangesService;
        private readonly Mock<IOptions<LimitationOptions>> _limitations;

        public ColumnServiceTests()
        {
            _mapper = new Mock<IMapper>();
            _columnStorage = new Mock<IColumnStorage>();
            _boardStorage = new Mock<IBoardStorage>();
            _boardChangesService = new Mock<IBoardChangesService>();
            _limitations = new Mock<IOptions<LimitationOptions>>();
        }
        [Fact]
        public async Task Get()
        {
            var service = new ColumnService(
                _columnStorage.Object, 
                _mapper.Object, 
                _boardStorage.Object, 
                _limitations.Object, 
                _boardChangesService.Object);

            

            var result = await service.CreateAsync(0, 0, string.Empty);
            
            //result.Should;
        }
    }
}