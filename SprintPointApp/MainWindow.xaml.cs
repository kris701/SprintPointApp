using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text;
using System.Text.Json;
using ClickupInterface.Models;

namespace SprintPointApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetSprintPointsButton_Click(object sender, RoutedEventArgs e)
        {
            SprintItemsPanel.Children.Clear();

            ClickupInterface.ClickupInterface clickupInterface = new ClickupInterface.ClickupInterface(Properties.Settings.Default.APIToken, Properties.Settings.Default.APIRoute);
            List<TaskItem> items = new List<TaskItem>();

            items.AddRange(await clickupInterface.GetTaskTasks());
            await clickupInterface.BindSprints(items);
            items.AddRange(await clickupInterface.GetEpicTasks());
            items.AddRange(await clickupInterface.GetPBITasks());

            foreach (TaskItem item in items)
            {
                TaskItemControl newControl = new TaskItemControl(
                    item.ID,
                    item.Name, 
                    item.SprintPoints.ToString(), 
                    item.Status, 
                    item.Type.ToString(), 
                    ConvertListToString(item.UpRelations),
                    ConvertListToString(item.DownRelations),
                    ConvertListToString(item.Sprints)
                    );

                SprintItemsPanel.Children.Add(newControl);
            }
        }

        private string ConvertListToString(List<string> items)
        {
            if (items == null)
                return "-";
            string outString = "";
            int count = 0;
            foreach (string item in items)
            {
                outString += $"[ {item} ] ";
                count++;
                if (count > 1)
                {
                    outString += Environment.NewLine;
                    count = 0;
                }
            }
            return outString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = HeaderItem.ActualWidth;
        }
    }
}
