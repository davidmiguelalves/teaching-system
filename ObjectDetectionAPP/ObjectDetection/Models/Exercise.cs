using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.Models
{
    public class Exercise
    {
        public string ExerciseName { get; set; } = "";
        public string OpeningSentence { get; set; } = "";
        public string FinishSentence { get; set; } = "";

        public List<ExerciseObject> Objects = new List<ExerciseObject>();

    }
}
