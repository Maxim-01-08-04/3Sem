using System;
using System.Windows;
using System.Windows.Controls;

namespace TaskPlanner
{
    public partial class AddTaskWindow : Window
    {
        public TaskItem NewTask { get; private set; }

        public AddTaskWindow()
        {
            InitializeComponent();
            CbPriority.SelectedIndex = 1;
            DpDate.SelectedDate = DateTime.Now;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                MessageBox.Show("Введите заголовок задачи");
                return;
            }

            string priority = ((ComboBoxItem)CbPriority.SelectedItem).Content.ToString();

            NewTask = new TaskItem(
                TxtTitle.Text,
                TxtDesc.Text,
                DpDate.SelectedDate ?? DateTime.Now,
                priority
            );

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}