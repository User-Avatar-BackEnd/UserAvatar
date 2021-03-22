using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Services
{
    public class ColumnService : IColumnService
    {
        private readonly IColumnStorage _columnStorage;
        private readonly IMapper _mapper;
        private IBoardService _boardService;
        public ColumnService(IColumnStorage columnStorage, IMapper mapper, IBoardService boardService)
        {
            _columnStorage = columnStorage;
            _mapper = mapper;
            _boardService = boardService;
        }

       //todo: add userId

        public async System.Threading.Tasks.Task<ColumnModel> Create(int boardId, string title)
        {
            var newColumn = new Column
            {
                Title = title,
                BoardId = boardId,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };
            
            var column = await _columnStorage.Create(newColumn);
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
        

        public async System.Threading.Tasks.Task ChangePosition(int columnId, int positionIndex)
        {
            await _columnStorage.ChangePosition(columnId,positionIndex);
        }

        public async System.Threading.Tasks.Task Delete(int columnId)
        {
            await _columnStorage.DeleteApparent(columnId);
        }
        
        public async System.Threading.Tasks.Task Update(int columnId, string title)
        {
            var thisColumn = await _columnStorage.GetColumnById(columnId);
            if (thisColumn is null)
                throw new Exception($"Found column with id {columnId} is not found!");
            thisColumn.Title = title;
            await _columnStorage.Update(thisColumn);
        }
        
        public async System.Threading.Tasks.Task<ColumnModel> GetColumnById(int columnId)
        {
            var foundColumn = await _columnStorage.GetColumnById(columnId);
            if (foundColumn is null)
                throw new Exception($"Found column with id {columnId} is not found!");
            return _mapper.Map<Column, ColumnModel>(foundColumn);
        }

        public async Task<List<ColumnModel>> GetAllColumns(int boardId)
        {
            var allColumns = await _columnStorage.GetAllColumns(boardId);
            if (allColumns.Count() < 0)
                throw new Exception($"No Columns in this board {boardId}");
            return _mapper.Map<List<Column>, List<ColumnModel>>(allColumns.ToList());

        }
    }
}