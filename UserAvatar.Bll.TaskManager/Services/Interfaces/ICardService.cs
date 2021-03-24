using System.Threading.Tasks;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface ICardService
    {
        Task<Result<CardModel>> GetByIdAsync(int boardId, int cardId, int userId);

        Task<Result<CardModel>> CreateCardAsync(string title, int boardId, int columnId, int userId);

        Task<int> DeleteCardAsync(int boardId, int cardId, int userId);

        Task<int> UpdateCardAsync(CardModel cardModel, int boardId, int userId);
    }
}