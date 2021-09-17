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

namespace SprintPointApp
{
    /// <summary>
    /// Interaction logic for TaskItemControl.xaml
    /// </summary>
    public partial class TaskItemControl : UserControl
    {
        public TaskItemControl()
        {
            InitializeComponent();
        }

        public TaskItemControl(string taskID, string taskName, string taskPoints, string status, string taskType, string relationUp, string relationDown, string sprintID)
        {
            InitializeComponent();
            TaskID.Content = taskID;
            TaskType.Content = taskType;
            TaskName.Content = taskName;
            TaskPoints.Content = taskPoints;
            StatusLabel.Content = status;
            RelationUp.Content = relationUp;
            RelationDown.Content = relationDown;
            SprintLabel.Content = sprintID;
        }
    }
}
