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
        public string Token { get; set; }
        public string BaseURL { get; set; }

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
                string tagName = "";
                if (arrayElement.GetProperty("tags").GetArrayLength() > 0)
                    tagName = arrayElement.GetProperty("tags")[0].GetProperty("name").GetRawText();

                TaskItem newItem = new TaskItem()
                {
                    ID = arrayElement.GetProperty("id").GetRawText(),
                    Name = arrayElement.GetProperty("name").GetRawText(),
                    SprintPoints = arrayElement.GetProperty("points").GetRawText(),
                    Status = arrayElement.GetProperty("status").GetProperty("status").GetRawText(),
                    Type = TaskTypeConverter.GetTypeFromString(tagName)
                };

                JsonElement dependencyArray = arrayElement.GetProperty("dependencies");
                if (dependencyArray.GetArrayLength() > 0)
                {
                    foreach (JsonElement element in dependencyArray.EnumerateArray())
                    {
                        string taskID = element.GetProperty("task_id").GetRawText();
                        string dependID = element.GetProperty("depends_on").GetRawText();
                        if (taskID != newItem.ID)
                            newItem.AddDependency(taskID, "1");
                        else
                            newItem.AddDependency(dependID, "0");
                    }
                }

                JsonElement listArray = arrayElement.GetProperty("list");
                string listName = listArray.GetProperty("name").GetRawText().ToUpper();
                if (listName.Contains("SPRINT"))
                    newItem.SprintID = listArray.GetProperty("id").GetRawText();

                returnList.Add(newItem);
            }
            return returnList;
        }
    }
}
