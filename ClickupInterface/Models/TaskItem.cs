using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickupInterface.Models
{
    public enum TaskType { Unknown, Epic, PBI, Task }

    public class TaskItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public TaskType Type { get; set; }
        public List<string> UpRelations { get; set; }
        public List<string> DownRelations { get; set; }
        public string SprintPoints { get; set; }
        public string Status { get; set; }
        public List<SprintModel> Sprints { get; set; }

        public void AddDependency(string dependencyID, string type)
        {
            if (type == "0")
            {
                if (DownRelations == null)
                    DownRelations = new List<string>();
                DownRelations.Add(dependencyID);
            }
            if (type == "1")
            {
                if (UpRelations == null)
                    UpRelations = new List<string>();
                UpRelations.Add(dependencyID);
            }
        }
    }
}
