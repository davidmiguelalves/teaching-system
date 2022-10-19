using ObjectDetection.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectDetection.API
{
    public class ZecaRobot : IRobotAPI
    {
        private readonly string URL;

        public ZecaRobot(string url = "http://localhost:8080/")
        {
            this.URL = url;
        }

        public bool Connect(string IP = "127.0.0.1")
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.PostAsync($"{URL}/Connect", new StringContent(IP, Encoding.UTF8, "text/plain")).Result;
                    if (response.IsSuccessStatusCode) return SetSpeakVelocity("80");
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public bool IsConnected()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.PostAsync($"{URL}/IsConnected", null).Result;
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool SetSpeakVelocity(string Velocity)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.PostAsync($"{URL}/SetSpeakVelocity", new StringContent(Velocity, Encoding.UTF8, "text/plain")).Result;
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool Disconnect()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.PostAsync($"{URL}/Disconnect", null).Result;
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool Speak(string Message)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.PostAsync($"{URL}/Speak", new StringContent(Message, Encoding.UTF8, "text/plain")).Result;
                    Thread.Sleep(2000);
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool PlayAnimation(string Animation)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.PostAsync($"{URL}/PlayAnimation", new StringContent(Animation, Encoding.UTF8, "text/plain")).Result;
                    Thread.Sleep(2000);
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }

        public string ParseAnimation(RobotAnimation animation)
        {
            switch (animation)
            {
                case RobotAnimation.rightAnswer01:
                    return "rightAnswer01";
                case RobotAnimation.wrongAnswer:
                    return "wrongAnswer";
            }
            return "";
        }
    }
}
