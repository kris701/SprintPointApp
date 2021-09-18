using ClickupInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickupInterface
{
    public interface IClickupInterface
    {
        public string TeamID { get; }
        public string SpaceID { get; }

        public string EpicFolderID { get; }
        public string PBIFolderID { get; }
        public string TasksFolderID { get; }
        public string SprintFolderID { get; }

        public string EpicListID { get; }
        public string PBIListID { get; }
        public string TasksListID { get; }
        public string SprintListID { get; }

        public List<SprintModel> Sprints { get; }

        public List<TaskItem> EpicTasks { get; }
        public List<TaskItem> PBITasks { get; }
        public List<TaskItem> TaskTasks { get; }

        public string Token { get; }
        public string BaseURL { get; }

        public Task<string> GetTeamID();
        public Task<string> GetDeveloperSpace();

        public Task<string> GetEpicFolder();
        public Task<string> GetPBIFolder();
        public Task<string> GetTasksFolder();
        public Task<string> GetSprintFolder();

        public Task<string> GetEpicList();
        public Task<string> GetPBIList();
        public Task<string> GetTasksList();
        public Task<List<SprintModel>> GetSprints();

        public Task<List<TaskItem>> GetEpicTasks();
        public Task<List<TaskItem>> GetPBITasks();
        public Task<List<TaskItem>> GetTaskTasks();

        public Task BindSprints(List<TaskItem> tasks);
    }
}
