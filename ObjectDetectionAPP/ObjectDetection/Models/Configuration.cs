using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ObjectDetection.Models
{
    public class Configuration
    {
        public string StudentName = "";
        public List<Activity> Activities = new List<Activity>();
    }
}
