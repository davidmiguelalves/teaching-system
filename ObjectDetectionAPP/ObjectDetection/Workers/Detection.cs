using ObjectDetection.API;
using ObjectDetection.Models;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectDetection.Workers
{
    public class Detection
    {
        #region private variables

        private ObjectDetectionAPI objectAPI;

        private string activity = "";

        private VideoCapture videoCapture = null;
        private Thread cameraThread = null;
        private bool isCameraRunning = false;

        #endregion

        #region public events

        public delegate void ImageChangeEvent(Bitmap image);
        public event ImageChangeEvent ImageChange;

        public delegate void ObjectDetectedEvent(string objectdetected);
        public event ObjectDetectedEvent ObjectDetected;

        #endregion

        #region public functions

        public Detection(ObjectDetectionAPI api, string act)
        {
            this.objectAPI = api;
            this.activity = act;
        }

        public void StartCamera()
        {
            isCameraRunning = true;
            cameraThread = new Thread(new ThreadStart(RunCamera));
            cameraThread.Start();
        }
        public void StopCamera()
        {
            try
            {
                isCameraRunning = false;
                if (videoCapture != null)
                    videoCapture.Release();
                if (cameraThread != null)
                    cameraThread.Abort();
                ImageChange?.Invoke(null);
            }
            catch
            {
            }
        }

        public bool IsCameraAvailable()
        {
            return videoCapture != null? videoCapture.IsOpened():false;
        }
        #endregion

        #region private functions


        private void RunCamera()
        {
            int index = 2;
            videoCapture = new VideoCapture(index);
            videoCapture.AutoFocus = false;
            videoCapture.Open(index);
            isCameraRunning = true;

            Mat image = new Mat();

            while (isCameraRunning)
            {
                videoCapture.Read(image);
                if (!image.Empty())
                {
                    List<ObjectDetected> list = objectAPI.DetectObject(activity, image.ToBytes());
                    foreach (ObjectDetected obj in list)
                    {
                        image.Rectangle(new OpenCvSharp.Point(obj.x1, obj.y1), new OpenCvSharp.Point(obj.x2, obj.y2), 1);
                        Cv2.PutText(image, $"{obj.objectname} {obj.score:0.00}%", new OpenCvSharp.Point(obj.x1, obj.y1), HersheyFonts.HersheyTriplex, 0.5, Scalar.Black);
                        ObjectDetected?.Invoke(obj.objectname);
                    }
                    ImageChange?.Invoke(BitmapConverter.ToBitmap(image));
                }
            }
        }
        #endregion

    }
}
