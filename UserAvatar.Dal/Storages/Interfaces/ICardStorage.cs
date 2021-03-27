using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.Dal.Storages.Interfaces
{
    public interface ICardStorage
    {
        Task<Card> GetByIdAsync(int id);

        Task<Card> CreateAsync(Card card);

        Task<int> GetCardsCountInColumnAsync(int columnId);

        Task<int> GetBoardIdAsync(int cardId);

        Task DeleteAsync(int cardId);

        Task UpdateAsync(Card card);

        Task<int> GetCardIdByColumnIdAsync(int columnId);
        
        Task<bool> IsCardCommentAsync(int cardId, int commentId);
    }
}