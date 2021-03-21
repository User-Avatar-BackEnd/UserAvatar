using System;
using AutoMapper;
using UserAvatar.BLL.Models;
using UserAvatar.BLL.Services.Interfaces;
using UserAvatar.DAL.Entities;
using UserAvatar.DAL.Storages;
using UserAvatar.DAL.Storages.Interfaces;

namespace UserAvatar.BLL.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskStorage _taskStorage;
        private readonly IMapper _mapper;

        public TaskService(ITaskStorage taskStorage, IMapper mapper)
        {
            _taskStorage = taskStorage;
            _mapper = mapper;
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
