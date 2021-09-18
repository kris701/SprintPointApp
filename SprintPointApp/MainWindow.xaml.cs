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
using ClickupInterface;
using ClickupInterface.Models;

namespace SprintPointApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IClickupInterface clickupInterface;

        public MainWindow()
        {
            InitializeComponent();
            clickupInterface = new ClickupInterface.ClickupInterface(Properties.Settings.Default.APIToken, Properties.Settings.Default.APIRoute);
        }

        private async void GetSprintPointsButton_Click(object sender, RoutedEventArgs e)
        {
            SprintItemsPanel.Children.Clear();
            clickupInterface.PurgeTaskLists();

            List<TaskItem> items = new List<TaskItem>();

            items.AddRange(await clickupInterface.GetTaskTasks());
            items.AddRange(await clickupInterface.GetEpicTasks());
            items.AddRange(await clickupInterface.GetPBITasks());

            await clickupInterface.BindSprints(items);

            foreach (TaskItem item in items)
            {
                string sprints = "-";
                if (item.Sprints != null)
                    sprints = ConvertListToString(item.Sprints.Select(x => x.Name).ToList(), 0);
                TaskItemControl newControl = new TaskItemControl(
                    item.ID,
                    item.Name, 
                    item.SprintPoints.ToString(), 
                    item.Status, 
                    item.Type.ToString(), 
                    ConvertListToString(item.UpRelations, 1),
                    ConvertListToString(item.DownRelations, 1),
                    sprints
                    );

                SprintItemsPanel.Children.Add(newControl);
            }
        }

        private string ConvertListToString(List<string> items, int wrapCount)
        {
            if (items == null)
                return "-";
            string outString = "";
            int count = 0;
            foreach (string item in items)
            {
                outString += $"[ {item} ] ";
                count++;
                if (count > wrapCount)
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
