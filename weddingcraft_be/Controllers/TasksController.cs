using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var pagedTasks = await _taskService.GetAllAsync(filter);
            return Ok(pagedTasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskItem task)
        {
            var created = await _taskService.CreateAsync(task);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskItem task)
        {
            await _taskService.UpdateAsync(id, task);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskService.DeleteAsync(id);
            return NoContent();
        }
    }
}