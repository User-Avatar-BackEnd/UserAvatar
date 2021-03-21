using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.API.Contracts.Requests;
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/task")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            var task = _taskService.GetById(id, userId);
            if (task == null) BadRequest();

            var taskDto = _mapper.Map<TaskModel, TaskDetailedDto>(task);

            if (taskDto.Comments == null)
            {
                taskDto.Comments = new List<CommentDto>();
            }
            taskDto.Comments.ForEach(x => x.Editable = x.UserId == userId);

            return Ok(_mapper.Map<TaskModel, TaskDetailedDto>(task));
        }

        [HttpPost]
        public IActionResult AddTask(AddTaskRequest addTaskRequest)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            return Ok();
        }
    }
}