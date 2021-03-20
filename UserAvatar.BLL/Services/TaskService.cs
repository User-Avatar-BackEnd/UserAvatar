using System;
using AutoMapper;
using UserAvatar.BLL.Models;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages;

namespace UserAvatar.BLL.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskStorage _taskStorage;
        private readonly IMapper _mapper;

        public TaskService(ITaskStorage taskStorage)
        {
            _taskStorage = taskStorage;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Task, TaskModel>()).CreateMapper();
        }

        public TaskModel GetById(int taskId, int userId)
        {
            if (false) return null;//isUserBoard

            var task = _taskStorage.GetById(taskId);

            if (task == null) return null;

            return _mapper.Map<Task, TaskModel>(task);
        }
    }
}
