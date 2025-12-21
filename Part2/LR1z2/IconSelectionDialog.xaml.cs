using System.Windows;
using System.Collections.Generic;

namespace EnemyEditor
{
    public partial class IconSelectionDialog : Window
    {
        public string SelectedIcon { get; private set; }

        public IconSelectionDialog(List<string> icons)
        {
            InitializeComponent();
            IconsListBox.ItemsSource = icons;
            if (icons.Count > 0)
            {
                IconsListBox.SelectedIndex = 0;
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (IconsListBox.SelectedItem is string selectedIcon)
            {
                SelectedIcon = selectedIcon;
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}