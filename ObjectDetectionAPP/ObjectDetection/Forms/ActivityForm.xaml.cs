using ObjectDetection.API;
using ObjectDetection.Models;
using ObjectDetection.Others;
using ObjectDetection.Workers;
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
    /// Interaction logic for ActivityForm.xaml
    /// </summary>
    public partial class ActivityForm : Window
    {
        private Activity activity = new Activity();
        private string studentName = "";
        private string IP = "127.0.0.1";

        private Detection objectDetection;
        private ObjectDetectionAPI objectAPI;

        private Thread activityThread = null;
        private bool isActivityRunning = false;
        private string currentState = "stop";
        
        private int currentActivity = 0;
        private string currentDetected = "";
        TimeSpan LastDetected = new TimeSpan(0, 0, 0);

        IRobotAPI robotAPI = null;

        public ActivityForm(Activity act, ObjectDetectionAPI api, string student, string IP, IRobotAPI robotAPI)
        {
            InitializeComponent();
            activity = act;
            studentName = student;
            this.robotAPI = robotAPI;
            this.IP = IP;
            this.objectAPI = api; 
            this.Title = "Activity: " + act.ActivityName;
        }

        private void ActivityForm_Loaded(object sender, RoutedEventArgs e)
        {
            StartCamera();
        }
        private void ActivityForm_Closed(object sender, EventArgs e)
        {
            objectDetection.StopCamera();
        }
        private void StartCamera()
        {
            try
            {
                currentState = "start";
                isActivityRunning = true;
                activityThread = new Thread(new ThreadStart(RunActivity));
                activityThread.Start();
            }
            catch
            {
            }
        }

        private void RunActivity()
        {
            int detectedok = 0;
            int detectedko = 0;
            int totalsecs = 2;
            int detectedcount = 2;
            string str = "";


            TimeSpan cleardetected = new TimeSpan(0, 0, 0);

            while (isActivityRunning)
            {
                switch (currentState)
                {
                    case "start":
                        UpdateState("Starting Activity...","");
                        currentState = "startyolo";
                        break;

                    case "startyolo":
                        UpdateState("Started!", "");

                        isActivityRunning = true;
                        objectDetection = new Detection(objectAPI, activity.ActivityName);
                        objectDetection.ImageChange += ImageChangeEvent;
                        objectDetection.ObjectDetected += ObjectDetectedEvent;
                        objectDetection.StartCamera();

                        currentState = "waitingcamera";
                        break;

                    case "waitingcamera":
                        UpdateState("Waiting Camera...", "");
                        
                        if (objectDetection.IsCameraAvailable())
                        {
                            currentState = "startrobot";
                        }
                        break;

                    case "startrobot":
                        UpdateState("Start Robot!", "");
                        currentState = "openingsentence";
                        if (robotAPI.Connect(IP))
                        {
                            currentState = "openingsentence";
                        }
                        else
                        {
                            currentState = "errorconnectingrobot";
                        }
                        break;

                    case "openingsentence":
                        str = activity.OpeningSentence.Replace("%%STUDENTNAME%%", studentName);
                        UpdateState("Robot Speaking...", str);
                        _ = robotAPI.Speak(str);
                        currentState = "questionnextactivity";
                        break;

                    case "questionnextactivity":
                        str = activity.Objects[currentActivity].Question.Replace("%%STUDENTNAME%%", studentName);
                        UpdateState("Robot Speaking...",str);
                        _ = robotAPI.Speak(str);
                        currentState = "waitingdetection";
                        break;

                    case "errorconnectingrobot":
                        UpdateState("Error Connecting to Robot!","");
                        isActivityRunning = false;
                        break;

                    case "waitingdetection":
                        UpdateState("Waiting Detection: " + activity.Objects[currentActivity].ObjectName,"");
                        if (!currentDetected.Equals(""))
                        {
                            currentState = "detected";
                        }
                        break;

                    case "detected":
                        UpdateState($"Detected: {currentDetected}","");

                        if (activity.Objects[currentActivity].ObjectName.Equals(currentDetected))
                        {
                            currentState = "waitingdetection";
                            currentDetected = "";
                            detectedok++;
                            if (detectedok >= detectedcount)
                            {
                                _ = robotAPI.Speak(activity.Objects[currentActivity].SuccessfulAnswer.Replace("%%STUDENTNAME%%", studentName));
                                _ = robotAPI.PlayAnimation(robotAPI.ParseAnimation(RobotAnimation.rightAnswer01));


                                currentState = "finishedactivitydetected";
                            }
                        }
                        else
                        {
                            detectedko++;
                            currentDetected = "";
                            if (detectedko >= detectedcount)
                            {
                                currentState = "detectedko";
                            }
                            else
                            {
                                currentState = "waitingdetection";
                            }
                        }
                        break;

                    case "detectedko":
                        str = activity.Objects[currentActivity].UnSuccessfulAnswer.Replace("%%STUDENTNAME%%", studentName) ;
                        UpdateState($"Detected Wrong Object: {currentDetected}! Robot Speaking...", str);
                        detectedko = 0;
                        detectedok = 0;
                        currentState = "waitingclear";

                        _ = robotAPI.Speak(str);
                        _ = robotAPI.PlayAnimation(robotAPI.ParseAnimation(RobotAnimation.wrongAnswer));


                        break;

                    case "finishedactivitydetected":
                        currentActivity++;
                        currentDetected = "";
                        detectedko = 0;
                        detectedok = 0;
                        if (activity.Objects.Count >= currentActivity + 1)
                        {
                            cleardetected = DateTime.Now.TimeOfDay;
                            currentState = "waitingclear";
                        }
                        else
                        {
                            currentState = "finishsentence";
                        }
                        break;

                    case "waitingclear":
                        UpdateState("Waiting image to clear...","");

                        var seconds = (cleardetected - LastDetected).TotalSeconds;
                        if (currentDetected == "")
                        {
                            UpdateState($"Waiting image to clear ({totalsecs - Math.Truncate(seconds)})...","");
                            if (seconds > totalsecs)
                            {
                                currentState = "questionnextactivity";
                            }
                        }
                        cleardetected = DateTime.Now.TimeOfDay;
                        currentDetected = "";
                        break;

                    case "finishsentence":
                        str = activity.FinishSentence.Replace("%%STUDENTNAME%%", studentName);
                        UpdateState("Robot Speaking...", str);
                        _ = robotAPI.Speak(str);
                        currentState = "stopyolo";
                        break;

                    case "stopyolo":
                        UpdateState("Stoping...","");
                        currentState = "stoprobot";
                        objectDetection.StopCamera();
                        break;

                    case "stoprobot":
                        UpdateState("Stoping...","");
                        currentState = "stop";
                        // TODO: Disconnect
                        //robotAPI.Disconnect();
                        break;

                    case "stop":
                        UpdateState("Activity Finished!","");
                        isActivityRunning = false;
                        break;

                    default:
                        break;
                }

                Console.WriteLine(currentState);
                Thread.Sleep(500);
            }
        }
        private void ImageChangeEvent(Bitmap image)
        {
            UpdateImage(image);
        }
        private void ObjectDetectedEvent(string objectdetected)
        {
            LastDetected = DateTime.Now.TimeOfDay;
            currentDetected = objectdetected;
        }

        private void UpdateImage(Bitmap bitmap)
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate ()
                {
                    if (bitmap != null)
                    {
                        imageSource.Source = ConvertBitmap.BitmapToBitmapSource(bitmap);
                    }
                    else
                    {
                        imageSource.Source = null;

                    }
                }));
            }
            catch (TaskCanceledException)
            {
            }
        }
        private void UpdateState(string state1, string state2)
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate ()
                {
                    statelabel1.Content = state1;
                    statelabel2.Content = state2;

                }));
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
