using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;

namespace UserAvatar.Bll.Gamification.Services.Interfaces;

public interface ISearchService
{
    Task<PagedUsersModel> GetAllUsersAsync(int pageNumber, int pageSize, string query);
}
