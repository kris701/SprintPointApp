using ClickupInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickupInterface.Helpers
{
    internal static class TaskTypeConverter
    {
        public static TaskType GetTypeFromString(string s)
        {
            s = s.ToUpper();
            if (s.Contains("EPI"))
                return TaskType.Epic;
            if (s.Contains("UST"))
                return TaskType.PBI;
            if (s.Contains("TAS"))
                return TaskType.Task;
            return TaskType.Unknown;
        }
    }
}
