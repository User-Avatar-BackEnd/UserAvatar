using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.Api.Contracts.Dtos;
using UserAvatar.Api.Contracts.Requests;
using UserAvatar.API.Contracts.Dtos;
using UserAvatar.Bll.Models;
using UserAvatar.Bll.Services.Interfaces;

namespace UserAvatar.Api.Controllers
{
    [ApiController]
    [Route("api/v1/Task")]
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

            taskDto.Comments.ForEach(x => x.Editable = x.UserId == userId);

            return Ok(taskDto);
        }

        [HttpPost]
        public IActionResult AddTask(AddTaskRequest request)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            var task = _taskService.CreateTask(request.Title, request.ColumnId, userId);

            var taskDto = _mapper.Map<TaskModel, TaskShortDto>(task);

            return Ok(taskDto);
        }

        [HttpPatch]
        public IActionResult UpdateTask(UpdateTaskRequest request)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            var taskModel = _mapper.Map<UpdateTaskRequest, TaskModel>(request);

            _taskService.UpdateTask(taskModel, request.ColumnId, request.ResponsibleId, userId);

            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteTask(int id)
        {
            var userCredentials = HttpContext.User.Claims.First(claim => claim.Type == "id");
            var userId = Convert.ToInt32(userCredentials.Value);

            _taskService.DeleteTask(userId, id);

            return Ok();
        }
    }
}