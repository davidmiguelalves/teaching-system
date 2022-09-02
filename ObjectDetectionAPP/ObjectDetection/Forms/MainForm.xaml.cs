using ObjectDetection.API;
using ObjectDetection.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
    /// Interaction logic for MainForm.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        #region variables
        private Configuration configuration = new Configuration();

        private Thread robotThread = null;
        private Thread objectsThread = null;
        private IRobotAPI robotAPI = null;
        private ObjectDetectionAPI objectAPI = null;
        private string location = @"D:\Tese\Code\ObjectDetectionAPP\ObjectDetection\Contents";
        private bool exercisesvalidaded = false;

        #endregion

        #region form

        public MainForm()
        {
            InitializeComponent();

            //Change robot type HERE
            robotAPI = new WindowsRobot();

            objectAPI = new ObjectDetectionAPI();

            LoadData();
            
            ValidateExercises();
            
            ExercisesGrid.ItemsSource = configuration.Exercises;
            ExercisesGrid.SelectedItem = configuration.Exercises.Count > 0 ? configuration.Exercises[0] : null;
            StudentText.Text = configuration.StudentName;
            FillData(configuration.Exercises.Count > 0 ? configuration.Exercises[0] : null);

            robotThread = new Thread(new ThreadStart(VerifyRobot));
            robotThread.Start();
            objectsThread = new Thread(new ThreadStart(VerifyObjects));
            objectsThread.Start();
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                robotThread.Abort();
                objectsThread.Abort();
                Application.Current.Shutdown();
            }
            catch { }
        }
        #endregion

        #region functions

        private void LoadData()
        {
            LoadConfiguration();
            SaveConfiguration();
        }

        public void ValidateExercises()
        {
            List<string> allexercises = LoadExercisesAPI();
            if (allexercises.Count() == 0) return;
            List<Exercise> exerciseList = new List<Exercise>(configuration.Exercises);
            bool changed = false;

            foreach (Exercise ex in exerciseList)
            {
                if (!allexercises.Contains(ex.ExerciseName)){
                    configuration.Exercises.Remove(ex);
                    changed = true;
                }
            }
            foreach (string ex in allexercises)
            {
                if (configuration.Exercises.Where(e => e.ExerciseName.Equals(ex)).Count() == 0)
                {
                    configuration.Exercises.Add(new Exercise()
                    {
                        ExerciseName = ex,
                        FinishSentence = "",
                        OpeningSentence = "",
                        Objects = new List<ExerciseObject>()
                    });
                    changed = true;
                }
            }
            if (changed) SaveConfiguration();
            exercisesvalidaded = true;
        }

        public List<string> LoadExercisesAPI()
        {
            if (objectAPI.Probe())
            {
                return objectAPI.GetExercises();
            }
            return new List<string>();
        }

        public void LoadConfiguration()
        {
            XmlSerializer reader = XmlSerializer.FromTypes(new[] { typeof(Configuration) })[0];
            StreamReader file = new StreamReader(System.IO.Path.Combine(location, "Configuraton.xml"));
            Configuration configuration = (Configuration)reader.Deserialize(file);
            this.configuration.StudentName = configuration.StudentName;
            this.configuration.Exercises = configuration.Exercises;
            file.Close();
        }

        public void SaveConfiguration()
        {
            XmlSerializer writer = XmlSerializer.FromTypes(new[] { typeof(Configuration) })[0];
            FileStream file = File.Create(System.IO.Path.Combine(location, "Configuraton.xml"));
            writer.Serialize(file, configuration);
            file.Close();
        }

        private void VerifyRobot()
        {
            while (true)
            {
                if (robotAPI.IsConnect())
                {
                    Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate ()
                    {
                        RobotStatus.Fill = new SolidColorBrush(Colors.Green);
                    }));
                }
                else
                {
                    Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate ()
                    {
                        RobotStatus.Fill = new SolidColorBrush(Colors.Red);
                    }));
                }
                Thread.Sleep(5000);
            }

        }

        private void VerifyObjects()
        {
            while (true)
            {
                if (objectAPI.Probe())
                {
                    Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate ()
                    {
                        ImageRecognitionStatus.Fill = new SolidColorBrush(Colors.Green);
                    }));
                    if (!exercisesvalidaded)
                    {
                        ValidateExercises();

                        Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate ()
                        {
                            ExercisesGrid.Items.Refresh();
                            ExercisesGrid.SelectedItem = configuration.Exercises.Count > 0 ? configuration.Exercises[0] : null;
                        }));
                    }
                }
                else
                {
                    Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate ()
                    {
                        ImageRecognitionStatus.Fill = new SolidColorBrush(Colors.Red);
                    }));
                }
                Thread.Sleep(5000);
            }

        }

        private static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        private void FillData(Exercise exercise)
        {
            if (exercise != null)
            {
                ObjectsGrid.ItemsSource = exercise.Objects;
                OpeningSentenceText.Text = exercise.OpeningSentence;
                FinishSentenceText.Text = exercise.FinishSentence;
            }
            else
            {
                ObjectsGrid.ItemsSource = null;
                OpeningSentenceText.Text = "";
                FinishSentenceText.Text = "";
            }
        }

        private void StartExercise(Exercise ex)
        {
            if (robotAPI.IsConnect() && objectAPI.Probe())
            {
                if (ex.Objects.Count == 0)
                {
                    MessageBox.Show($"Add objets to the list!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (MessageBox.Show($"Start Exercise {ex.ExerciseName}?", "Start Exercise?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ExerciseForm form = new ExerciseForm(ex, configuration.StudentName, robotAPI);
                    form.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show($"Objects API or robot are not working!", "Wait", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region buttons

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Exercise exercise = (Exercise)ExercisesGrid.SelectedItem;
                if (exercise != null)
                {
                    StartExercise(exercise);
                }
            }
            catch
            {
            }
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (objectAPI.Probe())
                {
                    Exercise exercise = (Exercise)ExercisesGrid.SelectedItem;
                    if (exercise != null)
                    {
                        AddExerciseForm form = new AddExerciseForm(configuration, exercise.ExerciseName);
                        form.ShowDialog();
                        ExerciseObject obj = form.exerciseObject;
                        if (obj != null)
                        {
                            configuration.Exercises[ExercisesGrid.SelectedIndex].Objects.Add(obj);
                            ObjectsGrid.Items.Refresh();
                            SaveConfiguration();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Exercise exercise = (Exercise)ExercisesGrid.SelectedItem;
                if (exercise != null)
                {
                    ExerciseObject obj = (ExerciseObject)ObjectsGrid.SelectedItem;
                    if (obj != null)
                    {
                        if (MessageBox.Show($"Remove Object {obj.ObjectName}?", "Remove?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            configuration.Exercises[ExercisesGrid.SelectedIndex].Objects.Remove(obj);
                            ObjectsGrid.Items.Refresh();
                            ObjectsGrid.SelectedIndex = 0;
                            SaveConfiguration();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void Up_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Exercise exercise = (Exercise)ExercisesGrid.SelectedItem;
                if (exercise != null)
                {
                    ExerciseObject obj = (ExerciseObject)ObjectsGrid.SelectedItem;
                    if (obj != null)
                    {
                        if (ObjectsGrid.SelectedIndex == 0) return;
                        Swap(configuration.Exercises[ExercisesGrid.SelectedIndex].Objects, ObjectsGrid.SelectedIndex, ObjectsGrid.SelectedIndex - 1);
                        ObjectsGrid.Items.Refresh();
                        SaveConfiguration();
                    }
                }
            }
            catch
            {
            }
        }
        private void Down_Button_Click(object sender, RoutedEventArgs e)
        {
            Exercise exercise = (Exercise)ExercisesGrid.SelectedItem;
            if (exercise != null)
            {
                ExerciseObject obj = (ExerciseObject)ObjectsGrid.SelectedItem;
                if (obj != null)
                {
                    if (ObjectsGrid.SelectedIndex == configuration.Exercises.Count) return;
                    Swap(configuration.Exercises[ExercisesGrid.SelectedIndex].Objects, ObjectsGrid.SelectedIndex, ObjectsGrid.SelectedIndex + 1);
                    ObjectsGrid.Items.Refresh();
                    SaveConfiguration();
                }
            }

        }

        #endregion

        #region grid
        private void ObjectsGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            SaveConfiguration();
        }
        private void ExercisesGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                e.Handled = true;
            }
        }

        private void ObjectsGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ExerciseObject obj = (ExerciseObject)ObjectsGrid.SelectedItem;
                MessageBoxResult res = MessageBox.Show(String.Format("Would you want to delete {0}?", obj.ObjectName), "Confirmation!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                e.Handled = (res == MessageBoxResult.No);
            }
        }

        private void ExercisesGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Exercise exercise = (Exercise)ExercisesGrid.SelectedItem;
            if (exercise != null)
            {
                StartExercise(exercise);
            }
        }

        private void ExercisesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillData((Exercise)ExercisesGrid.SelectedItem);
        }

        private void StudentText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ExercisesGrid.SelectedItem != null)
            {
                configuration.StudentName = StudentText.Text;
                SaveConfiguration();
            }
        }
       
        #endregion

        #region text

        private void OpeningSentenceText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ExercisesGrid.SelectedItem != null)
            {
                configuration.Exercises[ExercisesGrid.SelectedIndex].OpeningSentence = OpeningSentenceText.Text;
                SaveConfiguration();
            }
        }

        private void FinishSentenceText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ExercisesGrid.SelectedItem != null)
            {
                configuration.Exercises[ExercisesGrid.SelectedIndex].FinishSentence = FinishSentenceText.Text;
                SaveConfiguration();
            }
        }

        #endregion

    }
}
