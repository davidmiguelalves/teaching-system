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
    /// Interaction logic for ExerciseForm.xaml
    /// </summary>
    public partial class AddExerciseForm : Window
    {
        private ObjectDetectionAPI objectAPI = new ObjectDetectionAPI();
        private Configuration configuration = new Configuration();
        public ExerciseObject exerciseObject = null;
        public AddExerciseForm(Configuration config, string exercise)
        {
            configuration = config;

            InitializeComponent();
            LoadObjects(exercise);
        }
        private void LoadObjects(string exercise)
        {
            List<string> objects = objectAPI.GetObjects(exercise);
            foreach (string ex in objects)
            {
                comboexerciselist.Items.Add(ex);
            }
            comboexerciselist.SelectedIndex = 0;
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if(!QuestionText.Text.Equals("") && !SuccessfulAnswerText.Text.Equals("") && !UnSuccessfulAnswerText.Text.Equals(""))
            {
                ExerciseObject obj = new ExerciseObject()
                {
                    ObjectName = comboexerciselist.Text,
                    Question = QuestionText.Text,
                    SuccessfulAnswer = SuccessfulAnswerText.Text,
                    UnSuccessfulAnswer = UnSuccessfulAnswerText.Text
                };
                exerciseObject = obj;
                Close();
            }
            else
            {
                MessageBox.Show($"All fields must be filled!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
