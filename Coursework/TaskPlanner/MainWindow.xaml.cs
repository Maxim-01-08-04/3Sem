using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TaskPlanner
{
    public partial class MainWindow : Window
    {
        private List<TaskItem> _allTasks;
        private List<TaskItem> _filteredTasks;

        public MainWindow()
        {
            InitializeComponent();

            _allTasks = DataService.LoadTasks();

            if (CbFilter.Items.Count > 0) CbFilter.SelectedIndex = 0;
            if (CbSort.Items.Count > 0) CbSort.SelectedIndex = 0;

            RefreshList();
        }

        private void RefreshList()
        {
            if (_allTasks == null) return;

            string filter = "Все";
            if (CbFilter.SelectedItem is ComboBoxItem selectedFilterItem)
            {
                filter = selectedFilterItem.Content.ToString();
            }

            if (filter == "Все")
                _filteredTasks = new List<TaskItem>(_allTasks);
            else
                _filteredTasks = _allTasks.Where(t => t.Status == filter).ToList();

            string sort = "По дате";
            if (CbSort.SelectedItem is ComboBoxItem selectedSortItem)
            {
                sort = selectedSortItem.Content.ToString();
            }

            if (sort == "По дате")
                _filteredTasks = _filteredTasks.OrderBy(t => t.DueDate).ToList();
            else if (sort == "По приоритету")
                _filteredTasks = _filteredTasks.OrderBy(t => t.Priority).ToList();

            LvTasks.ItemsSource = null; 
            LvTasks.ItemsSource = _filteredTasks;

            TxtStatusInfo.Text = $"Всего задач: {_allTasks.Count} | Отображено: {_filteredTasks.Count}";
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddTaskWindow();
            if (addWindow.ShowDialog() == true)
            {
                _allTasks.Add(addWindow.NewTask);
                DataService.SaveTasks(_allTasks);
                RefreshList();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LvTasks.SelectedItem is TaskItem selectedTask)
            {
                var result = MessageBox.Show("Удалить задачу?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _allTasks.Remove(selectedTask);
                    DataService.SaveTasks(_allTasks);
                    RefreshList();
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу для удаления");
            }
        }

        private void BtnStatus_Click(object sender, RoutedEventArgs e)
        {
            if (LvTasks.SelectedItem is TaskItem selectedTask)
            {
                if (selectedTask.Status == "Запланировано")
                    selectedTask.Status = "В процессе";
                else if (selectedTask.Status == "В процессе")
                    selectedTask.Status = "Выполнено";
                else
                    selectedTask.Status = "Запланировано";

                DataService.SaveTasks(_allTasks);
                RefreshList();
            }
            else
            {
                MessageBox.Show("Выберите задачу для изменения статуса");
            }
        }

        private void CbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allTasks != null && IsLoaded)
            {
                RefreshList();
            }
        }

        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allTasks != null && IsLoaded)
            {
                RefreshList();
            }
        }
    }
}