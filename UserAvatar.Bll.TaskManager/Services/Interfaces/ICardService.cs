using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface ICardService
    {
        public CardModel GetById(int cardId, int userId);

        public CardModel CreateCard(string title, int columnId, int userId);

        public void UpdateCard(CardModel card, int columnId, int? responsibleId, int userId);

        public void DeleteCard(int taskId, int userId);
    }
}
