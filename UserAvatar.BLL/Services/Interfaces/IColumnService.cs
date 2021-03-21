namespace UserAvatar.Bll.Services.Interfaces
{
    public interface IColumnService
    {
        void Create(int boardId, string title);
        void ChangePosition(int columnId, int positionIndex);
        void Delete(int columnId);
    }
}