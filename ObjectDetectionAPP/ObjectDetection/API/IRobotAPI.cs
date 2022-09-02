using ObjectDetection.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.API
{
    public interface IRobotAPI
    {
        bool Connect(string IP);
        bool IsConnect();
        bool Disconnect();
        bool Speak(string message);
        bool PlayAnimation(string animation);
        string ParseAnimation(RobotAnimation animation);
    }
}
