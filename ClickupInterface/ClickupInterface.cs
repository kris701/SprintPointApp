using ClickupInterface.Helpers;
using ClickupInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickupInterface
{
    public class ClickupInterface : IClickupInterface
    {
        private ClickupAPIHelper apiHelper;

        #region Properties
        public string TeamID { get; internal set; }
        public string SpaceID { get; internal set; }

        public string EpicFolderID { get; internal set; }
        public string PBIFolderID { get; internal set; }
        public string TasksFolderID { get; internal set; }
        public List<string> SprintFolderIDs { get; internal set; }

        public string EpicListID { get; internal set; }
        public string PBIListID { get; internal set; }
        public string TasksListID { get; internal set; }

        public List<SprintModel> Sprints { get; internal set; }

        public List<TaskItem> EpicTasks { get; internal set; }
        public List<TaskItem> PBITasks { get; internal set; }
        public List<TaskItem> TaskTasks { get; internal set; }

        public string Token { get; internal set; }
        public string BaseURL { get; internal set; }
        #endregion

        public ClickupInterface(string token, string baseURL)
        {
            Token = token;
            BaseURL = baseURL;
            apiHelper = new ClickupAPIHelper(token,baseURL);
        }

        public void PurgeTaskLists()
        {
            if (EpicTasks != null)
            {
                EpicTasks.Clear();
                EpicTasks = null;
            }
            if (PBITasks != null)
            {
                PBITasks.Clear();
                PBITasks = null;
            }
            if (TaskTasks != null)
            {
                TaskTasks.Clear();
                TaskTasks = null;
            }
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

        public async Task<List<string>> GetSprintFolders()
        {
            if (SpaceID == null)
                SpaceID = await GetDeveloperSpace();
            if (SprintFolderIDs == null)
                SprintFolderIDs = await apiHelper.GetFolderByStartsWithName(SpaceID, "Sprint");
            return SprintFolderIDs;
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

        public async Task<List<SprintModel>> GetSprints()
        {
            if (SprintFolderIDs == null)
                SprintFolderIDs = await GetSprintFolders();
            if (Sprints == null)
            {
                Sprints = new List<SprintModel>();
                foreach(string sprintFolderID in SprintFolderIDs)
                    Sprints.AddRange(await apiHelper.GetSprints(sprintFolderID));
            }
            return Sprints;
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
            if (Sprints == null)
                Sprints = await GetSprints();

            List<TaskItem> addToTasks = new List<TaskItem>();

            foreach(SprintModel sprint in Sprints)
            {
                List<TaskItem> sprintItems = await apiHelper.GetViewTasks(sprint.ViewID);
                bool found = false;
                foreach(TaskItem sprintTask in sprintItems)
                {
                    found = false;
                    foreach (TaskItem task in tasks)
                    {
                        if (task.ID == sprintTask.ID)
                        {
                            found = true;
                            if (task.Sprints == null)
                                task.Sprints = new List<SprintModel>();
                            task.Sprints.Add(sprint);
                        }
                    }
                    if (!found)
                    {
                        addToTasks.Add(sprintTask);
                        if (sprintTask.Sprints == null)
                            sprintTask.Sprints = new List<SprintModel>();
                        sprintTask.Sprints.Add(sprint);
                    }
                }
            }

            tasks.AddRange(addToTasks);
        }

        public async Task<List<TaskItem>> GetAndBindAllTasks()
        {
            PurgeTaskLists();

            List<TaskItem> items = new List<TaskItem>();

            items.AddRange(await GetTaskTasks());
            items.AddRange(await GetEpicTasks());
            items.AddRange(await GetPBITasks());

            await BindSprints(items);

            return items;
        }
    }
}
