using System;
namespace UserAvatar.Bll.TaskManager.Services.Interfaces
{
    public interface IBoardChangesService
    {
        void DoChange(int boardId, int userId);

        bool HasChanges(int boardId, int userId, long ticks);
    }
}
