using System;

namespace TaskPlanner
{
    public class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }

        public TaskItem()
        {
            Status = "Запланировано";
            Priority = "Средний";
        }

        public TaskItem(string title, string description, DateTime dueDate, string priority)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Status = "Запланировано";
        }
    }
}