using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.Models
{
    public class ExerciseObject
    {
        public string ObjectName { get; set; } = "";
        public string Question { get; set; } = "";
        public string SuccessfulAnswer { get; set; } = "";
        public string UnSuccessfulAnswer { get; set; } = "";
    }
}
