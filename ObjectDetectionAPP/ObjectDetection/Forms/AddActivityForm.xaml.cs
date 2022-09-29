using ObjectDetection.API;
using ObjectDetection.Models;
using ObjectDetection.Others;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace ObjectDetection.Forms
{
    /// <summary>
    /// Interaction logic for AddActivityForm.xaml
    /// </summary>
    public partial class AddActivityForm : Window
    {
        private ObjectDetectionAPI objectAPI;
        private Configuration configuration = new Configuration();
        public ActivityObject activityObject = null;
        public AddActivityForm(Configuration config, ObjectDetectionAPI api, string activity)
        {
            configuration = config;
            objectAPI = api;
            InitializeComponent();
            LoadObjects(activity);
        }
        private void LoadObjects(string activity)
        {
            List<string> objects = objectAPI.GetObjects(activity);
            foreach (string ex in objects)
            {
                comboactivitylist.Items.Add(ex);
            }
            comboactivitylist.SelectedIndex = 0;
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if(!QuestionText.Text.Equals("") && !SuccessfulAnswerText.Text.Equals("") && !UnSuccessfulAnswerText.Text.Equals(""))
            {
                ActivityObject obj = new ActivityObject()
                {
                    ObjectName = comboactivitylist.Text,
                    Question = QuestionText.Text,
                    SuccessfulAnswer = SuccessfulAnswerText.Text,
                    UnSuccessfulAnswer = UnSuccessfulAnswerText.Text
                };
                activityObject = obj;
                Close();
            }
            else
            {
                MessageBox.Show($"All fields must be filled!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
