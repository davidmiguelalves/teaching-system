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
    /// Interaction logic for ExerciseForm.xaml
    /// </summary>
    public partial class ExerciseForm : Window
    {
        private Exercise exercise = new Exercise();
        private string studentName = "";

        private Detection objectDetection;

        private Thread exerciseThread = null;
        private bool isExerciseRunning = false;
        private string currentState = "stop";
        
        private int currentExercise = 0;
        private string currentDetected = "";
        TimeSpan LastDetected = new TimeSpan(0, 0, 0);

        IRobotAPI robotAPI = null;

        public ExerciseForm(Exercise ex, string student, IRobotAPI robotAPI)
        {
            InitializeComponent();
            exercise = ex;
            studentName = student;
            this.robotAPI = robotAPI;

            this.Title = "Exercise: " + ex.ExerciseName;
        }

        private void ExerciseForm_Loaded(object sender, RoutedEventArgs e)
        {
            StartCamera();
        }
        private void ExerciseForm_Closed(object sender, EventArgs e)
        {
            objectDetection.StopCamera();
        }
        private void StartCamera()
        {
            try
            {
                currentState = "start";
                isExerciseRunning = true;
                exerciseThread = new Thread(new ThreadStart(RunExercise));
                exerciseThread.Start();
            }
            catch
            {
            }
        }

        private void RunExercise()
        {
            int detectedok = 0;
            int detectedko = 0;
            int totalsecs = 3;
            TimeSpan cleardetected = new TimeSpan(0, 0, 0);

            while (isExerciseRunning)
            {
                switch (currentState)
                {
                    case "start":
                        UpdateState("Starting Exercise...");
                        currentState = "startyolo";
                        break;

                    case "startyolo":
                        UpdateState("Started!");

                        isExerciseRunning = true;
                        objectDetection = new Detection(exercise.ExerciseName);
                        objectDetection.ImageChange += ImageChangeEvent;
                        objectDetection.ObjectDetected += ObjectDetectedEvent;
                        objectDetection.StartCamera();

                        currentState = "waitingcamera";
                        break;

                    case "waitingcamera":
                        UpdateState("Waiting Camera...");
                        
                        if (objectDetection.IsCameraAvailable())
                        {
                            currentState = "startrobot";
                        }
                        break;

                    case "startrobot":
                        UpdateState("Start Robot!");
                        currentState = "openingsentence";
                        if (robotAPI.Connect("127.0.0.1"))
                        {
                            currentState = "openingsentence";
                        }
                        else
                        {
                            currentState = "errorconnectingrobot";
                        }
                        break;

                    case "openingsentence":
                        UpdateState("Robot Speaking...");
                        _ = robotAPI.Speak(exercise.OpeningSentence.Replace("%%STUDENTNAME%%", studentName));
                        currentState = "questionnextexercise";
                        break;

                    case "questionnextexercise":
                        UpdateState("Robot Speaking...");
                        _ = robotAPI.Speak(exercise.Objects[currentExercise].Question.Replace("%%STUDENTNAME%%", studentName));
                        currentState = "waitingdetection";
                        break;

                    case "errorconnectingrobot":
                        UpdateState("Error Connecting to Robot!");
                        isExerciseRunning = false;
                        break;

                    case "waitingdetection":
                        UpdateState("Waiting Detection: " + exercise.Objects[currentExercise].ObjectName);
                        if (!currentDetected.Equals(""))
                        {
                            currentState = "detected";
                        }
                        break;

                    case "detected":
                        UpdateState($"Detected: {currentDetected}");

                        if (exercise.Objects[currentExercise].ObjectName.Equals(currentDetected))
                        {
                            currentState = "waitingdetection";
                            currentDetected = "";
                            detectedok++;
                            if (detectedok >= 3)
                            {
                                _ = robotAPI.Speak(exercise.Objects[currentExercise].SuccessfulAnswer.Replace("%%STUDENTNAME%%", studentName));
                                _ = robotAPI.PlayAnimation(robotAPI.ParseAnimation(RobotAnimation.CLAP_HANDS));

                                currentState = "finishexercisedetected";
                            }
                        }
                        else
                        {
                            detectedko++;
                            currentDetected = "";
                            if (detectedko >= 3)
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
                        UpdateState("Detected Wrong Exercise! Robot Speaking...");
                        detectedko = 0;
                        detectedok = 0;
                        currentState = "waitingclear";

                        _ = robotAPI.Speak(exercise.Objects[currentExercise].UnSuccessfulAnswer.Replace("%%STUDENTNAME%%", studentName));
                        _ = robotAPI.PlayAnimation(robotAPI.ParseAnimation(RobotAnimation.CROSS_ARMS));

                        break;

                    case "finishexercisedetected":
                        currentExercise++;
                        currentDetected = "";
                        detectedko = 0;
                        detectedok = 0;
                        if (exercise.Objects.Count >= currentExercise + 1)
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
                        UpdateState("Waiting image to clear...");

                        var seconds = (cleardetected - LastDetected).TotalSeconds;
                        if (currentDetected == "")
                        {
                            UpdateState($"Waiting image to clear ({totalsecs - Math.Truncate(seconds)})...");
                            if (seconds > totalsecs)
                            {
                                currentState = "questionnextexercise";
                            }
                        }
                        cleardetected = DateTime.Now.TimeOfDay;
                        currentDetected = "";
                        break;

                    case "finishsentence":
                        UpdateState("Robot Speaking...");
                        _ = robotAPI.Speak(exercise.FinishSentence.Replace("%%STUDENTNAME%%", studentName));
                        currentState = "stopyolo";
                        break;

                    case "stopyolo":
                        UpdateState("Stoping...");
                        currentState = "stoprobot";
                        objectDetection.StopCamera();
                        break;

                    case "stoprobot":
                        UpdateState("Stoping...");
                        currentState = "stop";

                        robotAPI.Disconnect();
                        break;

                    case "stop":
                        UpdateState("Exercise Finished!");
                        isExerciseRunning = false;
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
        private void UpdateState(string state)
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate ()
                {
                    statelabel.Content = state;
                }));
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
