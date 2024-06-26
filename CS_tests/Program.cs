﻿using System;
using System.Text;
using SensorNameSpace;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace test{
    class Programm{
        public static void Main(string[] args){


            targetAmsNetId = new AmsNetId(AmsNetId.Local);
            client = new AdsClient();
            client.Connect(targetAmsNetId, 851);

            string[] array = GetStringArrayAds("MAIN", "aArray", 10, 1, 20);

            Console.WriteLine(array);

            //--------------- efficient string concat --------------------------------

            // StringBuilder payloadBuilder = new StringBuilder("{\"table\" : [");
            // for(int i=0; i<50; i++){
            //     payloadBuilder.Append($"{i},");
            // }
            // payloadBuilder.Remove(payloadBuilder.Length-1, 1);
            // payloadBuilder.Append("]}");
            // string payload = payloadBuilder.ToString();
            // Console.WriteLine(payload);

            //----------------- http send and receive --------------------------------

            // using (HttpClient client = new HttpClient()) {
            //     try {
            //         // Define the URL you want to send the request to
            //         string url = "https://example.com/api/endpoint";

            //         // Define your payload as a string
            //         // string payload = "{\"key\": \"value\"}";

            //         // Create the HTTP content from the payload
            //         HttpContent content = new StringContent(payload);

            //         // Set the Content-Type header
            //         content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            //         // Send the POST request and await the response
            //         HttpResponseMessage response = await client.PostAsync(url, content);

            //         // Check if the response is successful (status code 200)
            //         if (response.IsSuccessStatusCode) {
            //             // Read the response content as a string
            //             string responseContent = await response.Content.ReadAsStringAsync();
            //             Console.WriteLine("Response received:");
            //             Console.WriteLine(responseContent);
            //         } else {
            //             Console.WriteLine($"Failed to send request. Status code: {response.StatusCode}");
            //         }
            //     } catch (Exception ex) {
            //         Console.WriteLine($"An error occurred: {ex.Message}");
            //     }
            // }

            // AsyncTask();

            // //----------------- Sensor class ------------------------------------------------
            // Sensor IEPE = new Sensor("0000-0001", false, 10000);
            // Console.WriteLine("infos before changes :");
            // IEPE.getSensorInfos();
            // IEPE.setIsReady(true);
            // Console.WriteLine("\ninfos after changes :");
            // IEPE.getSensorInfos();
            // Console.WriteLine(IEPE);
            // AsyncMain().Wait();



        }

        public static async Task AsyncMain()
        {
            var firstTask = AsyncTask("T1", 5000);
            var secondTask = AsyncTask("T2", 3000);

            var result = await Task.WhenAny(firstTask, secondTask);
            Console.WriteLine(result + " is finished");

            //----------------- Sensor class ------------------------------------------------
            Sensor IEPE = new Sensor("0000-0001", false, 10000);
            Console.WriteLine("infos before changes :");
            IEPE.getSensorInfos();
            IEPE.setIsReady(true);
            Console.WriteLine("\ninfos after changes :");
            IEPE.getSensorInfos();
            Console.WriteLine(IEPE);
        }

        static async Task<string> AsyncTask(string taskName, int delay)
        {
            await Task.Delay(delay);
            return taskName;
        }

        /*
        foreach(var iter in table)
            ThreadPool.QueueUserWorkItem(function, iter)
        */

        
        static string[] GetStringArrayAds(string sensor, string variableName, ushort xLine, ushort xCol, ushort stringSize)
        {
            uint strArrayHandle = 0;
            string[] strArray = new string[xLine*xCol];

            try{
                strArrayHandle = client.CreateVariableHandle(sensor+"."+variableName);

                byte[] readStrArray = new byte[xCol*xLine*stringSize];

                client.Read(strArrayHandle, readStrArray.AsMemory());
                for(int i=0; i < xCol*xLine; i++)
                {
                    byte[] stringBytes = new byte[stringSize];
                    Array.Copy(readStrArray, i * stringSize, stringBytes, 0, stringSize);
                    // Find the index of the first null character
                    int nullIndex = Array.IndexOf(stringBytes, (byte)'\0');
                    // Trim the null characters from the end of the string
                    string str = nullIndex >= 0 ? Encoding.ASCII.GetString(stringBytes, 0, nullIndex) : Encoding.ASCII.GetString(stringBytes);
                    strArray[i] = str;
                }
            }
                catch(Exception e)
            {
                Console.WriteLine($"An error occurred fetching string array: {e.Message}");
            }
            finally
            {
                client.DeleteVariableHandle(strArrayHandle);
            }
            return strArray;
        }
    }
}

