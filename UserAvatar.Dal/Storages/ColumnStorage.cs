using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAvatar.Dal.Context;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages.Interfaces;
using Z.EntityFramework.Plus;

namespace UserAvatar.Dal.Storages
{
    public class ColumnStorage : IColumnStorage
    {
        private readonly UserAvatarContext _userAvatarContext;
        public ColumnStorage(UserAvatarContext userAvatarContext)
        {
           _userAvatarContext = userAvatarContext;
        }
        public async Task<int> CountColumnsInBoardAsync(int boardId)
        {
            return await _userAvatarContext.Columns
                .CountAsync(x => x.BoardId == boardId);
        }
        public async Task AddColumnAsync(Column column)
        {
            await _userAvatarContext.Columns.AddAsync(column);
            await  _userAvatarContext.SaveChangesAsync();
        }
        public async Task DeleteApparentAsync(Column column)
        {
            await _userAvatarContext.Columns.Where(x => x.Id == column.Id)
                .Include(x => x.Cards)
                .ThenInclude(x => x.Comments).SelectMany(x => x.Cards.SelectMany(card => card.Comments))
                .UpdateAsync(x => new Comment {IsDeleted = true});
            
            await _userAvatarContext.Cards.Where(x=> x.ColumnId == column.Id)
                .UpdateAsync(x => new Card {IsDeleted = true});
            
            column.IsDeleted = true;
            
            await _userAvatarContext.SaveChangesAsync();
        }
        public async Task<List<int>> GetAllColumnsAsync(int boardId)
        {
            return await Task.FromResult(_userAvatarContext.Columns
                .Where(x => x.BoardId == boardId)
                .Select(x => x.Id).ToList());
        }
        public async Task<int> GetColumnsCountInBoardAsync(int boardId)
        {
            return await _userAvatarContext.Columns
                .CountAsync(x => x.BoardId == boardId);
        }
        public async Task UpdateAsync(Column column)
        {
            _userAvatarContext.Entry(column).State = EntityState.Modified;
            await _userAvatarContext.SaveChangesAsync();
        }
        public async Task<List<Column>> InternalGetAllColumns(Column column)
        {
            return await _userAvatarContext.Columns
                .Where(x => x.BoardId == column.BoardId).ToListAsync();
        }
        public async Task<List<Column>> GetAllColumnsExceptThis(Column thisColumn)
        {
            return await _userAvatarContext.Columns
                .Where(x => x.BoardId == thisColumn.BoardId && x.Id != thisColumn.Id)
                .ToListAsync();
        }
        public async Task Update()
        {
            await _userAvatarContext.SaveChangesAsync();
        }
        public async Task<Column> GetColumnByIdAsync(int id)
        {
            return await _userAvatarContext.Columns.FindAsync(id);
        }
    }
}  