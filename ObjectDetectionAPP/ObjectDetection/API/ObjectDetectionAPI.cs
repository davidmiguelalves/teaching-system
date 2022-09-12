﻿using Newtonsoft.Json;
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
    public class ObjectDetectionAPI
    {
        public string URL { get; set; } = "http://127.0.0.1:5000";

        public List<ObjectDetected> DetectObject(string activity, byte[] byteArray)
        {
            List<ObjectDetected> ret = new List<ObjectDetected>();

            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.PutAsync($"{URL}/detect/{activity}", new ByteArrayContent(byteArray)).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        ret = JsonConvert.DeserializeObject<List<ObjectDetected>>(content);
                    }
                }
                catch
                {
                }
            }
            return ret;
        }

        public List<string> GetActivities()
        {
            List<string> ret = new List<string>();

            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync($"{URL}/activities").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(content);
                        ret = JsonConvert.DeserializeObject<List<string>>(content);
                    }
                }
                catch
                {
                }
            }
            return ret;
        }

        public List<string> GetObjects(string activity)
        {
            List<string> ret = new List<string>();

            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync($"{URL}/objects/{activity}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(content);
                        ret = JsonConvert.DeserializeObject<List<string>>(content);
                    }
                }
                catch
                {
                }
            }
            return ret;
        }
        public bool Probe()
        {
            bool ret = false;

            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync($"{URL}/probe").Result;
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                }
            }
            return ret;
        }
    }
}
