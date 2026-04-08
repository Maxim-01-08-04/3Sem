using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TaskPlanner
{
    public static class DataService
    {
        private static readonly string FilePath = "tasks.json";

        public static void SaveTasks(List<TaskItem> tasks)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(tasks, options);
            File.WriteAllText(FilePath, jsonString);
        }

        public static List<TaskItem> LoadTasks()
        {
            if (!File.Exists(FilePath))
                return new List<TaskItem>();

            string jsonString = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<TaskItem>>(jsonString) ?? new List<TaskItem>();
        }
    }
}