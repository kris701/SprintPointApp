using ClickupInterface.Helpers;
using ClickupInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickupInterface
{
    public class ClickupInterface
    {
        public string TeamID { get; internal set; }
        public string SpaceID { get; internal set; }

        public string EpicFolderID { get; internal set; }
        public string PBIFolderID { get; internal set; }
        public string TasksFolderID { get; internal set; }
        public string SprintFolderID { get; internal set; }

        public string EpicListID { get; internal set; }
        public string PBIListID { get; internal set; }
        public string TasksListID { get; internal set; }
        public string SprintListID { get; internal set; }

        public List<string> SprintIDs { get; internal set; }

        public List<TaskItem> EpicTasks { get; internal set; }
        public List<TaskItem> PBITasks { get; internal set; }
        public List<TaskItem> TaskTasks { get; internal set; }

        public string Token { get; set; }
        public string BaseURL { get; set; }
        private ClickupAPIHelper apiHelper;

        public ClickupInterface(string token, string baseURL)
        {
            Token = token;
            BaseURL = baseURL;
            apiHelper = new ClickupAPIHelper(token,baseURL);
        }

        #region Overall Calls
        public async Task<string> GetTeamID()
        {
            if (TeamID == null)
                TeamID = await apiHelper.GetTeamID();
            return TeamID;
        }

        public async Task<string> GetDeveloperSpace()
        {
            if (TeamID == null)
                TeamID = await GetTeamID();
            if (SpaceID == null)
                SpaceID = await apiHelper.GetSpaceByName(TeamID, "Software Development");
            return SpaceID;
        }
        #endregion

        #region Folder Calls
        public async Task<string> GetEpicFolder()
        {
            if (SpaceID == null)
                SpaceID = await GetDeveloperSpace();
            if (EpicFolderID == null)
                EpicFolderID = await apiHelper.GetFolderByName(SpaceID, "👑 Epics");
            return EpicFolderID;
        }

        public async Task<string> GetPBIFolder()
        {
            if (SpaceID == null)
                SpaceID = await GetDeveloperSpace();
            if (PBIFolderID == null)
                PBIFolderID = await apiHelper.GetFolderByName(SpaceID, "📰 Product Backlog");
            return PBIFolderID;
        }

        public async Task<string> GetTasksFolder()
        {
            if (SpaceID == null)
                SpaceID = await GetDeveloperSpace();
            if (TasksFolderID == null)
                TasksFolderID = await apiHelper.GetFolderByName(SpaceID, "✔ Tasks");
            return TasksFolderID;
        }

        public async Task<string> GetSprintFolder()
        {
            if (SpaceID == null)
                SpaceID = await GetDeveloperSpace();
            if (SprintFolderID == null)
                SprintFolderID = await apiHelper.GetFolderByName(SpaceID, "Sprint");
            return SprintFolderID;
        }

        #endregion

        #region List Calls

        public async Task<string> GetEpicList()
        {
            if (EpicFolderID == null)
                EpicFolderID = await GetEpicFolder();
            if (EpicListID == null)
                EpicListID = await apiHelper.GetListByName(EpicFolderID, "Epic Tasks");
            return EpicListID;
        }

        public async Task<string> GetPBIList()
        {
            if (PBIFolderID == null)
                PBIFolderID = await GetPBIFolder();
            if (PBIListID == null)
                PBIListID = await apiHelper.GetListByName(PBIFolderID, "PBI");
            return PBIListID;
        }

        public async Task<string> GetTasksList()
        {
            if (TasksFolderID == null)
                TasksFolderID = await GetTasksFolder();
            if (TasksListID == null)
                TasksListID = await apiHelper.GetListByName(TasksFolderID, "Task List");
            return TasksListID;
        }

        public async Task<List<string>> GetSprintLists()
        {
            if (SprintFolderID == null)
                SprintFolderID = await GetSprintFolder();
            if (SprintIDs == null)
                SprintIDs = await apiHelper.GetLists(SprintFolderID);
            return SprintIDs;
        }

        #endregion

        #region Tasks Calls
        public async Task<List<TaskItem>> GetEpicTasks()
        {
            if (EpicListID == null)
                EpicListID = await GetEpicList();
            if (EpicTasks == null)
                EpicTasks = await apiHelper.GetListTasks(EpicListID);
            return EpicTasks;
        }

        public async Task<List<TaskItem>> GetPBITasks()
        {
            if (PBIListID == null)
                PBIListID = await GetPBIList();
            if (PBITasks == null)
                PBITasks = await apiHelper.GetListTasks(PBIListID);
            return PBITasks;
        }

        public async Task<List<TaskItem>> GetTaskTasks()
        {
            if (TasksListID == null)
                TasksListID = await GetTasksList();
            if (TaskTasks == null)
                TaskTasks = await apiHelper.GetListTasks(TasksListID);
            return TaskTasks;
        }

        #endregion

        public async Task BindSprints(List<TaskItem> tasks)
        {
            if (SprintIDs == null)
                SprintIDs = await GetSprintLists();
            foreach(string sprintListID in SprintIDs)
            {
                List<TaskItem> sprintItems = await apiHelper.GetListTasks(sprintListID);
                foreach(TaskItem task in tasks)
                {
                    foreach (TaskItem sprintTask in sprintItems)
                    {
                        if (task.ID == sprintTask.ID)
                            task.Sprints.Add(sprintListID);
                    }
                }
            }
        }
    }
}
