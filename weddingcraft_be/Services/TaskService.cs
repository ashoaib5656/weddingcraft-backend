using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepo;

        public TaskService(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _taskRepo.GetAllAsync();
        }

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            await _taskRepo.AddAsync(task);
            await _taskRepo.SaveChangesAsync();
            return task;
        }

        public async Task UpdateAsync(int id, TaskItem task)
        {
            var existing = await _taskRepo.GetQueryable().FirstOrDefaultAsync(t => t.Id == id);
            if (existing != null)
            {
                existing.Title = task.Title;
                existing.AssignedTo = task.AssignedTo;
                existing.DueDate = task.DueDate;
                existing.Priority = task.Priority;
                existing.Status = task.Status;
                await _taskRepo.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _taskRepo.GetQueryable().FirstOrDefaultAsync(t => t.Id == id);
            if (task != null)
            {
                _taskRepo.Remove(task);
                await _taskRepo.SaveChangesAsync();
            }
        }
    }
}
