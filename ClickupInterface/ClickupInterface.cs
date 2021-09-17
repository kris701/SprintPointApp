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
        public async Task<string> GetTeamID() => await apiHelper.GetTeamID();

        public async Task<string> GetDeveloperSpace(string teamID) => await apiHelper.GetSpaceByName(teamID, "Software Development");
        
        public async Task<string> GetEpicFolder(string spaceID) => await apiHelper.GetFolderByName(spaceID, "👑 Epics");
        public async Task<string> GetPBIFolder(string spaceID) => await apiHelper.GetFolderByName(spaceID, "📰 Product Backlog");
        public async Task<string> GetTasksFolder(string spaceID) => await apiHelper.GetFolderByName(spaceID, "✔ Tasks");

        public async Task<string> GetEpicList(string folderID) => await apiHelper.GetListByName(folderID, "Epic Tasks");
        public async Task<string> GetPBIList(string folderID) => await apiHelper.GetListByName(folderID, "PBI");
        public async Task<string> GetTasksList(string folderID) => await apiHelper.GetListByName(folderID, "Task List");

        public async Task<List<TaskItem>> GetTasksForList(string listID) => await apiHelper.GetListTasks(listID);
        #endregion

        #region Short hand versions
        public async Task<List<TaskItem>> GetEpicTasks() =>
            await GetTasksForList(
                await GetEpicList(
                    await GetEpicFolder(
                        await GetDeveloperSpace(
                            await GetTeamID())
                        )
                    )
                );

        public async Task<List<TaskItem>> GetPBITasks() =>
            await GetTasksForList(
                await GetPBIList(
                    await GetPBIFolder(
                        await GetDeveloperSpace(
                            await GetTeamID())
                        )
                    )
                );

        public async Task<List<TaskItem>> GetTaskTasks() =>
            await GetTasksForList(
                await GetTasksList(
                    await GetTasksFolder(
                        await GetDeveloperSpace(
                            await GetTeamID())
                        )
                    )
                );

        #endregion
    }
}
