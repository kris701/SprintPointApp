using ClickupInterface.Helpers;
using ClickupInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ClickupInterface.Helpers
{
    internal class ClickupAPIHelper
    {
        public string Token { get; internal set; }
        public string BaseURL { get; internal set; }

        public ClickupAPIHelper(string token, string baseURL)
        {
            Token = token;
            BaseURL = baseURL;
        }

        public async Task<string> GetTeamID()
        {
            APIClient<string, Empty> aPIClient = new APIClient<string, Empty>(BaseURL, "team", Token);
            string response = await aPIClient.HTTPGet();
            JsonDocument json = JsonDocument.Parse(response);
            JsonElement root = json.RootElement;
            JsonElement array = root.GetProperty("teams");
            return array[0].GetProperty("id").GetString();
        }

        public async Task<string> GetSpaceByName(string teamID, string name)
        {
            APIClient<string, Empty> aPIClient = new APIClient<string, Empty>(BaseURL, $"team/{teamID}/space", Token);
            string response = await aPIClient.HTTPGet();
            JsonDocument json = JsonDocument.Parse(response);
            JsonElement root = json.RootElement;
            JsonElement array = root.GetProperty("spaces");
            foreach (JsonElement arrayElement in array.EnumerateArray())
                if (arrayElement.GetProperty("name").GetString() == name)
                    return arrayElement.GetProperty("id").GetString();
            return "";
        }

        public async Task<string> GetFolderByName(string spaceID, string name)
        {
            APIClient<string, Empty> aPIClient = new APIClient<string, Empty>(BaseURL, $"space/{spaceID}/folder", Token);
            string response = await aPIClient.HTTPGet();
            JsonDocument json = JsonDocument.Parse(response);
            JsonElement root = json.RootElement;
            JsonElement array = root.GetProperty("folders");
            foreach (JsonElement arrayElement in array.EnumerateArray())
                if (arrayElement.GetProperty("name").GetString() == name)
                    return arrayElement.GetProperty("id").GetString();
            return "";
        }

        public async Task<string> GetListByName(string folderID, string name)
        {
            APIClient<string, Empty> aPIClient = new APIClient<string, Empty>(BaseURL, $"folder/{folderID}/list", Token);
            string response = await aPIClient.HTTPGet();
            JsonDocument json = JsonDocument.Parse(response);
            JsonElement root = json.RootElement;
            JsonElement array = root.GetProperty("lists");
            foreach (JsonElement arrayElement in array.EnumerateArray())
                if (arrayElement.GetProperty("name").GetString().StartsWith(name))
                    return arrayElement.GetProperty("id").GetString();
            return "";
        }

        public async Task<List<SprintModel>> GetSprints(string folderID)
        {
            List<SprintModel> returnList = new List<SprintModel>();
            APIClient<string, Empty> aPIClient = new APIClient<string, Empty>(BaseURL, $"folder/{folderID}/list", Token);
            string response = await aPIClient.HTTPGet();
            JsonDocument json = JsonDocument.Parse(response);
            JsonElement root = json.RootElement;
            JsonElement array = root.GetProperty("lists");
            foreach (JsonElement arrayElement in array.EnumerateArray())
                returnList.Add(new SprintModel() { 
                    SprintID = arrayElement.GetProperty("id").GetString(),
                    Name = arrayElement.GetProperty("name").GetString(),
                    ViewID = await GetListView(arrayElement.GetProperty("id").GetString())
                });
            return returnList;
        }

        public async Task<string> GetListView(string listID)
        {
            APIClient<string, Empty> aPIClient = new APIClient<string, Empty>(BaseURL, $"list/{listID}/view", Token);
            string response = await aPIClient.HTTPGet();
            JsonDocument json = JsonDocument.Parse(response);
            JsonElement root = json.RootElement;
            JsonElement reqViews = root.GetProperty("required_views");
            JsonElement list = reqViews.GetProperty("list");
            return list.GetProperty("id").GetString();
        }

        public async Task<List<TaskItem>> GetListTasks(string listID)
        {
            List<TaskItem> returnList = new List<TaskItem>();
            APIClient<string, Empty> aPIClient = new APIClient<string, Empty>(BaseURL, $"list/{listID}/task", Token);
            string response = await aPIClient.HTTPGet();
            JsonDocument json = JsonDocument.Parse(response);
            JsonElement root = json.RootElement;
            JsonElement array = root.GetProperty("tasks");
            foreach (JsonElement arrayElement in array.EnumerateArray())
            {
                returnList.Add(TaskFormatter(arrayElement));
            }
            return returnList;
        }

        public async Task<List<TaskItem>> GetViewTasks(string viewID, int page = 0)
        {
            List<TaskItem> returnList = new List<TaskItem>();
            APIClient<string, Empty> aPIClient = new APIClient<string, Empty>(BaseURL, $"view/{viewID}/task?page=0", Token);
            string response = await aPIClient.HTTPGet();
            JsonDocument json = JsonDocument.Parse(response);
            JsonElement root = json.RootElement;
            JsonElement array = root.GetProperty("tasks");
            foreach (JsonElement arrayElement in array.EnumerateArray())
            {
                returnList.Add(TaskFormatter(arrayElement));
            }
            if (returnList.Count >= 30)
                returnList.AddRange(await GetViewTasks(viewID, page + 1));
            return returnList;
        }

        private TaskItem TaskFormatter(JsonElement element)
        {
            string tagName = "";
            if (element.GetProperty("tags").GetArrayLength() > 0)
                tagName = element.GetProperty("tags")[0].GetProperty("name").GetString();

            TaskItem newItem = new TaskItem()
            {
                ID = element.GetProperty("id").GetString(),
                Name = element.GetProperty("name").GetString(),
                SprintPoints = element.GetProperty("points").GetRawText().Replace("\"",""),
                Status = element.GetProperty("status").GetProperty("status").GetString(),
                Type = TaskTypeConverter.GetTypeFromString(tagName)
            };

            JsonElement dependencyArray = element.GetProperty("dependencies");
            if (dependencyArray.GetArrayLength() > 0)
            {
                foreach (JsonElement depElement in dependencyArray.EnumerateArray())
                {
                    string taskID = depElement.GetProperty("task_id").GetString();
                    string dependID = depElement.GetProperty("depends_on").GetString();
                    if (taskID != newItem.ID)
                        newItem.AddDependency(taskID, "1");
                    else
                        newItem.AddDependency(dependID, "0");
                }
            }

            return newItem;
        }
    }
}
