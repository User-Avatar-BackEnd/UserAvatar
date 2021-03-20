using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using UserAvatar.BLL.Models;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages;

namespace UserAvatar.BLL.Services
{
    public class BoardService : IBoardService
    {
        private readonly BoardStorage _boardStorage;
        private readonly IMapper _mapper;

        public BoardService(BoardStorage boardStorage)
        {
            _boardStorage = boardStorage;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Board, BoardModel>()).CreateMapper();
        }
        
        public List<BoardModel> GetAllBoardsById(int id)
        {
            if (!_boardStorage.DoesUserHasBoards(id))
                throw new Exception(); //todo: change into something else
            //var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Board, BoardsDto>()).CreateMapper();
            var boards = _boardStorage.ListBoardsById(id).ToList();
            
            var result = _mapper.Map<List<Board>, List<BoardModel>>(boards);
            //boards.ForEach(x => mapper.Map<Board, BoardsDto>(x));

            return result;
        }

        public bool CreateBoard(int id, string name)
        {
            try
            {
                _boardStorage.Create(new Board
                {
                    OwnerId = id,
                    Title = name,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now,
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }

            throw new NotImplementedException();
        }
    }
}