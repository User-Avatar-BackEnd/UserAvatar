using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.TaskManager.Models;
using UserAvatar.Bll.TaskManager.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.TaskManager.Services
{
    public class ColumnService : IColumnService
    {
        private readonly IColumnStorage _columnStorage;
        private readonly IMapper _mapper;
        private readonly IBoardStorage _boardStorage;
        public ColumnService(IColumnStorage columnStorage, IMapper mapper, IBoardStorage boardStorage)
        {
            _columnStorage = columnStorage;
            _mapper = mapper;
            _boardStorage = boardStorage;
        }

       //todo: add userId

        public async Task<ColumnModel> CreateAsync(int userId, int boardId, string title)
        {
            if (! await _boardStorage.IsUserBoardAsync(userId, boardId))
                throw new Exception($"This user {userId} does not have this board");
            var newColumn = new Column
            {
                Title = title,
                BoardId = boardId,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };
            
            var column = await _columnStorage.CreateAsync(newColumn);
            try
            {
                var mapped = _mapper.Map<Column, ColumnModel>(column);
                return mapped;
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
            }
            
            return null;
        }

        public async Task ChangePositionAsync(int userId, int columnId, int positionIndex)
        {
            // IsUserInBoardByColumnId maybe may be async

            if (!_columnStorage.IsUserInBoardByColumnId(userId,columnId))
                throw new Exception($"This user {userId} does not have this board");
            await _columnStorage.ChangePositionAsync(columnId,positionIndex);
        }

        public async Task DeleteAsync(int userId, int columnId)
        {
            // IsUserInBoardByColumnId maybe may be async

            if (!_columnStorage.IsUserInBoardByColumnId(userId,columnId))
                throw new Exception($"This user {userId} does not have this board");
            await _columnStorage.DeleteApparentAsync(columnId);
        }
        
        public async Task UpdateAsync(int userId, int columnId, string title)
        {
            var thisColumn = await _columnStorage.GetColumnByIdAsync(columnId);
            if (thisColumn is null)
                throw new Exception($"Found column with id {columnId} is not found!");
            thisColumn.Title = title;
            await _columnStorage.UpdateAsync(thisColumn);
        }
        
        public async Task<ColumnModel> GetColumnByIdAsync(int userId, int columnId)
        {
            var foundColumn = await _columnStorage.GetColumnByIdAsync(columnId);
            if (foundColumn is null)
                throw new Exception($"Found column with id {columnId} is not found!");
            return _mapper.Map<Column, ColumnModel>(foundColumn);
        }

        public async Task<List<ColumnModel>> GetAllColumnsAsync(int userId, int boardId)
        {
            if (! await _boardStorage.IsUserBoardAsync(userId, boardId))
                throw new Exception($"This user {userId} does not have this board");
            var allColumns = await _columnStorage.GetAllColumnsAsync(boardId);
            if (allColumns.Count() < 0)
                throw new Exception($"No Columns in this board {boardId}");
            return _mapper.Map<List<Column>, List<ColumnModel>>(allColumns.ToList());
        }
    }
}