using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.Models
{
    public class Activity
    {
        public string ActivityName { get; set; } = "";
        public string OpeningSentence { get; set; } = "";
        public string FinishSentence { get; set; } = "";

        public List<ActivityObject> Objects = new List<ActivityObject>();

    }
}
