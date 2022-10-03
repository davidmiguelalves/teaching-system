using Newtonsoft.Json;
using ObjectDetection.Models;
using ObjectDetection.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.API
{
    public interface IObjectDetectionAPI
    {
        List<ObjectDetected> DetectObject(string activity, byte[] byteArray);
        List<string> GetActivities();
        List<string> GetObjects(string activity);
        bool Probe();
    }
}
