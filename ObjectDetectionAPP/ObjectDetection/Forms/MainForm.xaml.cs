using ObjectDetection.API;
using ObjectDetection.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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
        private bool activityvalidaded = false;
        private string robotIP = "192.168.1.108";
        private string detectionIP = "http://127.0.0.1:5000";

        #endregion

        #region form

        public MainForm()
        {
            InitializeComponent();

            //Change robot type HERE
            robotAPI = new WindowsRobot();
            //robotAPI = new ZecaRobot();

            objectAPI = new ObjectDetectionAPI(detectionIP);

            LoadData();
            
            ValidateActivities();
            
            ActivitiesGrid.ItemsSource = configuration.Activities;
            ActivitiesGrid.SelectedItem = configuration.Activities.Count > 0 ? configuration.Activities[0] : null;
            StudentText.Text = configuration.StudentName;
            FillData(configuration.Activities.Count > 0 ? configuration.Activities[0] : null);

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

        public void ValidateActivities()
        {
            List<string> allactivities = LoadActivitiesAPI();
            if (allactivities.Count() == 0) return;
            List<Activity> activitiesList = new List<Activity>(configuration.Activities);
            bool changed = false;

            foreach (Activity ex in activitiesList)
            {
                if (!allactivities.Contains(ex.ActivityName)){
                    configuration.Activities.Remove(ex);
                    changed = true;
                }
            }
            foreach (string ex in allactivities)
            {
                if (configuration.Activities.Where(e => e.ActivityName.Equals(ex)).Count() == 0)
                {
                    configuration.Activities.Add(new Activity()
                    {
                        ActivityName = ex,
                        FinishSentence = "",
                        OpeningSentence = "",
                        Objects = new List<ActivityObject>()
                    });
                    changed = true;
                }
            }
            if (changed) SaveConfiguration();
            activityvalidaded = true;
        }

        public List<string> LoadActivitiesAPI()
        {
            if (objectAPI.Probe())
            {
                return objectAPI.GetActivities();
            }
            return new List<string>();
        }

        public void LoadConfiguration()
        {
            XmlSerializer reader = XmlSerializer.FromTypes(new[] { typeof(Configuration) })[0];
            StreamReader file = new StreamReader(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Configuraton.xml"));
            Configuration configuration = (Configuration)reader.Deserialize(file);
            this.configuration.StudentName = configuration.StudentName;
            this.configuration.Activities = configuration.Activities;
            file.Close();
        }

        public void SaveConfiguration()
        {
            XmlSerializer writer = XmlSerializer.FromTypes(new[] { typeof(Configuration) })[0];
            FileStream file = File.Create(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Configuraton.xml"));
            writer.Serialize(file, configuration);
            file.Close();
        }

        private void VerifyRobot()
        {
            while (true)
            {
                if (robotAPI.IsConnected())
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
                    if (!activityvalidaded)
                    {
                        ValidateActivities();

                        Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate ()
                        {
                            ActivitiesGrid.Items.Refresh();
                            ActivitiesGrid.SelectedItem = configuration.Activities.Count > 0 ? configuration.Activities[0] : null;
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

        private void FillData(Activity activity)
        {
            if (activity != null)
            {
                ObjectsGrid.ItemsSource = activity.Objects;
                OpeningSentenceText.Text = activity.OpeningSentence;
                FinishSentenceText.Text = activity.FinishSentence;
            }
            else
            {
                ObjectsGrid.ItemsSource = null;
                OpeningSentenceText.Text = "";
                FinishSentenceText.Text = "";
            }
        }

        private void StartActivity(Activity ex)
        {
            if (robotAPI.IsConnected() && objectAPI.Probe())
            {
                if (ex.Objects.Count == 0)
                {
                    MessageBox.Show($"Add objets to the list!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (MessageBox.Show($"Start activity {ex.ActivityName}?", "Start Activity?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ActivityForm form = new ActivityForm(ex, objectAPI, configuration.StudentName, robotIP, robotAPI);
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
                Activity activity = (Activity)ActivitiesGrid.SelectedItem;
                if (activity != null)
                {
                    StartActivity(activity);
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
                    Activity activity = (Activity)ActivitiesGrid.SelectedItem;
                    if (activity != null)
                    {
                        AddActivityForm form = new AddActivityForm(configuration, objectAPI, activity.ActivityName);
                        form.ShowDialog();
                        ActivityObject obj = form.activityObject;
                        if (obj != null)
                        {
                            configuration.Activities[ActivitiesGrid.SelectedIndex].Objects.Add(obj);
                            ObjectsGrid.Items.Refresh();
                            SaveConfiguration();
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Objects API not working!", "Wait", MessageBoxButton.OK, MessageBoxImage.Information);
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
                Activity activity = (Activity)ActivitiesGrid.SelectedItem;
                if (activity != null)
                {
                    ActivityObject obj = (ActivityObject)ObjectsGrid.SelectedItem;
                    if (obj != null)
                    {
                        if (MessageBox.Show($"Remove Object {obj.ObjectName}?", "Remove?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            configuration.Activities[ActivitiesGrid.SelectedIndex].Objects.Remove(obj);
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

                Activity activity = (Activity)ActivitiesGrid.SelectedItem;
                if (activity != null)
                {
                    ActivityObject obj = (ActivityObject)ObjectsGrid.SelectedItem;
                    if (obj != null)
                    {
                        if (ObjectsGrid.SelectedIndex == 0) return;
                        Swap(configuration.Activities[ActivitiesGrid.SelectedIndex].Objects, ObjectsGrid.SelectedIndex, ObjectsGrid.SelectedIndex - 1);
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
            Activity activity = (Activity)ActivitiesGrid.SelectedItem;
            if (activity != null)
            {
                ActivityObject obj = (ActivityObject)ObjectsGrid.SelectedItem;
                if (obj != null)
                {
                    if (ObjectsGrid.SelectedIndex == configuration.Activities.Count) return;
                    Swap(configuration.Activities[ActivitiesGrid.SelectedIndex].Objects, ObjectsGrid.SelectedIndex, ObjectsGrid.SelectedIndex + 1);
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
        private void ActivitiesGrid_PreviewKeyDown(object sender, KeyEventArgs e)
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
                ActivityObject obj = (ActivityObject)ObjectsGrid.SelectedItem;
                MessageBoxResult res = MessageBox.Show(String.Format("Would you want to delete {0}?", obj.ObjectName), "Confirmation!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                e.Handled = (res == MessageBoxResult.No);
            }
        }

        private void ActivitiesGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Activity activity = (Activity)ActivitiesGrid.SelectedItem;
            if (activity != null)
            {
                StartActivity(activity);
            }
        }

        private void ActivitiesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillData((Activity)ActivitiesGrid.SelectedItem);
        }

        private void StudentText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ActivitiesGrid.SelectedItem != null)
            {
                configuration.StudentName = StudentText.Text;
                SaveConfiguration();
            }
        }
       
        #endregion

        #region text

        private void OpeningSentenceText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ActivitiesGrid.SelectedItem != null)
            {
                configuration.Activities[ActivitiesGrid.SelectedIndex].OpeningSentence = OpeningSentenceText.Text;
                SaveConfiguration();
            }
        }

        private void FinishSentenceText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ActivitiesGrid.SelectedItem != null)
            {
                configuration.Activities[ActivitiesGrid.SelectedIndex].FinishSentence = FinishSentenceText.Text;
                SaveConfiguration();
            }
        }

        #endregion

    }
}
