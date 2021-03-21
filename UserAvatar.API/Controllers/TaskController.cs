using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAvatar.API.Contracts.Dtos;
using UserAvatar.BLL.Models;
using UserAvatar.BLL.Services.Interfaces;

namespace UserAvatar.API.Controllers
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

            return Ok(_mapper.Map<TaskModel, TaskDetailedDto>(task));
        }
    }
}