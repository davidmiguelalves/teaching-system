using ObjectDetection.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.API
{
    public class WindowsRobot : IRobotAPI
    {
        public bool Connect(string IP = "127.0.0.1")
        {
            return true;
        }
        public bool IsConnect()
        {
            return true;
        }
        public bool Disconnect()
        {
            return true;
        }
        public bool Speak(string message)
        {
            var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.Speak(message);
            return true;
        }
        public bool PlayAnimation(string animation)
        {
            return true;
        }

        public string ParseAnimation(RobotAnimation animation)
        {
            switch (animation)
            {
                case RobotAnimation.CLAP_HANDS:
                    return "clap-hands";
                case RobotAnimation.CROSS_ARMS:
                    return "cross-arms";
            }
            return "";
        }
    }
}
