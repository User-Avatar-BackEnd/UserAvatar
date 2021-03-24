using System.Collections.Generic;
using System.Threading.Tasks;
using UserAvatar.Bll.Infrastructure;
using UserAvatar.Bll.TaskManager.Models;

namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface IInviteService
    {
        Task<int> CreateInviteAsync(int boardId, int userId, string payload);
        Task<int> UpdateInviteAsync(int inviteId, int userId, int statusCode);
        Task<Result<List<UserModel>>> FindByQuery(int boardId, int userId, string query);
        Task<Result<List<InviteModel>>> GetAllInvitesAsync(int userId);
    }
}