using System;
using System.Collections.Generic;
using AutoMapper;
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;
using UserAvatar.Dal.Entities;
using UserAvatar.Dal.Storages;
using UserAvatar.Dal.Storages.Interfaces;

namespace UserAvatar.Bll.Services
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

        public TaskModel Test()
        {
            var task = _taskStorage.GetById(2);
            var test = _mapper.Map<Task, TaskModel>(task);
            return test;
        }
    }
}
