using System.Threading.Tasks;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface ICardService
    {
        Task<CardModel> GetByIdAsync(int cardId, int userId);

        CardModel CreateCard(string title, int columnId, int userId);

        Task UpdateCardAsync(CardModel cardModel, int columnId, int? responsibleId, int userId);

        Task DeleteCard(int cardId, int userId);
    }
}
