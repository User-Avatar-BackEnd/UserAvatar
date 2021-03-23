using System.Threading.Tasks;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface ICardService
    {
        Task<CardModel> GetByIdAsync(int cardId, int userId);

        Task<CardModel> CreateCardAsync(string title, int columnId, int userId);

        Task UpdateCardAsync(CardModel cardModel, int columnId, int? responsibleId, int userId);

        Task DeleteCardAsync(int cardId, int userId);
    }
}
