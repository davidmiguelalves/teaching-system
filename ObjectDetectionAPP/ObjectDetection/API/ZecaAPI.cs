using ObjectDetection.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetection.API
{
    public class ZecaAPI : IRobotAPI
    {
        private readonly string URL;

        public ZecaAPI(string url = "http://localhost:8080/")
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
                    if (response.IsSuccessStatusCode) return SetSpeakVelocity("100");
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public bool IsConnect()
        {
            return true;
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
                case RobotAnimation.CLAP_HANDS:
                    return "clap-hands";
                case RobotAnimation.CROSS_ARMS:
                    return "cross-arms";
            }
            return "";
        }
    }
}
