using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages.Interfaces;

namespace UserAvatar.DAL.Storages
{
    public class BoardStorage : IBoardStorage
    {
        private readonly UserAvatarContext _dbContext;

        public BoardStorage(UserAvatarContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public void Create(Board board)
        {
            _dbContext.Boards.Add(board);
            _dbContext.SaveChanges();
        }

        public List<Board> ListBoardsById(int id)
        {
            var result = _dbContext.Boards
                .Include(x => x.Members)
                .Where(x => x.OwnerId == id || x.Members.Any(y => y.UserId == id)).ToList();
                
            return result;
        }

        public Board GetBoardById(int id)
        {
            return _dbContext.Boards.FirstOrDefault(x => x.Id == id);
        }

        public bool IsBoardExists(int id)
        {
            return _dbContext.Boards.Any(x => x.Id == id);
        }

        public bool DoesUserHasBoards(int id)
        {
            return _dbContext.Members.Any(x => x.Id == id) || _dbContext.Boards.Any(x=> x.OwnerId == id);
        }
    }
}